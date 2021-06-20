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
        /// Gets the database.
        /// </summary>
        /// <returns>StackExchange.Redis.IDatabase.</returns>
        protected IDatabase GetDatabase()
        {
            return _connectionMultiplexer.GetDatabase(_dbIndex);
        }
    }
}
