using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  public interface IConcreteRestrictedJsonItem<T> : IJsonItem {
    IEnumerable<T> ConcreteRestrictedItems { get; set; }
  }
}
