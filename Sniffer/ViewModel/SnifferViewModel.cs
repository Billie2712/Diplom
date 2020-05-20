using SharpPcap;
using PacketDotNet;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Threading;
using System.Net.NetworkInformation;
using SharpPcap.LibPcap;
using System.IO;
using SharpPcap.AirPcap;

namespace Sniffer.ViewModel
{
    public class SnifferViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        //Fields
        private IMainWindowsCodeBehind _MainCodeBehind;
        // выбираем устройство в спиcке
        public ICaptureDevice captureDevice;
        //счётчик пакетов
        int num;
        // the output file
        CaptureFileWriterDevice captureFileWriter;
        // the input file
        public ICaptureDevice captureFileReader;
        //строка в таблице
        private ObservableCollection<MyTable> _col = new ObservableCollection<MyTable>();
        public ObservableCollection<MyTable> Col
        {
            get { return _col; }
            set { _col = value; OnPropertyChanged("Col"); }
        }
        //коллекция tcp пакетов
        private ObservableCollection<Tcp> _tcpcol = new ObservableCollection<Tcp>();
        public ObservableCollection<Tcp> TcpCol
        {
            get { return _tcpcol; }
            set { _tcpcol = value; OnPropertyChanged("TcpCol"); }
        }
        private ObservableCollection<Udp> _udpcol = new ObservableCollection<Udp>();
        public ObservableCollection<Udp> UdpCol
        {
            get { return _udpcol; }
            set { _udpcol = value; OnPropertyChanged("UdpCol"); }
        }
        private ObservableCollection<Arp> _arpcol = new ObservableCollection<Arp>();
        public ObservableCollection<Arp> ArpCol
        {
            get { return _arpcol; }
            set { _arpcol = value; OnPropertyChanged("ArpCol"); }
        }
        private ObservableCollection<Icmp> _icmpcol = new ObservableCollection<Icmp>();
        public ObservableCollection<Icmp> IcmpCol
        {
            get { return _icmpcol; }
            set { _icmpcol = value; OnPropertyChanged("IcmpCol"); }
        }
        private ObservableCollection<Igmp> _igmpcol = new ObservableCollection<Igmp>();
        public ObservableCollection<Igmp> IgmpCol
        {
            get { return _igmpcol; }
            set { _igmpcol = value; OnPropertyChanged("IgmpCol"); }
        }
        Dispatcher _dispatcher;

        //ctor
        public SnifferViewModel() { }
        public SnifferViewModel(IMainWindowsCodeBehind codeBehind)
        {
            if (codeBehind == null) throw new ArgumentNullException(nameof(codeBehind));

            _MainCodeBehind = codeBehind;

            _dispatcher = Dispatcher.CurrentDispatcher;

            SdeviceList = LibPcapLiveDeviceList.Instance;
            DeviceList = new ObservableCollection<LibPcapLiveDevice>();
            DeviceListDescription = new ObservableCollection<string>();
            CurrentDevice = "";
            for (int i = 0; i < SdeviceList.Count(); i++)
            {
                DeviceList.Add(SdeviceList[i]);
                if (SdeviceList[i].Interface.FriendlyName != null)
                    DeviceListDescription.Add(SdeviceList[i].Interface.FriendlyName + ": " + SdeviceList[i].Description);
                else
                    DeviceListDescription.Add(SdeviceList[i].Description);
            }
            Col = new ObservableCollection<MyTable>();
            TcpCol = new ObservableCollection<Tcp>();
            UdpCol = new ObservableCollection<Udp>();
            ArpCol = new ObservableCollection<Arp>();
            IcmpCol = new ObservableCollection<Icmp>();
            IgmpCol = new ObservableCollection<Igmp>();
        }



        // метод для получения списка устройств
        private ObservableCollection<LibPcapLiveDevice> _DeviceList;
        public ObservableCollection<LibPcapLiveDevice> DeviceList
        {
            get { return _DeviceList; }
            set { _DeviceList = value; OnPropertyChanged("DeviceList"); }
        }
        private LibPcapLiveDeviceList _sdeviceList;// = CaptureDeviceList.Instance;
        public LibPcapLiveDeviceList SdeviceList
        {
            get { return _sdeviceList; }
            set { _sdeviceList = value; OnPropertyChanged("SdeviceList"); }
        }
        private ObservableCollection<string> _deviceListDescription;
        public ObservableCollection<string> DeviceListDescription
        {
            get { return _deviceListDescription; }
            set { _deviceListDescription = value; OnPropertyChanged("DeviceListDescription"); }
        }
        private string _currentDevice;
        public string CurrentDevice
        {
            get { return _currentDevice; }
            set
            {
                _currentDevice = value;
                OnPropertyChanged(nameof(CurrentDevice));
            }
        }

