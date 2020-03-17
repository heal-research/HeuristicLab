using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  public interface IIntervalRestrictedJsonItem<T> : IJsonItem
    where T : IComparable {
    T Minimum { get; set; }
    T Maximum { get; set; }
  }
}
