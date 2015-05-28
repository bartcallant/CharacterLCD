using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CharacterLCD.TestApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Callant.CharacterLCD lcd = null;
        public MainPage()
        {
            this.InitializeComponent();
            Unloaded += MainPage_Unloaded;

            lcd = new Callant.CharacterLCD();
            lcd.WriteLCD("Hello world!");
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            lcd.Dispose();
        }

        private void WriteLCD_Click(object sender, RoutedEventArgs e)
        {
            lcd.WriteLCD(txtWriteLCD.Text);
        }
    }
}
