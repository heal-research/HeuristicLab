using System.Collections.Generic;

namespace HeuristicLab.JsonInterface {
  public interface IConcreteRestrictedJsonItem<T> : IJsonItem {
    /// <summary>
    /// The item, 
    /// </summary>
    IEnumerable<T> ConcreteRestrictedItems { get; set; }
  }
}
