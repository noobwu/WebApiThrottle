// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="ThrottlePolicyType.cs" company="stefanprodan.com">
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
    /// Enum ThrottlePolicyType
    /// </summary>
    public enum ThrottlePolicyType : int
    {
        /// <summary>
        /// The ip throttling
        /// </summary>
        IpThrottling = 1,
        /// <summary>
        /// The client throttling
        /// </summary>
        ClientThrottling,
        /// <summary>
        /// The endpoint throttling
        /// </summary>
        EndpointThrottling
    }
}
