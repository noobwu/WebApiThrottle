// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="ThrottlePolicyRuleConfigurationElement.cs" company="stefanprodan.com">
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
    /// Class ThrottlePolicyRuleConfigurationElement.
    /// Implements the <see cref="System.Configuration.ConfigurationElement" />
    /// </summary>
    /// <seealso cref="System.Configuration.ConfigurationElement" />
    public class ThrottlePolicyRuleConfigurationElement : ConfigurationElement
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
        /// Gets the entry.
        /// </summary>
        /// <value>The entry.</value>
        [ConfigurationProperty("entry", IsRequired = true)]
        public string Entry
        {
            get
            {
                return this["entry"] as string;
            }
        }

        /// <summary>
        /// Gets the type of the policy.
        /// </summary>
        /// <value>The type of the policy.</value>
        [ConfigurationProperty("policyType", IsRequired = true)]
        public int PolicyType
        {
            get
            {
                return (int)this["policyType"];
            }
        }
    }
}
