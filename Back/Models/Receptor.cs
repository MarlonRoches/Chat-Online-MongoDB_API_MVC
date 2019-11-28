using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Back.Models
{
    public class Receptor
    {
        public string HoraMensaje { get; set; }
        public string Texto { get; set; }
        public string Extension { get; set; }
        public bool Origen { get; set; }
        public string Emisor { get; set; }
        public string Recept { get; set; }
        public string IDEmisorReceptor { get; set; }
    

    }
}
