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
using HEAL.Fossil;

namespace HeuristicLab.Problems.TestFunctions {
  [Item("MatyasAdditiveMoveEvaluator", "Class for evaluating an additive move on the Matyas function.")]
  [StorableType("7B8A70DA-019D-4724-8AB8-6F42D60FB398")]
  public class MatyasAdditiveMoveEvaluator : AdditiveMoveEvaluator {
    public override System.Type EvaluatorType {
      get { return typeof(MatyasEvaluator); }
    }

    [StorableConstructor]
    protected MatyasAdditiveMoveEvaluator(StorableConstructorFlag _) : base(_) { }
    protected MatyasAdditiveMoveEvaluator(MatyasAdditiveMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    public MatyasAdditiveMoveEvaluator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MatyasAdditiveMoveEvaluator(this, cloner);
    }

    protected override double Evaluate(double quality, RealVector point, AdditiveMove move) {
      RealVectorAdditiveMoveWrapper wrapper = new RealVectorAdditiveMoveWrapper(move, point);
      return MatyasEvaluator.Apply(wrapper);
    }
  }
}
