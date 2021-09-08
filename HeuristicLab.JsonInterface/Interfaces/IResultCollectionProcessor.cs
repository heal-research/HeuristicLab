using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.JsonInterface {
  public interface IResultCollectionProcessor : IParameterizedNamedItem {
    void Apply(IObservableDictionary<string, IItem> results);
  }
}
