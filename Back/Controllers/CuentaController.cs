﻿using Back.Models;
using Back.Servicios;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Back.Controllers;
using Newtonsoft.Json;
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
        [HttpGet ]
        [Route ("GetUsuario/{user}")]
        public ActionResult<string> GetId(string user)
        {
            var modelo = _usuario.Get(user);
            if(modelo!= null)
            {
                return JsonConvert.SerializeObject(modelo);
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
                    var ok = Ok(modelo); 
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

        [HttpPut]
        [Route("ModificarUsuario/{user}")]
        public IActionResult ModificarInformacion(string user, [FromBody] Usuario ModificarUsuairo)
        {
            var modelo = _usuario.Get(user);
            if (modelo != null)
            {
                if (modelo.User == ModificarUsuairo.User)
                {
                    ModificarUsuairo.Id = modelo.Id;
                    ModificarUsuairo.Contactos = modelo.Contactos;
                    _usuario.Update(modelo.Id, ModificarUsuairo);
                    return NoContent();
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return NotFound("Usuario no encontrado");
            }



        }

        [HttpPut("{user}")]
        [Route("ModificarContactos")]
        public IActionResult ModificarContactos(string user, [FromBody] Usuario ModificarUsuairo)
        {
            var modelo = _usuario.Get(user);
            if (modelo != null)
            {

                ModificarUsuairo.Id = modelo.Id;
                ModificarUsuairo.Apellido = modelo.Apellido;
                ModificarUsuairo.eMail = modelo.eMail;
                ModificarUsuairo.Nombre = modelo.Nombre;
                ModificarUsuairo.Password = modelo.Password;
                ModificarUsuairo.LlaveSDES = modelo.LlaveSDES;
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