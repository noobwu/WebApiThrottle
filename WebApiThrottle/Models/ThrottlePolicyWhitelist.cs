// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="ThrottlePolicyWhitelist.cs" company="stefanprodan.com">
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
    /// Class ThrottlePolicyWhitelist.
    /// </summary>
    [Serializable]
    public class ThrottlePolicyWhitelist
    {
        /// <summary>
        /// Gets or sets the entry.
        /// </summary>
        /// <value>The entry.</value>
        public string Entry { get; set; }

        /// <summary>
        /// Gets or sets the type of the policy.
        /// </summary>
        /// <value>The type of the policy.</value>
        public ThrottlePolicyType PolicyType { get; set; }
    }
}
