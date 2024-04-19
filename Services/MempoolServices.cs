using Microsoft.AspNetCore.Mvc;
using BlockchainDemo.Models;
using BlockchainDemo.Config;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System;


namespace BlockchainDemo.Services {

    public class MempoolServices {

        public static List<TransactionModel> mempool = new List<TransactionModel>();

    }
}