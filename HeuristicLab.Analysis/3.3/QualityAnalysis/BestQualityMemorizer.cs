#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// An operator that updates the best quality found so far with those qualities contained in the scope tree.
  /// </summary>
  [Item("BestQualityMemorizer", "An operator that updates the best quality found so far with those qualities contained in the scope tree.")]
  [StorableClass]
  public class BestQualityMemorizer : SingleSuccessorOperator, ISingleObjectiveOperator {
    public ValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ValueLookupParameter<DoubleValue> BestQualityParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestQuality"]; }
    }

    #region Storing & Cloning
    [StorableConstructor]
    protected BestQualityMemorizer(bool deserializing) : base(deserializing) { }
    protected BestQualityMemorizer(BestQualityMemorizer original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestQualityMemorizer(this, cloner);
    }
    #endregion
    public BestQualityMemorizer()
      : base() {
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the current problem is a maximization problem, otherwise false."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The value contained in the scope tree which represents the solution quality."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestQuality", "The quality value of the best solution."));
    }

    public override IOperation Apply() {
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      bool maximization = MaximizationParameter.ActualValue.Value;
      DoubleValue best = BestQualityParameter.ActualValue;
      double max = (best != null) ? (best.Value) : ((maximization) ? (double.MinValue) : (double.MaxValue));

      foreach (DoubleValue quality in qualities)
        if (IsBetter(maximization, quality.Value, max)) max = quality.Value;

      if (best == null) BestQualityParameter.ActualValue = new DoubleValue(max);
      else best.Value = max;
      return base.Apply();
    }

    private bool IsBetter(bool maximization, double quality, double max) {
      return (maximization && quality > max || !maximization && quality < max);
    }
  }
}
