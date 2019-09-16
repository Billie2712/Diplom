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
        /// Переход к вьюшке сниффера
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
        /// Переход ко вьюшке ids
        /// </summary>
        private RelayCommand _LoadIdsUCCommand;
        public RelayCommand LoadIdsUCCommand
        {
            get
            {
                return _LoadIdsUCCommand = _LoadIdsUCCommand ??
                  new RelayCommand(OnLoadIdsUC, CanLoadIdsUC);
            }
        }
        private bool CanLoadIdsUC()
        {
            return true;
        }
        private void OnLoadIdsUC()
        {
            CodeBehind.LoadView(ViewType.IDS);
        }


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