        private RelayCommand _StartCapture;
        public RelayCommand StartCapture
        {
            get
            {
                return _StartCapture = _StartCapture ??
                  new RelayCommand(OnStartCapture, CanStartCapture);
            }
        }

        private bool CanStartCapture()
        {
            return true;
        }
        private void OnStartCapture()
        {
            try
            {
                for (int i = 0; i < DeviceList.Count(); i++)
                    if ((CurrentDevice == DeviceList[i].Interface.FriendlyName + ": " + SdeviceList[i].Description) || CurrentDevice == DeviceList[i].Description)
                    {
                        captureDevice = DeviceList[i];
                        if (DeviceList[i] is AirPcapDevice)
                            _MainCodeBehind.ShowMessage("Wi-Fi");
                    }
                
                int readTimeoutMilliseconds = 500;
                // открываем в режиме promiscuous, поддерживается также нормальный режим
                captureDevice.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
                // регистрируем событие, которое срабатывает, когда пришел новый пакет
                captureDevice.OnPacketArrival += new PacketArrivalEventHandler(Program_OnPacketArrivalAsync);
                if (Col.Count == 0)
                    num = 1;
                // начинаем захват пакетов
                captureDevice.StartCapture();

                captureFileWriter = new CaptureFileWriterDevice(Path.GetTempPath() + "SnifferLogs.pcapng");

                ReadFile();
            }
            catch
            {
                _MainCodeBehind.ShowMessage("Интерфейс не выбран");
            }
        }

        public void ReadFile()
        {
            var thread = new Thread(() =>
            {
                try
                {
                    // Get an offline device
                    captureFileReader = new CaptureFileReaderDevice(Path.GetTempPath() + "SnifferLogs2.pcapng");

                    // Open the device
                    captureFileReader.Open();
                    captureFileReader.OnPacketArrival += new PacketArrivalEventHandler(Program_OnPacketArrivalAsync);
                    captureFileReader.StartCapture();
                    _MainCodeBehind.ShowMessage("Reached EOF");
                    captureFileReader.Close();

                }
                catch (Exception e)
                {
                    _MainCodeBehind.ShowMessage("Caught exception when opening file" + e.ToString());
                    return;
                }
                Thread.Sleep(100);
            });
        }

        private RelayCommand _StopCapture;
        public RelayCommand StopCapture
        {
            get
            {
                return _StopCapture = _StopCapture ??
                  new RelayCommand(OnStopCapture, CanStopCapture);
            }
        }

        private bool CanStopCapture()
        {
            return true;
        }
        private void OnStopCapture()
        {
            try
            {
                captureDevice.StopCapture();
                captureDevice.Close();
            }
            catch { _MainCodeBehind.ShowMessage("Интерфейс не выбран"); }
        }

        public struct Tcp
        {
            public int pos;
            public uint srcPort;
            public uint dstPort;
            public uint seqNumber;       /*значения в десятеричной форме*/
            public uint ackNumber;
            public int offset;
            public ushort flags;
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
            public string tdata;
        }

        public struct Udp
        {
            public int pos;
            public uint srcPort;
            public uint dstPort;
            public int len;
            public ushort chksum;
            public string udata;
        }

        public struct Arp
        {
            public int pos;
            public string hwtype;
            public string prtype;
            public int hwsize;
            public int prtsize;
            public int opcode;
            public PhysicalAddress sender_mac;
            public System.Net.IPAddress sender_ip;
            public PhysicalAddress target_mac;
            public System.Net.IPAddress target_ip;
            public string adata;
        }

        public struct Icmp
        {
            public int pos;
            public string type;
            public ushort chksum;
            public int seq;
            public string icdata;
        }
        public struct Igmp
        {
            public int pos;
            public string type;
            public string max_resp_time;
            public short chksum;
            public string group_addr;
            public string igdata;
        }




