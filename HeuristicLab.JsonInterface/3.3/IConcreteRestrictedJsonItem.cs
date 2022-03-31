using System.Collections.Generic;

namespace HeuristicLab.JsonInterface {
  public interface IConcreteRestrictedJsonItem<T> : IJsonItem {
    /// <summary>
    /// array of restricted items
    /// </summary>
    IEnumerable<T> ConcreteRestrictedItems { get; set; }
  }
}
