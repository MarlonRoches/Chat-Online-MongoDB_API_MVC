using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Back.Models;
using Newtonsoft.Json;
namespace Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JWTController : ControllerBase
    {
        private string JWTToken { get; set; }
        [HttpPost("{key}", Name = "Cifrar")]
        public ActionResult<string> post(String key, [FromBody] Usuario json )
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var modelo = new CifradoJWT();
                    key = key.PadLeft(20, '0');
                    JWTToken = modelo.GenerateToken(key, json);
                    string token = JsonConvert.SerializeObject(JWTToken);
                    return token;
                }
                else
                {
                    return BadRequest("Modelo de json no valido");
                }
            }
            catch
            {
                //si la llave es muy pequeña
                return BadRequest("La llave es muy pequeña");
            }
        }
    }
}