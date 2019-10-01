using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.Manufacture {
  public interface IJsonItemConverter {
    /// <summary>
    /// Extracts all infos out of an IItem to create a Component. 
    /// (For template generation.)
    /// </summary>
    /// <param name="value">The IItem to extract infos.</param>
    /// <returns>Component with infos to reinitialise the IItem.</returns>
    Component Extract(IItem value);

    /// <summary>
    /// Injects the saved infos from the Component into the IItem.
    /// (Sets the necessary values.)
    /// </summary>
    /// <param name="item">The IItem which get the data injected.</param>
    /// <param name="data">The Component with the saved values.</param>
    void Inject(IItem item, Component data);
  }
}

