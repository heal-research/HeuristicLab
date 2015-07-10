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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("SymbolicRegressionPhenotypicDiversityAnalyzer", "An analyzer which calculates diversity based on the phenotypic distance between trees")]
  [StorableClass]
  public class SymbolicRegressionPhenotypicDiversityAnalyzer : PopulationSimilarityAnalyzer,
    ISymbolicDataAnalysisBoundedOperator, ISymbolicDataAnalysisInterpreterOperator, ISymbolicExpressionTreeOperator {
    #region parameter names
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string EvaluatedValuesParameterName = "EstimatedValues";
    private const string SymbolicDataAnalysisTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string ProblemDataParameterName = "ProblemData";
    private const string EstimationLimitsParameterName = "EstimationLimits";
    #endregion

    #region parameter properties
    public IScopeTreeLookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (IScopeTreeLookupParameter<ISymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    private IScopeTreeLookupParameter<DoubleArray> EvaluatedValuesParameter {
      get { return (IScopeTreeLookupParameter<DoubleArray>)Parameters[EvaluatedValuesParameterName]; }
    }
    public ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> SymbolicDataAnalysisTreeInterpreterParameter {
      get { return (ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[SymbolicDataAnalysisTreeInterpreterParameterName]; }
    }
    public IValueLookupParameter<IRegressionProblemData> ProblemDataParameter {
      get { return (IValueLookupParameter<IRegressionProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public IValueLookupParameter<DoubleLimit> EstimationLimitsParameter {
      get { return (IValueLookupParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName]; }
    }
    #endregion

    public SymbolicRegressionPhenotypicDiversityAnalyzer(IEnumerable<ISolutionSimilarityCalculator> validSimilarityCalculators)
      : base(validSimilarityCalculators) {
      #region add parameters
      Parameters.Add(new ScopeTreeLookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleArray>(EvaluatedValuesParameterName, "Intermediate estimated values to be saved in the scopes."));
      Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicDataAnalysisTreeInterpreterParameterName, "The interpreter that should be used to calculate the output values of the symbolic data analysis tree."));
      Parameters.Add(new ValueLookupParameter<IRegressionProblemData>(ProblemDataParameterName, "The problem data on which the symbolic data analysis solution should be evaluated."));
      Parameters.Add(new ValueLookupParameter<DoubleLimit>(EstimationLimitsParameterName, "The upper and lower limit that should be used as cut off value for the output values of symbolic data analysis trees."));
      #endregion

      UpdateCounterParameter.ActualName = "PhenotypicDiversityAnalyzerUpdateCounter";
      DiversityResultName = "Phenotypic Diversity";
    }

    [StorableConstructor]
    protected SymbolicRegressionPhenotypicDiversityAnalyzer(bool deserializing)
      : base(deserializing) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionPhenotypicDiversityAnalyzer(this, cloner);
    }

    protected SymbolicRegressionPhenotypicDiversityAnalyzer(SymbolicRegressionPhenotypicDiversityAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IOperation Apply() {
      int updateInterval = UpdateIntervalParameter.Value.Value;
      IntValue updateCounter = UpdateCounterParameter.ActualValue;

      if (updateCounter == null) {
        updateCounter = new IntValue(updateInterval);
        UpdateCounterParameter.ActualValue = updateCounter;
      }

      if (updateCounter.Value == updateInterval) {
        var trees = SymbolicExpressionTreeParameter.ActualValue;
        var interpreter = SymbolicDataAnalysisTreeInterpreterParameter.ActualValue;
        var ds = ProblemDataParameter.ActualValue.Dataset;
        var rows = ProblemDataParameter.ActualValue.TrainingIndices;

        var evaluatedValues = new ItemArray<DoubleArray>(trees.Select(t => new DoubleArray(interpreter.GetSymbolicExpressionTreeValues(t, ds, rows).ToArray())));
        EvaluatedValuesParameter.ActualValue = evaluatedValues;
      }
      return base.Apply();
    }
  }
}
