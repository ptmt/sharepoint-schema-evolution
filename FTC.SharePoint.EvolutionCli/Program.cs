using System;
using System.Linq;
using Microsoft.SharePoint;
using System.Configuration;
using FTC.SharePoint.Evolution;

namespace FTC.SharePoint.EvolutionCli
{
    class Program
    {
        static void Main(string[] args)
        {
            string weburl = System.Configuration.ConfigurationSettings.AppSettings["WebUrl"];
            if (!weburl.Contains(';'))
                weburl += ";";
            string[] weburls = weburl.Split(';');
            string version = String.Empty;
            foreach (string nextweburl in weburls)
            {
                if (nextweburl.TrimEnd().TrimStart() != String.Empty)
                {
                    Console.WriteLine("=== trying to make evolution on {0} ===", nextweburl);
                    string assembly = System.Configuration.ConfigurationSettings.AppSettings["Assembly"];
                    Console.WriteLine("|Assembly:\t{0}", assembly);
                    bool isDebug = false;
                    Boolean.TryParse(System.Configuration.ConfigurationSettings.AppSettings["debug"], out isDebug);
                    Console.WriteLine("|DebugMode:\t{0}", isDebug);
                    long versionto = -1;
                   
                    if (args.Count() == 0 || String.IsNullOrEmpty(args[0]))
                         version = System.Configuration.ConfigurationSettings.AppSettings["VersionTo"].ToString();//, out versionto);
                    else                                            
                        version = args[0];
                    
                    Console.WriteLine("|VersionTo:\t{0}", version);
                    using (SPSite site = new SPSite(nextweburl))
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPEvolutionManager evolMan = SPEvolutionFactory.GetManagerForWeb(assembly, web);
                        evolMan.EnableConsoleLogs();
                        if (isDebug)
                            evolMan.EnableDebug();
                        if (version == "prev")
                        {
                            evolMan.MakeEvolutionStepRollback();
                        }
                        else
                        {
                            if (versionto != -1)
                                evolMan.MakeEvolution(versionto);
                            else
                                evolMan.MakeEvolution();
                        }
                    }
                }
            }
        }
    }
}
