// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="ThrottlePolicyConfiguration.cs" company="stefanprodan.com">
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
    /// Class ThrottlePolicyConfiguration.
    /// Implements the <see cref="System.Configuration.ConfigurationSection" />
    /// </summary>
    /// <seealso cref="System.Configuration.ConfigurationSection" />
    public class ThrottlePolicyConfiguration : ConfigurationSection
    {
        /// <summary>
        /// Gets the limit per second.
        /// </summary>
        /// <value>The limit per second.</value>
        [ConfigurationProperty("limitPerSecond", DefaultValue = "0", IsRequired = false)]
        [LongValidator(ExcludeRange = false, MinValue = 0)]
        public long LimitPerSecond
        {
            get
            {
                return (long)this["limitPerSecond"];
            }
        }

        /// <summary>
        /// Gets the limit per minute.
        /// </summary>
        /// <value>The limit per minute.</value>
        [ConfigurationProperty("limitPerMinute", DefaultValue = "0", IsRequired = false)]
        [LongValidator(ExcludeRange = false, MinValue = 0)]
        public long LimitPerMinute
        {
            get
            {
                return (long)this["limitPerMinute"];
            }
        }

        /// <summary>
        /// Gets the limit per hour.
        /// </summary>
        /// <value>The limit per hour.</value>
        [ConfigurationProperty("limitPerHour", DefaultValue = "0", IsRequired = false)]
        [LongValidator(ExcludeRange = false, MinValue = 0)]
        public long LimitPerHour
        {
            get
            {
                return (long)this["limitPerHour"];
            }
        }

        /// <summary>
        /// Gets the limit per day.
        /// </summary>
        /// <value>The limit per day.</value>
        [ConfigurationProperty("limitPerDay", DefaultValue = "0", IsRequired = false)]
        [LongValidator(ExcludeRange = false, MinValue = 0)]
        public long LimitPerDay
        {
            get
            {
                return (long)this["limitPerDay"];
            }
        }

        /// <summary>
        /// Gets the limit per week.
        /// </summary>
        /// <value>The limit per week.</value>
        [ConfigurationProperty("limitPerWeek", DefaultValue = "0", IsRequired = false)]
        [LongValidator(ExcludeRange = false, MinValue = 0)]
        public long LimitPerWeek
        {
            get
            {
                return (long)this["limitPerWeek"];
            }
        }

        /// <summary>
        /// Gets a value indicating whether [ip throttling].
        /// </summary>
        /// <value><c>true</c> if [ip throttling]; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("ipThrottling", DefaultValue = "false", IsRequired = false)]
        public bool IpThrottling
        {
            get
            {
                return (bool)this["ipThrottling"];
            }
        }

        /// <summary>
        /// Gets a value indicating whether [client throttling].
        /// </summary>
        /// <value><c>true</c> if [client throttling]; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("clientThrottling", DefaultValue = "false", IsRequired = false)]
        public bool ClientThrottling
        {
            get
            {
                return (bool)this["clientThrottling"];
            }
        }

        /// <summary>
        /// Gets a value indicating whether [endpoint throttling].
        /// </summary>
        /// <value><c>true</c> if [endpoint throttling]; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("endpointThrottling", DefaultValue = "false", IsRequired = false)]
        public bool EndpointThrottling
        {
            get
            {
                return (bool)this["endpointThrottling"];
            }
        }

        /// <summary>
        /// Gets a value indicating whether [stack blocked requests].
        /// </summary>
        /// <value><c>true</c> if [stack blocked requests]; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("stackBlockedRequests", DefaultValue = "false", IsRequired = false)]
        public bool StackBlockedRequests
        {
            get
            {
                return (bool)this["stackBlockedRequests"];
            }
        }

        /// <summary>
        /// Gets the rules.
        /// </summary>
        /// <value>The rules.</value>
        [ConfigurationProperty("rules")]
        public ThrottlePolicyRuleConfigurationCollection Rules
        {
            get
            {
                return this["rules"] as ThrottlePolicyRuleConfigurationCollection;
            }
        }

        /// <summary>
        /// Gets the whitelists.
        /// </summary>
        /// <value>The whitelists.</value>
        [ConfigurationProperty("whitelists")]
        public ThrottlePolicyWhitelistConfigurationCollection Whitelists
        {
            get
            {
                return this["whitelists"] as ThrottlePolicyWhitelistConfigurationCollection;
            }
        }
    }
}
