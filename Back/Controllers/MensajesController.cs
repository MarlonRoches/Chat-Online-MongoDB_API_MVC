using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Back.Models;
using Back.Servicios;
using Newtonsoft.Json;
using Back.Data;
namespace Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MensajesController : ControllerBase
    {
        #region Olvidado

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
            if (modelo != null)
            {
                return JsonConvert.SerializeObject(modelo);
            }
            else
            {
                return NotFound("usurio no encontrado");

            }
        }



        [HttpPost]
        [Route("CrearConversacionEmisor")]
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
                else
                {
                    LlamadoCambiosAEmisor(modelo.IDEmisorReceptor, _nuevo);
                }




            }
            return NoContent();
        }
        #endregion

      

        [HttpPost]
        [Route("CrearConversacionReceptor")]
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
                    nuevo.Receptor = _nuevo.Emisor;
                    nuevo.Emisor = _nuevo.Recept;
                    AgregarTexto.Texto = _nuevo.Texto;
                    AgregarTexto.Extesion = _nuevo.Extension;
                    nuevo.MensajesOrdenados = Indice;
                    Emisor.Add(_nuevo.HoraMensaje, AgregarTexto);
                    nuevo.ReceptorMen = Emisor;
                    _mensajes.Create(nuevo);
                    UsuariosDatabaseSettings Coneccion = new UsuariosDatabaseSettings();
                    Coneccion.ConnectionString = "mongodb://localhost:27017";
                    Coneccion.DatabaseName = "Teules";
                    Coneccion.UsuariosCollectionName = "usuarios";
                    UsuarioServicios nuevo2 = new UsuarioServicios(Coneccion);
                    CuentaController ModificarContactos = new CuentaController(nuevo2);
                    ModificarContactos.ModificarContactos(_nuevo.Recept, _nuevo.Emisor);

                    // ESTO ESTA BIEN
                }
                else
                {
                    LlamadoCambiosAReceptor(modelo.IDEmisorReceptor, _nuevo);
                    //de lo contrario existe por lo tanto agregar el mensaje nuevo al put
                }



            }
            return NoContent();
        }



        [HttpPut(Name = "ModEmisor")]
        [Route("AgregarMensajeEmisor/{UsuarioCompuesto}")]
        public IActionResult MensajeNuevoEmisor(string UsuarioCompuesto, [FromBody] Mensaje nuevo)
        {
            if (ModelState.IsValid)
            {

                var modelo = _mensajes.Get(UsuarioCompuesto);
                if (modelo != null)
                {

                    modelo.EmisorMen = nuevo.EmisorMen;
                    modelo.ReceptorMen = nuevo.ReceptorMen;
                    modelo.MensajesOrdenados = nuevo.MensajesOrdenados;
                    _mensajes.Update(modelo.Id, modelo);
                }
                else
                {
                    return NotFound();
                }
            }
            return NoContent();
        }
        [HttpPut]
        [Route("AgregarMensajeReceptor/{UsuarioCompuesto}")]
        public IActionResult MensajeNuevoReceptor(string UsuarioCompuesto, [FromBody] Receptor _nuevo)
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
                    modelo.Receptor = _nuevo.Emisor;
                    modelo.Emisor = _nuevo.Recept;

                    if (modelo.ReceptorMen != null)
                    {
                        Emisor = modelo.ReceptorMen;
                        Indice = modelo.MensajesOrdenados;
                        Emisor.Add(_nuevo.HoraMensaje, AgregarTexto);
                        Indice.Add(_nuevo.HoraMensaje, _nuevo.Origen);
                        modelo.ReceptorMen = Emisor;
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

        [HttpDelete]
        public IActionResult EliminarConversacion(string userCompuesto)
        {
            var modelo = _mensajes.Get(userCompuesto);
            if (modelo != null)
            {
                _mensajes.Remove(modelo.Id);
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpDelete]
        public IActionResult DeleteAllMesaje(string user)
        {
            var modelo = _mensajes.Get(user);

            if (modelo != null)
            {
                while (modelo != null)
                {

                    _mensajes.MensajesComoEmisor(modelo.Id);
                    modelo = _mensajes.BuscarEmisor(user);



                }
            }
            return NoContent();
        }

        [HttpPut]
        [Route("BorrarMensaje/{UsuarioCompuesto}/{Usuario}/{Llave}")]
        public IActionResult BorrarMensaje(string UsuarioCompuesto, string Usuario, string Llave)
        {
            var modelo = _mensajes.Get(UsuarioCompuesto);
            if (modelo != null)
            {
                if (modelo.Emisor == Usuario)
                {
                    modelo.EmisorMen.Remove(Llave);
                }
                else if (modelo.Receptor == Usuario)
                {
                    modelo.ReceptorMen.Remove(Llave);
                }
                else
                {
                    return BadRequest();
                }
                _mensajes.Update(modelo.Id, modelo);

                return NoContent();
            }
            else
            {
                return NotFound();
            }

        }


        [HttpPut]
        [Route("BuscadorLike/{Emisor}")]
        public ActionResult<string> BuscadorLike(string Emisor, [FromBody] Extesiones TextoABuscar)
        {
            var Buscador = new Dictionary<string, Extesiones>();
            Extesiones prueba = new Extesiones();
            var modelo = _mensajes.BuscadorLike(Emisor);
            string agregar;
            if (modelo != null)
            {
                foreach (var item in modelo)
                {

                    var LlaveEmisor = 0;
                    var LlaveReceptor = 0;
                    foreach (var caracteritem in item.Emisor)
                    {
                        LlaveEmisor += caracteritem;
                    }

                    foreach (var caracteritem in item.Receptor)
                    {
                        LlaveReceptor += caracteritem;
                    }
                    var llaveTxt = LlaveEmisor + LlaveReceptor;
                        TextoABuscar.Texto = Singleton.Instance.DescifradoSDES(int.Parse(TextoABuscar.Extesion), TextoABuscar.Texto).ToLower();
                    if (llaveTxt >= 1023)
                    {
                        llaveTxt /= 2;
                        if (llaveTxt <= 512)
                        {
                            llaveTxt += 512;

                        }
                    }
                    foreach (var item2 in item.EmisorMen)
                    {
                        prueba = item.EmisorMen[item2.Key];

                        prueba.Texto = Singleton.Instance.DescifradoSDES(llaveTxt, prueba.Texto).ToLower();
                        if (prueba.Texto.ToLower() == TextoABuscar.Texto.ToLower())
                        {
                            agregar = $"{item.Receptor},{item2.Key}";
                            Buscador.Add(agregar, item2.Value);
                        }
                    }


                    foreach (var item2 in item.ReceptorMen)
                    {
                        prueba = item.ReceptorMen[item2.Key];
                        prueba.Texto = Singleton.Instance.DescifradoSDES(llaveTxt, prueba.Texto).ToLower();
                        if (prueba.Texto == TextoABuscar.Texto)
                        {
                            agregar = $"{item.Receptor},{item2.Key}";
                            Buscador.Add(agregar, item2.Value);
                        }
                    }



                }
                if (Buscador != null)
                {
                    var json = JsonConvert.SerializeObject(Buscador);
                    return Ok(json);
                }
                else
                {
                    return NotFound();
                }

            }
            else
            {
                return BadRequest();
            }



        }



        public void LlamadoCambiosAEmisor(string x, Mensaje nuevo)
        {
            MensajeNuevoEmisor(x, nuevo);
        }
        public void LlamadoCambiosAReceptor(string x, Receptor nuevo)
        {
            MensajeNuevoReceptor(x, nuevo);
        }



    }
}