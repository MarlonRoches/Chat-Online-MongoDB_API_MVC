using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Back.Models
{
    public class Usuario
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string User { get; set; }
        public string eMail { get; set; }
        public string Password { get; set; }
        public int LlaveSDES { get; set;}
    }
}
