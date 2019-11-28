using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Back.Models;
using Back.Servicios;

namespace Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MensajesController : ControllerBase
    {
        private readonly MensajesServicios _mensajes;

        public MensajesController(MensajesServicios menServ)
        {
            _mensajes = menServ;
        }
        [HttpGet]
        public ActionResult<List<Mensaje>> Get() =>
            _mensajes.Get();
        [HttpGet("{user}")]
        public ActionResult<string> GetId(string user)
        {
            var modelo = _mensajes.Get(user);
            if(modelo!=null)
            {
                return Ok(modelo);
            }
            else
            {
                return NotFound("usurio no encontrado");

            }
        }
        [HttpPost]
        public IActionResult post([FromBody] Mensaje _nuevo)
        {
            _mensajes.Create(_nuevo);
           // CuentaController nuevo =new CuentaController();
            return NoContent();
        }


        
           
        
    }
}