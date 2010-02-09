using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core
{
    public interface IVersion
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the current version.
        /// </summary>
        // ------------------------------------------------------------------
        double Version
        {
            get;
        }
    }
}
