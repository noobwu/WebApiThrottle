// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="ThrottleLogEntry.cs" company="stefanprodan.com">
//     Copyright © Stefan Prodan 2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiThrottle
{
    /// <summary>
    /// Class ThrottleLogEntry.
    /// </summary>
    [Serializable]
    public class ThrottleLogEntry
    {
        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>The request identifier.</value>
        public string RequestId { get; set; }

        /// <summary>
        /// Gets or sets the client ip.
        /// </summary>
        /// <value>The client ip.</value>
        public string ClientIp { get; set; }

        /// <summary>
        /// Gets or sets the client key.
        /// </summary>
        /// <value>The client key.</value>
        public string ClientKey { get; set; }

        /// <summary>
        /// Gets or sets the endpoint.
        /// </summary>
        /// <value>The endpoint.</value>
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the total requests.
        /// </summary>
        /// <value>The total requests.</value>
        public long TotalRequests { get; set; }

        /// <summary>
        /// Gets or sets the start period.
        /// </summary>
        /// <value>The start period.</value>
        public DateTime StartPeriod { get; set; }

        /// <summary>
        /// Gets or sets the rate limit.
        /// </summary>
        /// <value>The rate limit.</value>
        public long RateLimit { get; set; }

        /// <summary>
        /// Gets or sets the rate limit period.
        /// </summary>
        /// <value>The rate limit period.</value>
        public string RateLimitPeriod { get; set; }

        /// <summary>
        /// Gets or sets the log date.
        /// </summary>
        /// <value>The log date.</value>
        public DateTime LogDate { get; set; }

        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        /// <value>The request.</value>
        public HttpRequestMessage Request { get; set; }
    }
}
