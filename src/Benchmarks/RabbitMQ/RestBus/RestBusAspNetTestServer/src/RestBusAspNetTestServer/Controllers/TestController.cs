using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace RestBusTestServer.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {

        // POST api/test
        [HttpPost]
        public Message Post([FromBody]string value)
        {
            var msg = new Message { Body = BodyGenerator.GetNext() };
            return msg;
        }

    }

}
