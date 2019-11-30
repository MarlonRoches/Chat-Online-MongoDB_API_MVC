using Back.Models;
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
        {   var modelo = _usuario.Get(user);
            if(modelo!= null)
            {
                return JsonConvert.SerializeObject(modelo);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        [Route ("VerificarUsuario/{user}/{receptor}")]
        public IActionResult VerificarUsuario(string user, string receptor)
        {
            var modelo = _usuario.Get(user);
            if(modelo == null)
            {
                return NotFound();
            }
            else
            {
                if (user ==receptor)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok();
                }
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
        public IActionResult ModificarContactos(string emisor, [FromBody] string receptor)
        {
            var modelo = _usuario.Get(emisor);
            if (modelo != null)
            {
                List<string> contactos = new List<string>();
                if (modelo.Contactos == null)
                {
                    string x = receptor;
                    contactos.Add(x);
                    modelo.Contactos = contactos;
                }
                else
                {
                    if (modelo.Contactos.Contains(receptor) == false)
                    {
                        contactos = modelo.Contactos;
                        contactos.Add(receptor);
                        modelo.Contactos = contactos;
                    }


                }
              
               
                
                _usuario.Update(modelo.Id, modelo);

                return NoContent();
            }
            else
            {
                return NotFound("Usuario no encontrado");
            }



        }
       
        [HttpPut]
        [Route("Eliminarcontacto/{User}/{UserCompuesto}")]
        public IActionResult EliminarContacto(string User, string UserCompuesto, [FromBody] Usuario Vacio)
        {
            var modelo = _usuario.Get(User);
            if(modelo!= null)
            {
                if (modelo.Contactos != null)
                {
                    string[] eliminar = UserCompuesto.Split(",");

                    if (modelo.Contactos.Contains(eliminar[1]))
                    {
                        modelo.Contactos.Remove(eliminar[1]);
                        _usuario.Update(modelo.Id, modelo);
                        var Coneccion = new MensajesDatabaseSettings();
                        Coneccion.ConnectionString = "mongodb://localhost:27017";
                        Coneccion.DatabaseName = "Teules";
                        Coneccion.MensajeCollectionName = "mensajes";
                        var nuevo2 = new MensajesServicios(Coneccion);
                        var ModificarContactos = new MensajesController(nuevo2);
                        ModificarContactos.EliminarConversacion(UserCompuesto);
                        return NoContent();
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest();
                }

            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete]
        [Route ("EliminarUsuario/{user}")]
        public IActionResult Delete(string user)
        {
            var modelo = _usuario.Get(user);
            if (modelo != null)
            {
                _usuario.Remove(modelo.Id);
                var Coneccion = new MensajesDatabaseSettings();
                Coneccion.ConnectionString = "mongodb://localhost:27017";
                Coneccion.DatabaseName = "Teules";
                Coneccion.MensajeCollectionName = "mensajes";
                var nuevo2 = new MensajesServicios(Coneccion);
                var ModificarContactos = new MensajesController(nuevo2);
                ModificarContactos.DeleteAllMesaje(user);
                return NoContent();

               

                //Eliminar conversaciones
            }
            else
            {
                return NoContent();
            }
        }

    }
}