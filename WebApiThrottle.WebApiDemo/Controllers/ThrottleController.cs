using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiThrottle.WebApiDemo.Infrastructure;
using WebApiThrottle.WebApiDemo.Models;

namespace WebApiThrottle.WebApiDemo.Controllers
{
    public class ThrottleController : ApiController
    {
 
        /// <summary>
        /// The policy repository
        /// </summary>
        private readonly IPolicyRepository policyRepository = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottleController" /> class.
        /// </summary>
        public ThrottleController()
        {
            
            policyRepository = Singleton<IPolicyRepository>.Instance;

        }
        /// <summary>
        /// Gets the policy.
        /// carl.wu
        /// </summary>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult GetPolicy()
        {
            KmmResult<ThrottlePolicy> kmmResult = new KmmResult<ThrottlePolicy>();
            if (policyRepository == null)
            {
                return Json(kmmResult.Error("请先启用Api限流功能"));
            }
            var policy = policyRepository.FirstOrDefault(ThrottleManager.GetPolicyKey());
            return Json(kmmResult.Success(policy));
        }
        /// <summary>
        /// Updates the policy.
        /// carl.wu
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult UpdatePolicy([FromBody] ThrottlePolicyModel model)
        {
            KmmResult<ThrottlePolicy> kmmResult = new KmmResult<ThrottlePolicy>();
            if (model == null || (!model.Rules.Any() && !model.Whitelists.Any()))
            {
                return Json(kmmResult.Error(KmHttpError.ParameterError));
            }
            if (policyRepository == null)
            {
                return Json(kmmResult.Error("请先启用Api限流功能"));
            }

            var policy = policyRepository.FirstOrDefault(ThrottleManager.GetPolicyKey());
            if (policy == null)
            {
                return Json(kmmResult.Error("请先设置Api限流策略"));
            }

            if (model.Rules.Any())
            {
                if (model.Rules.Any(a => string.IsNullOrWhiteSpace(a.Entry)))
                {
                    return Json(kmmResult.Error(KmHttpError.ParameterError));
                }

                var ipThrottling = model.Rules.Where(a => a.PolicyType == ThrottlePolicyType.IpThrottling)
                    .Select(a => new KeyValuePair<string, RateLimits>(a.Entry, new RateLimits
                    {
                        PerSecond = a.LimitPerSecond,
                        PerMinute = a.LimitPerMinute,
                        PerHour = a.LimitPerHour,
                        PerDay = a.LimitPerDay,
                        PerWeek = a.LimitPerWeek
                    }));
                if (ipThrottling.Any())
                {
                    foreach (var item in ipThrottling)
                    {
                        if (policy.IpRules.ContainsKey(item.Key))
                        {
                            policy.IpRules[item.Key] = item.Value;
                        }
                        else
                        {
                            policy.IpRules.Add(item.Key, item.Value);
                        }
                    }
                }

                var clientRules = model.Rules.Where(a => a.PolicyType == ThrottlePolicyType.ClientThrottling)
                    .Select(a => new KeyValuePair<string, RateLimits>(a.Entry, new RateLimits
                    {
                        PerSecond = a.LimitPerSecond,
                        PerMinute = a.LimitPerMinute,
                        PerHour = a.LimitPerHour,
                        PerDay = a.LimitPerDay,
                        PerWeek = a.LimitPerWeek
                    }));
                if (clientRules.Any())
                {
                    foreach (var item in clientRules)
                    {
                        if (policy.ClientRules.ContainsKey(item.Key))
                        {
                            policy.ClientRules[item.Key] = item.Value;
                        }
                        else
                        {
                            policy.ClientRules.Add(item.Key, item.Value);
                        }
                    }
                }


                var endpointThrottling = model.Rules.Where(a => a.PolicyType == ThrottlePolicyType.EndpointThrottling)
                    .Select(a => new KeyValuePair<string, RateLimits>(a.Entry, new RateLimits
                    {
                        PerSecond = a.LimitPerSecond,
                        PerMinute = a.LimitPerMinute,
                        PerHour = a.LimitPerHour,
                        PerDay = a.LimitPerDay,
                        PerWeek = a.LimitPerWeek
                    }));
                if (endpointThrottling.Any())
                {
                    foreach (var item in endpointThrottling)
                    {
                        if (policy.EndpointRules.ContainsKey(item.Key))
                        {
                            policy.EndpointRules[item.Key] = item.Value;
                        }
                        else
                        {
                            policy.EndpointRules.Add(item.Key, item.Value);
                        }
                    }
                }
            }

            if (model.Whitelists.Any())
            {

                var ipWhitelists = model.Whitelists.Where(a => a.PolicyType == ThrottlePolicyType.IpThrottling && !policy.IpWhitelist.Contains(a.Entry)).Select(a => a.Entry);
                if (ipWhitelists.Any())
                {
                    policy.IpWhitelist.AddRange(ipWhitelists);
                }

                var clientWhitelists = model.Whitelists.Where(a => a.PolicyType == ThrottlePolicyType.ClientThrottling && !policy.ClientWhitelist.Contains(a.Entry)).Select(a => a.Entry);
                if (clientWhitelists.Any())
                {
                    policy.ClientWhitelist.AddRange(clientWhitelists);
                }

                var endpointWhitelists = model.Whitelists.Where(a => a.PolicyType == ThrottlePolicyType.EndpointThrottling && !policy.EndpointWhitelist.Contains(a.Entry)).Select(a => a.Entry);
                if (endpointWhitelists.Any())
                {
                    policy.EndpointWhitelist.AddRange(endpointWhitelists);
                }

            }

            //apply policy updates
            ThrottleManager.UpdatePolicy(policy, policyRepository);

            policy = policyRepository.FirstOrDefault(ThrottleManager.GetPolicyKey());
            return Json(kmmResult.Success(policy));
        }
       

    }
}