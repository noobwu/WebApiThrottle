// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="TracingThrottleLogger.cs" company="stefanprodan.com">
//     Copyright © Stefan Prodan 2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Web.Http.Tracing;

namespace WebApiThrottle
{
    /// <summary>
    /// Class TracingThrottleLogger.
    /// Implements the <see cref="WebApiThrottle.IThrottleLogger" />
    /// </summary>
    /// <seealso cref="WebApiThrottle.IThrottleLogger" />
    public class TracingThrottleLogger : IThrottleLogger
    {
        /// <summary>
        /// The trace writer
        /// </summary>
        private readonly ITraceWriter traceWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TracingThrottleLogger"/> class.
        /// </summary>
        /// <param name="traceWriter">The trace writer.</param>
        public TracingThrottleLogger(ITraceWriter traceWriter)
        {
            this.traceWriter = traceWriter;
        }

        /// <summary>
        /// Logs the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public void Log(ThrottleLogEntry entry)
        {
            if (null != traceWriter)
            {
                traceWriter.Info(
                    entry.Request, 
                    "WebApiThrottle",
                    "{0} Request {1} from {2} has been throttled (blocked), quota {3}/{4} exceeded by {5}",
                    entry.LogDate,
                    entry.RequestId,
                    entry.ClientIp,
                    entry.RateLimit,
                    entry.RateLimitPeriod,
                    entry.TotalRequests);
            }
        }
    }
}