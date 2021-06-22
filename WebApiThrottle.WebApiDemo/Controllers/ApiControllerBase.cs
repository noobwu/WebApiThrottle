using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace WebApiThrottle.WebApiDemo.Controllers
{

    public class ApiControllerBase : ApiController
    {
        /// <summary>
        /// The default settings
        /// </summary>
        private readonly JsonSerializerSettings defaultSettings = null;
        /// <summary>
        /// The policy repository
        /// </summary>
        protected readonly IPolicyRepository policyRepository;
        /// <summary>
        /// The throttle repository
        /// </summary>
        protected readonly IThrottleRepository throttleRepository;
        public ApiControllerBase()
        {
            //Json格式化默认设置
            defaultSettings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Converters = { new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" } }
            };
        }

        /// <summary>
        /// Creates a <see cref="T:System.Web.Http.Results.JsonResult`1" /> (200 OK) with the specified value.
        /// carl.wu
        /// </summary>
        /// <typeparam name="T">The type of content in the entity body.</typeparam>
        /// <param name="content">The content value to serialize in the entity body.</param>
        /// <returns>A <see cref="T:System.Web.Http.Results.JsonResult`1" /> with the specified value.</returns>
        [NonAction]
        protected internal new  JsonResult<T> Json<T>(T content)
        {
            return Json(content, defaultSettings);
        }

    }
}