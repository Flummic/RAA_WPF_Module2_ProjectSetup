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
using System.Windows.Controls;

#endregion

namespace RAA_WPF_Module2_ProjectSetup
{
    [Transaction(TransactionMode.Manual)]
    public class Modul04Code : IExternalCommand
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
            EventAction myAction = new EventAction();
            ExternalEvent myEvent = ExternalEvent.Create(myAction);

            // open form
            Modul04Form currentForm = new Modul04Form(myEvent)
            {
                Width = 300,
                Height = 300,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            currentForm.Show();
            return Result.Succeeded;
            
        }
    }
    public class EventAction : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.DB.Document Doc = uidoc.Document;

            // code to do GraphicOverrides
            List<ElementId> selectedElements = uidoc.Selection.GetElementIds().ToList(); // gets me all elements that are curretnly selected

            // create an OverrideGraphicSettings object
            OverrideGraphicSettings newSettings = new OverrideGraphicSettings();

            // create a variable for Color
            Color newColor = new Color(255, 0, 0); // red

            // set the color of the newSettings object
            newSettings.SetCutForegroundPatternColor(newColor);
            newSettings.SetSurfaceForegroundPatternColor(newColor);

            // create a variable for the fill pattern 
            FillPatternElement curPatt = GetFillPatternByName(Doc, "<Solid fill>"); // using a custom method I create below

            // set the fill pattern of the newSettings object
            newSettings.SetCutForegroundPatternId(curPatt.Id);
            newSettings.SetSurfaceForegroundPatternId(curPatt.Id);

            // create a transaction to apply the Override
            using (Transaction t = new Transaction(Doc))
            {
                t.Start("Apply Override");
                foreach (ElementId eId in selectedElements)
                {
                    Doc.ActiveView.SetElementOverrides(eId, newSettings);
                }
                t.Commit();
            }

            // Code to create a new sheet
            FilteredElementCollector collector = new FilteredElementCollector(Doc);
            collector.OfClass(typeof(FamilySymbol));
            collector.OfCategory(BuiltInCategory.OST_TitleBlocks);
            collector.WhereElementIsElementType();

            using (Transaction t = new Transaction(Doc))
            {
                t.Start("Create new sheet");

                ViewSheet newSheet;
                if(Globals.IsPlaceholder)
                {
                    newSheet = ViewSheet.CreatePlaceholder(Doc);    
                }
                else
                {
                    ElementId titelBlockId = collector.FirstElementId();
                    newSheet = ViewSheet.Create(Doc, titelBlockId);
                }

                newSheet.SheetNumber = Globals.SheetNumber;
                newSheet.Name = Globals.SheetName;

                t.Commit();
            }
        }

        private FillPatternElement GetFillPatternByName(Autodesk.Revit.DB.Document doc, string name)
        {
            FillPatternElement curFPE = null;

            curFPE = FillPatternElement.GetFillPatternElementByName(doc, FillPatternTarget.Drafting, name);
            return curFPE;
        }

        public string GetName()
        {
            return "EventAction";
        }
    }

}
