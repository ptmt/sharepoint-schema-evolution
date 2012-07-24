using System;
using System.Collections.Generic;
using Microsoft.SharePoint;

namespace FTC.SharePoint.Evolution.Core
{
    public static class SPEvolutionProviderConstants
    {
        public const string PROPERTY_NAME = "spevolutions";
        public const string LAST_ASSEMBLY_PROPERTY = "lastassembly";
    }

    /// <summary>
    /// not abstract, just for SP2010
    /// </summary>
    public class SPEvolutionProvider
    {
        
        private SPWeb web;

        public SPWeb Web { get { return web; } }

        public bool IsDebug { get; set; }

        #region internal
        internal SPEvolutionProvider(SPWeb web)
        {
            this.web = web;
           
        }
        
        internal List<long> GetAppliedEvolutions()
        {
            return GetAppliedEvolutions(String.Empty);
        }
        internal List<long> GetAppliedEvolutions(string key)
        {            
            var applied = new List<long>();
            if (web.AllProperties[SPEvolutionProviderConstants.PROPERTY_NAME] != null) 
            {
                applied = DeserializeStringToLongList(web.AllProperties[SPEvolutionProviderConstants.PROPERTY_NAME].ToString());
                applied.Remove(0);
            }           
            return applied;
        }

        internal string GetLastAppliedAssembly()
        {
            if (web.AllProperties[SPEvolutionProviderConstants.LAST_ASSEMBLY_PROPERTY] != null)
            {
                return web.AllProperties[SPEvolutionProviderConstants.LAST_ASSEMBLY_PROPERTY].ToString();
            }
            return String.Empty;
        }


        internal void AddEvolutionToApplied(long version, string key)
        {
            if (version == 0) return;
            List<long> appliedToWeb;
            if (web.AllProperties[SPEvolutionProviderConstants.PROPERTY_NAME] != null)
                appliedToWeb = DeserializeStringToLongList(web.AllProperties[SPEvolutionProviderConstants.PROPERTY_NAME].ToString());
            else
                appliedToWeb = new List<long> ();
            appliedToWeb.Add(version);
            web.AllProperties[SPEvolutionProviderConstants.PROPERTY_NAME] = SerializeLongListToString(appliedToWeb);
            web.Update();
        }       
      
        internal void RemoveEvolutionFromApplied(long version, string key)
        {
            if (web.AllProperties[SPEvolutionProviderConstants.PROPERTY_NAME] != null)
            {
                List<long> appliedToWeb = DeserializeStringToLongList(web.AllProperties[SPEvolutionProviderConstants.PROPERTY_NAME].ToString());
                if (appliedToWeb.Contains(version))
                {
                    appliedToWeb.Remove(version);
                    if (appliedToWeb.Count == 0)
                        web.AllProperties[SPEvolutionProviderConstants.PROPERTY_NAME] = null;
                    else
                        web.AllProperties[SPEvolutionProviderConstants.PROPERTY_NAME] = SerializeLongListToString(appliedToWeb);
                    web.Update();
                }
            }         
        }
        internal void AddLastAssembly(string assemblyFullString)
        {
            web.AllProperties[SPEvolutionProviderConstants.LAST_ASSEMBLY_PROPERTY] = assemblyFullString;
        }
        #endregion

        #region helpermethods
        public SPList AddEmptyList(string name, string description)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("\tAddEmptyList unsuccessful: listname cannot be null");     
            if (this.Web.Lists.TryGetList(name) == null)
            {
                try
                {
                    this.Web.Lists.Add(name, description, SPListTemplateType.GenericList);
                    SPEvolutionLog.Instance.Write(String.Format("\tAddEmptyList {0} successful", name));
                }
                catch (Exception e)
                {
                    SPEvolutionLog.Instance.Write(String.Format("\tAddEmptyList {0} successful but exception {1}", name, e.Message));
                }
            }
            return this.Web.Lists.TryGetList(name);
        }

        public void DeleteList(string name)
        {          
            SPList list = this.Web.Lists.TryGetList(name);
            if (list != null)
            {
                this.Web.Lists.Delete(list.ID);
                SPEvolutionLog.Instance.Write(String.Format("\tDeleteList {0} successful", name));
            }
            else
                SPEvolutionLog.Instance.Write(String.Format("\tDeleteList {0} unsuccessful: list not exist", name));           
           
        }

        public void AddColumn(SPList list, string fieldname, SPFieldType type, bool required)
        {
            if (String.IsNullOrEmpty(fieldname))
                throw new ArgumentException("\tAddColumn unsuccessful: fieldname cannot be null");              
            if (list == null)
                throw new ArgumentException(String.Format("\tAddColumn {0} unsuccessful: List cannot be null", fieldname));
            else
            {
                if (!list.Fields.ContainsField(fieldname))
                {
                    list.Fields.Add(fieldname, type, required);
                    SPEvolutionLog.Instance.Write(String.Format("\tAddColumn {0} successful", fieldname));
                }
                else
                    SPEvolutionLog.Instance.Write(String.Format("\tAddColumn {0} unsuccessful: Column with this name already exist", fieldname), SPEvolutionLogLevel.ValidationError);
            }
        }

        public void DeleteColumn(SPList list, string fieldname)
        {
            if (String.IsNullOrEmpty(fieldname))
                throw new ArgumentException("\tDeleteColumn unsuccessful: fieldname cannot be null");
                
            if (list == null)
                throw new ArgumentException("\tDeleteColumn unsuccessful: List cannot be null");

            if (list.Fields.ContainsField(fieldname))
            {
                list.Fields.Delete(fieldname);
                SPEvolutionLog.Instance.Write(String.Format("\tDeleteColumn {0} successful", fieldname));
            }
        }
        #endregion

        internal static string SerializeLongListToString(List<long> list)
        {
            return String.Join(";", list.ConvertAll(a => a.ToString()).ToArray());
        }

        internal static List<long> DeserializeStringToLongList(string str)
        {            
            if (str.Contains(";"))
                return new List<string>(str.Split(';')).ConvertAll(a => Convert.ToInt64(a));
            else  {
                long res = 0;
                Int64.TryParse(str, out res);
                return new List<long>() { res  };
            }
        }
    }
}