       /* private static int packetIndex = 0;
        private void Program_OnPacketArrivalAsync(object sender, CaptureEventArgs e)
        {
            if (e.Packet.LinkLayerType == PacketDotNet.LinkLayers.Ethernet)
            {
                var packet = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
                var ethernetPacket = (PacketDotNet.EthernetPacket)packet;

                _dispatcher.BeginInvoke(new ThreadStart(delegate
                {

                    _ShowTextBox += (packetIndex +
                                  e.Packet.Timeval.Date.ToString() +
                                  e.Packet.Timeval.Date.Millisecond +
                                  ethernetPacket.SourceHardwareAddress +
                                  ethernetPacket.DestinationHardwareAddress + "\n");
                    packetIndex++;
                }));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowTextBox)));
            }
        }*/
        void Program_OnPacketArrivalAsync(object sender, CaptureEventArgs e)
        {
            Task.Run(() => { captureFileWriter.Write(e.Packet); });
            // парсинг всего пакета
            Packet packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            // получение только TCP пакета из всего фрейма
            TcpPacket tcpPacket = (TcpPacket)packet.Extract<TcpPacket>();
            // получение только IP пакета из всего фрейма
            //IpPacket ipPacket = (IpPacket)packet.Extract(typeof(IpPacket));
            // получение только UDP пакета из всего фрейма
            UdpPacket udpPacket = (UdpPacket)packet.Extract<UdpPacket>();
            ArpPacket arpPacket = (ArpPacket)packet.Extract<ArpPacket>();
            //LinuxSLLPacket sllPacket = (LinuxSLLPacket)packet.Extract(typeof(LinuxSLLPacket));
            IcmpV4Packet icmpPacket = (IcmpV4Packet)packet.Extract<IcmpV4Packet>();
            IgmpV2Packet igmpPacket = (IgmpV2Packet)packet.Extract<IgmpV2Packet>();

            DateTime time = e.Packet.Timeval.Date.ToLocalTime();
            string StrTime = time.Hour + ":" + time.Minute + ":" + time.Second + ":" + time.Millisecond;
            int len = e.Packet.Data.Length;

            // Stream myStream;

            // using (myStream = File.Open(Path.GetTempPath() + "SnifferLogs.pcapng", FileMode.Append, FileAccess.Write))
            //  {
            //StreamWriter myWriter = new StreamWriter(myStream);
            //myWriter.WriteLine(e.Packet);
            //var thread = new Thread(() =>
            /*          Task.Run(() =>
                      {
                          //captureFileWriter = new CaptureFileWriterDevice(Path.GetTempPath() + "SnifferLogs.pcapng");
                          captureFileWriter.Write(e.Packet);
                          //Thread.Sleep(50);
                      });*/
            // }

            // using (myStream = File.Open(Path.GetTempPath() + "SnifferLogs.pcapng", FileMode.OpenOrCreate, FileAccess.Read))
            // {
            // StreamReader myReader = new StreamReader(myStream);
            // Console.Write(myReader.ReadToEnd());
            //captureFileReader = new CaptureFileReaderDevice(Path.GetTempPath() + "SnifferLogs.pcapng");

            //captureFileReader = new CaptureFileReaderDevice(Path.GetTempPath() + "SnifferLogs.pcapng");
            _ShowTextBox += (num +
                                 e.Packet.Timeval.Date.ToString() +
                                 e.Packet.Timeval.Date.Millisecond + "\n");
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowTextBox)));
      
            //_ShowTextBox += myReader.ReadToEnd();
            // }

            /* using (myStream = File.Open(Path.GetTempPath() + "SnifferLogs.pcapng", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
             {
                 StreamWriter myWriter = new StreamWriter(myStream);
                 StreamReader myReader = new StreamReader(myStream);
                 //_MainCodeBehind.ShowMessage(e.Packet.ToString());
                 myWriter.WriteLine(e.Packet.ToString());
                 _ShowTextBox += myReader.ReadLine();
                 _MainCodeBehind.ShowMessage(myReader.ReadLine());
                 //captureFileWriter = new CaptureFileWriterDevice(Path.GetTempPath() + "SnifferLogs.pcapng");
                 //captureFileWriter.Write(e.Packet);
             }*/


            /* _dispatcher.BeginInvoke(new ThreadStart(delegate
             {

                 captureFileWriter.Write(e.Packet);
                 File.Copy(Path.GetTempPath() + "SnifferLogs.pcapng", Path.GetTempPath() + "SnifferLogs2.pcapng");
             }));
             _dispatcher.BeginInvoke(new ThreadStart(delegate
             {

                 _ShowTextBox += (num +
                               e.Packet.Timeval.Date.ToString() +
                               e.Packet.Timeval.Date.Millisecond + "\n");
             }));*/

            if (tcpPacket != null)
            {
                IPPacket ipPacket = (IPPacket)tcpPacket.ParentPacket;

                Tcp tpck;
                // порт отправителя
                tpck.srcPort = tcpPacket.SourcePort;
                // порт получателя
                tpck.dstPort = tcpPacket.DestinationPort;
                tpck.seqNumber = tcpPacket.SequenceNumber;       /*значения в десятеричной форме*/
                tpck.ackNumber = tcpPacket.AcknowledgmentNumber;
                tpck.offset = tcpPacket.DataOffset;
                tpck.flags = tcpPacket.Flags;
                tpck.CWR = tcpPacket.CongestionWindowReduced;
                tpck.ECE = tcpPacket.ExplicitCongestionNotificationEcho;
                tpck.URG = tcpPacket.Urgent;
                tpck.ACK = tcpPacket.Acknowledgment;
                tpck.PSH = tcpPacket.Push;
                tpck.RST = tcpPacket.Reset;
                tpck.SYN = tcpPacket.Synchronize;
                tpck.FIN = tcpPacket.Finished;
                tpck.winSize = tcpPacket.WindowSize;
                tpck.chksum = tcpPacket.Checksum;
                tpck.urgpoint = tcpPacket.UrgentPointer;
                tpck.tdata = Hexdata(tcpPacket);

                string data = HexToAscii(tcpPacket);
                _dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    Col.Add(new MyTable
                    {
                        Number = num,
                        Time = StrTime,
                        Src = ipPacket.SourceAddress.ToString(),
                        Dst = ipPacket.DestinationAddress.ToString(),
                        Protocol = "TCP",
                        Length = len,
                        Info = Convert.ToString(data)
                    });
                    tpck.pos = num;
                    TcpCol.Add(tpck);
                    num++;
                }));
            }

            if (udpPacket != null)
            {
                IPPacket ipPacket = (IPPacket)udpPacket.ParentPacket;
                Udp udpk;
                udpk.srcPort = udpPacket.SourcePort;
                udpk.dstPort = udpPacket.DestinationPort;
                udpk.len = udpPacket.Length;
                udpk.chksum = udpPacket.Checksum;
                udpk.udata = Hexdata(udpPacket);

                // данные пакета
                string data = HexToAscii(udpPacket);
                _dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    Col.Add(new MyTable
                    {
                        Number = num,
                        Time = StrTime,
                        Src = ipPacket.SourceAddress.ToString(),
                        Dst = ipPacket.DestinationAddress.ToString(),
                        Protocol = "UDP",
                        Length = len,
                        Info = Convert.ToString(data)
                    });
                    udpk.pos = num;
                    UdpCol.Add(udpk);
                    num++;
                }));
            }

            if (arpPacket != null)
            {
                Arp arpk;
                arpk.hwtype = arpPacket.HardwareAddressType.ToString();
                arpk.prtype = arpPacket.ProtocolAddressType.ToString();
                arpk.hwsize = arpPacket.HardwareAddressLength;
                arpk.prtsize = arpPacket.ProtocolAddressLength;
                arpk.opcode = Convert.ToInt16(arpPacket.Operation);
                arpk.sender_mac = arpPacket.SenderHardwareAddress;
                arpk.sender_ip = arpPacket.SenderProtocolAddress;
                arpk.target_mac = arpPacket.TargetHardwareAddress;
                arpk.target_ip = arpPacket.TargetProtocolAddress;
                arpk.adata = Hexdata(arpPacket);

                // данные пакета
                string data = HexToAscii(arpPacket);

                _dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    Col.Add(new MyTable
                    {
                        Number = num,
                        Time = StrTime,
                        Src = arpk.sender_ip.ToString(),
                        Dst = arpk.target_ip.ToString(),
                        Protocol = "ARP",
                        Length = len,
                        Info = Convert.ToString(data)
                    });
                    arpk.pos = num;
                    ArpCol.Add(arpk);
                    num++;
                }));
            }


            if (icmpPacket != null)
            {
                IPPacket ipPacket = (IPPacket)icmpPacket.ParentPacket;
                Icmp icmpk;
                icmpk.type = icmpPacket.TypeCode.ToString();
                icmpk.chksum = icmpPacket.Checksum;
                icmpk.seq = icmpPacket.Sequence;
                icmpk.icdata = Hexdata(icmpPacket);
                // данные пакета
                string data = "";
                for (int i = 0; i < icmpPacket.Data.Count(); i++)
                    data += icmpPacket.Data[i];

                _dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    Col.Add(new MyTable
                    {
                        Number = num,
                        Time = StrTime,
                        Src = ipPacket.SourceAddress.ToString(),
                        Dst = ipPacket.DestinationAddress.ToString(),
                        Protocol = "ICMPv4",
                        Length = len,
                        Info = Convert.ToString(data)
                    });
                    icmpk.pos = num;
                    IcmpCol.Add(icmpk);
                    num++;
                }));
            }

            if (igmpPacket != null)
            {
                IPPacket ipPacket = (IPPacket)igmpPacket.ParentPacket;
                Igmp igmpk;
                igmpk.type = igmpPacket.Type.ToString();
                igmpk.max_resp_time = igmpPacket.MaxResponseTime.ToString();
                igmpk.chksum = igmpPacket.Checksum;
                igmpk.group_addr = igmpPacket.GroupAddress.ToString();
                igmpk.igdata = Hexdata(igmpPacket);
                // данные пакета
                string data = HexToAscii(igmpPacket);

                _dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    Col.Add(new MyTable
                    {
                        Number = num,
                        Time = StrTime,
                        Src = ipPacket.SourceAddress.ToString(),
                        Dst = ipPacket.DestinationAddress.ToString(),
                        Protocol = "IGMPv2",
                        Length = len,
                        Info = Convert.ToString(data)
                    });
                    igmpk.pos = num;
                    IgmpCol.Add(igmpk);
                    num++;
                }));
            }
        }
        string Hexdata(Packet packet)
        {
            if (packet.PayloadData != null)
            {
                var hex = BitConverter.ToString(packet.PayloadData);
                return packet.PrintHex();
               /* hex = hex.Replace("-", "");
                string res = "";
                byte[] raw = new byte[hex.Length / 2];
                for (int i = 0; i < raw.Length; i++)
                {
                    raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                    res += raw[i].ToString("X");
                }
                return res;*/
                
            }
            else
                return "";
        }
        string HexToAscii(Packet packet)
        {
            if (packet.PayloadData != null)
            {
                var hex = BitConverter.ToString(packet.PayloadData);
                //var hex = tcpPacket.PrintHex();
                hex = hex.Replace("-", "");
                byte[] raw = new byte[hex.Length / 2];
                for (int i = 0; i < raw.Length; i++)
                    raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                return (Encoding.ASCII.GetString(raw));
            }
            else
                return "";
        }

        //получение выделенной строки datagrid
        private MyTable _SelectedPacket;
        public MyTable SelectedPacket
        {
            get { return _SelectedPacket; }
            set
            {
                 _SelectedPacket = value;
                 PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedPacket)));
                 if (value.Protocol == "TCP")
                    for (int j = 0; j < TcpCol.Count(); j++)
                        if (value.Number == TcpCol[j].pos)
                            _ShowTextBox = ("№ пакета: " + TcpCol[j].pos + "\n" +
                                             "Порт отправителя: " + TcpCol[j].srcPort + "\n" +
                                             "Порт получателя: " + TcpCol[j].dstPort + "\n" +
                                             "Порядковый номер (SN): " + TcpCol[j].seqNumber.ToString("X") + "\n" +
                                             "Номер подтверждения (ACK SN): " + TcpCol[j].ackNumber.ToString("X") + "\n" +
                                             "Offset: " + TcpCol[j].offset + "\n" +
                                             "Flags: " + TcpCol[j].flags + "\n" +
                                             "  CWR: " + TcpCol[j].CWR + "\n" +
                                             "  ECE: " + TcpCol[j].ECE + "\n" +
                                             "  URG: " + TcpCol[j].URG + "\n" +
                                             "  ACK: " + TcpCol[j].ACK + "\n" +
                                             "  PSH: " + TcpCol[j].PSH + "\n" +
                                             "  RST: " + TcpCol[j].RST + "\n" +
                                             "  SYN: " + TcpCol[j].SYN + "\n" +
                                             "  FIN: " + TcpCol[j].FIN + "\n" +
                                             "Размер окна: " + TcpCol[j].winSize + "\n" +
                                             "Контрольная сумма: " + TcpCol[j].chksum.ToString("X") + "\n" +
                                             "Срочность: " + TcpCol[j].urgpoint + "\n" +
                                             "Данные: " + TcpCol[j].tdata + "\n");
                 if (value.Protocol == "UDP")
                            for (int j = 0; j < UdpCol.Count(); j++)
                                if (value.Number == UdpCol[j].pos)
                                    _ShowTextBox = ("№ пакета: " + UdpCol[j].pos + "\n" +
                                                     "Порт отправителя: " + UdpCol[j].srcPort + "\n" +
                                                     "Порт получателя: " + UdpCol[j].dstPort + "\n" +
                                                     "Длина датаграммы: " + UdpCol[j].len + "\n" +
                                                     "Контрольная сумма: " + UdpCol[j].chksum.ToString("X") + "\n" +
                                                     "Данные: " + UdpCol[j].udata + "\n");
                if (value.Protocol == "ARP")
                    for (int j = 0; j < ArpCol.Count(); j++)
                        if (value.Number == ArpCol[j].pos)
                            _ShowTextBox = ("№ пакета: " + ArpCol[j].pos + "\n" +
                                             "Тип сети: " + ArpCol[j].hwtype + "\n" +
                                             "Тип протокола: " + ArpCol[j].prtype + "\n" +
                                             "Длина MAC адреса: " + ArpCol[j].hwsize + "\n" +
                                             "Длина IP адреса: " + ArpCol[j].prtsize + "\n" +
                                             "Код операции: " + ArpCol[j].opcode + "\n" +
                                             "MAC адрес отправителя: " + ArpCol[j].sender_mac + "\n" +
                                             "IP адрес отправителя: " + ArpCol[j].sender_ip + "\n" +
                                             "MAC адрес получателя: " + ArpCol[j].target_mac + "\n" +
                                             "IP адрес получателя: " + ArpCol[j].target_ip + "\n" +
                                             "Hex: " + ArpCol[j].adata + "\n");
                if (value.Protocol == "ICMPv4")
                    for (int j = 0; j < IcmpCol.Count(); j++)
                        if (value.Number == IcmpCol[j].pos)
                            _ShowTextBox = ("№ пакета: " + IcmpCol[j].pos + "\n" +
                                             "TypeCode: " + IcmpCol[j].type + "\n" +
                                             "Sequence: " + IcmpCol[j].seq + "\n" +
                                             "Контрольная сумма: " + IcmpCol[j].chksum.ToString("X") + "\n" +
                                             "Hex: " + IcmpCol[j].icdata + "\n");
                if (value.Protocol == "IGMPv2")
                    for (int j = 0; j < IgmpCol.Count(); j++)
                        if (value.Number == IgmpCol[j].pos)
                            _ShowTextBox = ("№ пакета: " + IgmpCol[j].pos + "\n" +
                                             "Тип: " + IgmpCol[j].type + "\n" +
                                             "Max Resp Time: " + IgmpCol[j].max_resp_time + "\n" +
                                             "Контрольная сумма: " + IgmpCol[j].chksum.ToString("X") + "\n" +
                                             "Multicast Address: " + IgmpCol[j].group_addr + "\n" +
                                             "Hex: " + IgmpCol[j].igdata + "\n");

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowTextBox)));
            }
        }

        private string _ShowTextBox;
        public string ShowTextBox
        {
            get { return _ShowTextBox; }
        }

       
    }
}
    public class MyTable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public MyTable(int number, string time, string src, string dst, string protocol, int length, string Info)
        {
            this.Number = number;
            this.Time = time;
            this.Src = src;
            this.Dst = dst;
            this.Protocol = protocol;
            this.Length = length;
            this.Info = Info;
        }
        public MyTable() { }

        private int _number;
        public int Number
        {
            get { return _number; }
            set { _number = value; OnPropertyChanged("Number"); }
        }
        private string _time;
        public string Time
        {
            get { return _time; }
            set { _time = value; OnPropertyChanged("Time"); }
        }
        private string _src;
        public string Src
        {
            get { return _src; }
            set { _src = value; OnPropertyChanged("Src"); }
        }
        private string _dst;
        public string Dst
        {
            get { return _dst; }
            set { _dst = value; OnPropertyChanged("Dst"); }
        }
        private string _protocol;
        public string Protocol
        {
            get { return _protocol; }
            set { _protocol = value; OnPropertyChanged("Protocol"); }
        }
        private int _length;
        public int Length
        {
            get { return _length; }
            set { _length = value; OnPropertyChanged("Length"); }
        }
        private string _info;
        public string Info
        {
            get { return _info; }
            set { _info = value; OnPropertyChanged("Info"); }
        }
    }