using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using WebApiThrottle.WebApiDemo.Extensions;
using WebApiThrottle.WebApiDemo.Infrastructure;
using WebApiThrottle.WebApiDemo.Models;

namespace WebApiThrottle.WebApiDemo.Controllers
{
    /// <summary>
    /// Class ThrottleController.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class ThrottleController : ApiController
    {
        /// <summary>
        /// The default settings
        /// </summary>
        private readonly JsonSerializerSettings defaultSettings = null;
        /// <summary>
        /// The policy repository
        /// </summary>
        private readonly IPolicyRepository policyRepository = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottleController" /> class.
        /// </summary>
        public ThrottleController()
        {
            //Json格式化默认设置
            defaultSettings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Converters = { new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" } }
            };
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
            return GetPolicyResult();
        }

        /// <summary>
        /// Policies the setting.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult PolicySetting(PolicySettingModel model)
        {
            if (model == null || (!model.IpThrottling.HasValue && !model.ClientThrottling.HasValue && !model.EndpointThrottling.HasValue
             && !model.StackBlockedRequests.HasValue && model.Limit == null
                ))
            {
                return GetParameterError();
            }
            var policy = GetPolicyData();
            if (policy == null)
            {
                return GetError();
            }

            if (model.IpThrottling.HasValue)
            {
                policy.IpThrottling = model.IpThrottling.Value;
            }
            if (model.ClientThrottling.HasValue)
            {
                policy.ClientThrottling = model.ClientThrottling.Value;
            }
            if (model.EndpointThrottling.HasValue)
            {
                policy.EndpointThrottling = model.EndpointThrottling.Value;
            }

            if (model.StackBlockedRequests.HasValue)
            {
                policy.StackBlockedRequests = model.StackBlockedRequests.Value;
            }
            if (model.Limit != null)
            {
                if (model.Limit.PerSecond.HasValue)
                {
                    policy.Rates.AddOrUpdate(RateLimitPeriod.Second, (key) => { return model.Limit.PerSecond.Value; });
                }
                if (model.Limit.PerMinute.HasValue)
                {
                    policy.Rates.AddOrUpdate(RateLimitPeriod.Minute, (key) => { return model.Limit.PerMinute.Value; });
                }
                if (model.Limit.PerHour.HasValue)
                {
                    policy.Rates.AddOrUpdate(RateLimitPeriod.Hour, (key) => { return model.Limit.PerHour.Value; });
                }
                if (model.Limit.PerDay.HasValue)
                {
                    policy.Rates.AddOrUpdate(RateLimitPeriod.Day, (key) => { return model.Limit.PerDay.Value; });
                }
                if (model.Limit.PerWeek.HasValue)
                {
                    policy.Rates.AddOrUpdate(RateLimitPeriod.Week, (key) => { return model.Limit.PerWeek.Value; });
                }
            }

            return UpdatePolicyResult(policy);
        }


        /// <summary>
        /// Updates the policy.
        /// carl.wu
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult UpdatePolicyRule(PolicyRuleModel model)
        {
            if (model == null || model.Rules.IsEmpty())
            {
                return GetParameterError();
            }
            var policy = GetPolicyData();
            if (policy == null)
            {
                return GetError();
            }
            switch (model.PolicyType)
            {
                case ThrottlePolicyType.IpThrottling:
                    var ipThrottling = model.Rules.DistinctBy(a => a.Entry).Select(a => new KeyValuePair<string, RateLimits>(a.Entry, new RateLimits
                    {
                        PerSecond = a.LimitPerSecond,
                        PerMinute = a.LimitPerMinute,
                        PerHour = a.LimitPerHour,
                        PerDay = a.LimitPerDay,
                        PerWeek = a.LimitPerWeek
                    }));

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
                    return UpdatePolicyResult(policy);
                case ThrottlePolicyType.ClientThrottling:
                    var clientRules = model.Rules.DistinctBy(a => a.Entry).Select(a => new KeyValuePair<string, RateLimits>(a.Entry, new RateLimits
                    {
                        PerSecond = a.LimitPerSecond,
                        PerMinute = a.LimitPerMinute,
                        PerHour = a.LimitPerHour,
                        PerDay = a.LimitPerDay,
                        PerWeek = a.LimitPerWeek
                    }));
                    if (clientRules.IsAny())
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
                    return UpdatePolicyResult(policy);
                case ThrottlePolicyType.EndpointThrottling:
                    var endpointThrottling = model.Rules.DistinctBy(a => a.Entry).Select(a => new KeyValuePair<string, RateLimits>(a.Entry, new RateLimits
                    {
                        PerSecond = a.LimitPerSecond,
                        PerMinute = a.LimitPerMinute,
                        PerHour = a.LimitPerHour,
                        PerDay = a.LimitPerDay,
                        PerWeek = a.LimitPerWeek
                    }));
                    if (endpointThrottling.IsAny())
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
                    return UpdatePolicyResult(policy);
                default:
                    return GetPolicyResult();
            }

        }

        /// <summary>
        /// Updates the policy whitelist.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult UpdatePolicyWhitelist(PolicyWhitelistModel model)
        {
            if (model == null || model.Whitelists.IsEmpty())
            {
                return GetParameterError();
            }
            var policy = GetPolicyData();
            if (policy == null)
            {
                return GetError();
            }
            switch (model.PolicyType)
            {
                case ThrottlePolicyType.IpThrottling:
                    var ipWhitelists = model.Whitelists.Where(a => !policy.IpWhitelist.Contains(a));
                    if (ipWhitelists.IsAny())
                    {
                        policy.IpWhitelist.AddRange(ipWhitelists);
                    }
                    return UpdatePolicyResult(policy);
                case ThrottlePolicyType.ClientThrottling:
                    var clientWhitelists = model.Whitelists.Where(a => !policy.ClientWhitelist.Contains(a));
                    if (clientWhitelists.IsAny())
                    {
                        policy.ClientWhitelist.AddRange(clientWhitelists);
                    }
                    return UpdatePolicyResult(policy);
                case ThrottlePolicyType.EndpointThrottling:
                    var endpointWhitelists = model.Whitelists.Where(a => !policy.EndpointWhitelist.Contains(a));
                    if (endpointWhitelists.IsAny())
                    {
                        policy.EndpointWhitelist.AddRange(endpointWhitelists);
                    }
                    return UpdatePolicyResult(policy);
                default:
                    return GetPolicyResult();
            }
        }

        /// <summary>
        /// Deletes the policy rule.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult DeletePolicyRule(DeletePolicyModel model)
        {
            if (model == null || model.Entities.IsEmpty())
            {
                return GetParameterError();
            }
            var policy = GetPolicyData();
            if (policy == null)
            {
                return GetError();
            }
            switch (model.PolicyType)
            {
                case ThrottlePolicyType.IpThrottling:
                    model.Entities.Each(a =>
                    {
                        policy.IpRules.Remove(a);
                    });
                    return UpdatePolicyResult(policy);
                case ThrottlePolicyType.ClientThrottling:
                    model.Entities.Each(a =>
                    {
                        policy.ClientRules.Remove(a);
                    });
                    return UpdatePolicyResult(policy);
                case ThrottlePolicyType.EndpointThrottling:
                    model.Entities.Each(a =>
                    {
                        policy.EndpointRules.Remove(a);
                    });
                    return UpdatePolicyResult(policy);
                default:
                    return GetPolicyResult();
            }
        }

        /// <summary>
        /// Clears the policy rule.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult ClearPolicyRule(ClearPolicyModel model)
        {
            if (model == null)
            {
                return GetParameterError();
            }
            var policy = GetPolicyData();
            if (policy == null)
            {
                return GetError();
            }
            switch (model.PolicyType)
            {
                case ThrottlePolicyType.IpThrottling:
                    policy.IpRules = new Dictionary<string, RateLimits>();
                    return UpdatePolicyResult(policy);
                case ThrottlePolicyType.ClientThrottling:
                    policy.ClientRules = new Dictionary<string, RateLimits>();
                    return UpdatePolicyResult(policy);
                case ThrottlePolicyType.EndpointThrottling:
                    policy.EndpointRules = new Dictionary<string, RateLimits>();
                    return UpdatePolicyResult(policy);
                default:
                    return GetPolicyResult();
            }
        }

        /// <summary>
        /// Deletes the whitelist.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult DeleteWhitelist(DeletePolicyModel model)
        {
            if (model == null || model.Entities.IsEmpty())
            {
                return GetParameterError();
            }
            var policy = GetPolicyData();
            if (policy == null)
            {
                return GetError();
            }
            switch (model.PolicyType)
            {
                case ThrottlePolicyType.IpThrottling:
                    model.Entities.Each(a =>
                    {
                        policy.IpWhitelist.Remove(a);
                    });
                    return UpdatePolicyResult(policy);
                case ThrottlePolicyType.ClientThrottling:
                    model.Entities.Each(a =>
                    {
                        policy.ClientWhitelist.Remove(a);
                    });
                    return UpdatePolicyResult(policy);
                case ThrottlePolicyType.EndpointThrottling:
                    model.Entities.Each(a =>
                    {
                        policy.EndpointWhitelist.Remove(a);
                    });
                    return UpdatePolicyResult(policy);
                default:
                    return GetPolicyResult();
            }
        }
        /// <summary>
        /// Clears the whitelist.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult ClearWhitelist(ClearPolicyModel model)
        {
            if (model == null)
            {
                return GetParameterError();
            }
            var policy = GetPolicyData();
            if (policy == null)
            {
                return GetError();
            }
            switch (model.PolicyType)
            {
                case ThrottlePolicyType.IpThrottling:
                    policy.IpWhitelist = new List<string>();
                    return UpdatePolicyResult(policy);
                case ThrottlePolicyType.ClientThrottling:
                    policy.ClientWhitelist = new List<string>();
                    return UpdatePolicyResult(policy);
                case ThrottlePolicyType.EndpointThrottling:
                    policy.EndpointWhitelist = new List<string>();
                    return UpdatePolicyResult(policy);
                default:
                    return GetPolicyResult();
            }
        }

        /// <summary>
        /// Creates a <see cref="T:System.Web.Http.Results.JsonResult`1" /> (200 OK) with the specified value.
        /// carl.wu
        /// </summary>
        /// <typeparam name="T">The type of content in the entity body.</typeparam>
        /// <param name="content">The content value to serialize in the entity body.</param>
        /// <returns>A <see cref="T:System.Web.Http.Results.JsonResult`1" /> with the specified value.</returns>
        [NonAction]
        protected internal new JsonResult<T> Json<T>(T content)
        {
            return Json(content, defaultSettings);
        }

        /// <summary>
        /// Gets the policy data.
        /// </summary>
        /// <returns>ThrottlePolicy.</returns>
        [NonAction]
        private ThrottlePolicy GetPolicyData()
        {
            var policy = policyRepository?.FirstOrDefault(ThrottleManager.GetPolicyKey());
            if (policy == null)
            {
                return policy;
            }
            if (policy.IpRules == null)
            {
                policy.IpRules = new Dictionary<string, RateLimits>();
            }
            if (policy.ClientRules == null)
            {
                policy.ClientRules = new Dictionary<string, RateLimits>();
            }
            if (policy.EndpointRules == null)
            {
                policy.EndpointRules = new Dictionary<string, RateLimits>();
            }

            if (policy.IpWhitelist == null)
            {
                policy.IpWhitelist = new List<string>();
            }
            if (policy.ClientWhitelist == null)
            {
                policy.ClientWhitelist = new List<string>();
            }
            if (policy.EndpointWhitelist == null)
            {
                policy.EndpointWhitelist = new List<string>();
            }
            if (policy.Rates == null)
            {
                policy.Rates = new Dictionary<RateLimitPeriod, long>();
            }

            return policy;
        }

        /// <summary>
        /// Gets the policy result.
        /// </summary>
        /// <returns>IHttpActionResult.</returns>
        [NonAction]
        private IHttpActionResult GetPolicyResult()
        {
            if (policyRepository == null)
            {
                return GetError();
            }
            var policy = GetPolicyData();
            if (policy == null)
            {
                return GetError();
            }
            KmmResult<ThrottlePolicy> kmmResult = new KmmResult<ThrottlePolicy>();
            return Ok(kmmResult.Success(policy));
        }
        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <returns>IHttpActionResult.</returns>
        [NonAction]
        private IHttpActionResult GetError()
        {
            return Json(new KmmResult<ThrottlePolicy>().Error("请先设置Api限流策略"));
        }
        /// <summary>
        /// Gets the parameter error.
        /// </summary>
        /// <returns>IHttpActionResult.</returns>
        [NonAction]
        private IHttpActionResult GetParameterError()
        {
            return Json(new KmmResult<ThrottlePolicy>().Error(KmHttpError.ParameterError));
        }
        /// <summary>
        /// Updates the policy result.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <returns>IHttpActionResult.</returns>
        private IHttpActionResult UpdatePolicyResult(ThrottlePolicy policy)
        {
            //apply policy updates
            ThrottleManager.UpdatePolicy(policy, policyRepository);
            return GetPolicyResult();
        }

    }
}