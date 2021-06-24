using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiThrottle.Net;

namespace WebApiThrottle
{
    /// <summary>
    /// Class CustomThrottlingHandler.
    /// </summary>
    /// <seealso cref="WebApiThrottle.ThrottlingHandler" />
    public class CustomThrottlingHandler : ThrottlingHandler
    {

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
        public CustomThrottlingHandler(ThrottlePolicy policy,
            IPolicyRepository policyRepository,
            IThrottleRepository repository,
            IThrottleLogger logger,
            IIpAddressParser ipAddressParser = null) : base(policy, policyRepository, repository, logger, ipAddressParser)
        {

        }
        /// <summary>
        /// Sets the identity.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>RequestIdentity.</returns>
        protected override RequestIdentity SetIdentity(HttpRequestMessage request)
        {
            var identity = new RequestIdentity()
            {
                ClientKey = request.Headers.Contains("Authorization-Key") ? request.Headers.GetValues("Authorization-Key").First() : "anon",
                ClientIp = GetClientIp(request).ToString(),
                Endpoint = request.RequestUri.AbsolutePath.ToLowerInvariant()
            };
            var throttleKeys = new Dictionary<string, string>() {
                { "Second",base.ComputeThrottleKey(identity,RateLimitPeriod.Second)},
                { "Minute",base.ComputeThrottleKey(identity,RateLimitPeriod.Minute)},
                { "Day",base.ComputeThrottleKey(identity,RateLimitPeriod.Day)},
            };
            //SysLogHelper.Info("WebApiThrottle,", $"Identity:{JsonHelper.JsonConvertSerializeIgnoreNullValue(identity)},throttleKeys:{JsonHelper.JsonConvertSerializeIgnoreNullValue(throttleKeys)}");
            return identity;
        }
        /// <summary>
        /// Quotas the exceeded response.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="content">The content.</param>
        /// <param name="responseCode">The response code.</param>
        /// <param name="retryAfter">The retry after.</param>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        protected override Task<HttpResponseMessage> QuotaExceededResponse(HttpRequestMessage request, object content, HttpStatusCode responseCode, string retryAfter)
        {
            var response = request.CreateResponse(responseCode, content);
            response.Headers.Add("Retry-After", new string[] { retryAfter });

            //处理跨域的问题
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Headers", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "*");
            return Task.FromResult(response);
        }
    }
}