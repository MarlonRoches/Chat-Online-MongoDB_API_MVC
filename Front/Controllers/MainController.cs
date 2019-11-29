﻿using System;
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

        public async System.Threading.Tasks.Task<ActionResult> VerChat(string Emisor, string Receptor)
        {
            var cliente = new HttpClient();

            //verificar usuario
            var uri = "https://localhost:44338/api/Cuenta/VerificarUsuario/" + $"{Receptor}";
            var respose = await cliente.GetAsync(uri);

            if (respose.ReasonPhrase == "Not Found") //No existe el usuario
            {
                return RedirectToAction("ListaDeChats");
            }
            else
            {
            uri = "https://localhost:44338/api/Mensajes/ObtenerConversacion/" + $"{Emisor},{Receptor}";
            respose = await cliente.GetAsync(uri);
                if (respose.ReasonPhrase =="OK")
                {

                    //return conversacion que me devuelve el api
                    var clienteMensaje = new HttpClient();
                    var lol = "https://localhost:44338/api/Mensajes/ObtenerConversacion/" + $"{Emisor},{Receptor}";
                    var ChatExistente = await clienteMensaje.GetStringAsync(lol);

                    //var ChatExistente = new Mensaje();

                    var enviar = JsonConvert.DeserializeObject<Mensaje>(ChatExistente);
                    Singleton.Instance.ChatActual = enviar;
                    // Singleton.Instance.ChatActual = ChatExistente;
                     return View(enviar);
                }
                else//no existe
                {
                    //return nueva conversacion 

                    var NuevoChat = new Mensaje
                    {
                        Emisor = Emisor,
                        Receptor= Receptor,
                    };
                    Singleton.Instance.ChatActual = NuevoChat;
            return View(NuevoChat);

                }
            }
            
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> VerChat(string Texto, string Emisor ,string recep, string extencion, string ruta)
        {
            var cliente = new HttpClient();
            var tiempo = DateTime.Now;

            Singleton.Instance.ChatActual.MensajesOrdenados.Add($"{tiempo.Day}|{tiempo.Hour}|{tiempo.Minute}|{tiempo.Second}|{tiempo.Millisecond}", true);

            var exte = new Extesiones
            {
                Texto = Texto,
                Extesion = ""
            };
            
            Singleton.Instance.ChatActual.EmisorMen.Add($"{tiempo.Day}|{tiempo.Hour}|{tiempo.Minute}|{tiempo.Second}|{tiempo.Millisecond}",exte);
            Singleton.Instance.ChatActual.IDEmisorReceptor = $"{Emisor},{recep}";
            var json = JsonConvert.SerializeObject(Singleton.Instance.ChatActual);
           var EnviarAlRecp = new Receptor
            {
                HoraMensaje = $"{tiempo.Day}|{tiempo.Hour}|{tiempo.Minute}|{tiempo.Second}|{tiempo.Millisecond}",
                Texto= Texto,
                Extension = "",
                Origen= false,
                Emisor = Emisor,
                Recept = recep,
                IDEmisorReceptor= $"{recep},{Emisor}"
            };
            var json2 = JsonConvert.SerializeObject(EnviarAlRecp);


            var MensajePalEmisor = new StringContent(json, Encoding.UTF8, "application/json");
            var lol = "https://localhost:44338/api/Mensajes/CrearConversacionEmisor";
            var MensajeAgregadoEmi = await cliente.PostAsync(lol, MensajePalEmisor);


            var MensajePalReceptor = new StringContent(json2, Encoding.UTF8, "application/json");
            lol = "https://localhost:44338/api/Mensajes/CrearConversacionReceptor";
            var MensajeAgregadoRecep = await cliente.PostAsync(lol, MensajePalReceptor);

            return View(Singleton.Instance.ChatActual);
                }
        public ActionResult NuevoChat()
        {

            return View();
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> NuevoChat(string Emisor, string Receptor)
        {
            var cliente = new HttpClient();


            var uri = "https://localhost:44338/api/Cuenta/VerificarUsuario/" + $"{Receptor}";
            var respose = await cliente.GetAsync(uri);

            return View();
        }

        public ActionResult Obtener_Mensaje(string Texto)
        {
            return View();
        }
    }
}
