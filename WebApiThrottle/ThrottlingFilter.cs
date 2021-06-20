// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="ThrottlingFilter.cs" company="stefanprodan.com">
//     Copyright © Stefan Prodan 2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApiThrottle.Net;

namespace WebApiThrottle
{
    /// <summary>
    /// Throttle action filter
    /// Implements the <see cref="System.Web.Http.Filters.ActionFilterAttribute" />
    /// Implements the <see cref="System.Web.Http.Filters.IActionFilter" />
    /// </summary>
    /// <seealso cref="System.Web.Http.Filters.ActionFilterAttribute" />
    /// <seealso cref="System.Web.Http.Filters.IActionFilter" />
    public class ThrottlingFilter : ActionFilterAttribute, IActionFilter
    {
        /// <summary>
        /// The core
        /// </summary>
        private readonly ThrottlingCore core;

        /// <summary>
        /// The policy repository
        /// </summary>
        private IPolicyRepository policyRepository;

        /// <summary>
        /// The policy
        /// </summary>
        private ThrottlePolicy policy;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottlingFilter" /> class.
        /// By default, the <see cref="QuotaExceededResponseCode" /> property
        /// is set to 429 (Too Many Requests).
        /// </summary>
        public ThrottlingFilter()
        {
            QuotaExceededResponseCode = (HttpStatusCode)429;
            Repository = new CacheRepository();
            core = new ThrottlingCore();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottlingFilter" /> class.
        /// Persists the policy object in cache using <see cref="IPolicyRepository" /> implementation.
        /// The policy object can be updated by <see cref="ThrottleManager" /> at runtime.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="policyRepository">The policy repository.</param>
        /// <param name="repository">The repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="ipAddressParser">The ip address provider</param>
        public ThrottlingFilter(ThrottlePolicy policy, 
            IPolicyRepository policyRepository, 
            IThrottleRepository repository, 
            IThrottleLogger logger, 
            IIpAddressParser ipAddressParser = null)
        {
            core = new ThrottlingCore();
            core.Repository = repository;
            Repository = repository;
            Logger = logger;
            if (ipAddressParser != null)
            {
                core.IpAddressParser = ipAddressParser;
            }

            QuotaExceededResponseCode = (HttpStatusCode)429;

            this.policy = policy;
            this.policyRepository = policyRepository;

            if (policyRepository != null)
            {
                policyRepository.Save(ThrottleManager.GetPolicyKey(), policy);
            }
        }

        /// <summary>
        /// Gets or sets a repository used to access throttling rate limits policy.
        /// </summary>
        /// <value>The policy repository.</value>
        public IPolicyRepository PolicyRepository
        {
            get { return policyRepository; }
            set { policyRepository = value; }
        }

        /// <summary>
        /// Gets or sets the throttling rate limits policy
        /// </summary>
        /// <value>The policy.</value>
        public ThrottlePolicy Policy
        {
            get { return policy; }
            set { policy = value; }
        }

        /// <summary>
        /// Gets or sets the throttle metrics storage
        /// </summary>
        /// <value>The repository.</value>
        public IThrottleRepository Repository { get; set; }

        /// <summary>
        /// Gets or sets an instance of <see cref="IThrottleLogger" /> that will log blocked requests
        /// </summary>
        /// <value>The logger.</value>
        public IThrottleLogger Logger { get; set; }

        /// <summary>
        /// Gets or sets a value that will be used as a formatter for the QuotaExceeded response message.
        /// If none specified the default will be:
        /// API calls quota exceeded! maximum admitted {0} per {1}
        /// </summary>
        /// <value>The quota exceeded message.</value>
        public string QuotaExceededMessage { get; set; }

        /// <summary>
        /// Gets or sets a value that will be used as a formatter for the QuotaExceeded response message.
        /// If none specified the default will be:
        /// API calls quota exceeded! maximum admitted {0} per {1}
        /// </summary>
        /// <value>The content of the quota exceeded.</value>
        public Func<long, RateLimitPeriod, object> QuotaExceededContent { get; set; }

        /// <summary>
        /// Gets or sets the value to return as the HTTP status
        /// code when a request is rejected because of the
        /// throttling policy. The default value is 429 (Too Many Requests).
        /// </summary>
        /// <value>The quota exceeded response code.</value>
        public HttpStatusCode QuotaExceededResponseCode { get; set; }

        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            EnableThrottlingAttribute attrPolicy = null;
            var applyThrottling = ApplyThrottling(actionContext, out attrPolicy);

            // get policy from repo
            if(policyRepository != null)
            {
                policy = policyRepository.FirstOrDefault(ThrottleManager.GetPolicyKey());
            }

            if (Policy != null && applyThrottling)
            {
                core.Repository = Repository;
                core.Policy = Policy;

                var identity = SetIdentity(actionContext.Request);

                if (!core.IsWhitelisted(identity))
                {
                    TimeSpan timeSpan = TimeSpan.FromSeconds(1);

                    // get default rates
                    var defRates = core.RatesWithDefaults(Policy.Rates.ToList());
                    if (Policy.StackBlockedRequests)
                    {
                        // all requests including the rejected ones will stack in this order: week, day, hour, min, sec
                        // if a client hits the hour limit then the minutes and seconds counters will expire and will eventually get erased from cache
                        defRates.Reverse();
                    }

                    // apply policy
                    foreach (var rate in defRates)
                    {
                        var rateLimitPeriod = rate.Key;
                        var rateLimit = rate.Value;

                        timeSpan = core.GetTimeSpanFromPeriod(rateLimitPeriod);

                        // apply EnableThrottlingAttribute policy
                        var attrLimit = attrPolicy.GetLimit(rateLimitPeriod);
                        if (attrLimit > 0)
                        {
                            rateLimit = attrLimit;
                        }

                        // apply global rules
                        core.ApplyRules(identity, timeSpan, rateLimitPeriod, ref rateLimit);

                        if (rateLimit > 0)
                        {
                            // increment counter
                            var requestId = ComputeThrottleKey(identity, rateLimitPeriod); 
                            var throttleCounter = core.ProcessRequest(timeSpan, requestId);

                            // check if key expired
                            if (throttleCounter.Timestamp + timeSpan < DateTime.UtcNow)
                            {
                                continue;
                            }

                            // check if limit is reached
                            if (throttleCounter.TotalRequests > rateLimit)
                            {
                                // log blocked request
                                if (Logger != null)
                                {
                                    Logger.Log(core.ComputeLogEntry(requestId, identity, throttleCounter, rateLimitPeriod.ToString(), rateLimit, actionContext.Request));
                                }

                                var message = !string.IsNullOrEmpty(this.QuotaExceededMessage) 
                                    ? this.QuotaExceededMessage 
                                    : "API calls quota exceeded! maximum admitted {0} per {1}.";

                                var content = this.QuotaExceededContent != null
                                    ? this.QuotaExceededContent(rateLimit, rateLimitPeriod)
                                    : string.Format(message, rateLimit, rateLimitPeriod);

                                // add status code and retry after x seconds to response
                                actionContext.Response = QuotaExceededResponse(
                                    actionContext.Request,
                                    content,
                                    QuotaExceededResponseCode,
                                    core.RetryAfterFrom(throttleCounter.Timestamp, rateLimitPeriod));
                            }
                        }
                    }
                }
            }

            base.OnActionExecuting(actionContext);
        }

