#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// A base class for operators for symbolic expression trees.
  /// </summary>
  [Item("SymbolicExpressionTreeOperator", "A base class for operators for symbolic expression trees.")]
  [StorableClass]
  public abstract class SymbolicExpressionTreeOperator : SingleSuccessorOperator, IStochasticOperator, ISymbolicExpressionTreeOperator {
    private const string RandomParameterName = "Random";

    public override bool CanChangeName {
      get { return false; }
    }

    #region Parameter Properties
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters[RandomParameterName]; }
    }
    #endregion

    #region Properties
    public IRandom Random {
      get { return RandomParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    protected SymbolicExpressionTreeOperator(bool deserializing) : base(deserializing) { }
    protected SymbolicExpressionTreeOperator(SymbolicExpressionTreeOperator original, Cloner cloner) : base(original, cloner) { }
    protected SymbolicExpressionTreeOperator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>(RandomParameterName, "The pseudo random number generator which should be used for symbolic expression tree operators."));
    }
  }
}
