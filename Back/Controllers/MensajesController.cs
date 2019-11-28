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
        [HttpGet]
        [Route("ObtenerConversacion/{userCompuesto}")]
        public ActionResult<string> GetId(string userCompuesto)
        {
            var modelo = _mensajes.Get(userCompuesto);
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
        [Route ("CrearConversacion")]
        public IActionResult post([FromBody] Mensaje _nuevo)
        {
            if (ModelState.IsValid)
            {
                var modelo = _mensajes.Get(_nuevo.IDEmisorReceptor);
                if (modelo == null)
                {
                    _mensajes.Create(_nuevo);
                    UsuariosDatabaseSettings nuevo = new UsuariosDatabaseSettings();
                    nuevo.ConnectionString = "mongodb://localhost:27017";
                    nuevo.DatabaseName = "Teules";
                    nuevo.UsuariosCollectionName = "usuarios";
                    UsuarioServicios nuevo2 = new UsuarioServicios(nuevo);
                    CuentaController ModificarContactos = new CuentaController(nuevo2);
                    ModificarContactos.ModificarContactos(_nuevo.Emisor, _nuevo.Receptor);
                }
              


            }
            return NoContent();
        }
        





    }
}