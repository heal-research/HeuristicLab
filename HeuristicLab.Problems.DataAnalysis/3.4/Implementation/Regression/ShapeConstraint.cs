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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("8109BE58-CCFB-4462-A2F4-EEE5DFADAFF7")]
  [Item("ShapeConstraint", "Constraint on the shape of a function e.g. monotonicity.")]
  public sealed class ShapeConstraint : Item {
    [Storable]
    private string variable;

    public string Variable {
      get => variable;
      set {
        if (variable == value)
          return;
        variable = value;
        OnToStringChanged();
        OnChanged();
      }
    }

    public bool IsDerivative => NumberOfDerivations > 0;

    [Storable]
    private int numberOfDerivations;

    public int NumberOfDerivations {
      get => numberOfDerivations;
      set {
        if (value < 0 || value > 3)
          throw new ArgumentException("Number of derivation has to be between 0 - 3.");
        if (numberOfDerivations == value)
          return;
        numberOfDerivations = value;
        OnToStringChanged();
        OnChanged();
      }
    }

    [Storable]
    private Interval interval;

    public Interval Interval {
      get => interval;
      set {
        if (interval == value)
          return;
        interval = value;
        OnToStringChanged();
        OnChanged();
      }
    }

    [Storable]
    private IntervalCollection regions;
    public IntervalCollection Regions {
      get => regions;
      set {
        if (regions != value) {
          if (regions != null) regions.Changed -= regions_Changed;
          regions = value;
          if (regions != null) regions.Changed += regions_Changed;
          OnToStringChanged();
          OnChanged();
        }
      }
    }

    [Storable]
    private double weight = 1.0;
    public double Weight {
      get => weight;
      set {
        if (weight <= 0.0) throw new ArgumentOutOfRangeException("Weight must be larger than zero.");
        if (weight != value) {
          weight = value;
          OnToStringChanged();
          OnChanged();
        }
      }
    }

    [Storable]
    private Interval threshold = new Interval(double.NegativeInfinity, double.PositiveInfinity);
    public Interval Threshold {
      get => threshold;
      set {
        if (threshold == value)
          return;
        threshold = value;
        OnToStringChanged();
        OnChanged();
      }
    }

    [StorableConstructor]
    private ShapeConstraint(StorableConstructorFlag _) : base(_) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (regions != null) regions.Changed += regions_Changed;
    }

    // without derivation
    public ShapeConstraint(Interval interval, double weight)
      : this(interval, weight, new Interval(double.NegativeInfinity, double.PositiveInfinity)) { }
    
    public ShapeConstraint(Interval interval, double weight, Interval threshold)
      : this(string.Empty, 0,
         interval, new IntervalCollection(), weight, threshold) { }

    public ShapeConstraint(Interval interval, IntervalCollection regions, double weight)
      : this(interval, regions, weight, new Interval(double.NegativeInfinity, double.PositiveInfinity)) { }

    public ShapeConstraint(Interval interval, IntervalCollection regions, double weight, Interval threshold)
      : this(string.Empty, 0,
         interval, regions, weight, threshold) { }

    public ShapeConstraint(string variable, int numberOfDerivations,
                              Interval interval, double weight)
      : this(variable, numberOfDerivations,
             interval, weight, new Interval(double.NegativeInfinity, double.PositiveInfinity)) { }

    public ShapeConstraint(string variable, int numberOfDerivations,
                              Interval interval, double weight, Interval threshold)
      : this(variable, numberOfDerivations,
             interval, new IntervalCollection(), weight, threshold) { }

    public ShapeConstraint(string variable, int numberOfDerivations,
                              Interval interval, IntervalCollection regions, double weight) 
      : this(variable, numberOfDerivations, interval, regions, weight, 
             new Interval(double.NegativeInfinity, double.PositiveInfinity)) { }

    public ShapeConstraint(string variable, int numberOfDerivations,
                              Interval interval, IntervalCollection regions, double weight, Interval threshold) {
      Variable = variable;
      NumberOfDerivations = numberOfDerivations;
      Interval = interval;
      Regions = regions;
      Weight = weight;
      Threshold = threshold;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ShapeConstraint(this, cloner);
    }

    private ShapeConstraint(ShapeConstraint original, Cloner cloner)
      : base(original, cloner) {
      Variable = original.Variable;
      NumberOfDerivations = original.NumberOfDerivations;
      Interval = original.Interval;
      Regions = cloner.Clone(original.Regions);
      Weight = original.weight;
    }


    public event EventHandler Changed;

    private void OnChanged() {
      var handlers = Changed;
      if (handlers != null)
        handlers(this, EventArgs.Empty);
    }


    private void regions_Changed(object sender, EventArgs e) {
      OnToStringChanged();
      OnChanged();
    }

    public override string ToString() {
      return GenerateExpressionString();
    }


    private string GenerateExpressionString() {
      string expression;
      string write(double val) => double.IsPositiveInfinity(val) ? "inf." : double.IsNegativeInfinity(val) ? "-inf." : $"{val}";
      if (!IsDerivative) {
        expression = string.Format($"f in [{write(Interval.LowerBound)} .. {write(Interval.UpperBound)}]");
      } else {
        var derivationString = string.Empty;
        switch (numberOfDerivations) {
          case 1:
            derivationString = ""; break;
          case 2:
            derivationString = "²"; break;
          case 3:
            derivationString = "³"; break;
        }
        expression = string.Format($"∂{derivationString}f/∂{Variable}{derivationString} in [{write(Interval.LowerBound)} .. {write(Interval.UpperBound)}]");
      }

      if (Regions != null)
        foreach (var region in Regions.GetReadonlyDictionary())
          expression += $", {region.Key} in [{write(region.Value.LowerBound)} .. {write(region.Value.UpperBound)}]";

      if (Weight != 1.0)
        expression += $" weight: {weight}";

      if (Threshold.LowerBound != double.NegativeInfinity || Threshold.UpperBound != double.PositiveInfinity)
        expression += $" threshold in [{write(Threshold.LowerBound)} .. {write(Threshold.UpperBound)}]";

      return expression;
    }
  }
}