using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace FTC.SharePoint.Evolution.Core
{
    class SPEvolutionAssembly
    {

            ///// <summary>
            ///// Список загруженных типов миграций
            ///// </summary>
            private readonly ReadOnlyCollection<SPEvolutionInfo> evolutionsTypes;

            ///// <summary>
            ///// Returns registered migration <see cref="System.Type">types</see>.
            ///// </summary>
            public ReadOnlyCollection<SPEvolutionInfo> EvolutionsTypes
            {
                get { return evolutionsTypes; }
            }

            
           

            /// <summary>
            /// Ключ миграций для данной сборки
            /// </summary>
            public string Key { get; private set; }           

     
            /// <summary>
            /// Максимальная доступная версия
            /// </summary>
            public long LastVersion {get; private set;}            
            
            public string FullName { get; private set; }

            /// <summary>
            /// Инициализация
            /// </summary>
            /// <param name="asm">Сборка с миграциями</param>
            public SPEvolutionAssembly(Assembly asm)
            {
                if (asm != null) 
                {
                    this.Key = GetAssemblyKey(asm);
                    var mt = GetEvolutionInfoList(asm);
                    var versions = mt.Select(info => info.Version);

                    CheckForDuplicatedVersion(versions);
                    this.evolutionsTypes = new ReadOnlyCollection<SPEvolutionInfo>(mt);

                    this.LastVersion = versions.Count() == 0 ? 0 : versions.Max();
                    this.FullName = asm.FullName;
                }
                
            }

            public static SPEvolutionAssembly Load(Assembly asm)
            {
                return new SPEvolutionAssembly(asm);
            }

            /// <summary>
            /// Получение ключа миграций для заданной сборки
            /// </summary>
            private static string GetAssemblyKey(Assembly assembly)
            {
                SPEvolutionAssemblyAttribute asmAttribute = AttributesHelper.GetEvolutionAssemblyAttribute(assembly);
                    //assembly.GetCustomAttribute<EvolutionAssemblyAttribute>();

                string assemblyKey = asmAttribute == null
                    ? string.Empty
                    : asmAttribute.Key ?? string.Empty;

                //MigratorLogManager.Log.DebugFormat("Migration key: {0}", assemblyKey);
                return assemblyKey;
            }

            /// <summary>
            /// Collect migrations in one <c>Assembly</c>.
            /// </summary>
            /// <param name="asm">The <c>Assembly</c> to browse.</param>
            private static List<SPEvolutionInfo> GetEvolutionInfoList(Assembly asm)
            {
                List<SPEvolutionInfo> evolutions = new List<SPEvolutionInfo>();

                foreach (Type type in asm.GetExportedTypes())
                {
                    SPEvolutionAttribute attribute = AttributesHelper.GetEvolutionAttribute(type);

                    if (attribute != null
                        && typeof(IEvolution).IsAssignableFrom(type)
                        && !attribute.Ignore)
                    {
                        SPEvolutionInfo mi = new SPEvolutionInfo(type);
                        evolutions.Add(mi);
                    }
                }

                evolutions.Sort(new SPEvolutionInfoComparer(true));

                // пишем в лог список загруженных миграций
                StringBuilder logMessageBuilder = new StringBuilder("Loaded evolutions step:").AppendLine();

                foreach (SPEvolutionInfo mi in evolutions)
                {
                    string msg = String.Format("{0} {1}", 
                        mi.Version.ToString().PadLeft(5),
                        ToHumanName(mi.Type.Name));
                    logMessageBuilder.AppendLine(msg);
                }

                SPEvolutionLog.Instance.Write(logMessageBuilder.ToString());
                //MigratorLogManager.Log.DebugFormat(logMessageBuilder.ToString());


                return evolutions;
            }

            /// <summary>
            /// Check for duplicated version in migrations.
            /// </summary>
            /// <exception cref="CheckForDuplicatedVersion">CheckForDuplicatedVersion</exception>
            public static void CheckForDuplicatedVersion(IEnumerable<long> migrationsTypes)
            {
                IEnumerable<long> list = migrationsTypes
                    .GroupBy(v => v)
                    .Where(x => x.Count() > 1)
                    .Select(x => x.Key);

                if (list.Count() > 0)
                {
                    throw new NotImplementedException();// DuplicatedVersionException(list);
                }
            }

            /// <summary>
            /// Создать миграцию по номеру версии
            /// </summary>
            /// <param name="version">Версия миграции</param>            
            public IEvolution InstantiateMigration(long version, SPEvolutionProvider provider)
            {
                //Require.IsNotNull(provider, "Не задан провайдер СУБД");

                var list = evolutionsTypes.Where(info => info.Version == version).ToList();

                if (list.Count == 0)
                {
                    return null;
                }

                IEvolution migration = (IEvolution)Activator.CreateInstance(list[0].Type);
                migration.EvoContext = provider;
                return migration;
            }

            public static string ToHumanName(string className)
            {
                string name = Regex.Replace(className, "([A-Z])", " $1").Substring(1);
                return name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower();
            }
        }
    
}
