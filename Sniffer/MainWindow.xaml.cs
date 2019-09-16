using Sniffer.ViewModel;
using Sniffer.View;
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

namespace Sniffer
{
    public interface IMainWindowsCodeBehind
    {
        /// <summary>
        /// Показ сообщения для пользователя
        /// </summary>
        /// <param name="message">текст сообщения</param>
        void ShowMessage(string message);

        /// <summary>
        /// Загрузка нужной View
        /// </summary>
        /// <param name="view">экземпляр UserControl</param>
        void LoadView(ViewType typeView);
    }

    public enum ViewType
    {
        Main,
        IDS,
        Sniffer
    }

    public partial class MainWindow : Window, IMainWindowsCodeBehind
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //загрузка вьюмодел для кнопок меню
            MenuViewModel vm = new MenuViewModel();
            //даем доступ к этому кодбихайнд
            vm.CodeBehind = this;
            //делаем эту вьюмодел контекстом данных
            this.DataContext = vm;

            //загрузка стартовой View
            LoadView(ViewType.Main);
        }

        public void LoadView(ViewType typeView)
        {
            switch (typeView)
            {
                case ViewType.Main:
                    //загружаем вьюшку, ее вьюмодель
                    MainUC view = new MainUC();
                    MainViewModel vm = new MainViewModel(this);
                    //связываем их м/собой
                    view.DataContext = vm;
                    //отображаем
                    this.OutputView.Content = view;
                    break;
                case ViewType.IDS:
                    IdsUC viewIds = new IdsUC();
                    IdsViewModel vmIds = new IdsViewModel(this);
                    viewIds.DataContext = vmIds;
                    this.OutputView.Content = viewIds;
                    break;
                case ViewType.Sniffer:
                    SnifferUC viewS = new SnifferUC();
                    SnifferViewModel vmS = new SnifferViewModel(this);
                    viewS.DataContext = vmS;
                    this.OutputView.Content = viewS;
                    break;
            }


        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

    }
        
}
