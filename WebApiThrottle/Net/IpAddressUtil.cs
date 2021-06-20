// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="IpAddressUtil.cs" company="stefanprodan.com">
//     Copyright © Stefan Prodan 2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace WebApiThrottle.Net
{
    /// <summary>
    /// Class IpAddressUtil.
    /// </summary>
    public class IpAddressUtil
    {
        /// <summary>
        /// Determines whether the specified ip rules contains ip.
        /// </summary>
        /// <param name="ipRules">The ip rules.</param>
        /// <param name="clientIp">The client ip.</param>
        /// <returns><c>true</c> if the specified ip rules contains ip; otherwise, <c>false</c>.</returns>
        public static bool ContainsIp(List<string> ipRules, string clientIp)
        {
            var ip = ParseIp(clientIp);
            if (ipRules != null && ipRules.Any())
            {
                foreach (var rule in ipRules)
                {
                    var range = new IPAddressRange(rule);
                    if (range.Contains(ip))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified ip rules contains ip.
        /// </summary>
        /// <param name="ipRules">The ip rules.</param>
        /// <param name="clientIp">The client ip.</param>
        /// <param name="rule">The rule.</param>
        /// <returns><c>true</c> if the specified ip rules contains ip; otherwise, <c>false</c>.</returns>
        public static bool ContainsIp(List<string> ipRules, string clientIp, out string rule)
        {
            rule = null;
            var ip = ParseIp(clientIp);
            if (ipRules != null && ipRules.Any())
            {
                foreach (var r in ipRules)
                {
                    var range = new IPAddressRange(r);
                    if (range.Contains(ip))
                    {
                        rule = r;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Parses the ip.
        /// </summary>
        /// <param name="ipAddress">The ip address.</param>
        /// <returns>IPAddress.</returns>
        public static IPAddress ParseIp(string ipAddress)
        {
            ipAddress = ipAddress.Trim();
            int portDelimiterPos = ipAddress.LastIndexOf(":", StringComparison.InvariantCultureIgnoreCase);
            bool ipv6WithPortStart = ipAddress.StartsWith("[");
            int ipv6End = ipAddress.IndexOf("]");
            if (portDelimiterPos != -1
                && portDelimiterPos == ipAddress.IndexOf(":", StringComparison.InvariantCultureIgnoreCase)
                || ipv6WithPortStart && ipv6End != -1 && ipv6End < portDelimiterPos)
            {
                ipAddress = ipAddress.Substring(0, portDelimiterPos);
            }
            
            return IPAddress.Parse(ipAddress);
        }

        /// <summary>
        /// Determines whether [is private ip address] [the specified ip address].
        /// </summary>
        /// <param name="ipAddress">The ip address.</param>
        /// <returns><c>true</c> if [is private ip address] [the specified ip address]; otherwise, <c>false</c>.</returns>
        public static bool IsPrivateIpAddress(string ipAddress)
        {
            // http://en.wikipedia.org/wiki/Private_network
            // Private IP Addresses are: 
            //  24-bit block: 10.0.0.0 through 10.255.255.255
            //  20-bit block: 172.16.0.0 through 172.31.255.255
            //  16-bit block: 192.168.0.0 through 192.168.255.255
            //  Link-local addresses: 169.254.0.0 through 169.254.255.255 (http://en.wikipedia.org/wiki/Link-local_address)

            var ip = ParseIp(ipAddress);
            var octets = ip.GetAddressBytes();

            bool isIpv6 = octets.Length == 16;

            if (isIpv6)
            {
                bool isUniqueLocalAddress = octets[0] == 253;
                return isUniqueLocalAddress;
            }
            else
            {
                var is24BitBlock = octets[0] == 10;
                if (is24BitBlock) return true; // Return to prevent further processing

                var is20BitBlock = octets[0] == 172 && octets[1] >= 16 && octets[1] <= 31;
                if (is20BitBlock) return true; // Return to prevent further processing

                var is16BitBlock = octets[0] == 192 && octets[1] == 168;
                if (is16BitBlock) return true; // Return to prevent further processing

                var isLinkLocalAddress = octets[0] == 169 && octets[1] == 254;
                return isLinkLocalAddress;
            }
        }
    }
}