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
        [Route ("CrearConversacionEmisor")]
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
        [HttpPost]
        [Route ("CrearConversacionReceptor")]
        public IActionResult PostReceptor([FromBody] Receptor _nuevo)
        {
            if (ModelState.IsValid)
            {
                var modelo = _mensajes.Get(_nuevo.IDEmisorReceptor);
                if (modelo == null)
                {
                    Mensaje nuevo = new Mensaje();
                    Extesiones AgregarTexto = new Extesiones();
                    Dictionary<string, Extesiones> Emisor = new Dictionary<string, Extesiones>();
                    Dictionary<string, bool> Indice = new Dictionary<string, bool>();
                    Indice.Add(_nuevo.HoraMensaje, _nuevo.Origen);
                    nuevo.IDEmisorReceptor = _nuevo.IDEmisorReceptor;
                    nuevo.Receptor = _nuevo.Recept;
                    nuevo.Emisor = _nuevo.Emisor;
                    AgregarTexto.Texto = _nuevo.Texto;
                    AgregarTexto.Extesion = _nuevo.Extension;
                    Emisor.Add(_nuevo.HoraMensaje, AgregarTexto);
                    nuevo.EmisorMen = Emisor;
                    _mensajes.Create(nuevo);
                    UsuariosDatabaseSettings Coneccion = new UsuariosDatabaseSettings();
                    Coneccion.ConnectionString = "mongodb://localhost:27017";
                    Coneccion.DatabaseName = "Teules";
                    Coneccion.UsuariosCollectionName = "usuarios";
                    UsuarioServicios nuevo2 = new UsuarioServicios(Coneccion);
                    CuentaController ModificarContactos = new CuentaController(nuevo2);
                    ModificarContactos.ModificarContactos(_nuevo.Emisor, nuevo.Receptor);
                }
                else
                {

                    //de lo contrario existe por lo tanto agregar el mensaje nuevo al put
                }



            }
            return NoContent();
        }
        [HttpPut]
        [Route ("AgregarMensajeEmisor/{UsuarioCompuesto}")]
        public IActionResult MensajeNuevo(string UsuarioCompuesto, [FromBody] Mensaje nuevo)
        {
            if(ModelState.IsValid)
            {
                
                var modelo = _mensajes.Get(UsuarioCompuesto);
                if(modelo != null)
                {
                    modelo.EmisorMen = nuevo.EmisorMen;
                    modelo.ReceptorMen = nuevo.ReceptorMen;

                    _mensajes.Update(modelo.Id, modelo);
                }
            }
            return NoContent();
        }
        [HttpPut]
        [Route("AgregarMensajeReceptor/{UsuarioCompuesto}")]
        public IActionResult MensajeNuevo(string UsuarioCompuesto, [FromBody] Receptor _nuevo)
        {

            if (ModelState.IsValid)
            {

                var modelo = _mensajes.Get(UsuarioCompuesto);
                if (modelo != null)
                {
                   
                    Extesiones AgregarTexto = new Extesiones();
                    Dictionary<string, Extesiones> Emisor = new Dictionary<string, Extesiones>();
                    Dictionary<string, bool> Indice = new Dictionary<string, bool>();
                    AgregarTexto.Texto = _nuevo.Texto;
                    AgregarTexto.Extesion = _nuevo.Extension;
                    modelo.Receptor = _nuevo.Recept;
                    modelo.Emisor = _nuevo.Emisor;
                    modelo.EmisorMen = Emisor;


                    if (modelo.EmisorMen != null)
                    {
                        Emisor = modelo.EmisorMen;
                        Indice = modelo.MensajesOrdenados;
                        Emisor.Add(_nuevo.HoraMensaje, AgregarTexto);
                        Indice.Add(_nuevo.HoraMensaje, _nuevo.Origen);
                        modelo.EmisorMen = Emisor;
                        modelo.MensajesOrdenados = Indice;

                    }
                    else
                    {
                        Emisor.Add(_nuevo.HoraMensaje, AgregarTexto);
                        Indice.Add(_nuevo.HoraMensaje, _nuevo.Origen);
                        modelo.EmisorMen = Emisor;
                        modelo.MensajesOrdenados = Indice;

                     
                    }
                   
                 
                    _mensajes.Update(modelo.Id, modelo);
                }
            }
        
                    return NoContent();

        }






    }
}