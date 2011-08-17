using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [Item("RunCollection Modification Evaluator", "Applies a series of RunCollection modifiers.")]
  [Creatable("Testing & Analysis")]
  [StorableClass]
  public class RunCollectionModificationEvaluator : ParameterizedNamedItem, IStorableContent {

    public string Filename { get; set; }

    #region Parameters
    public ValueParameter<RunCollection> RunCollectionParameter {
      get { return (ValueParameter<RunCollection>)Parameters["RunCollection"]; }
    }

    public ValueParameter<CheckedItemList<IRunCollectionModifier>> ModifiersParameter {
      get { return (ValueParameter<CheckedItemList<IRunCollectionModifier>>)Parameters["Modifiers"]; }
    }
    #endregion

    #region Parameter Values
    public RunCollection RunCollection {
      get { return RunCollectionParameter.Value; }
    }
    public CheckedItemList<IRunCollectionModifier> Modifiers {
      get { return ModifiersParameter.Value; }
    }
    #endregion

    #region Construction & Cloning
    [StorableConstructor]
    protected RunCollectionModificationEvaluator(bool deserializing) : base(deserializing) { }
    protected RunCollectionModificationEvaluator(RunCollectionModificationEvaluator original, Cloner cloner) : base(original, cloner) { }
    public RunCollectionModificationEvaluator() {
      Parameters.Add(new ValueParameter<RunCollection>("RunCollection", "The RunCollection to be modified.", new RunCollection()));
      Parameters.Add(new ValueParameter<CheckedItemList<IRunCollectionModifier>>("Modifiers", "A list of modifiers to be applied to the run collection.", new CheckedItemList<IRunCollectionModifier>()));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RunCollectionModificationEvaluator(this, cloner);
    }
    #endregion

    public void Evaluate() {
      RunCollection.UpdateOfRunsInProgress = true;
      var runs = RunCollection.Where(r => r.Visible).ToList();
      foreach (var modifier in Modifiers.CheckedItems)
        modifier.Value.Modify(runs);
      RunCollection.UpdateOfRunsInProgress = false;
      if (runs.Count > 0) { // force update
        var run = (IRun) runs[0].Clone();
        RunCollection.Add(run);
        RunCollection.Remove(run);
      }
    }
  }
}
