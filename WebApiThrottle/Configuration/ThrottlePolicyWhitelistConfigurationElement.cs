// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="ThrottlePolicyWhitelistConfigurationElement.cs" company="stefanprodan.com">
//     Copyright © Stefan Prodan 2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiThrottle
{
    /// <summary>
    /// Class ThrottlePolicyWhitelistConfigurationElement.
    /// Implements the <see cref="System.Configuration.ConfigurationElement" />
    /// </summary>
    /// <seealso cref="System.Configuration.ConfigurationElement" />
    public class ThrottlePolicyWhitelistConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// Gets the entry.
        /// </summary>
        /// <value>The entry.</value>
        [ConfigurationProperty("entry", IsRequired = true)]
        public string Entry
        {
            get
            {
                return this["entry"] as string;
            }
        }

        /// <summary>
        /// Gets the type of the policy.
        /// </summary>
        /// <value>The type of the policy.</value>
        [ConfigurationProperty("policyType", IsRequired = true)]
        public int PolicyType
        {
            get
            {
                return (int)this["policyType"];
            }
        }
    }
}
