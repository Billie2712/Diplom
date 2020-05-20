using System;
using System.ComponentModel;
using Sniffer.Model;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sniffer.ViewModel
{
    class IdsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        //Fields
        private IMainWindowsCodeBehind _MainCodeBehind;

        //ctor
        public IdsViewModel() { }
        public IdsViewModel(IMainWindowsCodeBehind codeBehind)
        {
            if (codeBehind == null) throw new ArgumentNullException(nameof(codeBehind));

            _MainCodeBehind = codeBehind;
        }

        private RelayCommand _CreateNN;
        public RelayCommand CreateNN
        {
            get
            {
                return _CreateNN = _CreateNN ??
                  new RelayCommand(OnCreateNN, CanCreateNN);
            }
        }

        private bool CanCreateNN()
        {
            return true;
        }
        private void OnCreateNN()
        {
            try
            {
                List<int[]> input = ReadX(@"D:\Dокументы\Все по диплому\123.txt");
                int output = ReadY(@"D:\Dокументы\Все по диплому\123.txt");
                var network = new ActivationNetwork(
                            new GaussFunction(), // activation function
                            12, // twelve inputs in the network
                            24, // twenty four neurons in the first layer
                            1); // one neuron in the second layer
                                // create teacher
                var teacher = new DeltaRuleLearning(network);

                /*while (true)
                {
                    // run epoch of learning procedure
                    var error = teacher.RunEpoch(input, output);
                    // check error value to see if we need to stop
                    if (error < 0.001)
                    {
                        break;
                    }
                }*/
            }
            catch { _MainCodeBehind.ShowMessage("Ошибка инициализации ИНС"); }
        }

        public List<int[]> ReadX(string pathToCsvFile)
        {
            var int_list = new List<int[]>();
            Dictionary<string, int> dict = new Dictionary<string, int>();
            try
            {
                IEnumerable<string> lines = File.ReadAllLines(@"D:\Dокументы\Все по диплому\dict.txt");
                foreach (string s in lines)
                {
                    string[] x = s.Split(new string[] { " " }, 0);
                    dict.Add(x[0], int.Parse(x[1]));
                }

                lines = File.ReadAllLines(pathToCsvFile);
                foreach (var line in lines)
                {
                    string[] slist = line.Split(',');
                    string[] new_slist = new string[12];
                    for (int i = 0; i < new_slist.Length; i++)
                        new_slist[i] = slist[i];
                    int[] ilist = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    for (int i = 0; i < new_slist.Length; i++)
                    {
                        if (dict.ContainsKey(new_slist[i]))
                            ilist[i] = dict[new_slist[i]];
                        else
                            ilist[i] = int.Parse(new_slist[i]);
                        //Debug.WriteLine(ilist[i]);
                    }
                    int_list.Add(ilist);
                }
                //_MainCodeBehind.ShowMessage(int_list[0][0]+ int_list[0][1]+ int_list[0][2]+ int_list[0][3]);                
            }
            catch (Exception ex)
            {
                _MainCodeBehind.ShowMessage(ex.Message);
            }
            return int_list;
        }

        public int ReadY(string pathToCsvFile)
        {
            int int_y = -1;
            try
            {
                IEnumerable<string> lines = File.ReadAllLines(pathToCsvFile);
                foreach (var line in lines)
                {
                    string[] slist = line.Split(',');
                    string y = slist[41];
                    if (y == "normal.")
                        int_y = 0;
                    else
                        int_y = 1;
                    Debug.WriteLine(int_y);
                }
                //_MainCodeBehind.ShowMessage(int_list[0][0]+ int_list[0][1]+ int_list[0][2]+ int_list[0][3]);                
            }
            catch (Exception ex)
            {
                _MainCodeBehind.ShowMessage(ex.Message);
            }
            return int_y;
        }
    }
}
