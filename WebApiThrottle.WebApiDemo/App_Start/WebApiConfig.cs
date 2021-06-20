using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Tracing;
using WebApiThrottle.WebApiDemo.Net;

namespace WebApiThrottle.WebApiDemo
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //trace provider
            var traceWriter = new SystemDiagnosticsTraceWriter()
            {
                IsVerbose = true
            };
            config.Services.Replace(typeof(ITraceWriter), traceWriter);
            config.EnableSystemDiagnosticsTracing();

            var muxer = GetConnectionMultiplexer("127.0.0.1:6379,password=,connectRetry=3,connectTimeout=15000,syncTimeout=15000,defaultDatabase=0,abortConnect=false");
            //IThrottleRepository throttleRepository = new CacheRepository();
            IThrottleRepository throttleRepository = new RedisRepository(muxer);

            //IPolicyRepository policyRepository = new PolicyCacheRepository();
            IPolicyRepository policyRepository = new PolicyRedisRepository(muxer);

            //Web API throttling handler
            config.MessageHandlers.Add(new ThrottlingHandler(
                policy: new ThrottlePolicy(perMinute: 20, perHour: 30, perDay: 35, perWeek: 3000)
                {
                    //scope to IPs
                    IpThrottling = true,
                    IpRules = new Dictionary<string, RateLimits>
                    {
                        { "::1/10", new RateLimits { PerSecond = 2 } },
                        { "192.168.2.1", new RateLimits { PerMinute = 30, PerHour = 30*60, PerDay = 30*60*24 } }
                    },
                    //white list the "::1" IP to disable throttling on localhost for Win8
                    //IpWhitelist = new List<string> { "127.0.0.1", "192.168.0.0/24" },

                    //scope to clients (if IP throttling is applied then the scope becomes a combination of IP and client key)
                    ClientThrottling = true,
                    ClientRules = new Dictionary<string, RateLimits>
                    {
                        { "api-client-key-1", new RateLimits { PerMinute = 60, PerHour = 600 } },
                        { "api-client-key-9", new RateLimits { PerDay = 5000 } }
                    },
                    //white list API keys that don’t require throttling
                    ClientWhitelist = new List<string> { "admin-key" },

                    //scope to endpoints
                    EndpointThrottling = true,
                    EndpointRules = new Dictionary<string, RateLimits>
                    {
                        { "api/search", new RateLimits { PerSecond = 10, PerMinute = 100, PerHour = 1000 } }
                    }
                },
                policyRepository: policyRepository,
                repository: throttleRepository,
                logger: new TracingThrottleLogger(traceWriter),
                ipAddressParser: new CustomIpAddressParser()));

            //Web API throttling handler load policy from web.config
            //config.MessageHandlers.Add(new ThrottlingHandler(
            //    policy: ThrottlePolicy.FromStore(new PolicyConfigurationProvider()),
            //    policyRepository: new PolicyCacheRepository(),
            //    repository: new CacheRepository(),
            //    logger: new TracingThrottleLogger(traceWriter)));

            //Web API throttling filter
            //config.Filters.Add(new ThrottlingFilter(
            //    policy: new ThrottlePolicy(perMinute: 20, perHour: 30, perDay: 35, perWeek: 3000)
            //    {
            //        //scope to IPs
            //        IpThrottling = true,
            //        IpRules = new Dictionary<string, RateLimits>
            //        { 
            //            { "::1/10", new RateLimits { PerSecond = 2 } },
            //            { "192.168.2.1", new RateLimits { PerMinute = 30, PerHour = 30*60, PerDay = 30*60*24 } }
            //        },
            //        //white list the "::1" IP to disable throttling on localhost for Win8
            //        IpWhitelist = new List<string> { "127.0.0.1", "192.168.0.0/24" },

            //        //scope to clients (if IP throttling is applied then the scope becomes a combination of IP and client key)
            //        ClientThrottling = true,
            //        ClientRules = new Dictionary<string, RateLimits>
            //        { 
            //            { "api-client-key-1", new RateLimits { PerMinute = 60, PerHour = 600 } },
            //            { "api-client-key-9", new RateLimits { PerDay = 5000 } }
            //        },
            //        //white list API keys that don’t require throttling
            //        ClientWhitelist = new List<string> { "admin-key" },

            //        //Endpoint rate limits will be loaded from EnableThrottling attribute
            //        EndpointThrottling = true
            //    },
            //    policyRepository: new PolicyCacheRepository(),
            //    repository: new CacheRepository(),
            //    logger: new TracingThrottleLogger(traceWriter)));

            //Web API throttling filter load policy from web.config
            //config.Filters.Add(new ThrottlingFilter(           
            //    policy: ThrottlePolicy.FromStore(new PolicyConfigurationProvider()),
            //    policyRepository: new PolicyCacheRepository(),
            //    repository: new CacheRepository(),
            //    logger: new TracingThrottleLogger(traceWriter)));
        }

        /// <summary>
        /// Gets the connection multiplexer.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="defaultDatabase">The default database.</param>
        /// <param name="clientName">Name of the client.</param>
        /// <param name="syncTimeout">The synchronize timeout.</param>
        /// <param name="allowAdmin">if set to <c>true</c> [allow admin].</param>
        /// <param name="keepAlive">The keep alive.</param>
        /// <param name="connectTimeout">The connect timeout.</param>
        /// <param name="password">The password.</param>
        /// <param name="tieBreaker">The tie breaker.</param>
        /// <param name="fail">if set to <c>true</c> [fail].</param>
        /// <param name="disabledCommands">The disabled commands.</param>
        /// <param name="enabledCommands">The enabled commands.</param>
        /// <param name="channelPrefix">The channel prefix.</param>
        /// <param name="proxy">The proxy.</param>
        /// <param name="caller">The caller.</param>
        /// <returns>ConnectionMultiplexer.</returns>
        /// <exception cref="TimeoutException">Connect timeout</exception>
        public static  ConnectionMultiplexer GetConnectionMultiplexer(
             string configuration = null, int? defaultDatabase = null,
        string clientName = null, int? syncTimeout = null, bool? allowAdmin = null, int? keepAlive = null,
        int? connectTimeout = null, string password = null, string tieBreaker = null,
        bool fail = true, string[] disabledCommands = null, string[] enabledCommands = null,
        string channelPrefix = null, Proxy? proxy = null,

        [CallerMemberName] string caller = null)
        {
            var config = ConfigurationOptions.Parse(configuration);
            if (disabledCommands != null && disabledCommands.Length != 0)
            {
                config.CommandMap = CommandMap.Create(new HashSet<string>(disabledCommands), false);
            }
            else if (enabledCommands != null && enabledCommands.Length != 0)
            {
                config.CommandMap = CommandMap.Create(new HashSet<string>(enabledCommands), true);
            }

            if (channelPrefix != null) config.ChannelPrefix = channelPrefix;
            if (tieBreaker != null) config.TieBreaker = tieBreaker;
            if (password != null) config.Password = string.IsNullOrEmpty(password) ? null : password;
            if (clientName != null) config.ClientName = clientName;
            else if (caller != null) config.ClientName = caller;
            if (syncTimeout != null) config.SyncTimeout = syncTimeout.Value;
            if (allowAdmin != null) config.AllowAdmin = allowAdmin.Value;
            if (keepAlive != null) config.KeepAlive = keepAlive.Value;
            if (connectTimeout != null) config.ConnectTimeout = connectTimeout.Value;
            if (proxy != null) config.Proxy = proxy.Value;
            if (defaultDatabase != null) config.DefaultDatabase = defaultDatabase.Value;
            var watch = Stopwatch.StartNew();
            var task = ConnectionMultiplexer.ConnectAsync(config);
            if (!task.Wait(config.ConnectTimeout >= (int.MaxValue / 2) ? int.MaxValue : config.ConnectTimeout * 2))
            {
                task.ContinueWith(x =>
                {
                    try
                    {
                        GC.KeepAlive(x.Exception);
                    }
                    catch { /* No boom */ }
                }, TaskContinuationOptions.OnlyOnFaulted);
                throw new TimeoutException("Connect timeout");
            }
            watch.Stop();
            var muxer = task.Result;
            return muxer;
        }
    }
}
