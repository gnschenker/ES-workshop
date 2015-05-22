using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Projects.Contracts.Commands;
using Projects.Domain;

namespace Projects.Controllers
{
    [RoutePrefix("api/samples")]
    public class SamplesController : ApiController
    {
        private readonly ISampleApplicationService _sampleApplicationService;

        public SamplesController(ISampleApplicationService sampleApplicationService)
        {
            _sampleApplicationService = sampleApplicationService;
        }

        [Route("")]
        [HttpGet]
        public IEnumerable Get()
        {
            return new object[]
            {
                new
                {
                    id = 1,
                    name = "Sample 1",
                    quantity = 11
                },
                new
                {
                    id = 2,
                    name = "Sample 2",
                    quantity = 15
                }
            };
        }

        [Route("{id}")]
        [HttpGet]
        public object Get(int id)
        {
            return new
            {
                id = 1,
                name = "Sample 1",
                quantity = 15
            };
        }

        [Route("start")]
        [HttpPost]
        public HttpResponseMessage Start([FromBody]StartRequest req)
        {
            var id = _sampleApplicationService.When(new StartSample {Name = req.Name});
            return Request.CreateResponse(HttpStatusCode.Created, new {sampleId = id});
        }

        [Route("{sampleId}/step1")]
        [HttpPost]
        public void Step1(Guid sampleId, [FromBody]Step1Request req)
        {
            _sampleApplicationService.When(
                new DoStep1
                {
                    SampleId = sampleId, 
                    Quantity = req.Quantity,
                    DueDate = req.DueDate
                });
        }

        [Route("{sampleId}/approve")]
        [HttpPost]
        public void Step1(Guid sampleId)
        {
            _sampleApplicationService.When(
                new ApproveSample
                {
                    SampleId = sampleId, 
                });
        }
    }

    public class Step1Request
    {
        public int Quantity { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class StartRequest
    {
        public string Name { get; set; }
    }
}