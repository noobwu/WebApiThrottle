// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="PolicyCacheRepository.cs" company="stefanprodan.com">
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
    /// Stores policy in asp.net cache
    /// Implements the <see cref="WebApiThrottle.IPolicyRepository" />
    /// </summary>
    /// <seealso cref="WebApiThrottle.IPolicyRepository" />
    public class PolicyCacheRepository : IPolicyRepository
    {
        /// <summary>
        /// Saves the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="policy">The policy.</param>
        public void Save(string id, ThrottlePolicy policy)
        {
            if (HttpContext.Current.Cache[id] != null)
            {
                HttpContext.Current.Cache[id] = policy;
            }
            else
            {
                HttpContext.Current.Cache.Add(
                    id,
                    policy,
                    null,
                    Cache.NoAbsoluteExpiration,
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.High,
                    null);
            }
        }

        /// <summary>
        /// Firsts the or default.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ThrottlePolicy.</returns>
        public ThrottlePolicy FirstOrDefault(string id)
        {
            var policy = (ThrottlePolicy)HttpContext.Current.Cache[id];
            return policy;
        }

        /// <summary>
        /// Removes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void Remove(string id)
        {
            HttpContext.Current.Cache.Remove(id);
        }
    }
}
