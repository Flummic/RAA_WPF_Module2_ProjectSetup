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
    public class LevelChecker : IExternalCommand
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
            List<Level> levelList = Utils.GetAllLevels(doc);

            SetColor setColorAction = new SetColor();
            ExternalEvent setColorEvent = ExternalEvent.Create(setColorAction);

            ReSetColor reSetColorAction = new ReSetColor();
            ExternalEvent reSetColorEvent = ExternalEvent.Create(reSetColorAction);

            // open form
            LevelCheckerForm currentForm = new LevelCheckerForm(levelList, setColorEvent, reSetColorEvent)
            {
                Width = 500,
                Height = 400,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            currentForm.Show();
            return Result.Succeeded;
            
        }

        public string GetName()
        {
            return "EventAction";
        }
    }

}
