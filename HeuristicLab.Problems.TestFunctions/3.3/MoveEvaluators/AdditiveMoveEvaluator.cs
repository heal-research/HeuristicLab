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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.TestFunctions {
  [Item("AdditiveMoveEvaluator", "Base class for evaluating an additive move.")]
  [StorableType("3B4F3C57-78CE-4881-8E3B-6E784A495734")]
  public class AdditiveMoveEvaluator : SingleSuccessorOperator, ISingleObjectiveTestFunctionAdditiveMoveEvaluator {

    public override bool CanChangeName {
      get { return false; }
    }

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<RealVector> RealVectorParameter {
      get { return (ILookupParameter<RealVector>)Parameters["Point"]; }
    }
    public ILookupParameter<AdditiveMove> AdditiveMoveParameter {
      get { return (ILookupParameter<AdditiveMove>)Parameters["AdditiveMove"]; }
    }
    public ILookupParameter<ISingleObjectiveTestFunction> TestFunctionParameter {
      get { return (ILookupParameter<ISingleObjectiveTestFunction>)Parameters["TestFunction"]; }
    }

    [StorableConstructor]
    protected AdditiveMoveEvaluator(StorableConstructorFlag _) : base(_) { }
    protected AdditiveMoveEvaluator(AdditiveMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    public AdditiveMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of a test function solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The evaluated quality of a move on a test function solution."));
      Parameters.Add(new LookupParameter<RealVector>("Point", "The point to evaluate the move on."));
      Parameters.Add(new LookupParameter<AdditiveMove>("AdditiveMove", "The move to evaluate."));
      Parameters.Add(new LookupParameter<ISingleObjectiveTestFunction>("TestFunction", "The test function that is used."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AdditiveMoveEvaluator(this, cloner);
    }

    public override IOperation Apply() {
      var function = TestFunctionParameter.ActualValue;
      var move = AdditiveMoveParameter.ActualValue;
      var vector = RealVectorParameter.ActualValue;
      var clone = (RealVector)vector.Clone();

      AdditiveMoveMaker.Apply(clone, move);
      var mq = function.Evaluate(clone);

      var moveQuality = MoveQualityParameter.ActualValue;
      if (moveQuality == null) {
        MoveQualityParameter.ActualValue = new DoubleValue(mq);
      } else moveQuality.Value = mq;
      return base.Apply();
    }
  }
}
