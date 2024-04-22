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

    public class MempoolController : ControllerBase {

        [HttpGet]
        public ActionResult<List<TransactionModel>> Get_mempool() {

            var MempoolServices = new MempoolServices();

            return Ok(MempoolServices.get_mempool());
        }

        [HttpPost]
        public ActionResult<List<TransactionModel>> Post_mempool([FromBody] dynamic dadosObtidos) {

            var MempoolServices = new MempoolServices();

            MempoolServices.get_mempool();
            MempoolServices.add_transaction(dadosObtidos);

            return Ok(MempoolServices.get_mempool());
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

