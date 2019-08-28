using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sniffer.ViewModel
{
    public class MenuViewModel
    {
        //ctor
        public MenuViewModel()
        {

        }

        public IMainWindowsCodeBehind CodeBehind { get; set; }


        /// <summary>
        /// Переход к первой вьюшке
        /// </summary>
        private RelayCommand _LoadSnifferUCCommand;
        public RelayCommand LoadSnifferUCCommand
        {
            get
            {
                return _LoadSnifferUCCommand = _LoadSnifferUCCommand ??
                  new RelayCommand(OnLoadSnifferUC, CanLoadSnifferUC);
            }
        }
        private bool CanLoadSnifferUC()
        {
            return true;
        }
        private void OnLoadSnifferUC()
        {
            CodeBehind.LoadView(ViewType.Sniffer);
        }


        /// <summary>
        /// Переход ко Второй вьюшке
        /// </summary>
       /* private RelayCommand _LoadSecondUCCommand;
        public RelayCommand LoadSecondUCCommand
        {
            get
            {
                return _LoadSecondUCCommand = _LoadSecondUCCommand ??
                  new RelayCommand(OnLoadSecondUC, CanLoadSecondUC);
            }
        }
        private bool CanLoadSecondUC()
        {
            return true;
        }
        private void OnLoadSecondUC()
        {
            CodeBehind.LoadView(ViewType.Second);
        }*/


        /// <summary>
        /// Возвращение к главной вьюшке
        /// </summary>
        private RelayCommand _LoadMainUCCommand;
        public RelayCommand LoadMainUCCommand
        {
            get
            {
                return _LoadMainUCCommand = _LoadMainUCCommand ??
                  new RelayCommand(OnLoadMainUC, CanLoadMainUC);
            }
        }
        private bool CanLoadMainUC()
        {
            return true;
        }
        private void OnLoadMainUC()
        {
            CodeBehind.LoadView(ViewType.Main);
        }
    }
}
