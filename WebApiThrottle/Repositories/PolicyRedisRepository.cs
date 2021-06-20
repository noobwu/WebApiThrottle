// ***********************************************************************
// Assembly         : WebApiThrottle
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2021-06-20
// ***********************************************************************
// <copyright file="PolicyRedisRepository.cs" company="stefanprodan.com">
//     Copyright Stefan Prodan © 2013-2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace WebApiThrottle
{
    /// <summary>
    /// Class PolicyRedisRepository.
    /// Implements the <see cref="WebApiThrottle.IPolicyRepository" />
    /// </summary>
    /// <seealso cref="WebApiThrottle.IPolicyRepository" />
    public class PolicyRedisRepository : RedisRespositoryBase, IPolicyRepository
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyRedisRepository" /> class.
        /// </summary>
        /// <param name="connectionMultiplexer">The connection multiplexer.</param>
        public PolicyRedisRepository(IConnectionMultiplexer connectionMultiplexer):base(connectionMultiplexer)
        { 
        }
        /// <summary>
        /// Firsts the or default.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ThrottlePolicy.</returns>
        public ThrottlePolicy FirstOrDefault(string id)
        {
            return Get<ThrottlePolicy>(GetKey(id));
        }

        /// <summary>
        /// Removes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Remove(string id)
        {
            GetDatabase().KeyDelete(GetKey(id));
        }

        /// <summary>
        /// Saves the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="policy">The policy.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Save(string id, ThrottlePolicy policy)
        {
            var database = GetDatabase();
            var value = JsonConvert.SerializeObject(policy);
             database.StringSet(GetKey(id), value, null, When.Always);
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>System.String.</returns>
        protected override string GetKey(string id)
        {
            return $"WebApiThrottle:Policy:{id}";
        }
    }
}
