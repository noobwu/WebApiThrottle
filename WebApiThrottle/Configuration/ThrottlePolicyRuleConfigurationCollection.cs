// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="ThrottlePolicyRuleConfigurationCollection.cs" company="stefanprodan.com">
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
    /// Class ThrottlePolicyRuleConfigurationCollection.
    /// Implements the <see cref="System.Configuration.ConfigurationElementCollection" />
    /// </summary>
    /// <seealso cref="System.Configuration.ConfigurationElementCollection" />
    public class ThrottlePolicyRuleConfigurationCollection : ConfigurationElementCollection
    {

        /// <summary>
        /// 在派生的类中重写时，创建一个新的 <see cref="T:System.Configuration.ConfigurationElement" />。
        /// </summary>
        /// <returns>一个新创建的 <see cref="T:System.Configuration.ConfigurationElement" />。</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ThrottlePolicyRuleConfigurationElement();
        }

        /// <summary>
        /// Gets the element key.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>System.Object.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ThrottlePolicyRuleConfigurationElement)element).Entry;
        }
    }
}
