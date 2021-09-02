using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.JsonInterface {
  public interface IResultCollectionPostProcessor : IItem {
    
    void Apply(IObservableDictionary<string, IItem> /*ResultCollection*/ results, IDictionary<string, string> output); //evtl. doch JToken als Dict?
  }
}
