// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="DefaultIpAddressParser.cs" company="stefanprodan.com">
//     Copyright © Stefan Prodan 2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;

namespace WebApiThrottle.Net
{
    /// <summary>
    /// Class DefaultIpAddressParser.
    /// Implements the <see cref="WebApiThrottle.Net.IIpAddressParser" />
    /// </summary>
    /// <seealso cref="WebApiThrottle.Net.IIpAddressParser" />
    public class DefaultIpAddressParser : IIpAddressParser
    {
        /// <summary>
        /// Determines whether the specified ip rules contains ip.
        /// </summary>
        /// <param name="ipRules">The ip rules.</param>
        /// <param name="clientIp">The client ip.</param>
        /// <returns><c>true</c> if the specified ip rules contains ip; otherwise, <c>false</c>.</returns>
        public bool ContainsIp(List<string> ipRules, string clientIp)
        {
            return IpAddressUtil.ContainsIp(ipRules, clientIp);
        }

        /// <summary>
        /// Determines whether the specified ip rules contains ip.
        /// </summary>
        /// <param name="ipRules">The ip rules.</param>
        /// <param name="clientIp">The client ip.</param>
        /// <param name="rule">The rule.</param>
        /// <returns><c>true</c> if the specified ip rules contains ip; otherwise, <c>false</c>.</returns>
        public bool ContainsIp(List<string> ipRules, string clientIp, out string rule)
        {
            return IpAddressUtil.ContainsIp(ipRules, clientIp, out rule);
        }

        /// <summary>
        /// Gets the client ip.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>IPAddress.</returns>
        public virtual IPAddress GetClientIp(HttpRequestMessage request)
        {
            return ParseIp(request.GetClientIpAddress());
        }

        /// <summary>
        /// Parses the ip.
        /// </summary>
        /// <param name="ipAddress">The ip address.</param>
        /// <returns>IPAddress.</returns>
        public IPAddress ParseIp(string ipAddress)
        {
            return IpAddressUtil.ParseIp(ipAddress);
        }

    }
}
