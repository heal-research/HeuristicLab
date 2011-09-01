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

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (RunCollection != null && RunCollection.Modifiers != null) {
        RunCollection.Modifiers.AddRange(Modifiers);
        foreach (var modifier in RunCollection.Modifiers) {
          RunCollection.Modifiers.SetItemCheckedState(modifier, Modifiers.ItemChecked(modifier));
        }
        Modifiers.Clear();
      }
    }

    public void Evaluate() {
      RunCollection.UpdateOfRunsInProgress = true;
      var runs = RunCollection.Select((r, i) => new { Run = r, r.Visible, Index = i }).ToList();
      var visibleRuns = runs.Where(r => r.Visible).Select(r => r.Run).ToList();
      int n = visibleRuns.Count;
      if (n == 0)
        return;
      foreach (var modifier in Modifiers.CheckedItems)
        modifier.Value.Modify(visibleRuns);
      if (n != visibleRuns.Count ||
          runs.Where(r => r.Visible).Zip(visibleRuns, (r1, r2) => r1.Run != r2).Any()) {
        var runIt = runs.GetEnumerator();
        var visibleRunIt = visibleRuns.GetEnumerator();
        var newRuns = new List<IRun>();
        while (runIt.MoveNext()) {
          if (!runIt.Current.Visible)
            newRuns.Add(runIt.Current.Run);
          else if (visibleRunIt.MoveNext())
            newRuns.Add(visibleRunIt.Current);
        }
        while (visibleRunIt.MoveNext())
          newRuns.Add(visibleRunIt.Current);
        RunCollection.Clear();
        RunCollection.AddRange(newRuns);
      } else if (runs.Count > 0) { // force update
        var run = (IRun)runs[0].Run.Clone();
        RunCollection.Add(run);
        RunCollection.Remove(run);
      }
      RunCollection.UpdateOfRunsInProgress = false;
    }
  }
}
