using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace nf.protoscript.bgservice
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return $"Value {id}";
        }
    }

}