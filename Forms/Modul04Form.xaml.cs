using Autodesk.Revit.UI;
using Microsoft.Win32;
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
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace RAA_WPF_Module2_ProjectSetup
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class Modul04Form : Window
    {
        ExternalEvent myEvent;

        public Modul04Form(ExternalEvent exEvent)
        {
            InitializeComponent();
            myEvent = exEvent;
        }

        private void btnButton_Click(object sender, RoutedEventArgs e)
        {
            if(cbXBoolean.IsChecked == true)
            {
                Globals.IsPlaceholder = true;
            }
            else
            {
                Globals.IsPlaceholder = false;
            }
            Globals.SheetNumber = tbxSheetNumber.Text;
            Globals.SheetName = tbxSheetName.Text;
            myEvent.Raise();
        }
    }
}
