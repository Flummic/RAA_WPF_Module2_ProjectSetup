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
    public class Command : IExternalCommand
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

            // open form
            ProjectSetupForm currentForm = new ProjectSetupForm()
            {
                Width = 500,
                Height = 400,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            currentForm.ShowDialog();

            if(currentForm.DialogResult == true)
            {
                // get form data and do something
                // 1. Declare variables
                string filePath = currentForm.getFilePath();
                bool isMetric = currentForm.isMetric();
                bool createFloorPlans = currentForm.createFloorPlans();
                bool createCeilingPlans = currentForm.createCeilingPlans();

                // 2. Create a list of string arrays for CSV data (Data Container)
                List<string[]> levelData = new List<string[]>(); // created the list "levelData" which will contain string-arrays

                // 3. read text file datas
                string[] levelArray = System.IO.File.ReadAllLines(filePath);

                // 4. loop though file data and put into list
                foreach (string levelString in levelArray)
                {
                    string[] rowArray = levelString.Split(','); // create a new Array ("rowArray") and fill it with the current line ("levelString") split by a comma 
                    levelData.Add(rowArray); // add the new array (which includes the separated parts of the CSV-line (In levelArray) to the list ("levelData")
                }

                // 5. remove Header-Row from the list
                levelData.RemoveAt(0); // removes the first "row" (item) from the list

                // 6. create transaction (because we are about to change the Revit-Model)
                Transaction t = new Transaction(doc);
                t.Start("Create Levels and Plans from CSV");

                // 7. loop through level data
                int levelCounter = 0;
                int floorPlanCounter = 0;
                int ceilingPlanCounter = 0;

                foreach (string[] currentLevelData in levelData)
                {
                    // 8. create height variables
                    double heightFeet = 0;
                    double heightMeters = 0;

                    // 9. get height and convert from string to double (The Conversion is neccessary becaus we read the csv-lines as strings)
                    bool convertFeet = double.TryParse(currentLevelData[1], out heightFeet); // try to convert the second array-item of the currentLevelData-String-Array into a double and if it works (bool = True) overwrtie heightFeet with the new value
                    bool convertMeters = double.TryParse(currentLevelData[2], out heightMeters);

                    // 10. if using metric, convert meters to feet (because the Revit-API works with feet)
                    double heightMetersConvert = heightMeters * 3.28084;
                    double heightMetersConvert2 = UnitUtils.ConvertToInternalUnits(heightMeters, UnitTypeId.Meters); // alternative Method of doing the conversion

                    // 11. create Level and rename it
                    Level currentLevel = null;
                    if (isMetric)
                    {
                        currentLevel = Level.Create(doc, heightMetersConvert2); // create a Level at the current height in Feet
                        currentLevel.Name = currentLevelData[0]; // rename the level to the Name taken from the CSV (index 0 in the CurrentLevelData-String-Array
                    }
                    else
                    {
                        currentLevel = Level.Create(doc, heightFeet); // create a Level at the current height in Feet
                        currentLevel.Name = currentLevelData[0]; // rename the level to the Name taken from the CSV (index 0 in the CurrentLevelData-String-Array
                    }

                    // 12. increment counter (to count how many Levels were created by the addin
                    levelCounter++;

                    // create floor plans if selected
                    if (createFloorPlans)
                    {

                        // get a floorPlan Element ID
                        FilteredElementCollector collector = new FilteredElementCollector(doc);
                        IList<Element> viewFamilyTypes = collector.OfClass(typeof(ViewFamilyType)).ToElements();
                        ElementId floorPlanId = new ElementId(-1);
                        foreach (Element e in viewFamilyTypes)
                        {
                            ViewFamilyType v = e as ViewFamilyType;

                            if (v != null && v.ViewFamily == ViewFamily.FloorPlan)
                            {
                                floorPlanId = e.Id;
                                break;
                            }
                        }


                        ViewPlan floorPlan = ViewPlan.Create(doc, floorPlanId, currentLevel.Id);

                        floorPlanCounter++;

                    }

                    // create ceiling plans if selected
                    if (createCeilingPlans)
                    {
                        FilteredElementCollector collector = new FilteredElementCollector(doc);
                        IList<Element> viewFamilyTypes = collector.OfClass(typeof(ViewFamilyType)).ToElements();
                        ElementId ceilingPlanId = new ElementId(-1);
                        foreach (Element e in viewFamilyTypes)
                        {
                            if (e.Name == "Deckenplan") // german because of german Revit (Ceiling Plan)
                            {
                                ceilingPlanId = e.Id;
                                break;
                            }
                        }

                        ViewPlan ceilingPlan = ViewPlan.Create(doc, ceilingPlanId, currentLevel.Id);

                        ceilingPlanCounter++;
                    }

                }



                // 13. Commit and Dispose the transaction
                t.Commit();
                t.Dispose();

                // 14. Prompt the user with a dialog that shows how many Levels were created from which file
                TaskDialog.Show("Complete", "Created " + levelCounter.ToString() + " levels, " + floorPlanCounter.ToString() + " floor plans and " + ceilingPlanCounter.ToString() + " ceiling plans from the CSV-File located at: " + filePath);

                return Result.Succeeded;
            }
            else
            {
                return Result.Failed;
            }
            
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
