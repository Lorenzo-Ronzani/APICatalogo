using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers
{
    [Route("api/v{version:ApiVersion}/test")]
    [ApiController]
    [ApiVersion("2.0")]
    public class TestV2Controller : ControllerBase
    {
        [HttpGet]
        public string GetVersion()
        {
            return "Teste V2 - GET - Api versão 2.0";
        }
    }
}
