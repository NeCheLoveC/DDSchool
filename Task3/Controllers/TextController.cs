using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TextParser;

namespace Task3.Controllers
{
    
    public class TextController : ApiController
    {
        [HttpPost]
        [Route("calculate-words")]
        public Dictionary<string, int> CalculateWords([FromBody]String text)
        {
            WordCalculatorThread parser = new WordCalculatorThread();
            return parser.Parse(text);
        }
    }
}
