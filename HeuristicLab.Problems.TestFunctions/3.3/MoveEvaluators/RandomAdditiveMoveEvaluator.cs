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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions {
  [Item("RandomAdditiveMoveEvaluator", "Class for evaluating an additive move on the Random function.")]
  [StorableClass]
  public class RandomAdditiveMoveEvaluator : AdditiveMoveEvaluator {
    public override System.Type EvaluatorType {
      get { return typeof(RandomEvaluator); }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected RandomAdditiveMoveEvaluator(bool deserializing) : base(deserializing) { }
    protected RandomAdditiveMoveEvaluator(RandomAdditiveMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    public RandomAdditiveMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomAdditiveMoveEvaluator(this, cloner);
    }

    protected override double Evaluate(double quality, RealVector point, AdditiveMove move) {
      return RandomParameter.ActualValue.NextDouble();
    }
  }
}
