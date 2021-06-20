// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2021-06-17
// ***********************************************************************
// <copyright file="ThrottlingHandler.cs" company="stefanprodan.com">
//     Copyright © Stefan Prodan 2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebApiThrottle.Net;

namespace WebApiThrottle
{
    /// <summary>
    /// Throttle message handler
    /// Implements the <see cref="System.Net.Http.DelegatingHandler" />
    /// </summary>
    /// <seealso cref="System.Net.Http.DelegatingHandler" />
    public class ThrottlingHandler : DelegatingHandler
    {
        /// <summary>
        /// The core
        /// </summary>
        private ThrottlingCore core;
        /// <summary>
        /// The policy repository
        /// </summary>
        private IPolicyRepository policyRepository;
        /// <summary>
        /// The policy
        /// </summary>
        private ThrottlePolicy policy;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottlingHandler" /> class.
        /// By default, the <see cref="QuotaExceededResponseCode" /> property
        /// is set to 429 (Too Many Requests).
        /// </summary>
        public ThrottlingHandler()
        {
            QuotaExceededResponseCode = (HttpStatusCode)429;
            Repository = new CacheRepository();
            core = new ThrottlingCore();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottlingHandler" /> class.
        /// Persists the policy object in cache using <see cref="IPolicyRepository" /> implementation.
        /// The policy object can be updated by <see cref="ThrottleManager" /> at runtime.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="policyRepository">The policy repository.</param>
        /// <param name="repository">The repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="ipAddressParser">The IpAddressParser</param>
        public ThrottlingHandler(ThrottlePolicy policy, 
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
        /// Gets or sets the throttling rate limits policy repository
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
        /// Gets or sets an instance of <see cref="IThrottleLogger" /> that logs traffic and blocked requests
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
        /// 异步发送 HTTP 请求到要发送到服务器的内部处理程序。
        /// </summary>
        /// <param name="request">要发送到服务器的 HTTP 请求消息。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>表示异步操作的任务对象。</returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // get policy from repo
            if (policyRepository != null)
            {
                policy = policyRepository.FirstOrDefault(ThrottleManager.GetPolicyKey());
            }

            if (policy == null || (!policy.IpThrottling && !policy.ClientThrottling && !policy.EndpointThrottling))
            {
                return base.SendAsync(request, cancellationToken);
            }

            core.Repository = Repository;
            core.Policy = policy;

            var identity = SetIdentity(request);

            if (core.IsWhitelisted(identity))
            {
                return base.SendAsync(request, cancellationToken);
            }

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
                            Logger.Log(core.ComputeLogEntry(requestId, identity, throttleCounter, rateLimitPeriod.ToString(), rateLimit, request));
                        }

                        var message = !string.IsNullOrEmpty(this.QuotaExceededMessage) 
                            ? this.QuotaExceededMessage 
                            : "API calls quota exceeded! maximum admitted {0} per {1}.";

                        var content = this.QuotaExceededContent != null
                            ? this.QuotaExceededContent(rateLimit, rateLimitPeriod)
                            : string.Format(message, rateLimit, rateLimitPeriod);

                        // break execution
                        return QuotaExceededResponse(
                            request,
                            content,
                            QuotaExceededResponseCode,
                            core.RetryAfterFrom(throttleCounter.Timestamp, rateLimitPeriod));
                    }
                }
            }

            // no throttling required
            return base.SendAsync(request, cancellationToken);
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
        /// Sets the identity.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>RequestIdentity.</returns>
        protected virtual RequestIdentity SetIdentity(HttpRequestMessage request)
        {
            var entry = new RequestIdentity();
            entry.ClientIp = core.GetClientIp(request).ToString();
            entry.Endpoint = request.RequestUri.AbsolutePath.ToLowerInvariant();
            entry.ClientKey = request.Headers.Contains("Authorization-Token") 
                ? request.Headers.GetValues("Authorization-Token").First() 
                : "anon";

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
        /// Quotas the exceeded response.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="content">The content.</param>
        /// <param name="responseCode">The response code.</param>
        /// <param name="retryAfter">The retry after.</param>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        protected virtual Task<HttpResponseMessage> QuotaExceededResponse(HttpRequestMessage request, object content, HttpStatusCode responseCode, string retryAfter)
        {
            var response = request.CreateResponse(responseCode, content);
            response.Headers.Add("Retry-After", new string[] { retryAfter });
            return Task.FromResult(response);
        }
    }
}
