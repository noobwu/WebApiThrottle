// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="IThrottlePolicyProvider.cs" company="stefanprodan.com">
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
    /// Implement this interface if you want to load the policy rules from a persistent store
    /// </summary>
    public interface IThrottlePolicyProvider
    {
        /// <summary>
        /// Reads the settings.
        /// </summary>
        /// <returns>ThrottlePolicySettings.</returns>
        ThrottlePolicySettings ReadSettings();

        /// <summary>
        /// Alls the rules.
        /// </summary>
        /// <returns>IEnumerable&lt;ThrottlePolicyRule&gt;.</returns>
        IEnumerable<ThrottlePolicyRule> AllRules();

        /// <summary>
        /// Alls the whitelists.
        /// </summary>
        /// <returns>IEnumerable&lt;ThrottlePolicyWhitelist&gt;.</returns>
        IEnumerable<ThrottlePolicyWhitelist> AllWhitelists();
    }
}
