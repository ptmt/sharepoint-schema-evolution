using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTC.SharePoint.Evolution.Core
{
    public abstract class SPEvolution : IEvolution
    {       
            public string Name
            {
                get { return SPEvolutionAssembly.ToHumanName(GetType().Name); }
            }

            /// <summary>
            /// Defines tranformations to port the SharePoint Web Schema to the current version.
            /// </summary>
            public abstract void Up();

            /// <summary>
            /// Defines transformations to revert things done in <c>Up</c>.
            /// </summary>
            public abstract void Down();

            /// <summary>
            /// Represents the SharePoint Context.            
            /// </summary>            
            public SPEvolutionProvider EvoContext { get; set; }
        
    }
}
