using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAA_WPF_Module2_ProjectSetup
{
    internal class SetColor : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            // Get the current application and document to work with
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Get the selected Elements from the Globals class (Categories)
            List<Element> selectedElements = new List<Element>();
            foreach (string category in Globals.selectedCategories)
            {
                FilteredElementCollector collector = new FilteredElementCollector(doc);
                collector.OfCategory((BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), category));
                collector.WhereElementIsNotElementType();
                selectedElements.AddRange(collector.ToElements());
            }

            // Setup the color to use for the selected elements
            Color highlightColor = new Color(255, 0, 0); // Red color

            // Setup the pattern to use for the selected elements
            FillPatternElement highlightPattern = Utils.GetFillPatternByName(doc, "<Flächenfüllung>"); // Solid Fill (in German)

            // Create OverrideGraphicSettings to set the color and pattern
            OverrideGraphicSettings overrideSettings = new OverrideGraphicSettings();
            overrideSettings.SetCutForegroundPatternColor(highlightColor);
            overrideSettings.SetSurfaceForegroundPatternColor(highlightColor);
            overrideSettings.SetSurfaceForegroundPatternId(highlightPattern.Id);
            overrideSettings.SetCutForegroundPatternId(highlightPattern.Id);

            // Apply Overrides to all selected elements
            using(Transaction t = new Transaction(doc))
            {
                t.Start("Set Color for Elements on selected Level");
                foreach (Element curElement in selectedElements)
                {
                    // make sure that the level matches the selected level
                    if (curElement.LevelId.Equals(Globals.selectedLevel.Id))
                    {
                        doc.ActiveView.SetElementOverrides(curElement.Id, overrideSettings);
                    }
                }
                t.Commit();
            }

        }

        public string GetName()
        {
            return "Set Color";
        }
    }

    internal class ReSetColor : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            // Get the current application and document to work with
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Get the selected Elements from the Globals class (Categories)
            List<Element> selectedElements = new List<Element>();
            foreach (string category in Globals.selectedCategories)
            {
                FilteredElementCollector collector = new FilteredElementCollector(doc);
                collector.OfCategory((BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), category));
                collector.WhereElementIsNotElementType();
                selectedElements.AddRange(collector.ToElements());
            }

            // Create OverrideGraphicSettings to reset the color and pattern
            OverrideGraphicSettings overrideSettings = new OverrideGraphicSettings();

            // Apply Overrides to all selected elements
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Reset Colors from LevelChecker");
                foreach (Element curElement in selectedElements)
                {
                    doc.ActiveView.SetElementOverrides(curElement.Id, overrideSettings);    
                }
                t.Commit();
            }
        }

        public string GetName()
        {
            return "Reset Color";
        }
    }
}
