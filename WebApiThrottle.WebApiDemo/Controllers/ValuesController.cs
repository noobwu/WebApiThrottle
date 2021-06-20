// ***********************************************************************
// Assembly         : WebApiThrottle.WebApiDemo
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2021-06-20
// ***********************************************************************
// <copyright file="ValuesController.cs" company="">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiThrottle.WebApiDemo.Controllers
{
    /// <summary>
    /// Class ValuesController.
    /// Implements the <see cref="System.Web.Http.ApiController" />
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class ValuesController : ApiController
    {
        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        [EnableThrottling(PerSecond = 2)]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>System.String.</returns>
        [DisableThrotting]
        public string Get(int id)
        {
            return "value";
        }
        // POST api/values
        /// <summary>
        /// Posts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        /// <summary>
        /// Puts the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="value">The value.</param>
        [HttpPut]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        [HttpDelete]
        public void Delete(int id)
        {
        }
        /// <summary>
        /// Policy runtime update example
        /// </summary>
        [NonAction]
        public void UpdateRateLimits()
        {
            //init policy repo
            var policyRepository = new PolicyCacheRepository();

            //get policy object from cache
            var policy = policyRepository.FirstOrDefault(ThrottleManager.GetPolicyKey());

            //update client rate limits
            policy.ClientRules["api-client-key-1"] =
                new RateLimits { PerMinute = 50, PerHour = 500 };

            //add new client rate limits
            policy.ClientRules.Add("api-client-key-3",
                new RateLimits { PerMinute = 60, PerHour = 600 });

            //apply policy updates
            ThrottleManager.UpdatePolicy(policy, policyRepository);

        }
    }
}
