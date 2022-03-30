using System;

namespace HeuristicLab.JsonInterface {
  public interface IRangedJsonItem<T> : IIntervalRestrictedJsonItem<T>
    where T : IComparable 
  {
    /// <summary>
    /// The lower bound of an ranged item. 
    /// </summary>
    T Start { get; set; }
    /// <summary>
    /// The upper bound of an ranged item.
    /// </summary>
    T End { get; set; }
  }
}
