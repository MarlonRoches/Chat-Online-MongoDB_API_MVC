using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Back.Models;
using Back.Data;
using Newtonsoft.Json;
using System.Net.Http;

namespace Front.Controllers
{
    public class MainController : Controller
    {
        static string UsuarioActual="";
        HttpClient ClienteHttp = new HttpClient();
        public ActionResult CrearUsuario()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CrearUsuario(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                var Nuevo = new Usuario
                {
                    Nombre = collection["Nombre"],
                    Apellido = collection["Apellido"],
                    Password = collection["Password"],
                    NombreUsuario = collection["User"],
                    CorreoElectronico = collection["eMail"],
                    Contactos = new List<string>()
                };
                foreach (var item in Nuevo.NombreUsuario)
                {
                    Nuevo.LlaveSDES += (int)item;
                }
                //Cifrar Contraseña
                if (Nuevo.LlaveSDES < 512)
                {
                    Nuevo.LlaveSDES += 512;
                }
                Nuevo.Password = Singleton.Instance.CifradoSDES(Nuevo.LlaveSDES, Nuevo.Password);
                var json = JsonConvert.SerializeObject(Nuevo);
                //enviar a api y generar token

                return RedirectToAction("");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Login(FormCollection collection)
        {
           
                // TODO: Add insert logic here
                var Nuevo = new Usuario
                {
                    NombreUsuario = collection["User"],
                    Password = collection["Password"]
                };
                //Cifrar Contraseña
                
                foreach (var item in Nuevo.NombreUsuario)
                {
                    Nuevo.LlaveSDES+= (int)item;
                }
                if (Nuevo.LlaveSDES < 512)
                {
                    Nuevo.LlaveSDES += 512;
                }
                Nuevo.Password = Singleton.Instance.CifradoSDES(Nuevo.LlaveSDES,Nuevo.Password);
                var json = JsonConvert.SerializeObject(Nuevo);
                var enviar = Nuevo.NombreUsuario+"/"+Nuevo.Password;
            //Generar Token
            //UsuarioActual= 
            //Verificar Que los campos sean correctos
            
            return RedirectToAction("ListaDeChats");
        }
        public ActionResult ListaDeChats()
        {
            //enviar usuario para obtener lista

            //obtener lista de mensajes
            //Clasificar
            var lista = new List<string>();
            
            lista.Add("Pedro");
            lista.Add("Jorge");
            lista.Add("Estuardo");
            lista.Add("Pablo");

            //Devolver
            return View(lista);
        }
       
    }
}
