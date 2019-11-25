using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Back.Models
{
    public class Chat
    {
        List<Mensaje> Emisor { get; set; }
        List<Mensaje> Receptor { get; set; }
    }
}
