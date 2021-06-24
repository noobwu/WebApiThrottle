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
        public RedisRepository(IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer)
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
            var key = GetKey(id);
            var counter = GetCounter(key);
            if (counter == null)
            {
                return null;
            }
            DateTime timestamp = DateTime.MaxValue;
            var expireTime = GetKeyExpireTime(key);
            if (expireTime.HasValue)
            {
                timestamp = expireTime.Value;
            }
            return new ThrottleCounter() { TotalRequests = counter.Value, Timestamp = timestamp };
            //return Get<ThrottleCounter?>(GetKey(id));
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
            //var now = DateTime.UtcNow;
            //var numberOfIntervals = now.Ticks / expirationTime.Ticks;
            //var intervalStart = new DateTime(numberOfIntervals * expirationTime.Ticks, DateTimeKind.Utc);
            var count = GetDatabase().ScriptEvaluate(_atomicIncrement, new { key = GetKey(id), timeout = expirationTime.TotalSeconds, delta = 1 });
            //GetDatabase().StringSet(GetKey(id), throttleCounter.TotalRequests, expirationTime);
            //StringSet(GetKey(id), throttleCounter, expirationTime);
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

        /// <summary>
        /// 设置缓存项(服务器上key存在就替换,不存在就添加)
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="value">缓存值</param>
        /// <param name="expiry">缓存过期时间(单位:分钟)；如果为空表示不设过期时间</param>
        /// <returns>是否设置成功</returns>
        public bool StringSet(string key, object value, TimeSpan? expiry)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            var database = GetDatabase();
            string valueAsString = JsonConvert.SerializeObject(value);
            return database.StringSet(key, valueAsString, expiry, When.Always);
        }
    }
}
