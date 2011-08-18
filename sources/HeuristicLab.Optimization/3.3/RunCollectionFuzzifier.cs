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
  [Item("RunCollection Fuzzifier", "Creates several levels from the distribution of a certain result accross a run collection and assignes a fuzzified value.")]
  [StorableClass]
  public class RunCollectionFuzzifier : ParameterizedNamedItem, IRunCollectionModifier {

    public override bool CanChangeName { get { return false; } }
    public override bool CanChangeDescription { get { return false; } }

    #region Parameters
    public ValueParameter<StringValue> SourceParameter {
      get { return (ValueParameter<StringValue>)Parameters["Source"]; }
    }
    public ValueParameter<StringValue> TargetParameter {
      get { return (ValueParameter<StringValue>)Parameters["Target"]; }
    }
    public ValueParameter<StringValue> SuffixParameter {
      get { return (ValueParameter<StringValue>)Parameters["Suffix"]; }
    }
    public ValueParameter<DoubleValue> SpreadParameter {
      get { return (ValueParameter<DoubleValue>)Parameters["Spread"]; }
    }
    public ValueParameter<StringValue> GroupByParameter {
      get { return (ValueParameter<StringValue>)Parameters["GroupBy"]; }
    }    
    public ValueParameter<ItemList<StringValue>> LevelsParameter {
      get { return (ValueParameter<ItemList<StringValue>>)Parameters["Levels"]; }
    }    
    #endregion

    private string Source { get { return SourceParameter.Value.Value; } }
    private string Target { get { return TargetParameter.Value.Value; } }
    private string Suffix { get { return SuffixParameter.Value.Value; } }
    private double Spread { get { return SpreadParameter.Value.Value; } }
    private string GroupBy { get { return GroupByParameter.Value.Value; } }
    private List<string> Levels { get { return LevelsParameter.Value.Select(v => v.Value).ToList(); } }

      #region Construction & Cloning
    [StorableConstructor]
    protected RunCollectionFuzzifier(bool deserializing) : base(deserializing) { }
    protected RunCollectionFuzzifier(RunCollectionFuzzifier original, Cloner cloner)
      : base(original, cloner) {
      RegisterEvents();
    }
    public RunCollectionFuzzifier() {
      Parameters.Add(new ValueParameter<StringValue>("Source", "Source value name to be fuzzified.", new StringValue("Value")));
      Parameters.Add(new ValueParameter<StringValue>("Target", "Target value name. The new, fuzzified variable to be created.", new StringValue("Calc.Value")));
      Parameters.Add(new ValueParameter<StringValue>("Suffix", "The suffix of all fuzzified values.", new StringValue()));
      Parameters.Add(new ValueParameter<DoubleValue>("Spread", "The number of standard deviations considered one additional level.", new DoubleValue(1)));
      Parameters.Add(new ValueParameter<StringValue>("GroupBy", "Create separate analyzes for different values of this variable.", new StringValue("")));
      Parameters.Add(new ValueParameter<ItemList<StringValue>>("Levels", "The list of levels to be assigned.",
        new ItemList<StringValue> {
          new StringValue("Very Low"),
          new StringValue("Low"),
          new StringValue("Average"),
          new StringValue("High"),
          new StringValue("Very High"),
        }));     
      RegisterEvents();
      UpdateName();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RunCollectionFuzzifier(this, cloner);
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }
    #endregion

    private void RegisterEvents() {
      SourceParameter.ToStringChanged += Parameter_NameChanged;
      TargetParameter.ToStringChanged += Parameter_NameChanged;
      SuffixParameter.ToStringChanged += Parameter_NameChanged;
      GroupByParameter.ToStringChanged += Parameter_NameChanged;
    }

    private void Parameter_NameChanged(object sender, EventArgs e) {
      UpdateName();
    }

    private void UpdateName() {
      name = string.Format("{0} := Fuzzy({1}) {3}",
        Target,
        Source,
        string.IsNullOrWhiteSpace(GroupBy) ? "" : string.Format("/{0}", GroupBy),
        Suffix);
      OnNameChanged();
    }

    #region IRunCollectionModifier Members

    public void Modify(List<IRun> runs) {
      foreach (var group in runs
        .Select(r => new {Run=r, Value=GetSourceValue(r)})
        .Where(r => r.Value.HasValue && !double.IsNaN(r.Value.Value) && !double.IsInfinity(r.Value.Value))
        .Select(r => new {r.Run, r.Value.Value, Bin=GetGroupByValue(r.Run)})
        .GroupBy(r => r.Bin).ToList()) {
        var values = group.Select(r => r.Value).ToList();
        if (values.Count > 0) {
          var avg = values.Average();
          var stdDev = values.StandardDeviation();
          foreach (var r in group) {
            r.Run.Results[Target] = new StringValue(Fuzzify(r.Value, avg, stdDev));
          }
        }
      }      
    }

    private double? GetSourceValue(IRun run) {
      return CastSourceValue(run.Results) ?? CastSourceValue(run.Parameters);
    }

    private string GetGroupByValue(IRun run) {
      if (string.IsNullOrWhiteSpace(GroupBy))
        return String.Empty;
      IItem value;
      run.Results.TryGetValue(GroupBy, out value);
      if (value == null)
        run.Parameters.TryGetValue(GroupBy, out value);
      if (value != null)
        return value.ToString();
      else
        return String.Empty;
    }

    private double? CastSourceValue(IDictionary<string, IItem> variables) {
      IItem value;
      variables.TryGetValue(Source, out value);
      var intValue = value as IntValue;
      if (intValue != null) {
        return intValue.Value;
      } else {
        var doubleValue = value as DoubleValue;
        if (doubleValue != null)
          return doubleValue.Value;
      }
      return null;
    }

    private string Fuzzify(double value, double avg, double stdDev) {
      double dev = (value - avg)/(stdDev*Spread);
      int index;
      if (Levels.Count % 2 == 1) {
        index = (int) Math.Floor(Math.Abs(dev));
        index = (Levels.Count - 1)/2 + Math.Sign(dev) * index;
      } else {
        index = (int) Math.Ceiling(Math.Abs(dev));
        if (dev > 0)
          index = Levels.Count/2 + index;
        else
          index = Levels.Count/2 + 1 - index;
      }
      return Levels[Math.Min(Levels.Count - 1, Math.Max(0, index))];
    }

    #endregion
  }
}
