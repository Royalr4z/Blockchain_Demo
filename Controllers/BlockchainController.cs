using Microsoft.AspNetCore.Mvc;
using BlockchainDemo.Models;
using BlockchainDemo.Config;
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

            var MainController = new MainController();

            return Ok(MainController.get_chain());
        }


        [HttpPost]
        public ActionResult<List<TransactionModel>> post_block([FromBody] dynamic dadosObtidos) {

            var MainController = new MainController();
            var P2PMethors = new P2PMethors();

            try {

                MainController.get_chain();
                MainController.create_block(
                    MainController.chain.Last().hash,
                    MainController.mine_block(dadosObtidos)
                );

                P2PMethors.SendBlockchain();

                return Ok(MainController.get_chain());
            } catch (Exception ex) {

                return BadRequest(ex.Message);
            }
        }

    }
}


//
//
//        ### Json Esperado ### - Método POST
//
//        {
//            "transactions": [
//                  { "from": "de", "towards": "para", "value": 0.2, "rate": 0.01 },
//                  { "from": "de", "towards": "para", "value": 0.7, "rate": 0.01 },
//                  { "from": "de", "towards": "para", "value": 0.01, "rate": 0.001 },
//                  { "from": "de", "towards": "para", "value": 0.5, "rate": 0.01 },
//                  { "from": "de", "towards": "para", "value": 0.1, "rate": 0.01 },
//                  { "from": "de", "towards": "para", "value": 0.4, "rate": 0.01 }
//            ]
//        }
//
//
