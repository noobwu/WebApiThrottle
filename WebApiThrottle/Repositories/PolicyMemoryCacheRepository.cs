// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="PolicyMemoryCacheRepository.cs" company="stefanprodan.com">
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
    /// Stores policy in runtime cache, intended for OWIN self host.
    /// Implements the <see cref="WebApiThrottle.IPolicyRepository" />
    /// </summary>
    /// <seealso cref="WebApiThrottle.IPolicyRepository" />
    public class PolicyMemoryCacheRepository : IPolicyRepository
    {
        /// <summary>
        /// The memory cache
        /// </summary>
        private ObjectCache memCache = MemoryCache.Default;

        /// <summary>
        /// Saves the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="policy">The policy.</param>
        public void Save(string id, ThrottlePolicy policy)
        {
            if (memCache[id] != null)
            {
                memCache[id] = policy;
            }
            else
            {
                memCache.Add(
                    id,
                    policy, 
                    new CacheItemPolicy());
            }
        }

        /// <summary>
        /// Firsts the or default.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ThrottlePolicy.</returns>
        public ThrottlePolicy FirstOrDefault(string id)
        {
            var policy = (ThrottlePolicy)memCache[id];
            return policy;
        }

        /// <summary>
        /// Removes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void Remove(string id)
        {
            memCache.Remove(id);
        }
    }
}
