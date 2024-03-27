using Microsoft.AspNetCore.Mvc;
using BlockchainDemo.Models;
using System.Collections.Generic;
using System;


namespace BlockchainDemo.Controllers {

    [Route("[controller]")]
    [ApiController]

    public class BlockController : ControllerBase {
    
        List<Dictionary<string, object>> chain = [];

        private Dictionary<string, object> create_block(int proof, string previous_hash) {

            Dictionary<string, object> block = new Dictionary<string, object>();

            block["index"] = chain.Count;
            block["timestamp"] = DateTime.Now.ToString();
            block["proof"] = proof;
            block["previous_hash"] = previous_hash;

            chain.Add(block);

            return block;
        }

        [HttpGet]
        public ActionResult<List<Dictionary<string, object>>> Get() {
            create_block(1, "0");

            return Ok(chain);
        }

    }

}