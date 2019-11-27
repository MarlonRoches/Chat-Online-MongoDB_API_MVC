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
        [HttpGet ("{user}")]
        public ActionResult<Usuario> GetId(string user)
        {
            var modelo = _usuario.Get(user);
            if(modelo!= null)
            {
                return modelo;
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("Login/{user}")]
        public ActionResult<string> GetUsuario(string user,[FromBody] Usuario password)
        {
                var modelo = _usuario.Get(user);
            if (modelo != null)
            {
                if (modelo.Password == password.Password)
                {
                    return Ok(modelo);
                }
                else
                {
                    return BadRequest("Contraseña incorrecta");
                }
            }
            else
            {
                return NotFound("Usuario no encontrado");
            }

        }
        [HttpPost]
        [Route("Crear")]
        public ActionResult<Usuario> Create([FromBody] Usuario nuevo)
        {
            var modelo = _usuario.Get(nuevo.User);
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