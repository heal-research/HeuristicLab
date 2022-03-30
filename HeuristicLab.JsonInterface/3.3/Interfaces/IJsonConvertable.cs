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
    /// <param name="item">The IItem which get the data injected.</param>
    /// <param name="data">The JsonItem with the saved values.</param>
    void Inject(JsonItem data, JsonItemConverter converter);

    /// <summary>
    /// Extracts all infos out of an IItem to create a JsonItem. 
    /// (For template generation.)
    /// </summary>
    /// <param name="value">The IItem to extract infos.</param>
    /// <returns>JsonItem with infos to reinitialise the IItem.</returns>
    JsonItem Extract(JsonItemConverter converter);
  }
}
