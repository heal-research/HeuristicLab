#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Optimization {
  [Item("RunCollection Discretizer",
    "Creates several levels from the distribution of a certain result accross a run collection and " +
    "assigns a discretized value. Non-existing numbers as well as NaN and infinities are excluded from the caluclation.")]
  [StorableType("6EFD185F-FE23-4E23-90F0-63CB9D38E57D")]
  public class RunCollectionDiscretizer : ParameterizedNamedItem, IRunCollectionModifier {

    public override bool CanChangeName { get { return false; } }
    public override bool CanChangeDescription { get { return false; } }

    #region Parameters
    public ValueParameter<StringValue> SourceParameter {
      get { return (ValueParameter<StringValue>)Parameters["Source"]; }
    }
    public ValueParameter<StringValue> TargetParameter {
      get { return (ValueParameter<StringValue>)Parameters["Target"]; }
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
    private string Target {
      get { return TargetParameter.Value.Value; }
      set { TargetParameter.Value.Value = value; }
    }    
    private double Spread { get { return SpreadParameter.Value.Value; } }
    private string GroupBy { get { return GroupByParameter.Value.Value; } }
    private List<string> Levels { get { return LevelsParameter.Value.Select(v => v.Value).ToList(); } }

      #region Construction & Cloning
    [StorableConstructor]
    protected RunCollectionDiscretizer(StorableConstructorFlag _) : base(_) { }
    protected RunCollectionDiscretizer(RunCollectionDiscretizer original, Cloner cloner)
      : base(original, cloner) {
      RegisterEvents();
    }
    public RunCollectionDiscretizer() {
      Parameters.Add(new ValueParameter<StringValue>("Source", "Source value name to be fuzzified.", new StringValue("Value")));
      Parameters.Add(new ValueParameter<StringValue>("Target", "Target value name. The new, fuzzified variable to be created.", new StringValue("Calc.Value")));
      Parameters.Add(new ValueParameter<DoubleValue>("Spread", "The number of standard deviations considered one additional level. Set to zero to use empirical distribution instead.", new DoubleValue(1)));
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
      return new RunCollectionDiscretizer(this, cloner);
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }
    #endregion

    private void RegisterEvents() {
      SourceParameter.ToStringChanged += SourceParameter_NameChanged;
      TargetParameter.ToStringChanged += Parameter_NameChanged;
      GroupByParameter.ToStringChanged += Parameter_NameChanged;
    }

    private void SourceParameter_NameChanged(object sender, EventArgs e) {
      Target = string.Format("{0}/Level", Source);
    }

    private void Parameter_NameChanged(object sender, EventArgs e) {
      UpdateName();
    }

    private void UpdateName() {
      name = string.Format("{0} := Discrete({1}{2})",
        Target,
        Source,
        string.IsNullOrWhiteSpace(GroupBy) ? "" : string.Format("/{0}", GroupBy));        
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
          if (Spread > 0) {
            var avg = values.Average();
            var stdDev = values.StandardDeviation();
            foreach (var r in group) {
              r.Run.Results[Target] = new StringValue(Discretize(r.Value, avg, stdDev));
            }
          } else {
            values.Sort();
            var a = values.ToArray();
            foreach (var r in group) {
              r.Run.Results[Target] = new StringValue(Discretize(r.Value, a));
            }
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
      if (intValue != null) 
        return intValue.Value;
      var doubleValue = value as DoubleValue;
      if (doubleValue != null)
        return doubleValue.Value;
      return null;
    }

    private string Discretize(double value, double avg, double stdDev) {
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

    private string Discretize(double value, double[] values) {
      var index = Array.BinarySearch(values, value);
      var pos = 1.0*(index < 0 ? ~index : index)/(values.Length-1);
      return Levels[Math.Min(Levels.Count - 1, Math.Max(0, (int) Math.Round(pos*(Levels.Count-1))))];
    }

    #endregion
  }
}
