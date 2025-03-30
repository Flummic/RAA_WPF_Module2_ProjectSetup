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
    public partial class SheetMakerForm : Window
    {
        // Attributes
        // Property of MyForm: List to store all my DataClass1-Items for the DataGrid
        ObservableCollection<SheetCreationObject> dataList { get; set; }
        ObservableCollection<string> dataItems { get; set; }
        ObservableCollection<string> viewNames { get; set; }
        ObservableCollection<string> existingSheetNumbers { get; set; }

        // Constructor
        public SheetMakerForm(List<Element> titleBlocks, List<string> sheetNumbers, List<string> existingViews)
        {
            InitializeComponent();

            // Instatiate my List-Property 
            dataList = new ObservableCollection<SheetCreationObject>();
            // Instatiate my item-Lists for the ComboBox in the DataGrid
            dataItems = new ObservableCollection<string>();
            viewNames = new ObservableCollection<string>();
            // Instatiate my List-Property
            existingSheetNumbers = new ObservableCollection<string>(sheetNumbers);

            // bind DataGrid to my List as an ItemSource
            dgSheetInfo.ItemsSource = dataList;

            // bind ComboBox in DataGrid to my List as an ItemSource
            foreach (Element e in titleBlocks)
            {
                string curTitleBlockName = (e as FamilySymbol).FamilyName + " | " + e.Name;
                

                if (!string.IsNullOrEmpty(curTitleBlockName))
                    dataItems.Add(curTitleBlockName);

                 
            }
            titleBlockCol.ItemsSource = dataItems;

            // bind ComboBox in DataGrid to my List as an ItemSource
            foreach (string s in existingViews)
            {
                viewNames.Add(s);
            }
            viewCol.ItemsSource = viewNames;

            // add a first row to the sheet-maker
            dataList.Add(new SheetCreationObject());

        }

        // class to store data for the DataGrid about SheetCreation
        public class SheetCreationObject
        {
            public string SheetNumber { get; set; }
            public string SheetName { get; set; }
            public bool IsPlaceholder { get; set; }
            public string TitleBlockName { get; set; }
            public string ViewName { get; set; }
        }

        // method to get Data from DataGrid
        public List<SheetCreationObject> GetSheetCreationData()
        {
            return dataList.ToList();
        }


        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            dataList.Add(new SheetCreationObject());
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            // Create a list to store items to be removed
            List<SheetCreationObject> itemsToRemove = new List<SheetCreationObject>();


            // iterate through all Items (Rows) in my DataGrid (dataList)
            foreach (SheetCreationObject curRow in dataList)
            {
                // check if the current Row is selected in my DataGrid
                if (dgSheetInfo.SelectedItem == curRow)
                {
                    // remove the selected Row
                    itemsToRemove.Add(curRow);
                }
            }

            // Remove the collected items from the dataList
            foreach (SheetCreationObject item in itemsToRemove)
            {
                dataList.Remove(item);
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            // check if there are any duplicate sheet numbers
            List<string> sheetNumbers = new List<string>();
            foreach (SheetCreationObject s in dataList)
            {
                sheetNumbers.Add(s.SheetNumber);
            }
            foreach (string s in sheetNumbers)
            {
                if (existingSheetNumbers.Contains(s))
                {
                    MessageBox.Show($"Sheet numbers must be unique. Please change the Number: {s}");
                    return;
                }
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
