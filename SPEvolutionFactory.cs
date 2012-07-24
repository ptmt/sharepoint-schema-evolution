using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.SharePoint;
using FTC.SharePoint.Evolution.Core;


namespace FTC.SharePoint.Evolution
{
    public class SPEvolutionFactory
    {
        public static SPEvolutionManager GetManagerForWeb(string AssemblyFile, SPWeb web)
        {
            return new SPEvolutionManager(SharePointVersion.SharePointFoundation2010, GetAssembly(AssemblyFile, false), web);
        }
        public static SPEvolutionManager GetManagerForWeb(Assembly assembly, SPWeb web)
        {
            return new SPEvolutionManager(SharePointVersion.SharePointFoundation2010, assembly, web);
        }
        
        private static Assembly GetAssembly(string AssemblyFile, bool isFullPath)
        {
            Assembly assembly = null;

            if (isFullPath)
                assembly = Assembly.LoadFrom(AssemblyFile);
            else
                assembly = Assembly.Load(AssemblyFile);

            if (assembly == null)
                throw new NotImplementedException("Evolutions Assembly path is wrong");
            //Require.IsNotNull(assembly, "Не задана сборка, содержащая миграции");
            return assembly;
        }

        public static void Log(string message)
        {
            SPEvolutionLog.Instance.Write(message);
        }

    }
}
