using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Tracing;
using WebApiThrottle.WebApiDemo.Extensions;
using WebApiThrottle.WebApiDemo.Infrastructure;
using WebApiThrottle.WebApiDemo.Net;

namespace WebApiThrottle.WebApiDemo
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();
            
            /*
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            */

            config.Routes.MapHttpRoute(
                 name: "DefaultApi",
                 routeTemplate: "api/{controller}/{action}/{id}",
                 defaults: new { id = RouteParameter.Optional }
            );

            // 将 SerializerSettings 重置为默认值 IgnoreSerializableAttribute = true
            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Converters = { new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" } }
            };
            UsingThrottle(config);

        }
        /// <summary>
        /// Usings the throttle.
        /// </summary>
        /// <param name="config">The configuration.</param>
        private static void UsingThrottle(HttpConfiguration config)
        {
            //trace provider
            var traceWriter = new SystemDiagnosticsTraceWriter()
            {
                IsVerbose = true
            };
            config.Services.Replace(typeof(ITraceWriter), traceWriter);
            config.EnableSystemDiagnosticsTracing();

            var muxer = GetConnectionMultiplexer("127.0.0.1:6379,password=,connectRetry=3,connectTimeout=15000,syncTimeout=15000,defaultDatabase=0,abortConnect=false");
            Singleton<IThrottleRepository>.Instance = new RedisRepository(muxer);

            var policyRepository = new PolicyRedisRepository(muxer);
            Singleton<IPolicyRepository>.Instance = policyRepository;
            Singleton<ThrottlePolicy>.Instance = GetThrottlePolicy(policyRepository);


            config.Services.Replace(typeof(ITraceWriter), traceWriter);
            config.EnableSystemDiagnosticsTracing();
            IThrottleLogger logger = new TracingThrottleLogger(traceWriter);

            //Web API throttling handler load policy from web.config
            config.MessageHandlers.Add(new ThrottlingHandler(
                policy: Singleton<ThrottlePolicy>.Instance,
                policyRepository: Singleton<IPolicyRepository>.Instance,
                repository: Singleton<IThrottleRepository>.Instance,
                logger: logger
                )
            {
                QuotaExceededContent = (rateLimit, period) => QuotaExceededContent(rateLimit, period),
                QuotaExceededResponseCode = HttpStatusCode.OK
            });


        }
        /// <summary>
        /// Gets the throttle policy.
        /// </summary>
        /// <param name="policyRepository">The policy repository.</param>
        /// <returns>ThrottlePolicy.</returns>
        private static ThrottlePolicy GetThrottlePolicy(IPolicyRepository policyRepository)
        {
            var policy = policyRepository?.FirstOrDefault(ThrottleManager.GetPolicyKey());
            if (policy == null)
            {
                policy = new ThrottlePolicy
                {
                    IpThrottling = true,     //是否IP地址限流
                    ClientThrottling = true, //是否客户端Key限流
                    EndpointThrottling = true,  //是否启用Url限流
                    StackBlockedRequests = false, //获取或设置一个值，表示所有的请求，包括被拒绝的请求，是否应该按照这个顺序堆叠：日、小时、分钟、秒。
                };
                //policy=ThrottlePolicy.FromStore(new PolicyConfigurationProvider());
            }
            return policy;
        }

        /// <summary>
        /// Quotas the content of the exceeded.
        /// </summary>
        /// <param name="rateLimit">The rate limit.</param>
        /// <param name="period">The period.</param>
        /// <returns>System.Object.</returns>
        private static object QuotaExceededContent(long rateLimit, RateLimitPeriod period)
        {
            Dictionary<RateLimitPeriod, string> dicPeriods = new Dictionary<RateLimitPeriod, string>() {
                { RateLimitPeriod.Second,$"每秒" }, { RateLimitPeriod.Minute,$"每分" }, { RateLimitPeriod.Hour,$"每时" }, { RateLimitPeriod.Day,$"每天" }
            };
            string periodText = dicPeriods.GetValue(period, () => { return "当前时间内"; });
            return new { code = "429", msg = $"你调用接口太频繁，{periodText}只允许调用{rateLimit}次，请稍后再试。" };
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
        public static ConnectionMultiplexer GetConnectionMultiplexer(
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
