using System;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public interface IJsonItemConverter {
    /// <summary>
    /// Injects the saved infos from the JsonItem into the IItem.
    /// (Sets the necessary values.)
    /// </summary>
    /// <param name="item">The IItem which get the data injected.</param>
    /// <param name="data">The JsonItem with the saved values.</param>
    void Inject(IItem item, IJsonItem data, IJsonItemConverter root);

    /// <summary>
    /// Extracts all infos out of an IItem to create a JsonItem. 
    /// (For template generation.)
    /// </summary>
    /// <param name="value">The IItem to extract infos.</param>
    /// <returns>JsonItem with infos to reinitialise the IItem.</returns>
    IJsonItem Extract(IItem value, IJsonItemConverter root);

    /// <summary>
    /// A given priority, higher numbers are prior.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Checks if the given type can be converted.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    bool CanConvertType(Type t);
  }
}

