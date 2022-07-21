using System;
using System.Windows;
using TextProcessor.ViewModels;

namespace TextProcessor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            vm = DataContext as MainViewModel;
        }

        private MainViewModel vm;

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            vm.CommandLoaded?.Execute(null);
        }
    }
}
