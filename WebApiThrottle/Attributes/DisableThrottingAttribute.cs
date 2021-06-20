// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="DisableThrottingAttribute.cs" company="stefanprodan.com">
//     Copyright © Stefan Prodan 2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace WebApiThrottle
{
    /// <summary>
    /// Class DisableThrottingAttribute.
    /// Implements the <see cref="System.Web.Http.Filters.ActionFilterAttribute" />
    /// Implements the <see cref="System.Web.Http.Filters.IActionFilter" />
    /// </summary>
    /// <seealso cref="System.Web.Http.Filters.ActionFilterAttribute" />
    /// <seealso cref="System.Web.Http.Filters.IActionFilter" />
    public class DisableThrottingAttribute : ActionFilterAttribute, IActionFilter
    {
    }
}
