#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [Item("SymbolicExpressionTreeSimplificationOperator", "Simplifies symbolic expression trees encoding a mathematical formula.")]
  [StorableClass]
  public class SymbolicDataAnalysisExpressionTreeSimplificationOperator : SingleSuccessorOperator, ISymbolicExpressionTreeOperator {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";

    public ILookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<ISymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }

    [StorableConstructor]
    protected SymbolicDataAnalysisExpressionTreeSimplificationOperator(bool deserializing) : base(deserializing) { }
    protected SymbolicDataAnalysisExpressionTreeSimplificationOperator(SymbolicDataAnalysisExpressionTreeSimplificationOperator original, Cloner cloner)
      : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisExpressionTreeSimplificationOperator(this, cloner);
    }

    public SymbolicDataAnalysisExpressionTreeSimplificationOperator() {
      Parameters.Add(new LookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression tree to simplify."));
    }

    public override IOperation Apply() {
      var tree = SymbolicExpressionTreeParameter.ActualValue;
      var simplifiedTree = TreeSimplifier.Simplify(tree);
      simplifiedTree = TreeSimplifier.Simplify(simplifiedTree);
      SymbolicExpressionTreeParameter.ActualValue = simplifiedTree;

      return base.Apply();
    }
  }
}
