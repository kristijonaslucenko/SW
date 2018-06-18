using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMC_SW.Models
{
    class Packet
    {
        public int Id { get; set;}
        public byte Cmd { get; set; }
        public byte Data { get; set; }
        public byte Crc { get; set; }
    }
}       
