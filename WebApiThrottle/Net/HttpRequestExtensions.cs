﻿// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="HttpRequestExtensions.cs" company="stefanprodan.com">
//     Copyright © Stefan Prodan 2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;

namespace WebApiThrottle.Net
{
    /// <summary>
    /// Class HttpRequestExtensions.
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Gets the client ip address.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>System.String.</returns>
        public static string GetClientIpAddress(this HttpRequestMessage request)
        {
            // Always return all zeroes for any failure (my calling code expects it)
            string ipAddress = "0.0.0.0";

            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                ipAddress = ((HttpContextBase)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                ipAddress = ((RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name]).Address;
            }

            if (request.Properties.ContainsKey("MS_OwinContext"))
            {
                ipAddress = ((Microsoft.Owin.OwinContext) request.Properties["MS_OwinContext"]).Request.RemoteIpAddress;
            }

            // get the X-Forward-For headers (should only really be one)
            IEnumerable<string> xForwardForList;
            if (!request.Headers.TryGetValues("X-Forwarded-For", out xForwardForList))
            {
               return ipAddress;
            }

            var xForwardedFor = xForwardForList.FirstOrDefault();

            // check that we have a value
            if (string.IsNullOrEmpty(xForwardedFor))
            {
                return ipAddress;
            }

            // Get a list of public ip addresses in the X_FORWARDED_FOR variable
            var publicForwardingIps = xForwardedFor.Split(',').Where(ip => !IpAddressUtil.IsPrivateIpAddress(ip)).ToList();

            // If we found any, return the last one, otherwise return the user host address
            return publicForwardingIps.Any() ? publicForwardingIps.Last() : ipAddress;

        }

    }
}