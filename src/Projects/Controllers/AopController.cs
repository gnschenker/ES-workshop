using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Projects.Aspects;

namespace Projects.Controllers
{
    [RoutePrefix("api/aop")]
    public class AopController : ApiController
    {
        [StatsdMetrics]
        [Route("")]
        [HttpGet]
        public string Get()
        {
            Random r = new Random();
            int test = r.Next(1, 10);
            if(test%3==0)
                throw new Exception("Something really bad happened!");

            return "";
        }
    }
}
