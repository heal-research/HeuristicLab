#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.TestFunctions.Evaluators;

namespace HeuristicLab.Problems.TestFunctions {
  [Item("MultinormalAdditiveMoveEvaluator", "Class for evaluating an additive move on the multinormal function.")]
  [StorableClass]
  public class MultinormalAdditiveMoveEvaluator : AdditiveMoveEvaluator {
    public LookupParameter<ISingleObjectiveTestFunctionProblemEvaluator> EvaluatorParameter {
      get { return (LookupParameter<ISingleObjectiveTestFunctionProblemEvaluator>)Parameters["Evaluator"]; }
    }

    [StorableConstructor]
    protected MultinormalAdditiveMoveEvaluator(bool deserializing) : base(deserializing) { }
    protected MultinormalAdditiveMoveEvaluator(MultinormalAdditiveMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    public MultinormalAdditiveMoveEvaluator() {
      Parameters.Add(new LookupParameter<ISingleObjectiveTestFunctionProblemEvaluator>("Evaluator", ""));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultinormalAdditiveMoveEvaluator(this, cloner);
    }

    public override System.Type EvaluatorType {
      get { return typeof(MultinormalEvaluator); }
    }

    protected override double Evaluate(double quality, RealVector point, AdditiveMove move) {
      RealVectorAdditiveMoveWrapper wrapper = new RealVectorAdditiveMoveWrapper(move, point);
      var eval = EvaluatorParameter.ActualValue as MultinormalEvaluator;
      if (eval != null)
        return eval.Evaluate(wrapper);
      throw new InvalidOperationException("evaluator is not a multinormal evaluator");
    }
  }
}
