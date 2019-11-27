using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Back.Servicios;
using Back.Models;

namespace Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        private readonly UsuarioServicios _usuario;

        public CuentasController(UsuarioServicios usuario)
        {
            _usuario = usuario;
        }

        [HttpGet]
        public ActionResult<List<Usuario>> Get() =>
            _usuario.Get();

        [HttpGet("{id}")]
        public ActionResult<string> GetUsuario(string user)
        {
            var modelo = _usuario.Get(user);
            if(modelo!=null)
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
            if(modelo == null)
            {
                _usuario.Create(nuevo);
                return nuevo;
            }
            else
            {
                return BadRequest("Nombre de usario invalido");
            }
        }
        
        [HttpPut("{id}")]
        public IActionResult ModificarInformacion(string id, [FromBody] Usuario ModificarUsuairo)
        {
            var modelo = _usuario.Get(id);
            if (modelo != null)
            {

                //if (ModificarUsuairo.Apellido == "") { ModificarUsuairo.Apellido = modelo.Apellido; }
                //if (ModificarUsuairo.Nombre == "") { ModificarUsuairo.Nombre = modelo.Nombre; }
                //if(ModificarUsuairo.Password == "") { ModificarUsuairo.Password = modelo.Password; }
                //if(ModificarUsuairo.NombreUsuario == "") 
                //{
                //    ModificarUsuairo.NombreUsuario = modelo.NombreUsuario;
                //}
                //else
                //{

                //}
                _usuario.Update(modelo.NombreUsuario, ModificarUsuairo);
                return NoContent();
            }
            else
            {
                return NotFound("Usuario no encontrado");
            }
          


        }
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var modelo = _usuario.Get(id);
            if(modelo!= null)
            {
                _usuario.Remove(id);
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