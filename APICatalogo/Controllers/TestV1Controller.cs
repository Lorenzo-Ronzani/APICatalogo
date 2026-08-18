using System.Reflection.Metadata.Ecma335;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers
{
    [Route("api/v{version:ApiVersion}/test")]
    [ApiController]
    [ApiVersion("1.0")] 
    public class TestV1Controller : ControllerBase
    {
        [HttpGet]
        public string GetVersion()
        {
            return "Teste V1 - GET - Api versão 1.0";
        }
    }
}
