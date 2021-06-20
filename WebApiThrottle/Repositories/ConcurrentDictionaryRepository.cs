// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="ConcurrentDictionaryRepository.cs" company="stefanprodan.com">
//     Copyright © Stefan Prodan 2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiThrottle
{
    /// <summary>
    /// Stores throttle metrics in a thread safe dictionary, has no clean-up mechanism, expired counters are deleted on renewal
    /// Implements the <see cref="WebApiThrottle.IThrottleRepository" />
    /// </summary>
    /// <seealso cref="WebApiThrottle.IThrottleRepository" />
    public class ConcurrentDictionaryRepository : IThrottleRepository
    {
        /// <summary>
        /// The cache
        /// </summary>
        private static ConcurrentDictionary<string, ThrottleCounterWrapper> cache = new ConcurrentDictionary<string, ThrottleCounterWrapper>();

        /// <summary>
        /// Anies the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Any(string id)
        {
            return cache.ContainsKey(id);
        }

        /// <summary>
        /// Insert or update
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="ThrottleCounter" />.</returns>
        public ThrottleCounter? FirstOrDefault(string id)
        {
            var entry = new ThrottleCounterWrapper();

            if (cache.TryGetValue(id, out entry))
            {
                // remove expired entry
                if (entry.Timestamp + entry.ExpirationTime < DateTime.UtcNow)
                {
                    cache.TryRemove(id, out entry);
                    return null;
                }
            }

            return new ThrottleCounter
            {
                Timestamp = entry.Timestamp,
                TotalRequests = entry.TotalRequests
            };
        }

        /// <summary>
        /// Saves the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="throttleCounter">The throttle counter.</param>
        /// <param name="expirationTime">The expiration time.</param>
        public void Save(string id, ThrottleCounter throttleCounter, TimeSpan expirationTime)
        {
            var entry = new ThrottleCounterWrapper
            {
                ExpirationTime = expirationTime,
                Timestamp = throttleCounter.Timestamp,
                TotalRequests = throttleCounter.TotalRequests
            };

            cache.AddOrUpdate(id, entry, (k, e) => entry);
        }

        /// <summary>
        /// Removes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void Remove(string id)
        {
            var entry = new ThrottleCounterWrapper();
            cache.TryRemove(id, out entry);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            cache.Clear();
        }

        /// <summary>
        /// Struct ThrottleCounterWrapper
        /// </summary>
        [Serializable]
        internal struct ThrottleCounterWrapper
        {
            /// <summary>
            /// Gets or sets the timestamp.
            /// </summary>
            /// <value>The timestamp.</value>
            public DateTime Timestamp { get; set; }

            /// <summary>
            /// Gets or sets the total requests.
            /// </summary>
            /// <value>The total requests.</value>
            public long TotalRequests { get; set; }

            /// <summary>
            /// Gets or sets the expiration time.
            /// </summary>
            /// <value>The expiration time.</value>
            public TimeSpan ExpirationTime { get; set; }
        }
    }
}