        /// <summary>
        /// Sets the identity.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>RequestIdentity.</returns>
        protected virtual RequestIdentity SetIdentity(HttpRequestMessage request)
        {
            var entry = new RequestIdentity();
            entry.ClientIp = core.GetClientIp(request).ToString();
            entry.Endpoint = request.RequestUri.AbsolutePath.ToLowerInvariant();
            entry.ClientKey = request.Headers.Contains("Authorization-Token") ? request.Headers.GetValues("Authorization-Token").First() : "anon";

            return entry;
        }

        /// <summary>
        /// Computes the throttle key.
        /// </summary>
        /// <param name="requestIdentity">The request identity.</param>
        /// <param name="period">The period.</param>
        /// <returns>System.String.</returns>
        protected virtual string ComputeThrottleKey(RequestIdentity requestIdentity, RateLimitPeriod period)
        {
            return core.ComputeThrottleKey(requestIdentity, period);
        }

        /// <summary>
        /// Gets the client ip.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>IPAddress.</returns>
        protected IPAddress GetClientIp(HttpRequestMessage request)
        {
            return core.GetClientIp(request);
        }

        /// <summary>
        /// Quotas the exceeded response.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="content">The content.</param>
        /// <param name="responseCode">The response code.</param>
        /// <param name="retryAfter">The retry after.</param>
        /// <returns>HttpResponseMessage.</returns>
        protected virtual HttpResponseMessage QuotaExceededResponse(HttpRequestMessage request, object content, HttpStatusCode responseCode, string retryAfter)
        {
            var response = request.CreateResponse(responseCode, content);
            response.Headers.Add("Retry-After", new string[] { retryAfter });
            return response;
        }

        /// <summary>
        /// Applies the throttling.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        /// <param name="attr">The attribute.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool ApplyThrottling(HttpActionContext filterContext, out EnableThrottlingAttribute attr)
        {
            var applyThrottling = false;
            attr = null;

            if (filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<EnableThrottlingAttribute>(true).Any())
            {
                attr = filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<EnableThrottlingAttribute>(true).First();
                applyThrottling = true;
            }

            if (filterContext.ActionDescriptor.GetCustomAttributes<EnableThrottlingAttribute>(true).Any())
            {
                attr = filterContext.ActionDescriptor.GetCustomAttributes<EnableThrottlingAttribute>(true).First();
                applyThrottling = true;
            }

            // explicit disabled
            if (filterContext.ActionDescriptor.GetCustomAttributes<DisableThrottingAttribute>(true).Any())
            {
                applyThrottling = false;
            }

            return applyThrottling;
        }
    }
}
