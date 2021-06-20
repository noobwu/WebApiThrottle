// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="ThrottlePolicyRule.cs" company="stefanprodan.com">
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
    /// Class ThrottlePolicyRule.
    /// </summary>
    [Serializable]
    public class ThrottlePolicyRule
    {
        /// <summary>
        /// Gets or sets the limit per second.
        /// </summary>
        /// <value>The limit per second.</value>
        public long LimitPerSecond { get; set; }

        /// <summary>
        /// Gets or sets the limit per minute.
        /// </summary>
        /// <value>The limit per minute.</value>
        public long LimitPerMinute { get; set; }

        /// <summary>
        /// Gets or sets the limit per hour.
        /// </summary>
        /// <value>The limit per hour.</value>
        public long LimitPerHour { get; set; }

        /// <summary>
        /// Gets or sets the limit per day.
        /// </summary>
        /// <value>The limit per day.</value>
        public long LimitPerDay { get; set; }

        /// <summary>
        /// Gets or sets the limit per week.
        /// </summary>
        /// <value>The limit per week.</value>
        public long LimitPerWeek { get; set; }

        /// <summary>
        /// Gets or sets the entry.
        /// </summary>
        /// <value>The entry.</value>
        public string Entry { get; set; }

        /// <summary>
        /// Gets or sets the type of the policy.
        /// </summary>
        /// <value>The type of the policy.</value>
        public ThrottlePolicyType PolicyType { get; set; }
    }
}
