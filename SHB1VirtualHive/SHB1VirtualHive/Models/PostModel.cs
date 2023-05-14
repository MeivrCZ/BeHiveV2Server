using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHB1VirtualHive.Models
{
    internal class PostModel
    {
        public int Id { get; set; }
        public string Serial { get; set; }
        public long unixTimestamp { get; set; }
        public double outsideTemperature { get; set; }
        public double pressure { get; set; }
        public double humidity { get; set; }
        public double weight { get; set; }
        public double insideTemperature { get; set; }
    }
}
