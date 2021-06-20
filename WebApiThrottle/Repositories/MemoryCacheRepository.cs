// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="MemoryCacheRepository.cs" company="stefanprodan.com">
//     Copyright © Stefan Prodan 2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace WebApiThrottle
{
    /// <summary>
    /// Stors throttle metrics in runtime cache, intented for owin self host.
    /// Implements the <see cref="WebApiThrottle.IThrottleRepository" />
    /// </summary>
    /// <seealso cref="WebApiThrottle.IThrottleRepository" />
    public class MemoryCacheRepository : IThrottleRepository
    {
        /// <summary>
        /// The memory cache
        /// </summary>
        ObjectCache memCache = MemoryCache.Default;

        /// <summary>
        /// Insert or update
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="throttleCounter">The throttle counter.</param>
        /// <param name="expirationTime">The expiration time.</param>
        public void Save(string id, ThrottleCounter throttleCounter, TimeSpan expirationTime)
        {
            if (memCache[id] != null)
            {
                memCache[id] = throttleCounter;
            }
            else
            {
                memCache.Add(
                    id,
                    throttleCounter, new CacheItemPolicy()
                    {
                        SlidingExpiration = expirationTime
                    });
            }
        }

        /// <summary>
        /// Anies the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Any(string id)
        {
            return memCache[id] != null;
        }

        /// <summary>
        /// Firsts the or default.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>System.Nullable&lt;ThrottleCounter&gt;.</returns>
        public ThrottleCounter? FirstOrDefault(string id)
        {
            return (ThrottleCounter?)memCache[id];
        }

        /// <summary>
        /// Removes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void Remove(string id)
        {
            memCache.Remove(id);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            var cacheKeys = memCache.Where(kvp => kvp.Value is ThrottleCounter).Select(kvp => kvp.Key).ToList();
            foreach (string cacheKey in cacheKeys)
            {
                memCache.Remove(cacheKey);
            }
        }
    }
}
