using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace RAA_WPF_Module2_ProjectSetup
{
    public static class Globals
    {
        public static string SheetName;
        public static bool IsPlaceholder;
        public static string SheetNumber;


        // Variables for the Level Checker
        public static Level selectedLevel;
        public static bool setColor;
        public static List<string> selectedCategories;
    }
}
