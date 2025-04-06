using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAA_WPF_Module2_ProjectSetup
{
    internal static class Utils
    {
        internal static RibbonPanel CreateRibbonPanel(UIControlledApplication app, string tabName, string panelName)
        {
            RibbonPanel currentPanel = GetRibbonPanelByName(app, tabName, panelName);

            if (currentPanel == null)
                currentPanel = app.CreateRibbonPanel(tabName, panelName);

            return currentPanel;
        }

        internal static List<Level> GetAllLevels(Document doc)
        {
            FilteredElementCollector levelCollector = new FilteredElementCollector(doc);
            levelCollector.OfClass(typeof(Level));
            levelCollector.WhereElementIsNotElementType();

            return levelCollector.Cast<Level>().ToList();
        }

        internal static FillPatternElement GetFillPatternByName(Document doc, string patternName)
        {
            FillPatternElement myFillPatternElement = null;

            myFillPatternElement = FillPatternElement.GetFillPatternElementByName(doc, FillPatternTarget.Drafting, patternName);
            return myFillPatternElement;
        }

        internal static RibbonPanel GetRibbonPanelByName(UIControlledApplication app, string tabName, string panelName)
        {
            foreach (RibbonPanel tmpPanel in app.GetRibbonPanels(tabName))
            {
                if (tmpPanel.Name == panelName)
                    return tmpPanel;
            }

            return null;
        }
    }
}
