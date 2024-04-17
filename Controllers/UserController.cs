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

    public class UserController : ControllerBase {

        [HttpGet]
        public ActionResult<UserModel> GetUser() {

            var MainController = new MainController();

            return Ok(MainController.get_user());
        }

    }
}