using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Threading;
using Goodfornothing.AesCrypter.ComponentModel;

namespace Goodfornothing.AesCrypter
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = new MainWindowViewModel(this.Dispatcher);
        }
    }
}
