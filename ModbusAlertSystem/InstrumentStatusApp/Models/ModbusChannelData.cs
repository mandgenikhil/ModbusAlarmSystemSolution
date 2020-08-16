using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentStatusApp.Models
{
    public class ModbusChannelData
    {
        public List<InstrumentInfo> _instrument_info_list { get; set; }
    }
    public class InstrumentInfo
    {
        public string instrument_IP_Address { get; set; }
        public int instrument_Id { get; set; }
        public List<ChannelData> channel_data { get; set; }
    }
    public class ChannelData
    {
        public string channel_id { get; set; }
        public int channel_value { get; set; }
        public int high_value_threshold { get; set; }
        public int low_value_threshold { get; set; }
        public bool is_alert_on_high { get; set; }
        public bool is_alert_on_low { get; set; }

    }

}
