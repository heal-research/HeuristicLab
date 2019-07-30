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
using HeuristicLab.Parameters;
using HEAL.Attic;
using System.Collections.Generic;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("0547D067-792A-462C-90B9-BE09E91CE255")]
  public abstract class SymbolicDataAnalysisSingleObjectiveEvaluator<T> : SymbolicDataAnalysisEvaluator<T>, ISymbolicDataAnalysisSingleObjectiveEvaluator<T>
   where T : class, IDataAnalysisProblemData {
    private const string QualityParameterName = "Quality";
    #region parameter properties
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }
    #endregion
    #region properties
    public abstract bool Maximization { get; }
    #endregion
    [StorableConstructor]
    protected SymbolicDataAnalysisSingleObjectiveEvaluator(StorableConstructorFlag _) : base(_) { }
    protected SymbolicDataAnalysisSingleObjectiveEvaluator(SymbolicDataAnalysisSingleObjectiveEvaluator<T> original, Cloner cloner)
      : base(original, cloner) {
    }

    protected SymbolicDataAnalysisSingleObjectiveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>(QualityParameterName, "The quality of the evaluated symbolic data analysis solution."));
    }

    public abstract double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, T problemData, IEnumerable<int> rows);
  }
}
