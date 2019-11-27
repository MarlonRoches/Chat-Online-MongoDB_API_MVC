using Back.Models;
using Back.Servicios;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Back.Controllers;

namespace Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentaController : ControllerBase
    {
        private readonly UsuarioServicios _usuario;
        

        public CuentaController(UsuarioServicios usuario)
        {
            _usuario = usuario;
        }

        [HttpGet]
        public ActionResult<List<Usuario>> Get() =>
            _usuario.Get();

        [HttpGet("{user}")]
        public ActionResult<string> GetUsuario(string user)
        {
            var modelo = _usuario.Get(user);
            if (modelo != null)
            {
                return Ok(modelo);
            }
            else
            {
                return NotFound("Usuario no encontrado");
            }

        }
        public ActionResult<Usuario> Post([FromBody] Usuario nuevo)
        {
            var modelo = _usuario.Get(nuevo.NombreUsuario);
            if (modelo == null)
            {
                _usuario.Create(nuevo);
                return nuevo;
            }
            else
            {
                return BadRequest("Nombre de usario invalido");
            }
        }

        [HttpPut("{user}")]
        public IActionResult ModificarInformacion(string user, [FromBody] Usuario ModificarUsuairo)
        {
            var modelo = _usuario.Get(user);
            if (modelo != null)
            {

                ModificarUsuairo.Id = modelo.Id;
                _usuario.Update(modelo.Id, ModificarUsuairo);
                return NoContent();
            }
            else
            {
                return NotFound("Usuario no encontrado");
            }



        }
        [HttpDelete("{user}")]
        public IActionResult Delete(string user)
        {
            var modelo = _usuario.Get(user);
            if (modelo != null)
            {
                _usuario.Remove(modelo.Id);
             
                return NoContent();

                //Eliminar chats
                //Eliminar conversaciones
            }
            else
            {
                return NoContent();
            }
        }

    }
}