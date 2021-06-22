using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiThrottle.WebApiDemo.Helpers
{
    public class RedisCacheClient
    {
        /// <summary>
        /// The atomic increment
        /// </summary>
        private readonly LuaScript _atomicIncrement = LuaScript.Prepare("local count count = redis.call(\"INCRBYFLOAT\", @key, tonumber(@delta)) if count == @delta then redis.call(\"EXPIRE\", @key, @timeout) end return count");
    }
}