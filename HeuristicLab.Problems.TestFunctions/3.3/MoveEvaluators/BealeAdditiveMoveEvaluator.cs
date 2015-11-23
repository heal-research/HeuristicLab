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
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions {
  [Item("BealeAdditiveMoveEvaluator", "Class for evaluating an additive move on the Beale function.")]
  [StorableClass]
  public class BealeAdditiveMoveEvaluator : AdditiveMoveEvaluator {
    public override System.Type EvaluatorType {
      get { return typeof(BealeEvaluator); }
    }

    [StorableConstructor]
    protected BealeAdditiveMoveEvaluator(bool deserializing) : base(deserializing) { }
    protected BealeAdditiveMoveEvaluator(BealeAdditiveMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    public BealeAdditiveMoveEvaluator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BealeAdditiveMoveEvaluator(this, cloner);
    }

    protected override double Evaluate(double quality, RealVector point, AdditiveMove move) {
      RealVectorAdditiveMoveWrapper wrapper = new RealVectorAdditiveMoveWrapper(move, point);
      return BealeEvaluator.Apply(wrapper);
    }
  }
}
