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
using HeuristicLab.Optimization;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.TimeSeriesPrognosis {
  /// <summary>
  /// Represents a symbolic time-series prognosis solution (model + data) and attributes of the solution like accuracy and complexity
  /// </summary>
  [StorableType("7B8E8077-9304-44C0-941C-EF50210B09C4")]
  [Item(Name = "SymbolicTimeSeriesPrognosisSolution", Description = "Represents a symbolic time-series prognosis solution (model + data) and attributes of the solution like accuracy and complexity.")]
  public sealed class SymbolicTimeSeriesPrognosisSolution : TimeSeriesPrognosisSolution, ISymbolicTimeSeriesPrognosisSolution {
    private const string ModelLengthResultName = "Model Length";
    private const string ModelDepthResultName = "Model Depth";

    public new ISymbolicTimeSeriesPrognosisModel Model {
      get { return (ISymbolicTimeSeriesPrognosisModel)base.Model; }
      set { base.Model = value; }
    }
    ISymbolicDataAnalysisModel ISymbolicDataAnalysisSolution.Model {
      get { return (ISymbolicDataAnalysisModel)base.Model; }
    }
    public int ModelLength {
      get { return ((IntValue)this[ModelLengthResultName].Value).Value; }
      private set { ((IntValue)this[ModelLengthResultName].Value).Value = value; }
    }

    public int ModelDepth {
      get { return ((IntValue)this[ModelDepthResultName].Value).Value; }
      private set { ((IntValue)this[ModelDepthResultName].Value).Value = value; }
    }

    [StorableConstructor]
    private SymbolicTimeSeriesPrognosisSolution(StorableConstructorFlag _) : base(_) { }
    private SymbolicTimeSeriesPrognosisSolution(SymbolicTimeSeriesPrognosisSolution original, Cloner cloner)
      : base(original, cloner) {
    }
    public SymbolicTimeSeriesPrognosisSolution(ISymbolicTimeSeriesPrognosisModel model, ITimeSeriesPrognosisProblemData problemData)
      : base(model, problemData) {
      Add(new Result(ModelLengthResultName, "Length of the symbolic regression model.", new IntValue()));
      Add(new Result(ModelDepthResultName, "Depth of the symbolic regression model.", new IntValue()));
      CalculateResults();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicTimeSeriesPrognosisSolution(this, cloner);
    }

    protected override void RecalculateResults() {
      base.RecalculateResults();
      CalculateResults();
    }
    private void CalculateResults() {
      ModelLength = Model.SymbolicExpressionTree.Length;
      ModelDepth = Model.SymbolicExpressionTree.Depth;
    }
  }
}
