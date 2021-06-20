// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="ThrottlePolicySettings.cs" company="stefanprodan.com">
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
    /// Class ThrottlePolicySettings.
    /// </summary>
    [Serializable]
    public class ThrottlePolicySettings
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
        /// Gets or sets a value indicating whether [ip throttling].
        /// </summary>
        /// <value><c>true</c> if [ip throttling]; otherwise, <c>false</c>.</value>
        public bool IpThrottling { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [client throttling].
        /// </summary>
        /// <value><c>true</c> if [client throttling]; otherwise, <c>false</c>.</value>
        public bool ClientThrottling { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [endpoint throttling].
        /// </summary>
        /// <value><c>true</c> if [endpoint throttling]; otherwise, <c>false</c>.</value>
        public bool EndpointThrottling { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [stack blocked requests].
        /// </summary>
        /// <value><c>true</c> if [stack blocked requests]; otherwise, <c>false</c>.</value>
        public bool StackBlockedRequests { get; set; }
    }
}
