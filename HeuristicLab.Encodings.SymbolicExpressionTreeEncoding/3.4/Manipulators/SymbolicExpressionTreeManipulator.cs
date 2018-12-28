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
using HEAL.Fossil;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// A base class for operators that manipulate real-valued vectors.
  /// </summary>
  [Item("SymbolicExpressionTreeManipulator", "A base class for operators that manipulate symbolic expression trees.")]
  [StorableType("9391A979-616B-4808-80E7-99D8802599AF")]
  public abstract class SymbolicExpressionTreeManipulator : SymbolicExpressionTreeOperator, ISymbolicExpressionTreeManipulator {
    [StorableConstructor]
    protected SymbolicExpressionTreeManipulator(StorableConstructorFlag _) : base(_) { }
    protected SymbolicExpressionTreeManipulator(SymbolicExpressionTreeManipulator original, Cloner cloner) : base(original, cloner) { }
    public SymbolicExpressionTreeManipulator()
      : base() {
      
    }

    public sealed override IOperation InstrumentedApply() {
      ISymbolicExpressionTree tree = SymbolicExpressionTreeParameter.ActualValue;
      Manipulate(RandomParameter.ActualValue, tree);

      return base.InstrumentedApply();
    }

    protected abstract void Manipulate(IRandom random, ISymbolicExpressionTree symbolicExpressionTree);
  }
}