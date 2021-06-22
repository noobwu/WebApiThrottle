using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiThrottle.WebApiDemo.Models
{
    /// <summary>
    /// Class ThrottlePolicyModel.
    /// </summary>
    public class ThrottlePolicyModel
    {
        ///// <summary>
        ///// Gets or sets the rules.
        ///// </summary>
        ///// <value>The rules.</value>
        public List<ThrottlePolicyRule> Rules { get; set; }

        /// <summary>
        ///  白名单
        /// </summary>
        /// <value>The whitelists.</value>
        public List<ThrottlePolicyWhitelist> Whitelists { get; set; }

    }
}