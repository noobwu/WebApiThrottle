using System;
using System.Collections.Generic;

namespace WebApiThrottle.WebApiDemo.Models
{
    /// <summary>
    /// 限流设置
    /// </summary>
    public class PolicySettingModel
    {
        /// <summary>
        /// 是否启用Ip地址限流
        /// </summary>
        public bool? IpThrottling { get; set; }

        /// <summary>
        /// 是否启用客户端Key限流
        /// </summary>
        public bool? ClientThrottling { get; set; }

        /// <summary>
        ///  是否启用Url地址限流
        /// </summary>
        public bool? EndpointThrottling { get; set; }

        /// <summary>
        /// 获取或设置一个值，表示所有的请求，包括被拒绝的请求，是否应该按照这个顺序堆叠：日、小时、分钟、秒。
        /// </summary>
        public bool? StackBlockedRequests { get; set; }

        /// <summary>
        /// Gets or sets the rate limit.
        /// </summary>
        /// <value>The rate limit.</value>
        public RateLimit Limit { get; set; }
        /// <summary>
        /// Class RateLimit.
        /// </summary>
        public class RateLimit
        {
            /// <summary>
            /// 每秒钟最大请求数
            /// </summary>
            /// <value>The per second.</value>
            public long? PerSecond { get; set; }

            /// <summary>
            /// 每分钟最大请求数
            /// </summary>
            /// <value>The per minute.</value>
            public long? PerMinute { get; set; }

            /// <summary>
            /// 每小时最大请求数
            /// </summary>
            /// <value>The per hour.</value>
            public long? PerHour { get; set; }

            /// <summary>
            /// 每天最大请求数
            /// </summary>
            /// <value>The per day.</value>
            public long? PerDay { get; set; }

            /// <summary>
            /// 每周最大请求数
            /// </summary>
            /// <value>The per week.</value>
            public long? PerWeek { get; set; }
        }
    }

    /// <summary>
    ///  限流规则
    /// </summary>
    public class PolicyRuleModel : PolicyModelBase
    {
        /// <summary>
        /// 规则
        /// </summary>
        /// <value>The ip rules.</value>
        public IEnumerable<ThrottlePolicyRule> Rules { get; set; }

    }

    /// <summary>
    ///  白名单
    /// </summary>
    public class PolicyWhitelistModel : PolicyModelBase
    {
        /// <summary>
        /// 白名单
        /// </summary>
        /// <value>The ip whitelist.</value>
        public IEnumerable<string> Whitelists { get; set; }
    }

    /// <summary>
    /// Class ClearPolicyModel.
    /// </summary>
    public class ClearPolicyModel : PolicyModelBase
    {

    }
    /// <summary>
    /// Class DeletePolicyModel.
    /// </summary>
    public class DeletePolicyModel : PolicyModelBase
    {
        /// <summary>
        /// 规则(Ip地址|客户端Key|Url地址)
        /// </summary>
        /// <value>The entry.</value>
        public IEnumerable<string> Entities { get; set; }
    }
    /// <summary>
    /// Class PolicyModelBase.
    /// </summary>
    public class PolicyModelBase
    {
        /// <summary>
        /// 类型(1:IP,2:客户端Key,3:Url地址)
        /// </summary>
        /// <value>The type of the policy.</value>
        public ThrottlePolicyType PolicyType { get; set; }
    }

}