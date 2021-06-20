// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="ThrottleManager.cs" company="stefanprodan.com">
//     Copyright © Stefan Prodan 2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiThrottle
{
    /// <summary>
    /// Allows changing the cache keys prefix and suffix, exposes ways to refresh the policy object at runtime.
    /// </summary>
    public static class ThrottleManager
    {
        /// <summary>
        /// The application name
        /// </summary>
        private static string applicationName = string.Empty;

        /// <summary>
        /// The throttle key
        /// </summary>
        private static string throttleKey = "throttle";

        /// <summary>
        /// The policy key
        /// </summary>
        private static string policyKey = "throttle_policy";

        /// <summary>
        /// Gets or sets the global prefix
        /// </summary>
        /// <value>The name of the application.</value>
        public static string ApplicationName
        {
            get
            {
                return applicationName;
            }

            set
            {
                applicationName = value;
            }
        }

        /// <summary>
        /// Gets or sets the key prefix for rate limits
        /// </summary>
        /// <value>The throttle key.</value>
        public static string ThrottleKey
        {
            get
            {
                return throttleKey;
            }

            set
            {
                throttleKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the policy key suffix
        /// </summary>
        /// <value>The policy key.</value>
        public static string PolicyKey
        {
            get
            {
                return policyKey;
            }

            set
            {
                policyKey = value;
            }
        }

        /// <summary>
        /// Returns key prefix for rate limits
        /// </summary>
        /// <returns>The throttle key.</returns>
        public static string GetThrottleKey()
        {
            return ApplicationName + ThrottleKey;
        }

        /// <summary>
        /// Returns the policy key (global prefix + policy key suffix)
        /// </summary>
        /// <returns>The policy key.</returns>
        public static string GetPolicyKey()
        {
            return ApplicationName + PolicyKey;
        }

        /// <summary>
        /// Updates the policy object cached value
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="cacheRepository">The policy repository.</param>
        public static void UpdatePolicy(ThrottlePolicy policy, IPolicyRepository cacheRepository)
        {
            cacheRepository.Save(GetPolicyKey(), policy);
        }

        /// <summary>
        /// Reads the policy object from store and updates the cache
        /// </summary>
        /// <param name="storeProvider">The store provider.</param>
        /// <param name="cacheRepository">The cache repository.</param>
        public static void UpdatePolicy(IThrottlePolicyProvider storeProvider, IPolicyRepository cacheRepository)
        {
            var policy = ThrottlePolicy.FromStore(storeProvider);
            cacheRepository.Save(GetPolicyKey(), policy);
        }
    }
}
