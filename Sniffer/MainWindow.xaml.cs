using PacketDotNet;
using SharpPcap;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
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

namespace Sniffer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // метод для получения списка устройств
        public CaptureDeviceList deviceList = CaptureDeviceList.Instance;
        ICaptureDevice captureDevice;
        ObservableCollection<MyTable> col = new ObservableCollection<MyTable>();
        int num;
        Collection<Tcp> tcpcol = new Collection<Tcp>();
        private struct Tcp
        {
            public int pos;
            public string srcIp;
            public string dstIp;
            public uint srcPort;
            public uint dstPort;
            public uint seqNumber;       /*значения в десятеричной форме*/
            public uint ackNumber;
            public int offset;
            public byte flags;
            public bool CWR;
            public bool ECE;
            public bool URG;
            public bool ACK;
            public bool PSH;
            public bool RST;
            public bool SYN;
            public bool FIN;
            public ushort winSize;
            public ushort chksum;
            public int urgpoint;
        }

        public MainWindow()
        {
            InitializeComponent();
            rbox1.Document.Blocks.Clear();

            if (deviceList.Count == 0)
                listBox1.Items[0] = "Устройств не обнаружено";
            else
            {
                for (int i = 0; i < deviceList.Count; i++)
                    listBox1.Items.Add(deviceList[i].Description);
            }

        }

        void Program_OnPacketArrivalAsync(object sender, CaptureEventArgs e)
        {
            // парсинг всего пакета
            Packet packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            // получение только TCP пакета из всего фрейма
            TcpPacket tcpPacket = (TcpPacket)packet.Extract(typeof(TcpPacket));
            // получение только IP пакета из всего фрейма
            //IpPacket ipPacket = (IpPacket)packet.Extract(typeof(IpPacket));
            // получение только UDP пакета из всего фрейма
            UdpPacket udpPacket = (UdpPacket)packet.Extract(typeof(UdpPacket));
            ARPPacket arpPacket = (ARPPacket)packet.Extract(typeof(ARPPacket));
            //LinuxSLLPacket sllPacket = (LinuxSLLPacket)packet.Extract(typeof(LinuxSLLPacket));
            ICMPv4Packet icmpPacket = (ICMPv4Packet)packet.Extract(typeof(ICMPv4Packet));
            IGMPv2Packet igmpPacket = (IGMPv2Packet)packet.Extract(typeof(IGMPv2Packet));
            
            DateTime time = e.Packet.Timeval.Date.ToLocalTime();
            string StrTime = time.Hour+ ":" + time.Minute+ ":" +time.Second+ ":" + time.Millisecond;
            string prt = "";

            if (tcpPacket != null)
            {//Dispatcher.BeginInvoke(new ThreadStart(delegate { rbox1.AppendText(Convert.ToString(tcpPacket)+"\n"); }));
                prt = "TCP";
                int len = e.Packet.Data.Length;
                IpPacket ipPacket = (IpPacket)tcpPacket.ParentPacket;
               
                Tcp tpck = new Tcp();
                tpck.pos = num;
                tpck.srcIp = ipPacket.SourceAddress.ToString();
                tpck.dstIp = ipPacket.DestinationAddress.ToString();
                // порт отправителя
                tpck.srcPort = tcpPacket.SourcePort;
                // порт получателя
                tpck.dstPort = tcpPacket.DestinationPort;
                tpck.seqNumber = tcpPacket.SequenceNumber;       /*значения в десятеричной форме*/
                tpck.ackNumber = tcpPacket.AcknowledgmentNumber;
                tpck.offset = tcpPacket.DataOffset;
                tpck.flags = tcpPacket.AllFlags;
                tpck.CWR = tcpPacket.CWR;
                tpck.ECE = tcpPacket.ECN;
                tpck.URG = tcpPacket.Urg;
                tpck.ACK = tcpPacket.Ack;
                tpck.PSH = tcpPacket.Psh;
                tpck.RST = tcpPacket.Rst;
                tpck.SYN = tcpPacket.Syn;
                tpck.FIN = tcpPacket.Fin;
                tpck.winSize = tcpPacket.WindowSize;
                tpck.chksum = tcpPacket.Checksum;
                tpck.urgpoint = tcpPacket.UrgentPointer;

                // данные пакета
                //var data = tcpPacket.PayloadPacket;

                // List<MyTable> result = new List<MyTable>(1);
                //result.Add(new MyTable(num, num, srcIp, dstIp, "prt", len, "inf"));
                //datagrid1.ItemsSource = result;

                //Dispatcher.BeginInvoke(new ThreadStart(delegate { listBox2.Items.Add(Convert.ToString(tcpPacket)); }));
                //Dispatcher.BeginInvoke(new ThreadStart(delegate { datagrid1.ItemsSource = result; }));

                //col.Add(new MyTable(num, num, srcIp, dstIp, "prt", len, "inf"));
                var hex = BitConverter.ToString(tcpPacket.PayloadData);
                //var hex = tcpPacket.PrintHex();
                hex = hex.Replace("-", "");
                byte[] raw = new byte[hex.Length / 2];
                for (int i = 0; i < raw.Length; i++)
                {
                    raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                }

                string data = Encoding.ASCII.GetString(raw);
                //tcpcol.Add(tpck);

                Dispatcher.BeginInvoke(new ThreadStart(delegate { col.Add(new MyTable(num, StrTime, tpck.srcIp, tpck.dstIp, prt, len, Convert.ToString(data)));
                                                                  datagrid1.ItemsSource = col;
                                                                  tpck.pos = num;
                                                                  num++; tcpcol.Add(tpck);
                    /*rbox1.AppendText("Hex: "+hex + "\n" + seqNumber + "\n" + ackNumber + "\n" + offset + "\n" + flags + "\n" + CWR + "\n" + ECE + "\n" + URG + "\n" + ACK + "\n" + PSH + "\n" + RST + "\n" + SYN + "\n" + FIN + "\n" + winSize + "\n" + chksum + "\n" + urgpoint + "\n");    */
                }));
            }

            if (udpPacket != null)
            {
                prt = "UDP";
                int len = e.Packet.Data.Length;
                IpPacket ipPacket = (IpPacket)udpPacket.ParentPacket;
                var srcIp = ipPacket.SourceAddress.ToString();
                var dstIp = ipPacket.DestinationAddress.ToString();
                // порт отправителя
                var srcPort = udpPacket.SourcePort.ToString();
                // порт получателя
                var dstPort = udpPacket.DestinationPort.ToString();
                // данные пакета
                var data = udpPacket.PayloadPacket;

                Dispatcher.BeginInvoke(new ThreadStart(delegate { col.Add(new MyTable(num, StrTime, srcIp, dstIp, prt, len, Convert.ToString(data))); datagrid1.ItemsSource = col; num++; }));
            }

            if (arpPacket != null)
            {
                prt = "ARP";
                int len = e.Packet.Data.Length;
                var srcIp = arpPacket.SenderProtocolAddress.ToString();
                var dstIp = arpPacket.TargetProtocolAddress.ToString();
                // данные пакета
                var data = arpPacket.PayloadPacket;

                Dispatcher.BeginInvoke(new ThreadStart(delegate { col.Add(new MyTable(num, StrTime, srcIp, dstIp, prt, len, Convert.ToString(data))); datagrid1.ItemsSource = col; num++; }));
            }

            if (icmpPacket != null)
            {
                prt = "ICMPv4";
                int len = e.Packet.Data.Length;
                IpPacket ipPacket = (IpPacket)icmpPacket.ParentPacket;
                var srcIp = ipPacket.SourceAddress.ToString();
                var dstIp = ipPacket.DestinationAddress.ToString();
                // данные пакета
                var data = icmpPacket.PayloadPacket;

                Dispatcher.BeginInvoke(new ThreadStart(delegate { col.Add(new MyTable(num, StrTime, srcIp, dstIp, prt, len, Convert.ToString(data))); datagrid1.ItemsSource = col; num++; }));
            }

            if (igmpPacket != null)
            {
                prt = "IGMPv2";
                int len = e.Packet.Data.Length;
                IpPacket ipPacket = (IpPacket)igmpPacket.ParentPacket;
                var srcIp = ipPacket.SourceAddress.ToString();
                var dstIp = ipPacket.DestinationAddress.ToString();
                // данные пакета
                var data = igmpPacket.PayloadPacket;

                Dispatcher.BeginInvoke(new ThreadStart(delegate { col.Add(new MyTable(num, StrTime, srcIp, dstIp, prt, len, Convert.ToString(data))); datagrid1.ItemsSource = col; num++; }));
            }
            /* 
         
             DateTime time = e.Packet.Timeval.Date;
             int len = e.Packet.Data.Length;
             String str = time.Hour + ":" + time.Minute +":"+time.Second + ":" + time.Millisecond + ":" + len;
             listBox2.Items.Add(str);

          /*   // парсинг всего пакета
             Packet packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
             // получение только TCP пакета из всего фрейма
             var tcpPacket = TcpPacket.GetEncapsulated(packet);
             // получение только IP пакета из всего фрейма
             var ipPacket = IpPacket.GetEncapsulated(packet);
             if (tcpPacket != null && ipPacket != null)
                 {
                 DateTime time = e.Packet.Timeval.Date;
                 int len = e.Packet.Data.Length;

                 // IP адрес отправителя
                 var srcIp = ipPacket.SourceAddress.ToString();
                 // IP адрес получателя
                 var dstIp = ipPacket.DestinationAddress.ToString();

                 // порт отправителя
                 var srcPort = tcpPacket.SourcePort.ToString();
                 // порт получателя
                 var dstPort = tcpPacket.DestinationPort.ToString();
                 // данные пакета
                 var data = tcpPacket.PayloadPacket; 
                 }*/
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // выбираем устройство в спиcке
                captureDevice = deviceList[listBox1.SelectedIndex];

                int readTimeoutMilliseconds = 1000;
                // открываем в режиме promiscuous, поддерживается также нормальный режим
                captureDevice.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
                // регистрируем событие, которое срабатывает, когда пришел новый пакет
                captureDevice.OnPacketArrival += new PacketArrivalEventHandler(Program_OnPacketArrivalAsync);
                if (col.Count == 0)
                    num = 1;
                // начинаем захват пакетов
                captureDevice.StartCapture(); 
            }
            catch
            {
                MessageBox.Show("Интерфейс не выбран");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                captureDevice.StopCapture();
                captureDevice.Close();
            }
            catch {MessageBox.Show("Интерфейс не выбран"); }
            //num = 1;
            //col.Clear();
        }

        private void datagrid1_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            rbox1.Document.Blocks.Clear();
            //System.Data.DataRowView row = (System.Data.DataRowView)datagrid1.SelectedItems[0];
            //Приводим строку к классу, объект которого отображается в таблице
            //MyTable n = (MyTable)datagrid1.Items[datagrid1.SelectedIndex] 
            try
            {
                DataGrid dataGrid = sender as DataGrid;
                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                DataGridCell RowColumn = dataGrid.Columns[0].GetCellContent(row).Parent as DataGridCell;
                int CellValue = Convert.ToInt32(((TextBlock)RowColumn.Content).Text);
                //int row = n.number;
                //for (int i = CellValue; i < CellValue; i++)
                for (int j = 0; j < tcpcol.Count(); j++)
                    if (tcpcol[j].pos == CellValue)
                        // foreach (Tcp t in tcpcol)
                        //rbox1.AppendText(t.pos+" "+t.srcIp + " " + t.dstIp + "\n");
                        rbox1.AppendText("№ пакета: " + tcpcol[j].pos + "\n" +
                                         "Порт отправителя: " + tcpcol[j].srcPort + "\n" +
                                         "Порт получателя: " + tcpcol[j].dstPort + "\n" +
                                         "Порядковый номер (SN): " + tcpcol[j].seqNumber.ToString("X") + "\n" +
                                         "Номер подтверждения (ACK SN): " + tcpcol[j].ackNumber.ToString("X") + "\n" +
                                         "Offset: " + tcpcol[j].offset + "\n" +
                                         "Flags: " + tcpcol[j].flags + "\n" +
                                         "  CWR: " + tcpcol[j].CWR + "\n" +
                                         "  ECE: " + tcpcol[j].ECE + "\n" +
                                         "  URG: " + tcpcol[j].URG + "\n" +
                                         "  ACK: " + tcpcol[j].ACK + "\n" +
                                         "  PSH: " + tcpcol[j].PSH + "\n" +
                                         "  RST: " + tcpcol[j].RST + "\n" +
                                         "  SYN: " + tcpcol[j].SYN + "\n" +
                                         "  FIN: " + tcpcol[j].FIN + "\n" +
                                         "Размер окна: " + tcpcol[j].winSize + "\n" +
                                         "Контрольная сумма: " + tcpcol[j].chksum.ToString("X") + "\n" +
                                         "Срочность: " + tcpcol[j].urgpoint + "\n");
            }
            catch(Exception err)
            {
                MessageBox.Show("Ошибка: " + err.Message);
            }
        }
    }

    class MyTable
    {
        public MyTable(int number, string time, string src, string dst, string protocol, int length, string Info)
        {
            this.number = number;
            this.time = time;
            this.src = src;
            this.dst = dst;
            this.protocol = protocol;
            this.length = length;
            this.Info = Info;
        }
        public int number { get; set; }
        public string time { get; set; }
        public string src { get; set; }
        public string dst { get; set; }
        public string protocol { get; set; }
        public int length { get; set; }
        public string Info { get; set; }
    }
}
