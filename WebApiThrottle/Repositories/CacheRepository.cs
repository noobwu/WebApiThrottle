// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="CacheRepository.cs" company="stefanprodan.com">
//     Copyright © Stefan Prodan 2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace WebApiThrottle
{
    /// <summary>
    /// Stores throttle metrics in asp.net cache
    /// Implements the <see cref="WebApiThrottle.IThrottleRepository" />
    /// </summary>
    /// <seealso cref="WebApiThrottle.IThrottleRepository" />
    public class CacheRepository : IThrottleRepository
    {
        /// <summary>
        /// Insert or update
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="throttleCounter">The throttle Counter.</param>
        /// <param name="expirationTime">The expiration Time.</param>
        public void Save(string id, ThrottleCounter throttleCounter, TimeSpan expirationTime)
        {
            if (HttpContext.Current.Cache[id] != null)
            {
                HttpContext.Current.Cache[id] = throttleCounter;
            }
            else
            {
                HttpContext.Current.Cache.Add(
                    id,
                    throttleCounter,
                    null,
                    Cache.NoAbsoluteExpiration,
                    expirationTime,
                    CacheItemPriority.Low,
                    null);
            }
        }

        /// <summary>
        /// Anies the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Any(string id)
        {
            return HttpContext.Current.Cache[id] != null;
        }

        /// <summary>
        /// Firsts the or default.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>System.Nullable&lt;ThrottleCounter&gt;.</returns>
        public ThrottleCounter? FirstOrDefault(string id)
        {
            return (ThrottleCounter?)HttpContext.Current.Cache[id];
        }

        /// <summary>
        /// Removes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void Remove(string id)
        {
            HttpContext.Current.Cache.Remove(id);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            var cacheEnumerator = HttpContext.Current.Cache.GetEnumerator();
            while (cacheEnumerator.MoveNext())
            {
                if (cacheEnumerator.Value is ThrottleCounter)
                {
                    HttpContext.Current.Cache.Remove(cacheEnumerator.Key.ToString());
                }
            }
        }
    }
}
