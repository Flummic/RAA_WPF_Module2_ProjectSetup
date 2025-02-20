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


namespace RAA_WPF_Module2_ProjectSetup
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class ProjectSetupForm : Window
    {
        private string directoryPath = string.Empty;
        private bool fileSelected = false;

        public ProjectSetupForm()
        {
            InitializeComponent();
        }

        private void btnFileSelect_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                // Set filter for file extension and default file extension
                Filter = "All Files (*.csv*)|*.*",
                Multiselect = true
            };

            // Show open file dialog
            bool? result = openFileDialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Set the directory path based on the first selected file
                directoryPath = openFileDialog.FileName;

                // set textbox to the directory path
                tbxFilePath.Text = directoryPath;

                // set bool to true
                fileSelected = true;
            }
            else
            {
                tbxFilePath.Text = "File-Selecttion failed";
            }
        }

        public bool isMetric()
        {
           return (bool)rbMetric.IsChecked;
        }

        public string getFilePath()
        {
            return directoryPath;
        }

        public bool createFloorPlans()
        {
            return (bool)cbFloorPlans.IsChecked;
        }

        public bool createCeilingPlans()
        {
            return (bool)cbCeilingPlans.IsChecked;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if(!fileSelected)
            {
                TaskDialog.Show("Alert", "Please select a csv-file before you click ok");
                return;
            }
            
            // Close the form and keep the changes
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Close the form without applying changes
            this.DialogResult = false;
            this.Close();
        }
    }
}
