using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentStatusApp.Models
{
    public class InstrumentStatus
    {
        public int ID { get; set; }
        public int CH1 { get; set; }
        public int CH2 { get; set; }
        public int CH3 { get; set; }
        public int CH4 { get; set; }
    }

    public class Alert
    {
        public int ID { get; set; }
        public string Channel { get; set; }
        public string Message { get; set; }
        public int PV { get; set; }

    }
}
