using Microsoft.AspNetCore.Mvc;
using BlockchainDemo.Models;
using System;


namespace BlockchainDemo.Controllers {

    [Route("[controller]")]
    [ApiController]

    public class BlockController : ControllerBase {

        [HttpGet]
        public ActionResult Get() {

            return Ok("teste");
        }

    }

}