#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  /// <summary>
  /// An operator for visualizing the best symbolic regression solution based on the validation set.
  /// </summary>
  [Item("BestSymbolicExpressionTreeVisualizer", "An operator for visualizing the best symbolic regression solution based on the validation set.")]
  [StorableClass]
  public sealed class BestValidationSymbolicRegressionSolutionVisualizer : SingleSuccessorOperator, ISingleObjectiveSolutionsVisualizer, ISolutionsVisualizer {
    private const string SymbolicRegressionModelParameterName = "SymbolicRegressionModel";
    private const string DataAnalysisProblemDataParameterName = "DataAnalysisProblemData";
    private const string BestValidationSolutionParameterName = "BestValidationSolution";
    private const string QualityParameterName = "Quality";
    public ILookupParameter<ItemArray<SymbolicExpressionTree>> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<ItemArray<SymbolicExpressionTree>>)Parameters[SymbolicRegressionModelParameterName]; }
    }
    public ILookupParameter<DataAnalysisProblemData> DataAnalysisProblemDataParameter {
      get { return (ILookupParameter<DataAnalysisProblemData>)Parameters[DataAnalysisProblemDataParameterName]; }
    }
    public ILookupParameter<SymbolicRegressionSolution> BestValidationSolutionParameter {
      get { return (ILookupParameter<SymbolicRegressionSolution>)Parameters[BestValidationSolutionParameterName]; }
    }
    ILookupParameter ISolutionsVisualizer.VisualizationParameter {
      get { return BestValidationSolutionParameter; }
    }

    public ILookupParameter<ItemArray<DoubleValue>> QualityParameter {
      get { return (ILookupParameter<ItemArray<DoubleValue>>)Parameters[QualityParameterName]; }
    }

    public BestValidationSymbolicRegressionSolutionVisualizer()
      : base() {
      Parameters.Add(new SubScopesLookupParameter<SymbolicExpressionTree>(SymbolicRegressionModelParameterName, "The symbolic regression solutions from which the best solution should be visualized."));
      Parameters.Add(new SubScopesLookupParameter<DoubleValue>(QualityParameterName, "The quality of the symbolic regression solutions."));
      Parameters.Add(new LookupParameter<DataAnalysisProblemData>(DataAnalysisProblemDataParameterName, "The symbolic regression problme data on which the best solution should be evaluated."));
      Parameters.Add(new LookupParameter<SymbolicRegressionSolution>(BestValidationSolutionParameterName, "The best symbolic expression tree based on the validation data for the symbolic regression problem."));
      Parameters.Add(new LookupParameter<ResultCollection>("Results"));
    }

    public override IOperation Apply() {
      ItemArray<SymbolicExpressionTree> expressions = SymbolicExpressionTreeParameter.ActualValue;
      DataAnalysisProblemData problemData = DataAnalysisProblemDataParameter.ActualValue;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;

      var bestExpressionIndex = (from index in Enumerable.Range(0, qualities.Count())
                                 select new { Index = index, Quality = qualities[index] }).OrderBy(x => x.Quality).Select(x => x.Index).First();

      var bestExpression = expressions[bestExpressionIndex];
      SymbolicRegressionSolution bestSolution = BestValidationSolutionParameter.ActualValue;
      if (bestSolution == null) BestValidationSolutionParameter.ActualValue = CreateDataAnalysisSolution(problemData, bestExpression);
      else {
        bestSolution.Model = CreateModel(problemData, bestExpression);
      }
      // ((ResultCollection)Parameters["Results"].ActualValue).Add(new Result("ValidationMSE", new DoubleValue(3.15)));
      return base.Apply();
    }

    private SymbolicRegressionModel CreateModel(DataAnalysisProblemData problemData, SymbolicExpressionTree expression) {
      return new SymbolicRegressionModel(expression, problemData.InputVariables.Select(x => x.Value));
    }

    private SymbolicRegressionSolution CreateDataAnalysisSolution(DataAnalysisProblemData problemData, SymbolicExpressionTree expression) {
      return new SymbolicRegressionSolution(problemData, CreateModel(problemData, expression));
    }
  }
}
