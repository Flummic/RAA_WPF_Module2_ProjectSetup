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
using System.Collections.ObjectModel;

namespace RAA_WPF_Module2_ProjectSetup
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class LevelCheckerForm : Window
    {
        ExternalEvent SetEvent;
        ExternalEvent ReSetEvent;
        ObservableCollection<Level> levelList { get; set; } 

        public LevelCheckerForm(List<Level> levels, ExternalEvent setEvent, ExternalEvent reSetEvent)
        {
            InitializeComponent();

            levelList = new ObservableCollection<Level>(levels);
            cbxLevel.ItemsSource = levelList;
            cbxLevel.DisplayMemberPath = "Name";
            // cbxLevel.SelectedIndex = 0;

            SetEvent = setEvent;
            ReSetEvent = reSetEvent;
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            Globals.selectedLevel = cbxLevel.SelectedItem as Level;
            // SetSelectedColorMode();
            SetSelectedCategories();

            if (rbSetColor.IsChecked == true)
            {
                SetEvent.Raise();
            }
            else if (rbReSetColor.IsChecked == true)
            {
                ReSetEvent.Raise();
            }
        }

        private void SetSelectedCategories()
        {
            List<string> curSelectedCategories = new List<string>();

            if (cbWalls.IsChecked == true)
                curSelectedCategories.Add("OST_Walls");
            if (cbColumns.IsChecked == true)
                curSelectedCategories.Add("OST_StructuralColumns");
            if (cbWindows.IsChecked == true)
                curSelectedCategories.Add("OST_Windows");

            Globals.selectedCategories = curSelectedCategories;
        }

        private void SetSelectedColorMode()
        {
            if (rbSetColor.IsChecked == true)
                Globals.setColor = true;
            if (rbReSetColor.IsChecked == true)
                Globals.setColor = false;
        }

        //private void btnCancel_Click(object sender, RoutedEventArgs e)
        //{

        //}
    }
}
