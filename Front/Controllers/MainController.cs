using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Back.Models;
using Back.Data;
using Newtonsoft.Json;
namespace Front.Controllers
{
    public class MainController : Controller
    {
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
                    Apellido= collection["Apellido"],
                    eMail= collection["eMail"],
                    User= collection["User"],
                    Password = collection["Password"]
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
        public ActionResult Login(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
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
                //Enviar Api y generar Token
                return RedirectToAction("");
            }
            catch
            {
                return View();
            }
        }


    }
}
