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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [Item("BalancedTreeCreator", "An operator that creates new symbolic expression trees using the 'Balanced' method")]
  [StorableType("E268BE19-BBEB-46EF-9632-1799A43D01F9")]
  public class SymbolicDataAnalysisExpressionBalancedTreeCreator : BalancedTreeCreator, ISymbolicDataAnalysisSolutionCreator {
    [StorableConstructor]
    protected SymbolicDataAnalysisExpressionBalancedTreeCreator(StorableConstructorFlag _) : base(_) { }
    protected SymbolicDataAnalysisExpressionBalancedTreeCreator(SymbolicDataAnalysisExpressionBalancedTreeCreator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new SymbolicDataAnalysisExpressionBalancedTreeCreator(this, cloner); }

    public SymbolicDataAnalysisExpressionBalancedTreeCreator() : base() { }
  }
}
