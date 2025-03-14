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
using System.Reflection;

#endregion

namespace RAA_WPF_Module2_ProjectSetup
{
    [Transaction(TransactionMode.Manual)]
    public class ViewRenumberer : IExternalCommand
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
            List<string> itemList = new List<string>();
            for (int i = 1; i < 11; i++)
            {
                itemList.Add(i.ToString());
            }

            List<string> selectedViews = new List<string>();
            

            // open form
            ViewRenumbererForm currentForm = new ViewRenumbererForm(itemList, selectedViews, doc)
            {
                Width = 500,
                Height = 400,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            currentForm.ShowDialog();


            if(currentForm.DialogResult == true)
            {
                // check if we are in viewSelection-Mode
                bool viewSelectionMode = currentForm.viewSelection;

                if(viewSelectionMode)
                {
                    // Allow user to make a selection
                    List<Reference> refList = new List<Reference>();
                    bool flag = true;
                    ISelectionFilter filter = new ViewportSelectionFilter(); // Apply the filter


                    while (flag == true)
                    {
                        try
                        {
                            Reference curRef = uidoc.Selection.PickObject(ObjectType.Element, filter,"Pick a view");
                            refList.Add(curRef);
                        }
                        catch (Exception)
                        {
                            flag = false;
                        }
                    }

                    selectedViews.Clear();
                    foreach (var item in refList)
                    {
                        selectedViews.Add(item.ElementId.ToString());
                    }

                    // open form
                    ViewRenumbererForm currentForm2 = new ViewRenumbererForm(itemList, selectedViews, doc)
                    {
                        Width = 500,
                        Height = 400,
                        WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                        Topmost = true,
                    };

                    currentForm2.ShowDialog();

                    if(currentForm2.DialogResult == true)
                    {
                        viewSelectionMode = currentForm2.viewSelection;
                        if (viewSelectionMode)
                        {
                            TaskDialog.Show("Help", "Currently you can only select views once. Please restart the function");
                            return Result.Failed;
                        }
                        else
                        {
                            // get form data and do something
                            // 1. Declare variables
                            int selectedStartNum = Convert.ToInt32(currentForm2.GetSelectedStartNumber());
                            List<string> viewSelection = currentForm2.GetSelectedViews();

                            // 2. create transaction (because we are about to change the Revit-Model)
                            Transaction t = new Transaction(doc);
                            t.Start("Rename selected views in Selection-Prder");

                            // 3. loop through list of views
                            FilteredElementCollector collector = new FilteredElementCollector(doc);
                            collector.OfCategory(BuiltInCategory.OST_Viewports);
                            collector.WhereElementIsNotElementType();

                            // List<Viewport> viewportList = new List<Viewport>();

                            // renumber the views with a prefix "zzz" to avoid creating duplicate-numbers in the process
                            foreach (Viewport currentView in collector)
                            {
                                if (currentView != null)
                                {
                                    // get the current viewport-number-Parameter
                                    Parameter curNum = currentView.get_Parameter(BuiltInParameter.VIEWPORT_DETAIL_NUMBER);
                                    string curString = curNum.AsString();
                                    bool rename1 = curNum.Set("zzz" + curString);
                                }
                            }

                            // renumber the selected views with the correct number
                            foreach (string elemId in selectedViews)
                            {
                                foreach (Viewport currentView in collector)
                                {
                                    if (currentView != null && currentView.Id.ToString() == elemId)
                                    {
                                        // get the current viewport-number-Parameter
                                        Parameter curNum = currentView.get_Parameter(BuiltInParameter.VIEWPORT_DETAIL_NUMBER);
                                        // string beforeChange = curNum.AsString();
                                        bool rename2 = curNum.Set("" + selectedStartNum);
                                        // string afterChange = curNum.AsString();
                                        rename2 = false;
                                        selectedStartNum++;
                                        // viewportList.Add(currentView);
                                    }
                                }
                            }

                            //// renumber the view again to remove the prefix ("zzz")
                            //foreach (Viewport curView in viewportList)
                            //{
                            //    // get the current viewport-number-Parameter
                            //    Parameter curNum = curView.get_Parameter(BuiltInParameter.VIEWPORT_DETAIL_NUMBER);
                            //    string curString = curNum.AsString();
                            //    string newString = curString.Remove(0, 3);
                            //    curNum.Set(newString);
                            //}

                            // Commit and Dispose the transaction
                            t.Commit();
                            t.Dispose();

                            // 14. Prompt the user with a dialog that shows how many Levels were created from which file
                            TaskDialog.Show("Complete", "In total " + selectedViews.Count + " views were renumbered.");

                            return Result.Succeeded;
                        }
                        
                    }
                    else
                    {
                        TaskDialog.Show("Cancel", "The View-Renumberer was canceled");
                        return Result.Failed;
                    }
                    
                }
                else
                {
                    TaskDialog.Show("Help", "You left the View-Renumberer without initiating the view-selection-process");
                    return Result.Failed;
                }         
            }
            else
            {
                return Result.Failed;
            }
            
        }

        public class ViewportSelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element elem)
            {
                return elem.Category != null && elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Viewports;
            }

            public bool AllowReference(Reference reference, XYZ position)
            {
                return false; // We only filter elements, not geometry references
            }
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
