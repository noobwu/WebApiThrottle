// ***********************************************************************
// Assembly         : WebApiThrottle
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2021-06-20
// ***********************************************************************
// <copyright file="RedisRepository.cs" company="stefanprodan.com">
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
    /// Class RedisRepository.
    /// Implements the <see cref="WebApiThrottle.IThrottleRepository" />
    /// </summary>
    /// <seealso cref="WebApiThrottle.IThrottleRepository" />
    public class RedisRepository : RedisRespositoryBase, IThrottleRepository
    {
        /// <summary>
        /// The atomic increment
        /// </summary>
        private readonly LuaScript _atomicIncrement = LuaScript.Prepare("local count count = redis.call(\"INCRBYFLOAT\", @key, tonumber(@delta)) if count == @delta then redis.call(\"EXPIRE\", @key, @timeout) end return count");

        /// <summary>
        /// The wild card keys
        /// </summary>
        private readonly LuaScript _wildCardKeys = LuaScript.Prepare(" local res = redis.call(\"KEYS\",@keys) return res ");

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisRepository"/> class.
        /// </summary>
        /// <param name="connectionMultiplexer">The connection multiplexer.</param>
        public RedisRepository(IConnectionMultiplexer connectionMultiplexer):base(connectionMultiplexer)
        {
        }
        /// <summary>
        /// Anies the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Any(string id)
        {
            return Get<ThrottleCounter>(GetKey(id)).Equals(default(ThrottleCounter));
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Clear()
        {
            var database = GetDatabase();
            RedisResult deleteKeys = database.ScriptEvaluate(_wildCardKeys, new { keys = "WebApiThrottle:" });
            if (!deleteKeys.IsNull)
            {
                database.KeyDelete((RedisKey[])deleteKeys); //删除一组key
            }
        }

        /// <summary>
        /// Firsts the or default.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>System.Nullable&lt;ThrottleCounter&gt;.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public ThrottleCounter? FirstOrDefault(string id)
        {
            return Get<ThrottleCounter>(GetKey(id));
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
        /// <param name="throttleCounter">The throttle counter.</param>
        /// <param name="expirationTime">The expiration time.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Save(string id, ThrottleCounter throttleCounter, TimeSpan expirationTime)
        {
            GetDatabase().ScriptEvaluate(_atomicIncrement, new { key = GetKey(id), timeout = expirationTime.TotalSeconds, delta = throttleCounter.TotalRequests });
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>System.String.</returns>
        protected override string GetKey(string id)
        {
            return $"WebApiThrottle:Counter:{id}";
        }
    }
}
