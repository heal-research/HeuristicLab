#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Trading {
  /// <summary>
  /// An operator that analyzes the training best symbolic trading solution for single objective symbolic trading problems.
  /// </summary>
  [Item("SymbolicTradingSingleObjectiveTrainingBestSolutionAnalyzer", "An operator that analyzes the training best symbolic trading solution for single objective symbolic trading problems.")]
  [StorableClass]
  public sealed class SymbolicTradingSingleObjectiveTrainingBestSolutionAnalyzer : SymbolicDataAnalysisSingleObjectiveTrainingBestSolutionAnalyzer<ISymbolicTradingSolution>,
  ISymbolicDataAnalysisInterpreterOperator {
    private const string ProblemDataParameterName = "ProblemData";
    private const string SymbolicDataAnalysisTreeInterpreterParameterName = "SymbolicDataAnalysisTreeInterpreter";
    #region parameter properties
    public ILookupParameter<ITradingProblemData> ProblemDataParameter {
      get { return (ILookupParameter<ITradingProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> SymbolicDataAnalysisTreeInterpreterParameter {
      get { return (ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[SymbolicDataAnalysisTreeInterpreterParameterName]; }
    }
    #endregion

    [StorableConstructor]
    private SymbolicTradingSingleObjectiveTrainingBestSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    private SymbolicTradingSingleObjectiveTrainingBestSolutionAnalyzer(SymbolicTradingSingleObjectiveTrainingBestSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public SymbolicTradingSingleObjectiveTrainingBestSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<ITradingProblemData>(ProblemDataParameterName, "The problem data for the symbolic regression solution."));
      Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicDataAnalysisTreeInterpreterParameterName, "The symbolic data analysis tree interpreter for the symbolic expression tree."));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicTradingSingleObjectiveTrainingBestSolutionAnalyzer(this, cloner);
    }

    protected override ISymbolicTradingSolution CreateSolution(ISymbolicExpressionTree bestTree, double bestQuality) {
      var model = new SymbolicTradingModel((ISymbolicExpressionTree)bestTree.Clone(), SymbolicDataAnalysisTreeInterpreterParameter.ActualValue);
      return new SymbolicTradingSolution(model, (ITradingProblemData)ProblemDataParameter.ActualValue.Clone());
    }
  }
}
