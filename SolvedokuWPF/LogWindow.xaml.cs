using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace SolvedokuWPF
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        public ObservableCollection<string> LogsContainer { get; set; }

        public LogWindow()
        {
            InitializeComponent();
            Closing += (_, _) =>
            {
                Environment.Exit(0);
            };

            LogsContainer = new();
            LogsContainer.Insert(0, "Initializing..");
            DataContext = this;
        }
    }
}
