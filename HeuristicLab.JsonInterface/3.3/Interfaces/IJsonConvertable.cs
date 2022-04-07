using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  public interface IJsonConvertable {
    /// <summary>
    /// Injects the saved infos from the JsonItem into the IItem.
    /// (Sets the necessary values.)
    /// </summary>
    /// <param name="item">The JsonItem with the saved values.</param>
    /// <param name="converter">The convorter which started the 
    /// injection (necessary to prevent loops).</param>
    void Inject(JsonItem item, JsonItemConverter converter);

    /// <summary>
    /// Extracts all infos out of an IItem to create a JsonItem. 
    /// (For template generation.)
    /// </summary>
    /// <param name="converter">The converter which started the extraction (necessary to prevent loops).</param>
    /// <returns>JsonItem with infos to reinitialise the IItem.</returns>
    JsonItem Extract(JsonItemConverter converter);
  }
}
