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
using System.Web.Routing;
using System.IO;

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
            Singleton.Instance.Actual = new Usuario();
            Singleton.Instance.ChatActual= new Mensaje();
            Singleton.Instance.UsuarioActual = "";
            string path = Server.MapPath("/");
            //or 
            string path2 = Server.MapPath("~");                // TODO: Add insert logic here
            var Nuevo = new Usuario
                {
                    User = collection["User"],
                    Password = collection["Password"]
                };
            //Cifrar Contraseña
            Singleton.Instance.UsuarioActual = collection["User"];
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

            
           
            //Devolver

            return View(Singleton.Instance.Actual);
        }
        [HttpPost]
        public ActionResult ListaDeChats(string UsuarioABuscar, string EmisorNuevo)
        {





            return RedirectToAction("VerChat", new RouteValueDictionary(new { Controller = "Main", Action = "VerChat", Emisor = EmisorNuevo, ReceptorRecibido = UsuarioABuscar }));




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

        public async System.Threading.Tasks.Task<ActionResult> VerChat(string Emisor, string ReceptorRecibido)
        {
            var cliente = new HttpClient();

            //verificar usuario
            var uri = "https://localhost:44338/api/Cuenta/VerificarUsuario/" + $"{ReceptorRecibido}/{Emisor}";
            var respose = await cliente.GetAsync(uri);

            if (respose.ReasonPhrase == "Not Found") //No existe el usuario
            {
                return RedirectToAction("ListaDeChats");
            }
            else
            {
            uri = "https://localhost:44338/api/Mensajes/ObtenerConversacion/" + $"{Emisor},{ReceptorRecibido}";
            respose = await cliente.GetAsync(uri);
                if (respose.ReasonPhrase == "OK")
                {
                    //return conversacion que me devuelve el api
                    var clienteMensaje = new HttpClient();
                    var lol = "https://localhost:44338/api/Mensajes/ObtenerConversacion/" + $"{Emisor},{ReceptorRecibido}";
                    var ChatExistente = await clienteMensaje.GetStringAsync(lol);
                    var enviar = JsonConvert.DeserializeObject<Mensaje>(ChatExistente);
                    Singleton.Instance.ChatActual = enviar;
                    #region Decifrado
                    uri = "https://localhost:44338/api/Cuenta/GetUsuario/" + ReceptorRecibido;
                    var Receptorn = await cliente.GetStringAsync(uri);
                    uri = "https://localhost:44338/api/Cuenta/GetUsuario/" + Emisor;
                    var usuarioEmisor = JsonConvert.DeserializeObject<Usuario>(await cliente.GetStringAsync(uri));
                    var UsuarioReceptor = JsonConvert.DeserializeObject<Usuario>(Receptorn);
                    #endregion
                    #region CifradoDeMensaje

                    var llaveTxt = usuarioEmisor.LlaveSDES + UsuarioReceptor.LlaveSDES;
                    if (llaveTxt >= 1023)
                    {
                        llaveTxt /= 2;
                        if (llaveTxt <= 512)
                        {
                            llaveTxt += 512;

                        }
                    }
                    #endregion
                    var json = JsonConvert.SerializeObject(enviar);
                    var nuevo = JsonConvert.DeserializeObject<Mensaje>(json);
                    var devolver = DecifrarDiccionario(llaveTxt, nuevo);
                    return View(devolver);
                }
                else//no existe
                {
                    //return nueva conversacion 

                    var NuevoChat = new Mensaje
                    {
                        Emisor = Emisor,
                        Receptor= ReceptorRecibido,
                    };
                    Singleton.Instance.ChatActual = NuevoChat;
            return View(NuevoChat);

                }
            }
            
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> VerChat(string Recibido, string Emisor ,string recep, string extencion,  string RutaArchivo)
        {
            var devolver = new Mensaje();
            if ((Recibido == "" || Recibido == " ") && RutaArchivo == "")
            {
                var cliente = new HttpClient();

                var uri = "https://localhost:44338/api/Cuenta/GetUsuario/" + recep ;
                var Receptor = await cliente.GetStringAsync(uri);
                uri = "https://localhost:44338/api/Cuenta/GetUsuario/" + Emisor;
                var usuarioEmisor = JsonConvert.DeserializeObject<Usuario>(await cliente.GetStringAsync(uri));
                var UsuarioReceptor = JsonConvert.DeserializeObject<Usuario>(Receptor);

                var llaveTxt = usuarioEmisor.LlaveSDES + UsuarioReceptor.LlaveSDES;
                if (llaveTxt >= 1023)
                {
                    llaveTxt /= 2;
                    if (llaveTxt <= 512)
                    {
                        llaveTxt += 512;

                    }
                }
                Recibido = Singleton.Instance.CifradoSDES(llaveTxt, Recibido);


                var aux = JsonConvert.SerializeObject(Singleton.Instance.ChatActual);
                var nuevo = JsonConvert.DeserializeObject<Mensaje>(aux);
                devolver = DecifrarDiccionario(llaveTxt, nuevo);
                
            return View(devolver);
            } //ambos vacios
            else if (RutaArchivo=="") 
            {
                var cliente = new HttpClient();
                var uri = "https://localhost:44338/api/Cuenta/GetUsuario/" + recep;
                var Receptor = await cliente.GetStringAsync(uri);
                uri = "https://localhost:44338/api/Cuenta/GetUsuario/" + Emisor;
                var usuarioEmisor = JsonConvert.DeserializeObject<Usuario>(await cliente.GetStringAsync(uri));
                var UsuarioReceptor = JsonConvert.DeserializeObject<Usuario>(Receptor);
                #region CifradoDeMensaje

                var llaveTxt = usuarioEmisor.LlaveSDES + UsuarioReceptor.LlaveSDES;
                if (llaveTxt >= 1023)
                {
                    llaveTxt /= 2;
                    if (llaveTxt <= 512)
                    {
                        llaveTxt += 512;

                    }
                }
                Recibido = Singleton.Instance.CifradoSDES(llaveTxt, Recibido);
                #endregion
                

                var tiempo = DateTime.Now;
                Singleton.Instance.ChatActual.MensajesOrdenados.Add($"{tiempo.Day}|{tiempo.Hour}|{tiempo.Minute}|{tiempo.Second}|{tiempo.Millisecond}", true);
                var exte = new Extesiones
                {
                    Texto = Recibido,
                    Extesion = ""
                };
                Singleton.Instance.ChatActual.EmisorMen.Add($"{tiempo.Day}|{tiempo.Hour}|{tiempo.Minute}|{tiempo.Second}|{tiempo.Millisecond}", exte);
                Singleton.Instance.ChatActual.IDEmisorReceptor = $"{Emisor},{recep}";
                var json = JsonConvert.SerializeObject(Singleton.Instance.ChatActual);
                var EnviarAlRecp = new Receptor
                {
                    HoraMensaje = $"{tiempo.Day}|{tiempo.Hour}|{tiempo.Minute}|{tiempo.Second}|{tiempo.Millisecond}",
                    Texto = Recibido,
                    Extension = "",
                    Origen = false,
                    Emisor = Emisor,
                    Recept = recep,
                    IDEmisorReceptor = $"{recep},{Emisor}"
                };
                var json2 = JsonConvert.SerializeObject(EnviarAlRecp);
                var MensajePalEmisor = new StringContent(json, Encoding.UTF8, "application/json");
                var Ruta = "https://localhost:44338/api/Mensajes/CrearConversacionEmisor";
                var MensajeAgregadoEmi = await cliente.PostAsync(Ruta, MensajePalEmisor); var MensajePalReceptor = new StringContent(json2, Encoding.UTF8, "application/json");
                Ruta = "https://localhost:44338/api/Mensajes/CrearConversacionReceptor";
                var MensajeAgregadoRecep = await cliente.PostAsync(Ruta, MensajePalReceptor);

                var aux = JsonConvert.SerializeObject(Singleton.Instance.ChatActual);
                var nuevo = JsonConvert.DeserializeObject<Mensaje>(aux);
                devolver = DecifrarDiccionario(llaveTxt, nuevo);
                

            return View(devolver);
            }//mensaje normal
            else if (Recibido=="")
            {

                var cliente = new HttpClient();
                var uri = "https://localhost:44338/api/Cuenta/GetUsuario/" + recep;
                var Receptor = await cliente.GetStringAsync(uri);
                uri = "https://localhost:44338/api/Cuenta/GetUsuario/" + Emisor;
                var usuarioEmisor = JsonConvert.DeserializeObject<Usuario>(await cliente.GetStringAsync(uri));
                var UsuarioReceptor = JsonConvert.DeserializeObject<Usuario>(Receptor);
                #region CifradoDeMensaje

                var llaveTxt = usuarioEmisor.LlaveSDES + UsuarioReceptor.LlaveSDES;
                if (llaveTxt >= 1023)
                {
                    llaveTxt /= 2;
                    if (llaveTxt <= 512)
                    {
                        llaveTxt += 512;

                    }
                }
                #endregion


                var tiempo = DateTime.Now;
                Singleton.Instance.ChatActual.MensajesOrdenados.Add($"{tiempo.Day}|{tiempo.Hour}|{tiempo.Minute}|{tiempo.Second}|{tiempo.Millisecond}", true);


                var lector = new StreamReader(RutaArchivo);
                var TexoPlano = Singleton.Instance.CifradoSDES(llaveTxt, lector.ReadToEnd());
                var exten = Path.GetExtension(RutaArchivo);

                var exte = new Extesiones
                {
                    Texto = TexoPlano,
                    Extesion = exten
                };
                Singleton.Instance.ChatActual.EmisorMen.Add($"{tiempo.Day}|{tiempo.Hour}|{tiempo.Minute}|{tiempo.Second}|{tiempo.Millisecond}", exte);
                Singleton.Instance.ChatActual.IDEmisorReceptor = $"{Emisor},{recep}";
                var json = JsonConvert.SerializeObject(Singleton.Instance.ChatActual);
                var EnviarAlRecp = new Receptor
                {
                    HoraMensaje = $"{tiempo.Day}|{tiempo.Hour}|{tiempo.Minute}|{tiempo.Second}|{tiempo.Millisecond}",
                    Texto = Recibido,
                    Extension = "",
                    Origen = false,
                    Emisor = Emisor,
                    Recept = recep,
                    IDEmisorReceptor = $"{recep},{Emisor}"
                };
                var json2 = JsonConvert.SerializeObject(EnviarAlRecp);
                var MensajePalEmisor = new StringContent(json, Encoding.UTF8, "application/json");
                var Ruta = "https://localhost:44338/api/Mensajes/CrearConversacionEmisor";
                var MensajeAgregadoEmi = await cliente.PostAsync(Ruta, MensajePalEmisor); var MensajePalReceptor = new StringContent(json2, Encoding.UTF8, "application/json");
                Ruta = "https://localhost:44338/api/Mensajes/CrearConversacionReceptor";
                var MensajeAgregadoRecep = await cliente.PostAsync(Ruta, MensajePalReceptor);

                var aux = JsonConvert.SerializeObject(Singleton.Instance.ChatActual);
                var nuevo = JsonConvert.DeserializeObject<Mensaje>(aux);
                devolver = DecifrarDiccionario(llaveTxt, nuevo);


                return View(devolver);
            }//mensaje Archivo
            else
            {
                var cliente = new HttpClient();

                var uri = "https://localhost:44338/api/Cuenta/GetUsuario/" + recep;
                var Receptor = await cliente.GetStringAsync(uri);
                uri = "https://localhost:44338/api/Cuenta/GetUsuario/" + Emisor;
                var usuarioEmisor = JsonConvert.DeserializeObject<Usuario>(await cliente.GetStringAsync(uri));
                var UsuarioReceptor = JsonConvert.DeserializeObject<Usuario>(Receptor);

                var llaveTxt = usuarioEmisor.LlaveSDES + UsuarioReceptor.LlaveSDES;
                if (llaveTxt >= 1023)
                {
                    llaveTxt /= 2;
                    if (llaveTxt <= 512)
                    {
                        llaveTxt += 512;

                    }
                }
                Recibido = Singleton.Instance.CifradoSDES(llaveTxt, Recibido);


                var aux = JsonConvert.SerializeObject(Singleton.Instance.ChatActual);
                var nuevo = JsonConvert.DeserializeObject<Mensaje>(aux);
                devolver = DecifrarDiccionario(llaveTxt, nuevo);

                return View(devolver);
            }//Vacios
        }
        public ActionResult NuevoChat()
        {

            return View();
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> NuevoChat(string Emisor, string Receptor)
        {
            var cliente = new HttpClient();


            var uri = "https://localhost:44338/api/Cuenta/VerificarUsuario/" + $"{Receptor}/{Emisor}";
            var respose = await cliente.GetAsync(uri);

            return View();
        }

        public ActionResult Obtener_Mensaje(string Texto)
        {
            return View();
        }

        public Mensaje DecifrarDiccionario(int llaveTxt, Mensaje Cifrado)
        {
            var devolver = Cifrado;
            foreach (var item in devolver.EmisorMen.Keys.ToList())
            {
                var Decifrado = Singleton.Instance.DescifradoSDES(llaveTxt, devolver.EmisorMen[item].Texto);
                devolver.EmisorMen[item].Texto = Decifrado;
            }
            foreach (var item in devolver.ReceptorMen.Keys.ToList())
            {
                var Decifrado = Singleton.Instance.DescifradoSDES(llaveTxt, devolver.ReceptorMen[item].Texto);
                devolver.ReceptorMen[item].Texto = Decifrado;
            }
            return devolver;
        }

        public async System.Threading.Tasks.Task<ActionResult> EliminarChat( string Emisor,string ReceptorRecibido)
        {
            var cliente = new HttpClient();
            var json = JsonConvert.SerializeObject(new Usuario());
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var uri = "https://localhost:44338/api/Cuenta/Eliminarcontacto/" + Emisor +$"/{Emisor},{ReceptorRecibido}";
            var summit = await cliente.PutAsync(uri,content);
            return Redirect("ListaDeChats");
        }

        public ActionResult LogOut()
        {
            Singleton.Instance.Actual = new Usuario();
            Singleton.Instance.ChatActual = new Mensaje();
            Singleton.Instance.UsuarioActual = string.Empty;
            Singleton.Instance.Log= false;
            
            return RedirectToAction("Login");
        }
    }
}
