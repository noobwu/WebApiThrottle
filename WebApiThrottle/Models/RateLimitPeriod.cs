// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2021-05-16
// ***********************************************************************
// <copyright file="RateLimitPeriod.cs" company="stefanprodan.com">
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
    /// Enum RateLimitPeriod
    /// </summary>
    public enum RateLimitPeriod
    {
        /// <summary>
        /// The second
        /// </summary>
        Second = 1,
        /// <summary>
        /// The minute
        /// </summary>
        Minute,
        /// <summary>
        /// The hour
        /// </summary>
        Hour,
        /// <summary>
        /// The day
        /// </summary>
        Day,
        /// <summary>
        /// The week
        /// </summary>
        Week
    }
}
