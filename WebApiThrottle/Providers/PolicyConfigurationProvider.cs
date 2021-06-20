// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="PolicyConfigurationProvider.cs" company="stefanprodan.com">
//     Copyright © Stefan Prodan 2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiThrottle
{
    /// <summary>
    /// Class PolicyConfigurationProvider.
    /// Implements the <see cref="WebApiThrottle.IThrottlePolicyProvider" />
    /// </summary>
    /// <seealso cref="WebApiThrottle.IThrottlePolicyProvider" />
    public class PolicyConfigurationProvider : IThrottlePolicyProvider
    {
        /// <summary>
        /// The policy configuration
        /// </summary>
        private readonly ThrottlePolicyConfiguration policyConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyConfigurationProvider"/> class.
        /// </summary>
        public PolicyConfigurationProvider()
        {
            this.policyConfig = ConfigurationManager.GetSection("throttlePolicy") as ThrottlePolicyConfiguration;
        }

        /// <summary>
        /// Reads the settings.
        /// </summary>
        /// <returns>ThrottlePolicySettings.</returns>
        public ThrottlePolicySettings ReadSettings()
        {
            var settings = new ThrottlePolicySettings()
            {
                IpThrottling = policyConfig.IpThrottling,
                ClientThrottling = policyConfig.ClientThrottling,
                EndpointThrottling = policyConfig.EndpointThrottling,
                StackBlockedRequests = policyConfig.StackBlockedRequests,
                LimitPerSecond = policyConfig.LimitPerSecond,
                LimitPerMinute = policyConfig.LimitPerMinute,
                LimitPerHour = policyConfig.LimitPerHour,
                LimitPerDay = policyConfig.LimitPerDay,
                LimitPerWeek = policyConfig.LimitPerWeek
            };

            return settings;
        }

        /// <summary>
        /// Alls the rules.
        /// </summary>
        /// <returns>IEnumerable&lt;ThrottlePolicyRule&gt;.</returns>
        public IEnumerable<ThrottlePolicyRule> AllRules()
        {
            var rules = new List<ThrottlePolicyRule>();
            if (policyConfig.Rules != null)
            {
                foreach (ThrottlePolicyRuleConfigurationElement rule in policyConfig.Rules)
                {
                    rules.Add(new ThrottlePolicyRule
                    {
                        Entry = rule.Entry,
                        PolicyType = (ThrottlePolicyType)rule.PolicyType,
                        LimitPerSecond = rule.LimitPerSecond,
                        LimitPerMinute = rule.LimitPerMinute,
                        LimitPerHour = rule.LimitPerHour,
                        LimitPerDay = rule.LimitPerDay,
                        LimitPerWeek = rule.LimitPerWeek
                    });
                }
            }
            return rules;
        }

        /// <summary>
        /// Alls the whitelists.
        /// </summary>
        /// <returns>IEnumerable&lt;ThrottlePolicyWhitelist&gt;.</returns>
        public IEnumerable<ThrottlePolicyWhitelist> AllWhitelists()
        {
            var whitelists = new List<ThrottlePolicyWhitelist>();
            if (policyConfig.Whitelists != null)
            {
                foreach (ThrottlePolicyWhitelistConfigurationElement whitelist in policyConfig.Whitelists)
                {
                    whitelists.Add(new ThrottlePolicyWhitelist
                    {
                        Entry = whitelist.Entry,
                        PolicyType = (ThrottlePolicyType)whitelist.PolicyType,
                    });
                }
            }

            return whitelists;
        }
    }
}
