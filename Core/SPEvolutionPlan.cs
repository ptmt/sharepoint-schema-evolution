using System.Collections;
using System.Collections.Generic;
       

namespace FTC.SharePoint.Evolution.Core
{
    public sealed class SPEvolutionPlan : IEnumerable<long>
    {
        private readonly IEnumerable<long> versions;
        public long StartVersion { get; private set; }
        public SPEvolutionPlan(IEnumerable<long> versions, long startVersion)
        {
            //Require.IsNotNull(versions, "Не задан список версий");
            this.versions = versions;
            StartVersion = startVersion;
        }
        public IEnumerator<long> GetEnumerator()
        {
            return versions.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    
      

}
