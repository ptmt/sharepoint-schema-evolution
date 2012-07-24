using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace FTC.SharePoint.Evolution.Core
{
    public class SPEvolutionAttribute : Attribute
    {
        private long _version;
        private bool _ignore = false;

        /// <summary>
        /// Describe the migration
        /// </summary>
        /// <param name="version">The unique version of the migration.</param>	
        public SPEvolutionAttribute(long version)
        {
            Version = version;
        }

        /// <summary>
        /// The version reflected by the migration
        /// </summary>
        public long Version
        {
            get { return _version; }
            private set { _version = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to ignore this migration.
        /// </summary>
        public bool Ignore
        {
            get { return _ignore; }
            set { _ignore = value; }
        }
    }

    public class SPEvolutionAssemblyAttribute : Attribute
    {
        private string _key;

        /// <summary>
        /// Describe the migration
        /// </summary>
        /// <param name="key">Key of the migration.</param>	
        public SPEvolutionAssemblyAttribute(string key)
        {
            Key = key;
        }

        /// <summary>
        /// The key of the migration
        /// </summary>
        public string Key
        {
            get { return _key; }
            private set { _key = value; }
        }
    }

    public static class AttributesHelper
    {
        public static SPEvolutionAttribute GetEvolutionAttribute(Type type)
        {
            try
            {
                return type.GetCustomAttributes(typeof(SPEvolutionAttribute), false)[0] as SPEvolutionAttribute;
            }
            catch
            {
                return null;
            }
        }

        public static SPEvolutionAssemblyAttribute GetEvolutionAssemblyAttribute(Assembly asm)
        {
            try
            {                
                return asm.GetCustomAttributes(typeof(SPEvolutionAssemblyAttribute), false)[0] as SPEvolutionAssemblyAttribute;
            }
            catch
            {
                return null;
            }
        }
    }
}
