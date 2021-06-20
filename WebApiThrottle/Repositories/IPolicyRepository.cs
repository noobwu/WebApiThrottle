// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="IPolicyRepository.cs" company="stefanprodan.com">
//     Copyright © Stefan Prodan 2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
namespace WebApiThrottle
{
    /// <summary>
    /// Interface IPolicyRepository
    /// </summary>
    public interface IPolicyRepository
    {
        /// <summary>
        /// Firsts the or default.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ThrottlePolicy.</returns>
        ThrottlePolicy FirstOrDefault(string id);

        /// <summary>
        /// Removes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        void Remove(string id);

        /// <summary>
        /// Saves the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="policy">The policy.</param>
        void Save(string id, ThrottlePolicy policy);
    }
}
