using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;

namespace FTC.SharePoint.Evolution.Core
{
    public enum SPEvolutionLogType
    {
        Console,
        SharePointLog,
        File
    }
    public enum SPEvolutionLogLevel
    {
        Debug,
        ValidationError,
        Exception
    }
    public class SPEvolutionLog // fixed slngleton bug for sharepoint
    {

        private static SPEvolutionLog _Instance = null;
        private static readonly object _Lock = new object();

        private SPEvolutionLog() { }

        public static SPEvolutionLog Instance
        {
            get
            {
                lock (_Lock)
                {
                    if (_Instance == null)
                        _Instance = new SPEvolutionLog();

                    return _Instance;
                }
            }
        }
      
        //private sealed class SingletonCreator
        //{
        //    private static readonly SPEvolutionLog instance = new SPEvolutionLog();
        //    public static SPEvolutionLog Instance { get { return instance; } }
        //}

        //public static SPEvolutionLog Instance
        //{
        //    get { return SingletonCreator.Instance; }
        //}

        private List<SPEvolutionLogType> logtypes;
        public List<SPEvolutionLogType> LogTypes
        {
            get
            {
                if (logtypes == null)
                    logtypes = new List<SPEvolutionLogType>();
                return logtypes;
            }
            set { logtypes = value; }
        }


        public void Write(string message)
        {
            Write(message, SPEvolutionLogLevel.Debug);
        }
        public void Write(string message, SPEvolutionLogLevel level)
        {
            
            if (LogTypes != null)
            {
                foreach (SPEvolutionLogType log in LogTypes)
                {
                    switch (log)
                    {
                        case SPEvolutionLogType.Console:
                            Console.WriteLine(message);
                            break;
                        case SPEvolutionLogType.SharePointLog:
                            TraceSeverity severity = TraceSeverity.Monitorable;
                            if (level == SPEvolutionLogLevel.Exception)                            
                                severity = TraceSeverity.Unexpected;                                                           
                            if (level == SPEvolutionLogLevel.ValidationError)
                                severity = TraceSeverity.High;
                            
                            SPDiagnosticsService.Local.WriteTrace(0,
                                   new SPDiagnosticsCategory("SPEvolution", severity, EventSeverity.Information),
                                       severity, message);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
