using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Back.Models;
using Back.Data;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Front.Models;
using Back.Data;

namespace Front.Controllers
{
    public class MainController : Controller
    {
        
        HttpClient ClienteHttp = new HttpClient();

        public ActionResult CrearUsuario()
        {
            return View();
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> CrearUsuario(FormCollection collection)
        {

            // TODO: Add insert logic here
            var Nuevo = new Usuario
            {
                Nombre = collection["Nombre"],
                Apellido = collection["Apellido"],
                Password = collection["Password"],
                User = collection["User"],
                eMail = collection["eMail"],
                Contactos = new List<string>()
            };
            foreach (var item in Nuevo.User)
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

            var cliente = new HttpClient();

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var respose = await cliente.PostAsync("https://localhost:44338/api/Cuenta/Crear", content);


            return RedirectToAction("Login");
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Login(FormCollection collection)
        {
            string path = Server.MapPath("/");
            //or 
            string path2 = Server.MapPath("~");                // TODO: Add insert logic here
            var Nuevo = new Usuario
                {
                    User = collection["User"],
                    Password = collection["Password"]
                };
                //Cifrar Contraseña
                
                foreach (var item in Nuevo.User)
                {
                    Nuevo.LlaveSDES+= (int)item;
                }
                if (Nuevo.LlaveSDES < 512)
                {
                    Nuevo.LlaveSDES += 512;
                }
                Nuevo.Password = Singleton.Instance.CifradoSDES(Nuevo.LlaveSDES,Nuevo.Password);
                var json = JsonConvert.SerializeObject(Nuevo);
                var enviar = Nuevo.User+"/"+Nuevo.Password;
            //Generar Token
            //UsuarioActual= 
            //Verificar Que los campos sean correctos
            var cliente = new HttpClient();

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var uri = "https://localhost:44338/api/Cuenta/Login/" + Nuevo.User;
            var respose = await cliente.PostAsync(uri, content);

            if (respose.ReasonPhrase == "OK")
            {

                Back.Data.Singleton.Instance.UsuarioActual = Nuevo.User;

                var nuevo = new ListaContactos
                {
                    Contacto = "1"
                };
                var GetUsuario = new HttpClient();
                Singleton.Instance.Actual = JsonConvert.DeserializeObject<Usuario>(await GetUsuario.GetStringAsync("https://localhost:44338/api/Cuenta/GetUsuario/"+ Nuevo.User));
                //obtenemos Usuario
                return RedirectToAction("ListaDeChats");
            }
            else if (respose.ReasonPhrase == "Bad Request")
            {

            return View();
            }
            else
            {
                //not Found
            return View();

            }
            
        }
        public ActionResult ListaDeChats()
        {
            //enviar usuario para obtener lista

            //obtener lista de mensajes
            //Clasificar
            //var lista = new List<ListaContactos>();

            Singleton.Instance.Actual.Contactos.Add("asasd");
            Singleton.Instance.Actual.Contactos.Add("Jorge");
            Singleton.Instance.Actual.Contactos.Add("Estuardo");
            Singleton.Instance.Actual.Contactos.Add("Pablo");
           
            //Devolver

            return View(Singleton.Instance.Actual);
        }
        public ActionResult Modificar()
        {
            Singleton.Instance.Actual.Password = Singleton.Instance.DescifradoSDES(Singleton.Instance.Actual.LlaveSDES,Singleton.Instance.Actual.Password);  

            return View(Singleton.Instance.Actual);
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Modificar(FormCollection collection)
        {
            Singleton.Instance.Actual.Nombre = collection["Nombre"];
            Singleton.Instance.Actual.Apellido= collection["Apellido"];
            Singleton.Instance.Actual.Password = Singleton.Instance.CifradoSDES(Singleton.Instance.Actual.LlaveSDES, collection["Password"]);
            Singleton.Instance.Actual.eMail = collection["eMail"];
            var json = JsonConvert.SerializeObject(Singleton.Instance.Actual);
            var cliente = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var respose = await cliente.PutAsync("https://localhost:44338/api/Cuenta/ModificarUsuario/"+ Singleton.Instance.Actual.User, content);

            return RedirectToAction("ListaDeChats");
        }

        public ActionResult VerChat(string Emisor, string Receptor)
        {

            var Prueba = new Mensaje
            {
                Emisor= Emisor,Receptor= Receptor, IDEmisorReceptor =$"{Emisor},{Receptor}", EmisorMen = new Dictionary<DateTime, Extesiones>(), Id =null,
                ReceptorMen = new Dictionary<DateTime, Extesiones>()
            };
            var Ejemlo = new Extesiones
            {
                Texto= "Hola emisor", Extesion=""
            };
            Prueba.ReceptorMen.Add(DateTime.Now, Ejemlo);
            Ejemlo.Texto = "Hola Archivo";
            Ejemlo.Extesion = ".txt";
            Prueba.EmisorMen.Add(DateTime.Now, Ejemlo);
            var json = JsonConvert.SerializeObject(Prueba);
            return View(Prueba);
        }

        public ActionResult Tablas ()
        {
            return View();
        }
    }
}
