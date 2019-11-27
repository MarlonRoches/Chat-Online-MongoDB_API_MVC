using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Back.Models;
using System.Net.Http;
using System.Web;

namespace Back.Controllers
{
    class Update
    {
        public object Status { get; internal set; }
        public DateTime Date { get; internal set; }
    }
    [ApiController]
    public class LogInController : ControllerBase
    {
        // GET Login/Usuario/Contraseña
        [HttpGet]
        [Route("Login/{Usuario}/{Contraseña}")]
        public ActionResult<string> Get(string Usuario, string Contraseña)
        {
            return "jeej";
        }

        
    }
}