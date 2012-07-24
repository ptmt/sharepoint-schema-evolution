using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTC.SharePoint.Evolution.Core
{
    interface IEvolution
    {
        string Name { get; }
        SPEvolutionProvider EvoContext { get; set; }

        /// <summary>
        /// Defines tranformations to port the sharepoint web to the current version.
        /// </summary>
        void Up();

        /// <summary>
        /// Defines transformations to revert things done in <c>Up</c>.
        /// </summary>
        void Down();
    }
}
