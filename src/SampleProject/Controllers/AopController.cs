using System;
using System.Web.Http;
using SampleProject.Aspects;

namespace SampleProject.Controllers
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
