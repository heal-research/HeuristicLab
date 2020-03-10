using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  public interface IRangedJsonItem<T> : IIntervalRestrictedJsonItem<T>
    where T : IComparable 
  {
    T MinValue { get; set; }
    T MaxValue { get; set; }
  }
}
