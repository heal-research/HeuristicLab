#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Optimization {
  [Item("ComparisonTerminator", "An termination criterion which compares two values.")]
  [StorableType("4059C985-CA18-4C95-AC1C-BA8AAE64AD98")]
  public class ComparisonTerminator<T> : ThresholdTerminator<T> where T : class, IItem, IComparable, IStringConvertibleValue, new() {
    public ILookupParameter<T> ComparisonValueParameter {
      get { return (ILookupParameter<T>)Parameters["ComparisonValue"]; }
    }

    private IFixedValueParameter<Comparison> ComparisonParameter {
      get { return (IFixedValueParameter<Comparison>)Parameters["Comparison"]; }
    }

    public ComparisonType ComparisonType {
      get { return ComparisonParameter.Value.Value; }
      set { ComparisonParameter.Value.Value = value; }
    }

    [StorableConstructor]
    protected ComparisonTerminator(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }
    protected ComparisonTerminator(ComparisonTerminator<T> original, Cloner cloner)
      : base(original, cloner) {
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ComparisonTerminator<T>(this, cloner);
    }

    public ComparisonTerminator()
      : this(new T()) { }
    public ComparisonTerminator(T threshold)
      : base(threshold) {
      Parameters.Add(new LookupParameter<T>("ComparisonValue", "The left side value of the comparison.") { Hidden = false });
      Parameters.Add(new FixedValueParameter<Comparison>("Comparison", "The type of comparison."));
      Initialize();
    }
    public ComparisonTerminator(string comparisonValueActualName, ComparisonType comparisonType, T threshold)
      : this(threshold) {
      ComparisonValueParameter.ActualName = comparisonValueActualName;
      ComparisonType = comparisonType;
    }
    public ComparisonTerminator(string comparisonValueActualName, ComparisonType comparisonType, IFixedValueParameter<T> thresholdParameter)
      : this() {
      ComparisonValueParameter.ActualName = comparisonValueActualName;
      ComparisonType = comparisonType;
      ThresholdParameter = thresholdParameter;
    }

    protected override bool CheckContinueCriterion() {
      IComparable lhs = ComparisonValueParameter.ActualValue;
      IComparable rhs = ThresholdParameter.Value;

      return ComparisonType.Compare(lhs, rhs);
    }

    private void Initialize() {
      ComparisonParameter.Value.ValueChanged += new EventHandler(ComparisonType_ValueChanged);
    }
    private void ComparisonType_ValueChanged(object sender, EventArgs e) {
      OnComparisonTypeChanged();
    }
    protected virtual void OnComparisonTypeChanged() {
      OnToStringChanged();
    }

    public override void CollectParameterValues(IDictionary<string, IItem> values) {
      base.CollectParameterValues(values);
      values.Add(ComparisonValueParameter.Name, new StringValue(ComparisonValueParameter.ActualName));
    }

    public override string ToString() {
      if (Threshold == null) return Name;
      else return string.Format("{0} {1} {2}", Name, ComparisonType.ToSymbol(), ThresholdParameter.Value);
    }
  }

  internal static class ComparisonTypeHelper {
    public static bool Compare(this ComparisonType comparison, IComparable lhs, IComparable rhs) {
      int i = lhs.CompareTo(rhs);
      switch (comparison) {
        case ComparisonType.Less: return i < 0;
        case ComparisonType.LessOrEqual: return i <= 0;
        case ComparisonType.Equal: return i == 0;
        case ComparisonType.GreaterOrEqual: return i >= 0;
        case ComparisonType.Greater: return i > 0;
        case ComparisonType.NotEqual: return i != 0;
        default: throw new InvalidOperationException(comparison + " is not supported.");
      }
    }

    public static string ToSymbol(this ComparisonType comparison) {
      switch (comparison) {
        case ComparisonType.Less: return "<";
        case ComparisonType.LessOrEqual: return "<=";
        case ComparisonType.Equal: return "=";
        case ComparisonType.GreaterOrEqual: return ">=";
        case ComparisonType.Greater: return ">";
        case ComparisonType.NotEqual: return "!=";
        default: throw new InvalidOperationException(comparison + " is not supported.");
      }
    }
  }
}