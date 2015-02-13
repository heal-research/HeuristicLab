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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Trading.Symbolic {
  /// <summary>
  /// An operator that analyzes the training best symbolic trading solution for single objective symbolic trading problems.
  /// </summary>
  [Item("Training-best Solution Analyzer (symbolic trading)", "An operator that analyzes the training best symbolic trading solution for single objective symbolic trading problems.")]
  [StorableClass]
  public sealed class TrainingBestSolutionAnalyzer : SymbolicDataAnalysisSingleObjectiveTrainingBestSolutionAnalyzer<ISolution>,
  ISymbolicDataAnalysisInterpreterOperator {
    private const string ProblemDataParameterName = "ProblemData";
    private const string SymbolicDataAnalysisTreeInterpreterParameterName = "SymbolicDataAnalysisTreeInterpreter";
    #region parameter properties
    public ILookupParameter<IProblemData> ProblemDataParameter {
      get { return (ILookupParameter<IProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> SymbolicDataAnalysisTreeInterpreterParameter {
      get { return (ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[SymbolicDataAnalysisTreeInterpreterParameterName]; }
    }
    #endregion

    [StorableConstructor]
    private TrainingBestSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    private TrainingBestSolutionAnalyzer(TrainingBestSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public TrainingBestSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<IProblemData>(ProblemDataParameterName, "The problem data for the symbolic regression solution."));
      Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicDataAnalysisTreeInterpreterParameterName, "The symbolic data analysis tree interpreter for the symbolic expression tree."));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new TrainingBestSolutionAnalyzer(this, cloner);
    }

    protected override ISolution CreateSolution(ISymbolicExpressionTree bestTree, double bestQuality) {
      var model = new Model((ISymbolicExpressionTree)bestTree.Clone(), SymbolicDataAnalysisTreeInterpreterParameter.ActualValue);
      return new SymbolicSolution(model, (IProblemData)ProblemDataParameter.ActualValue.Clone());
    }
  }
}
