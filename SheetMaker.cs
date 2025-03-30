#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

#endregion

namespace RAA_WPF_Module2_ProjectSetup
{
    [Transaction(TransactionMode.Manual)]
    public class SheetMaker : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Autodesk.Revit.DB.Document doc = uidoc.Document;

            // put any code needed for the form here
            List<Element> titleBlocks = new List<Element>();
            List<string> existingSheetNumbers = new List<string>();
            List<string> existingViewNames = new List<string>();


            // collect all Titleblocks into a list for the Form
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(FamilySymbol));
            collector.OfCategory(BuiltInCategory.OST_TitleBlocks);
            collector.WhereElementIsElementType();

            

            foreach (Element e in collector)
            {
                titleBlocks.Add(e);
            }

            // collect all existing sheets in the project - to make sure that the new sheet number is unique
            FilteredElementCollector viewSheetCollector = new FilteredElementCollector(doc);
            viewSheetCollector.OfClass(typeof(ViewSheet));
            viewSheetCollector.OfCategory(BuiltInCategory.OST_Sheets);
            viewSheetCollector.WhereElementIsNotElementType();

            foreach (Element e in viewSheetCollector)
            {
                existingSheetNumbers.Add(e.get_Parameter(BuiltInParameter.SHEET_NUMBER).AsString());
            }

            // collect all existing views in the project
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfClass(typeof(View));
            viewCollector.OfCategory(BuiltInCategory.OST_Views);
            viewCollector.WhereElementIsNotElementType();
            
            foreach (Element e in viewCollector) 
            {
                View curView = e as View;
                if(curView.GetPlacementOnSheetStatus().ToString() == "NotPlaced")
                {
                    existingViewNames.Add(e.Name);
                }
            }

            // open form
            SheetMakerForm myForm = new SheetMakerForm(titleBlocks, existingSheetNumbers, existingViewNames)
            {
                Height = 500,
                Width = 600,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };
            myForm.ShowDialog();

            // get data from the form (List of SheetCreationObjects)
            if (myForm.DialogResult == true)
            {
                // fetch Data from my Form via the "GetData"-Method I wrote
                List<SheetMakerForm.SheetCreationObject> dataList = myForm.GetSheetCreationData();

                // now I can do something with that data -> e.g. iterate over it
                foreach (SheetMakerForm.SheetCreationObject curClass in dataList)
                {
                    // create a new sheets with the selected titleblock, Name and number
                    using (Transaction t = new Transaction(doc, "SheetMaker: Create Sheet"))
                    {
                        t.Start();
                        try
                        {
                            // get the selected View to add to the sheet
                            List<View> selectedView = new List<View>();

                            foreach (Element e in viewCollector)
                            {
                                if (e.Name == curClass.ViewName)
                                {
                                    View view = e as View;
                                    selectedView.Add(view);
                                }
                            }

                            // get the selected titleblock for the sheet
                            List<FamilySymbol> familySymbols = new List<FamilySymbol>();

                            foreach (Element e in titleBlocks)
                            {
                                string curTitleBlockName = (e as FamilySymbol).FamilyName + " | "  + e.Name;
                                if (curTitleBlockName == curClass.TitleBlockName)
                                {
                                    FamilySymbol fs = e as FamilySymbol;
                                    familySymbols.Add(fs);
                                }
                            }

                            // check if a sheet or a placeholder sheet should be created
                            if(curClass.IsPlaceholder)
                            {
                                // create a Placholder Sheet and set the Parameters
                                ViewSheet viewSheet = ViewSheet.CreatePlaceholder(doc);
                                viewSheet.Name = curClass.SheetName;
                                viewSheet.get_Parameter(BuiltInParameter.SHEET_NUMBER).Set(curClass.SheetNumber);
                            }
                            else
                            {
                                // Create a sheet view and set the Parameters
                                ViewSheet viewSheet = ViewSheet.Create(doc, familySymbols.FirstOrDefault().Id);
                                viewSheet.Name = curClass.SheetName;
                                viewSheet.get_Parameter(BuiltInParameter.SHEET_NUMBER).Set(curClass.SheetNumber);
                                if (null == viewSheet)
                                {
                                    throw new Exception("Failed to create new ViewSheet.");
                                }

                                // Add passed in view onto the center of the sheet
                                UV location = new UV((viewSheet.Outline.Max.U - viewSheet.Outline.Min.U) / 2,
                                                     (viewSheet.Outline.Max.V - viewSheet.Outline.Min.V) / 2);

                                //viewSheet.AddView(view3D, location);
                                if(selectedView.Count >= 1)
                                {
                                    Viewport.Create(doc, viewSheet.Id, selectedView.FirstOrDefault().Id, new XYZ(location.U, location.V, 0));

                                }
                            }

                            t.Commit();

                        }
                        catch
                        {
                            t.RollBack();
                        }
                    }

                }
                TaskDialog.Show("SheetMaker", $"SheetMaker has finished creating {dataList.Count} sheets.");
            }

            return Result.Succeeded;

        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
