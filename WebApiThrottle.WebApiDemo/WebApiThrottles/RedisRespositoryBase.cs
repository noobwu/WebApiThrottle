// ***********************************************************************
// Assembly         : WebApiThrottle
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2021-06-20
// ***********************************************************************
// <copyright file="RedisRespositoryBase.cs" company="stefanprodan.com">
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
    /// Class RedisRespositoryBase.
    /// </summary>
    public class RedisRespositoryBase
    {
        /// <summary>
        /// The database index
        /// </summary>
        private readonly int _dbIndex = 0;
        /// <summary>
        /// The connection multiplexer
        /// </summary>
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        /// <summary>
        /// Initializes a new instance of the <see cref="RedisRepository" /> class.
        /// </summary>
        /// <param name="connectionMultiplexer">The connection multiplexer.</param>
        /// <exception cref="ArgumentException">IConnectionMultiplexer was null. Ensure StackExchange.Redis was successfully registered</exception>
        public RedisRespositoryBase(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentException("IConnectionMultiplexer was null. Ensure StackExchange.Redis was successfully registered");
        }
        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetKey(string id)
        {
            return $"WebApiThrottle:{id}";
        }
        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <returns>StackExchange.Redis.IDatabase.</returns>
        protected IDatabase GetDatabase()
        {
            return _connectionMultiplexer.GetDatabase(_dbIndex);
        }
        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns>T.</returns>
        /// <exception cref="ArgumentNullException">key</exception>
        protected T Get<T>(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            RedisValue value = RedisValue.Null;
            try
            {
                var database = GetDatabase();
                value = database.StringGet(key);
                if (value.IsNull)
                {
                    return default(T);
                }
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch
            {
                return default(T);
            }
        }
        /// <summary>
        /// Gets the counter.
        /// </summary>
        /// <typeparam name="">The type of the .</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>T.</returns>
        /// <exception cref="ArgumentNullException">key</exception>
        public long? GetCounter(string key)
        {
            if (key == null) throw new ArgumentNullException("key");
            var database = GetDatabase();
            var redisValue = database.StringGet(key);
            long counter = 0;
            if (long.TryParse(redisValue, out counter))
            {
                return counter;
            }
            return null;
        }
        /// <summary>
        /// Gets the key expire time.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.Nullable&lt;DateTime&gt;.</returns>
        /// <exception cref="ArgumentNullException">key</exception>
        public DateTime? GetKeyExpireTime(string key)
        {
            if (key == null) throw new ArgumentNullException("key");
            var database = GetDatabase();
            var tsExpire = database.KeyTimeToLive(key);
            if (tsExpire == null)
            {
                return null;
            }
            var now = DateTime.UtcNow;
            var numberOfIntervals = now.Ticks / tsExpire.Value.Ticks;
            return new DateTime(numberOfIntervals * tsExpire.Value.Ticks, DateTimeKind.Utc);
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
