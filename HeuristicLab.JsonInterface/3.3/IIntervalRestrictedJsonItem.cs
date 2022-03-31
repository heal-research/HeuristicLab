using System;

namespace HeuristicLab.JsonInterface {
  /// <summary>
  /// Interface to get a interval restrict JsonItems.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IIntervalRestrictedJsonItem<T> : IJsonItem
    where T : IComparable {
    /// <summary>
    /// The allowed minimum of the JsonItem.
    /// </summary>
    T Minimum { get; set; }
    /// <summary>
    /// The allowed maximum of the JsonItem.
    /// </summary>
    T Maximum { get; set; }
  }
}
