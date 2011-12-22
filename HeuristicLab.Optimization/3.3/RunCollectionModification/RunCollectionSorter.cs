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
  [Item("RunCollection Sorter", "Sorts a run collection according the specified key variable.")]
  [StorableClass]
  public class RunCollectionSorter : ParameterizedNamedItem, IRunCollectionModifier {

    public override bool CanChangeName { get { return false; } }
    public override bool CanChangeDescription { get { return false; } }

    #region Parameters
    public ValueParameter<StringValue> ValueParameter {
      get { return (ValueParameter<StringValue>)Parameters["Value"]; }
    }
    #endregion

    private string Value { get { return ValueParameter.Value.Value; } }

    #region Construction & Cloning
    [StorableConstructor]
    protected RunCollectionSorter(bool deserializing) : base(deserializing) { }
    protected RunCollectionSorter(RunCollectionSorter original, Cloner cloner)
      : base(original, cloner) {
      RegisterEvents();
    }
    public RunCollectionSorter() {
      Parameters.Add(new ValueParameter<StringValue>("Value", "The variable name used as sorting key.", new StringValue("Value")));
      RegisterEvents();
      UpdateName();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RunCollectionSorter(this, cloner);
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }
    #endregion

    private void RegisterEvents() {
      ValueParameter.ToStringChanged += Parameter_NameChanged;
    }

    private void Parameter_NameChanged(object sender, EventArgs e) {
      UpdateName();
    }

    private void UpdateName() {
      name = string.Format("Sort by {0}", Value);
      OnNameChanged();
    }

    private class ValueComparer : IComparer<IComparable> {

      #region IComparer<IComparable> Members

      public int Compare(IComparable x, IComparable y) {
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        return x.CompareTo(y);
      }

      #endregion
    }

    private static readonly ValueComparer Comparer = new ValueComparer();

    #region IRunCollectionModifier Members

    public void Modify(List<IRun> runs) {
      var sortedRuns = runs
        .Select(r => new {Run = r, Key = GetValue(r)})
        .OrderBy(r => r.Key, Comparer)
        .Select(r => r.Run).ToList();
      runs.Clear();
      runs.AddRange(sortedRuns);
    }

    private IComparable GetValue(IRun run) {
      return GetValue(run.Results) ?? GetValue(run.Parameters);
    }

    private IComparable GetValue(IDictionary<string, IItem> variables) {
      IItem value;
      variables.TryGetValue(Value, out value);
      var intValue = value as IntValue;
      if (intValue != null)
        return intValue;
      var doubleValue = value as DoubleValue;
      if (doubleValue != null)
        return doubleValue;
      if (value != null)
        return value.ToString();
      return null;
    }

    #endregion
  }
}
