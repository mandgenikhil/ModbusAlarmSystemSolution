using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstrumentStatusApp.Models;

namespace InstrumentStatusApp
{
    public class SqlClientConnect
    {
        

        public ModbusChannelData _modbusChannelData;
        public List<InstrumentStatus> _instrumentStatusList;
        public List<Alert> _alertList;

        public SqlClientConnect()
        {
            _modbusChannelData = new ModbusChannelData();
            _instrumentStatusList = new List<InstrumentStatus>();           
            _modbusChannelData._instrument_info_list = new List<InstrumentInfo>();
            LoadData();
            LoadAlerts();
        }

        private void LoadAlerts()
        {
            _alertList = new List<Alert>();
            using (SqlConnection connection = new SqlConnection(
               Config.Get_SQL_Connection_String()))
            {
                SqlCommand command = new SqlCommand("select *  from Alarm", connection);
                command.Connection.Open();
                using (SqlDataReader oReader = command.ExecuteReader())
                {

                    while (oReader.Read())
                    {
                        _alertList.Add(new Alert()
                        {
                            ID = (int)oReader["ID"],
                            Channel = (string)oReader["Channel"],
                            PV = (int)oReader["PV"],
                            Message = (string)oReader["Message"]

                        }) ;


                    }

                }

            }
        }

        private void LoadData()
        {
            try
            {
                
                using (SqlConnection connection = new SqlConnection(
                Config.Get_SQL_Connection_String()))
                {
                    SqlCommand command = new SqlCommand("select *  from Status", connection);
                    command.Connection.Open();                                        
                    using (SqlDataReader oReader = command.ExecuteReader())
                    {

                        while (oReader.Read())
                        {
                            _modbusChannelData._instrument_info_list.Add(new
                                InstrumentInfo()
                            {
                                instrument_Id = (int)oReader["ID"],
                                channel_data = LoadInstrumentData(oReader)
                               
                            });                          
                        }

                    }
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private List<ChannelData> LoadInstrumentData(SqlDataReader reader)
        {
            var channel_data = new List<ChannelData>();
            for (int i=1;i<=Config.Get_Number_Of_Channels();i++)
            {
                channel_data.Add(new ChannelData()
                {
                    channel_id = "CH" + i.ToString(),
                    channel_value = (int)reader["CH"+ i.ToString()],
                    high_value_threshold = (int)reader["HI"],
                    low_value_threshold = (int)reader["LI"],
                });
            }
            return channel_data;
        }

        public void PollInstrumentsStatus()
        {
            try
            {
                LoadAlerts();


                using (SqlConnection connection = new SqlConnection(
                    Config.Get_SQL_Connection_String()))
                {
                    SqlCommand command = new SqlCommand("select *  from Status", connection);
                    command.Connection.Open();
                    using (SqlDataReader oReader = command.ExecuteReader())
                    {
                        _instrumentStatusList = new List<InstrumentStatus>();

                        while (oReader.Read())
                        {

                            var instrument = _modbusChannelData._instrument_info_list.Find(
                                x => x.instrument_Id == (Int32)oReader["ID"]);

                            foreach (var channelData in instrument.channel_data)
                            {
                                channelData.channel_value = (Int32)oReader[channelData.channel_id];
                            }

                            
                            _instrumentStatusList.Add(new InstrumentStatus()
                            {
                                ID = instrument.instrument_Id,
                                CH1 = instrument.channel_data.Find(x=>x.channel_id == "CH1").channel_value,
                                CH2 = instrument.channel_data.Find(x => x.channel_id == "CH2").channel_value,
                                CH3 = instrument.channel_data.Find(x => x.channel_id == "CH3").channel_value,
                                CH4 = instrument.channel_data.Find(x => x.channel_id == "CH4").channel_value,

                            }); ;
                        }
                        SendAlert();
                    }
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void SendAlert()
        {            
            foreach (var item in _modbusChannelData._instrument_info_list)
            {

                foreach (var channelData in item.channel_data)
                {
                    var value = GetStatus(channelData.channel_value, channelData.high_value_threshold, channelData.low_value_threshold);

                    switch (value)
                    {
                        case "Normal":
                            if (channelData.is_alert_on_high == true)
                            {
                                InsertAlarmData(item.instrument_Id, channelData.channel_id, channelData.channel_value, "Channel Getting Normal From High");
                                channelData.is_alert_on_high = false;
                            }
                            else if (channelData.is_alert_on_low == true)
                            {
                                InsertAlarmData(item.instrument_Id, channelData.channel_id, channelData.channel_value, "Channel Getting Normal From Low");
                                channelData.is_alert_on_low = false;
                            }
                            break;
                        case "Low":
                            if (channelData.is_alert_on_low == false)
                            {
                                if (channelData.is_alert_on_high == true)
                                {
                                    InsertAlarmData(item.instrument_Id, channelData.channel_id, channelData.channel_value, "Channel Getting Normal From High");
                                    channelData.is_alert_on_high = false;
                                }
                                InsertAlarmData(item.instrument_Id, channelData.channel_id, channelData.channel_value, "Channel Low Alarm From Normal");
                                channelData.is_alert_on_low = true;
                            }
                            break;
                        case "High":
                            if (channelData.is_alert_on_high == false)
                            {
                                if (channelData.is_alert_on_low == true)
                                {
                                    InsertAlarmData(item.instrument_Id, channelData.channel_id, channelData.channel_value, "Channel Getting Normal From Low");
                                    channelData.is_alert_on_low = false;
                                }
                                // Raise an alarm                                 
                                InsertAlarmData(item.instrument_Id, channelData.channel_id, channelData.channel_value, "Channel High Alarm From Normal");
                                channelData.is_alert_on_high = true;
                            }
                            break;
                        default:
                            break;
                    }

                }
            }

        }

        public void InsertAlarmData(int id, string channelName, int value, string message)
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(
                Config.Get_SQL_Connection_String()))
                {
                    SqlCommand command = new SqlCommand(string.Format("insert into alarm (ID, Channel, PV, Message) values({0} ,'{1}', {2}, '{3}') "
                        , id, channelName, value, message), connection);
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private string GetStatus(int value, int high_limit, int low_limit)
        {
            if (value <= high_limit && value >= low_limit)
            {
                return "Normal";
            }
            else if (value > high_limit)
            {
                return "High";
            }
            else if (value < low_limit)
            {
                return "Low";
            }

            return "Normal";

        }     
    }

}
