﻿// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="RateLimits.cs" company="stefanprodan.com">
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
    /// Class RateLimits.
    /// </summary>
    [Serializable]
    public class RateLimits
    {
        /// <summary>
        /// Gets or sets the per second.
        /// </summary>
        /// <value>The per second.</value>
        public long PerSecond { get; set; }

        /// <summary>
        /// Gets or sets the per minute.
        /// </summary>
        /// <value>The per minute.</value>
        public long PerMinute { get; set; }

        /// <summary>
        /// Gets or sets the per hour.
        /// </summary>
        /// <value>The per hour.</value>
        public long PerHour { get; set; }

        /// <summary>
        /// Gets or sets the per day.
        /// </summary>
        /// <value>The per day.</value>
        public long PerDay { get; set; }

        /// <summary>
        /// Gets or sets the per week.
        /// </summary>
        /// <value>The per week.</value>
        public long PerWeek { get; set; }

        /// <summary>
        /// Gets the limit.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <returns>System.Int64.</returns>
        public long GetLimit(RateLimitPeriod period)
        {
            switch (period)
            {
                case RateLimitPeriod.Second:
                    return PerSecond;
                case RateLimitPeriod.Minute:
                    return PerMinute;
                case RateLimitPeriod.Hour:
                    return PerHour;
                case RateLimitPeriod.Day:
                    return PerDay;
                case RateLimitPeriod.Week:
                    return PerWeek;
                default:
                    return PerSecond;
            }
        }
    }
}
