// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="ThrottlePolicy.cs" company="stefanprodan.com">
//     Copyright © Stefan Prodan 2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiThrottle
{
    /// <summary>
    /// Rate limits policy
    /// </summary>
    [Serializable]
    public class ThrottlePolicy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottlePolicy"/> class.
        /// </summary>
        public ThrottlePolicy()
        {
            IpWhitelist = new List<string>();
            IpRules = new Dictionary<string, RateLimits>();
            ClientWhitelist = new List<string>();
            ClientRules = new Dictionary<string, RateLimits>();
            EndpointWhitelist = new List<string>();
            EndpointRules = new Dictionary<string, RateLimits>();
            Rates = new Dictionary<RateLimitPeriod, long>();
        }

        /// <summary>
        /// Configure default request limits per second, minute, hour or day
        /// </summary>
        /// <param name="perSecond">The per second.</param>
        /// <param name="perMinute">The per minute.</param>
        /// <param name="perHour">The per hour.</param>
        /// <param name="perDay">The per day.</param>
        /// <param name="perWeek">The per week.</param>
        public ThrottlePolicy(long? perSecond = null, long? perMinute = null, long? perHour = null, long? perDay = null, long? perWeek = null)
            : this()
        {
            Rates = new Dictionary<RateLimitPeriod, long>();
            if (perSecond.HasValue)
            {
                Rates.Add(RateLimitPeriod.Second, perSecond.Value);
            }

            if (perMinute.HasValue)
            {
                Rates.Add(RateLimitPeriod.Minute, perMinute.Value);
            }


            if (perHour.HasValue)
            {
                Rates.Add(RateLimitPeriod.Hour, perHour.Value);
            }


            if (perDay.HasValue)
            {
                Rates.Add(RateLimitPeriod.Day, perDay.Value);
            }
            if (perWeek.HasValue)
            {
                Rates.Add(RateLimitPeriod.Week, perWeek.Value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IP throttling is enabled.
        /// </summary>
        /// <value><c>true</c> if [ip throttling]; otherwise, <c>false</c>.</value>
        public bool IpThrottling { get; set; }

        /// <summary>
        /// Gets or sets the ip whitelist.
        /// </summary>
        /// <value>The ip whitelist.</value>
        public List<string> IpWhitelist { get; set; }

        /// <summary>
        /// Gets or sets the ip rules.
        /// </summary>
        /// <value>The ip rules.</value>
        public IDictionary<string, RateLimits> IpRules { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether client key throttling is enabled.
        /// </summary>
        /// <value><c>true</c> if [client throttling]; otherwise, <c>false</c>.</value>
        public bool ClientThrottling { get; set; }

        /// <summary>
        /// Gets or sets the client whitelist.
        /// </summary>
        /// <value>The client whitelist.</value>
        public List<string> ClientWhitelist { get; set; }

        /// <summary>
        /// Gets or sets the client rules.
        /// </summary>
        /// <value>The client rules.</value>
        public IDictionary<string, RateLimits> ClientRules { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether route throttling is enabled
        /// </summary>
        /// <value><c>true</c> if [endpoint throttling]; otherwise, <c>false</c>.</value>
        public bool EndpointThrottling { get; set; }

        /// <summary>
        /// Gets or sets the endpoint whitelist.
        /// </summary>
        /// <value>The endpoint whitelist.</value>
        public List<string> EndpointWhitelist { get; set; }

        /// <summary>
        /// Gets or sets the endpoint rules.
        /// </summary>
        /// <value>The endpoint rules.</value>
        public IDictionary<string, RateLimits> EndpointRules { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all requests, including the rejected ones, should be stacked in this order: day, hour, min, sec
        /// </summary>
        /// <value><c>true</c> if [stack blocked requests]; otherwise, <c>false</c>.</value>
        public bool StackBlockedRequests { get; set; }

        /// <summary>
        /// Gets or sets the rates.
        /// </summary>
        /// <value>The rates.</value>
        public Dictionary<RateLimitPeriod, long> Rates { get; set; }

        /// <summary>
        /// Froms the store.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>ThrottlePolicy.</returns>
        public static ThrottlePolicy FromStore(IThrottlePolicyProvider provider)
        {
            var settings = provider.ReadSettings();
            var whitelists = provider.AllWhitelists();
            var rules = provider.AllRules();

            var policy = new ThrottlePolicy(
                perSecond: settings.LimitPerSecond,
               perMinute: settings.LimitPerMinute,
               perHour: settings.LimitPerHour,
               perDay: settings.LimitPerDay,
               perWeek: settings.LimitPerWeek);

            policy.IpThrottling = settings.IpThrottling;
            policy.ClientThrottling = settings.ClientThrottling;
            policy.EndpointThrottling = settings.EndpointThrottling;
            policy.StackBlockedRequests = settings.StackBlockedRequests;

            policy.IpRules = new Dictionary<string, RateLimits>();
            policy.ClientRules = new Dictionary<string, RateLimits>();
            policy.EndpointRules = new Dictionary<string, RateLimits>();
            policy.EndpointWhitelist = new List<string>();
            policy.IpWhitelist = new List<string>();
            policy.ClientWhitelist = new List<string>();

            foreach (var item in rules)
            {
                var rateLimit = new RateLimits
                                    {
                                        PerSecond = item.LimitPerSecond,
                                        PerMinute = item.LimitPerMinute,
                                        PerHour = item.LimitPerHour,
                                        PerDay = item.LimitPerDay,
                                        PerWeek = item.LimitPerWeek
                                    };

                switch (item.PolicyType)
                {
                    case ThrottlePolicyType.IpThrottling:
                        policy.IpRules.Add(item.Entry, rateLimit);
                        break;
                    case ThrottlePolicyType.ClientThrottling:
                        policy.ClientRules.Add(item.Entry, rateLimit);
                        break;
                    case ThrottlePolicyType.EndpointThrottling:
                        policy.EndpointRules.Add(item.Entry, rateLimit);
                        break;
                }
            }

            if (whitelists != null)
            {
                policy.IpWhitelist.AddRange(whitelists.Where(x => x.PolicyType == ThrottlePolicyType.IpThrottling).Select(x => x.Entry));
                policy.ClientWhitelist.AddRange(whitelists.Where(x => x.PolicyType == ThrottlePolicyType.ClientThrottling).Select(x => x.Entry));
                policy.EndpointWhitelist.AddRange(whitelists.Where(x => x.PolicyType == ThrottlePolicyType.EndpointThrottling).Select(x => x.Entry));
            }
            return policy;
        }
    }
}
