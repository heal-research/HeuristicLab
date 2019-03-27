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
 */
#endregion

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// An operator that outputs the length of a symbolic expression tree.
  /// </summary>
  [Item("SymbolicExpressionTreeLengthCalculator", "An operator that outputs the length of a symbolic expression tree.")]
  [StorableType("33E2EF4F-AA65-4E23-9E8C-0D0674A4186B")]
  public sealed class SymbolicExpressionTreeLengthCalculator : SingleSuccessorOperator {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string SymbolicExpressionTreeLengthParameterName = "SymbolicExpressionTreeLength";

    #region parameter properties
    public ILookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ILookupParameter<DoubleValue> SymbolicExpressionTreeLengthParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[SymbolicExpressionTreeLengthParameterName]; }
    }
    #endregion

    #region properties
    public SymbolicExpressionTree SymbolicExpressionTree {
      get { return SymbolicExpressionTreeParameter.ActualValue; }
    }
    public DoubleValue SymbolicExpressionTreeLength {
      get { return SymbolicExpressionTreeLengthParameter.ActualValue; }
      set { SymbolicExpressionTreeLengthParameter.ActualValue = value; }
    }
    #endregion

    [StorableConstructor]
    private SymbolicExpressionTreeLengthCalculator(StorableConstructorFlag _) : base(_) { }
    private SymbolicExpressionTreeLengthCalculator(SymbolicExpressionTreeLengthCalculator original, Cloner cloner) : base(original, cloner) { }
    public SymbolicExpressionTreeLengthCalculator()
      : base() {
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression tree whose length should be calculated."));
      Parameters.Add(new LookupParameter<DoubleValue>(SymbolicExpressionTreeLengthParameterName, "The length of the symbolic expression tree."));
    }

    public override IOperation Apply() {
      SymbolicExpressionTreeLength = new DoubleValue(SymbolicExpressionTree.Length);
      return base.Apply();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicExpressionTreeLengthCalculator(this, cloner);
    }
  }
}
