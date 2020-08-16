using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace InstrumentStatusApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer _dispatcher;        
        public SqlClientConnect _sqlClientConnect;
        public MainWindow()
        {
            _dispatcher = new DispatcherTimer(Config.GetTimeSpan(), DispatcherPriority.Background,
                 Display_Current_Status, Dispatcher.CurrentDispatcher);
            _dispatcher.Start();            
            _sqlClientConnect = new SqlClientConnect();
            InitializeComponent();
        }
        private void Display_Current_Status(object sender, EventArgs e)
        {                        
            _sqlClientConnect.PollInstrumentsStatus();
           
            //InfoDetails.ItemsSource = _sqlClientConnect._modbusChannelData._instrument_info_list;
        }

        private void Instrument_Details_Click(object sender, RoutedEventArgs e)
        {
            //
            InfoDetails.ItemsSource = _sqlClientConnect._instrumentStatusList;

        }

        private void Alert_Details_Click(object sender, RoutedEventArgs e)
        {
            //
            InfoDetails.ItemsSource = _sqlClientConnect._alertList;
        }

    }
}
