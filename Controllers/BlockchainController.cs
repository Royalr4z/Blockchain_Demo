using Microsoft.AspNetCore.Mvc;
using BlockchainDemo.Models;
using BlockchainDemo.Config;
using BlockchainDemo.Services;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Text;
using System;


namespace BlockchainDemo.Controllers {

    [Route("[controller]")]
    [ApiController]

    public class BlockchainController : ControllerBase {

        [HttpGet]
        public ActionResult<List<BlockModel>> Get_blockchain() {

            var BlockServices = new BlockServices();

            return Ok(BlockServices.get_chain());
        }


        [HttpPost]
        public ActionResult<List<TransactionModel>> post_block() {

            var BlockServices = new BlockServices();
            var P2PMethors = new P2PMethors();

            try {

                BlockServices.get_chain();
                BlockServices.create_block(
                    BlockServices.chain.Last().hash,
                    BlockServices.mine_block()
                );

                P2PMethors.SendBlock();

                return Ok(BlockServices.get_chain());
            } catch (Exception ex) {

                return BadRequest(ex.Message);
            }
        }

    }
}

