// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="IIpAddressParser.cs" company="stefanprodan.com">
//     Copyright © Stefan Prodan 2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace WebApiThrottle.Net
{
    /// <summary>
    /// Interface IIpAddressParser
    /// </summary>
    public interface IIpAddressParser
    {
        /// <summary>
        /// Determines whether the specified ip rules contains ip.
        /// </summary>
        /// <param name="ipRules">The ip rules.</param>
        /// <param name="clientIp">The client ip.</param>
        /// <returns><c>true</c> if the specified ip rules contains ip; otherwise, <c>false</c>.</returns>
        bool ContainsIp(List<string> ipRules, string clientIp);

        /// <summary>
        /// Determines whether the specified ip rules contains ip.
        /// </summary>
        /// <param name="ipRules">The ip rules.</param>
        /// <param name="clientIp">The client ip.</param>
        /// <param name="rule">The rule.</param>
        /// <returns><c>true</c> if the specified ip rules contains ip; otherwise, <c>false</c>.</returns>
        bool ContainsIp(List<string> ipRules, string clientIp, out string rule);

        /// <summary>
        /// Gets the client ip.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>IPAddress.</returns>
        IPAddress GetClientIp(HttpRequestMessage request);

        /// <summary>
        /// Parses the ip.
        /// </summary>
        /// <param name="ipAddress">The ip address.</param>
        /// <returns>IPAddress.</returns>
        IPAddress ParseIp(string ipAddress);
    }
}
