using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Reflection;
using Microsoft.SharePoint;
using FTC.SharePoint.Evolution.Core;

namespace FTC.SharePoint.Evolution
{
    public enum SharePointVersion
    {
        SharePointServer2010,
        SharePointFoundation2010
    }
    public class SPEvolutionManager
    {
        private readonly SPEvolutionAssembly evoAssembly;
        private string Key
        {
            get { return evoAssembly.Key; }
        }

        private readonly SPEvolutionProvider provider;

        public SPEvolutionManager(SharePointVersion sharepointVersion, Assembly asm, SPWeb web)
		{			
            if (asm == null)
                throw new ArgumentNullException("Assembly file not found");
            evoAssembly = new SPEvolutionAssembly(asm);
            provider = new SPEvolutionProvider(web);
		}
        
        
        public void MakeEvolutionStepRollback()
        {
            IList<long> applied = provider.GetAppliedEvolutions(Key);
            long prevVersion = 0;
            if (applied.Count() == 0)
                SPEvolutionLog.Instance.Write("No applied evolutions"); 
            else {
                if (applied.Count() == 1)
                    prevVersion = 0;
                else
                    prevVersion = applied.OrderByDescending(c => c).ToList()[1];
                MakeEvolution(prevVersion);
            }
            
        }

        public void MakeEvolution()
        {
            MakeEvolution(-1);
        }
        public void MakeEvolution(long toVersion)
        {

            long targetVersion = toVersion < 0 ? this.evoAssembly.LastVersion : toVersion;

            IList<long> applied = provider.GetAppliedEvolutions(Key);
            IList<long> available = this.evoAssembly.EvolutionsTypes.Select(mInfo => mInfo.Version).ToList();

            SPEvolutionPlan plan = BuildMigrationPlan(targetVersion, applied, available);

            long currentDatabaseVersion = plan.StartVersion;
            //MigratorLogManager.Log.Started(currentDatabaseVersion, targetVersion);
            SPEvolutionLog.Instance.Write(String.Format("| Already aplied to Web:{0}, in assembly available:{1}", applied.Count, available.Count));
            SPEvolutionLog.Instance.Write(String.Format("| Evolution will be Executed From {0} --> {1}", currentDatabaseVersion, targetVersion));

            foreach (long currentExecutedVersion in plan)
            {
                ExecuteStep(currentExecutedVersion, currentDatabaseVersion);
                currentDatabaseVersion = currentExecutedVersion;
            }
        }
        public void ExecuteStep(long targetVersion, long currentDatabaseVersion)
        {
            IEvolution migration = this.evoAssembly.InstantiateMigration(targetVersion, this.provider);
            if (migration == null)
                throw new ArgumentNullException(String.Format("Evolution step with version {0} not found", targetVersion));
        
                if (targetVersion <= currentDatabaseVersion)

                {
                    SPEvolutionLog.Instance.Write(String.Format("<=Rollback Start TargetVersion={0} Name={1}", targetVersion, migration.Name));
                    try
                    {
                        migration.Down();
                        SPEvolutionLog.Instance.Write(String.Format("<=Rollback Successful TargetVersion={0} Name={1}", targetVersion, migration.Name));
                        this.provider.RemoveEvolutionFromApplied(targetVersion, Key);
                    }
                    catch (Exception e)
                    {
                        SPEvolutionLog.Instance.Write(String.Format("<=Rollback Unsuccessful, Details: {0}", e.Message));
                    }
                    
                }
                else
                {
                    SPEvolutionLog.Instance.Write(String.Format("=>Evolution Start TargetVersion={0} Name={1}", targetVersion, migration.Name));
                    try
                    {
                        migration.Up();
                        SPEvolutionLog.Instance.Write(String.Format("=>Evolution Successfull Evolution Step Up TargetVersion={0} Name={1}", targetVersion, migration.Name));
                        this.provider.AddEvolutionToApplied(targetVersion, Key);
                        this.provider.AddLastAssembly(evoAssembly.FullName);
                    }
                    catch (Exception e)
                    {
                        SPEvolutionLog.Instance.Write(String.Format("=>Evolution Unsuccessfull, will be rollback to previous version. Details: {0}", e.Message + e.StackTrace));                 
                        migration.Down();
                        SPEvolutionLog.Instance.Write(String.Format("<=Rollback Successful, Back To {0} ", currentDatabaseVersion, migration.Name));                 
                    }
                   
                    
                }

              
        }

        public static SPEvolutionPlan BuildMigrationPlan(long target, IEnumerable<long> applied, IEnumerable<long> availableEvos)
        {
            long startVersion = applied.Count() < 1 ? 0 : applied.Max();
            HashSet<long> set = new HashSet<long>(applied);

            // проверки
            var list = availableEvos.Where(x => x < startVersion && !set.Contains(x));
            if (list.Count() > 1)
            {
                throw new ArgumentException
                    ("Current Evolution Schema Version Error: There is not applied evolutions step");//, list);
            }

            set.UnionWith(availableEvos);

            var versions = target < startVersion
                            ? set.Where(n => n <= startVersion && n > target).OrderByDescending(x => x).ToList()
                            : set.Where(n => n > startVersion && n <= target).OrderBy(x => x).ToList();

            return new SPEvolutionPlan(versions, startVersion);
        }

        public void EnableSharePointLogs()
        {         
            SPEvolutionLog.Instance.LogTypes.Add(SPEvolutionLogType.SharePointLog);
        }

        public void EnableConsoleLogs()
        {         
            SPEvolutionLog.Instance.LogTypes.Add(SPEvolutionLogType.Console);
        }

        public void EnableDebug()
        {
            this.provider.IsDebug = true;
        }
    }   
   
}
