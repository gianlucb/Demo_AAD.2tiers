using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AAD._2tiers.Api.Controllers
{
    [Authorize]
    public class HelloController : Controller
    {
        [HttpGet]
        [Route("[controller]")]
        public string Get()
        {
            return "Hello from API";
        }

        [HttpGet]
        [Route("[controller]/ScopeA")]
        [Authorize(Policy = "A")]
        public string GetWithScopeA()
        {
            return "Hello from API (Scope A)";
        }

        [HttpGet]
        [Route("[controller]/ScopeB")]
        [Authorize(Policy = "B")]
        public string GetWithScopeB()
        {
            return "Hello from API (Scope B)";
        }

    }
}
