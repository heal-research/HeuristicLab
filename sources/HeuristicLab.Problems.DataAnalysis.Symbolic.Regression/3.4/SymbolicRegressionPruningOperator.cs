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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [StorableClass]
  [Item("SymbolicRegressionPruningOperator", "An operator which prunes symbolic regression trees.")]
  public class SymbolicRegressionPruningOperator : SymbolicDataAnalysisExpressionPruningOperator {
    private const string ImpactValuesCalculatorParameterName = "ImpactValuesCalculator";

    protected SymbolicRegressionPruningOperator(SymbolicRegressionPruningOperator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionPruningOperator(this, cloner);
    }

    [StorableConstructor]
    protected SymbolicRegressionPruningOperator(bool deserializing) : base(deserializing) { }

    public SymbolicRegressionPruningOperator() {
      var impactValuesCalculator = new SymbolicRegressionSolutionImpactValuesCalculator();
      Parameters.Add(new ValueParameter<ISymbolicDataAnalysisSolutionImpactValuesCalculator>(ImpactValuesCalculatorParameterName, "The impact values calculator to be used for figuring out the node impacts.", impactValuesCalculator));
    }

    protected override ISymbolicDataAnalysisModel CreateModel() {
      return new SymbolicRegressionModel(SymbolicExpressionTree, Interpreter, EstimationLimits.Lower, EstimationLimits.Upper);
    }

    protected override double Evaluate(IDataAnalysisModel model) {
      var regressionModel = (IRegressionModel)model;
      var regressionProblemData = (IRegressionProblemData)ProblemData;
      var trainingIndices = Enumerable.Range(FitnessCalculationPartition.Start, FitnessCalculationPartition.Size).ToArray();
      var estimatedValues = regressionModel.GetEstimatedValues(ProblemData.Dataset, trainingIndices); // also bounds the values
      var targetValues = ProblemData.Dataset.GetDoubleValues(regressionProblemData.TargetVariable, trainingIndices);
      OnlineCalculatorError errorState;
      var quality = OnlinePearsonsRSquaredCalculator.Calculate(targetValues, estimatedValues, out errorState);
      if (errorState != OnlineCalculatorError.None) return double.NaN;
      return quality;
    }
  }
}
