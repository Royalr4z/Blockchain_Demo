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

            TransactionModel transaction = new TransactionModel();
            MempoolServices.mempool.Clear();
            MempoolServices.mempool.Add(transaction);

            return Ok(MempoolServices.mempool);
        }

        [HttpPost]
        public ActionResult<List<TransactionModel>> Post_mempool() {

            TransactionModel transaction = new TransactionModel();
            MempoolServices.mempool.Clear();
            MempoolServices.mempool.Add(transaction);

            return Ok(MempoolServices.mempool);
        }

    }
}