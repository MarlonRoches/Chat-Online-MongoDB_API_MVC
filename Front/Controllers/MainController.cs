using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Back.Models;
using Newtonsoft.Json;
namespace Front.Controllers
{
    public class MainController : Controller
    {
        // GET: Main
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
                var json = JsonConvert.SerializeObject(Nuevo);
                //Cifrar Contraseña
                return RedirectToAction("Index");
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
                    Nombre = collection["Nombre"],
                    Apellido = collection["Apellido"],
                    eMail = collection["eMail"],
                    User = collection["User"],
                    Password = collection["Password"]
                };
                var json = JsonConvert.SerializeObject(Nuevo);
                //Cifrar Contraseña
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
