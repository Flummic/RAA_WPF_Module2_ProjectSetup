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
    public partial class ViewRenumbererForm : Window
    {
        // Attributes
        public bool viewSelection;
        public Document doc;
        private List<string> cbxItems = new List<string>();
        private List<string> selectedViews = new List<string>(); 

        // Constructor
        public ViewRenumbererForm(List<string> cbxItems, List<string> selectedViews, Document doc)
        {
            InitializeComponent();
            viewSelection = false;

            // get items from the form-call
            this.doc = doc;
            this.cbxItems = cbxItems;
            
            if (selectedViews.Count != 0)
            {
                this.selectedViews = selectedViews;
            }

            // fill the ComboBox with numbers
            cbxStartNumber.Items.Clear();
            cbxStartNumber.ItemsSource = cbxItems;
            cbxStartNumber.SelectedIndex = 0;

            lbxSelectedViews.Items.Clear();
            foreach(var item in  selectedViews)
            {
                lbxSelectedViews.Items.Add(item.ToString());
            }
        }

        private void btnViewSelect_Click(object sender, RoutedEventArgs e)
        {
            viewSelection = true;
            this.DialogResult = true;
            this.Close();
        }

        public string GetSelectedStartNumber()
        {
            return cbxStartNumber.SelectedItem.ToString();
        }

        public List<string> GetSelectedViews() 
        { 
            return selectedViews;
        }


        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            
            
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
