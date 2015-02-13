#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.OneMax {
  /// <summary>
  /// A base class for operators which evaluates OneMax solutions given in BinaryVector encoding.
  /// </summary>
  [Item("OneMaxEvaluator", "Evaluates solutions for the OneMax problem.")]
  [StorableClass]
  public class OneMaxEvaluator : InstrumentedOperator, IOneMaxEvaluator {
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public ILookupParameter<BinaryVector> BinaryVectorParameter {
      get { return (ILookupParameter<BinaryVector>)Parameters["BinaryVector"]; }
    }

    [StorableConstructor]
    protected OneMaxEvaluator(bool deserializing) : base(deserializing) { }
    protected OneMaxEvaluator(OneMaxEvaluator original, Cloner cloner) : base(original, cloner) { }
    public OneMaxEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The evaluated quality of the OneMax solution."));
      Parameters.Add(new LookupParameter<BinaryVector>("BinaryVector", "The OneMax solution given in path representation which should be evaluated."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OneMaxEvaluator(this, cloner);
    }

    public sealed override IOperation InstrumentedApply() {
      BinaryVector v = BinaryVectorParameter.ActualValue;

      double quality = 0;
      for (int i = 0; i < v.Length; i++) {
        if (v[i])
          quality += 1.0;
      }

      QualityParameter.ActualValue = new DoubleValue(quality);

      return base.InstrumentedApply();
    }
  }
}
