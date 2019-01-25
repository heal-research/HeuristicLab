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
 * 
 * Author: Sabine Winkler
 */

#endregion

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;

namespace HeuristicLab.Problems.GrammaticalEvolution {
  [StorableType("85880E49-DE2F-4FB4-8C1E-F1C51D862FDF")]
  public class GESymbolicRegressionSingleObjectiveEvaluator : GESymbolicDataAnalysisSingleObjectiveEvaluator<IRegressionProblemData>,
                                                              IGESymbolicRegressionSingleObjectiveEvaluator {

    public const string EvaluatorParameterName = "Evaluator";
    public const string RandomParameterName = "Random";
    public const string BoundsParameterName = "Bounds";
    public const string MaximumSymbolicExpressionTreeLengthParameterName = "MaximumSymbolicExpressionTreeLength";

    public IValueParameter<ISymbolicRegressionSingleObjectiveEvaluator> EvaluatorParameter {
      get { return (IValueParameter<ISymbolicRegressionSingleObjectiveEvaluator>)Parameters[EvaluatorParameterName]; }
    }
    public ILookupParameter<IntMatrix> BoundsParameter {
      get { return (ILookupParameter<IntMatrix>)Parameters[BoundsParameterName]; }
    }
    public ILookupParameter<IntValue> MaximumSymbolicExpressionTreeLengthParameter {
      get { return (ILookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeLengthParameterName]; }
    }

    public ISymbolicRegressionSingleObjectiveEvaluator Evaluator {
      get { return EvaluatorParameter.Value; }
    }


    [StorableConstructor]
    protected GESymbolicRegressionSingleObjectiveEvaluator(StorableConstructorFlag _) : base(_) { }
    protected GESymbolicRegressionSingleObjectiveEvaluator(GESymbolicRegressionSingleObjectiveEvaluator original, Cloner cloner) : base(original, cloner) { }
    public GESymbolicRegressionSingleObjectiveEvaluator()
      : base() {
      Parameters.Add(new ValueParameter<ISymbolicRegressionSingleObjectiveEvaluator>(EvaluatorParameterName, "The symbolic regression evaluator that should be used to assess the quality of trees.", new SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator()));
      Parameters.Add(new LookupParameter<IntMatrix>(BoundsParameterName, "The integer number range in which the single genomes of a genotype are created."));
      Parameters.Add(new LookupParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName, "Genotype length."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GESymbolicRegressionSingleObjectiveEvaluator(this, cloner);
    }

    public override bool Maximization {
      get { return Evaluator.Maximization; }
    }

    public override IOperation Apply() {
      var genotype = IntegerVectorParameter.ActualValue;

      // translate to phenotype
      var tree = GenotypeToPhenotypeMapperParameter.ActualValue.Map(
        RandomParameter.ActualValue,
        BoundsParameter.ActualValue,
        MaximumSymbolicExpressionTreeLengthParameter.ActualValue.Value,
        SymbolicExpressionTreeGrammarParameter.ActualValue,
        genotype
      );
      SymbolicExpressionTreeParameter.ActualValue = tree; // write to scope for analyzers

      // create operation for evaluation
      var evalOp = ExecutionContext.CreateChildOperation(Evaluator);
      var successorOp = base.Apply();

      return new OperationCollection(evalOp, successorOp);
    }
  }
}
