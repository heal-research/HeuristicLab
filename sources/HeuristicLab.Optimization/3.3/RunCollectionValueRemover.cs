using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {

  [Item("RunCollection Value Remover", "Modifies a RunCollection by removing results or parameters.")]
  [StorableClass]
  public class RunCollectionValueRemover : ParameterizedNamedItem, IRunCollectionModifier {
    
    public ValueParameter<CheckedItemCollection<StringValue>> ValuesParameter {
      get { return (ValueParameter<CheckedItemCollection<StringValue>>)Parameters["Values"]; }
    }

    public IEnumerable<string> Values {
      get { return ValuesParameter.Value.CheckedItems.Select(v => v.Value); }
    }

    #region Construction & Cloning    
    [StorableConstructor]
    protected RunCollectionValueRemover(bool deserializing) : base(deserializing) { }
    protected RunCollectionValueRemover(RunCollectionValueRemover original, Cloner cloner)
      : base(original, cloner) {
    }
    public RunCollectionValueRemover() {
      Parameters.Add(new ValueParameter<CheckedItemCollection<StringValue>>("Values", "The result or parameter values to be removed from each run."));      
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RunCollectionValueRemover(this, cloner);
    }    
    #endregion    

    public void Modify(List<IRun> runs) {      
      foreach (var run in runs) {
        foreach (var value in Values) {
          run.Parameters.Remove(value);
          run.Results.Remove(value);
        }        
      }      
    }
    
  }
}
