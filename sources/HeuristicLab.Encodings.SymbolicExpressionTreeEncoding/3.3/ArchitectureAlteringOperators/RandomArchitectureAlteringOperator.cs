#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.GeneralSymbols;
using System.Collections.Generic;
using System.Text;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.ArchitectureAlteringOperators {
  /// <summary>
  /// Manipulates a symbolic expression by applying one architecture altering operator randomly.
  /// </summary>
  [Item("RandomArchitectureAlteringOperator", "Manipulates a symbolic expression by applying one architecture altering operator randomly.")]
  [StorableClass]
  public class RandomArchitectureAlteringOperator : SymbolicExpressionTreeArchitectureAlteringOperator {
    #region Parameter Properties
    public IValueParameter<ItemList<SymbolicExpressionTreeArchitectureAlteringOperator>> OperatorsParameter {
      get { return (IValueParameter<ItemList<SymbolicExpressionTreeArchitectureAlteringOperator>>)Parameters["Operators"]; }
    }
    #endregion
    #region Properties
    public ItemList<SymbolicExpressionTreeArchitectureAlteringOperator> Operators {
      get { return OperatorsParameter.Value; }
    }
    #endregion

    public RandomArchitectureAlteringOperator()
      : base() {
      var operators = new ItemList<SymbolicExpressionTreeArchitectureAlteringOperator>();
      operators.Add(new ArgumentCreater());
      operators.Add(new ArgumentDeleter());
      operators.Add(new ArgumentDuplicater());
      operators.Add(new SubroutineCreater());
      operators.Add(new SubroutineDeleter());
      operators.Add(new SubroutineDuplicater());
      Parameters.Add(new ValueParameter<ItemList<SymbolicExpressionTreeArchitectureAlteringOperator>>("Operators",
        "The list of architecture altering operators from which a random operator should be called.",
        operators));
    }

    public override void ModifyArchitecture(IRandom random, SymbolicExpressionTree tree, ISymbolicExpressionGrammar grammar, IntValue maxTreeSize, IntValue maxTreeHeight, IntValue maxFunctionDefiningBranches, IntValue maxFunctionArguments, out bool success) {
      var selectedOperator = Operators.SelectRandom(random);
      selectedOperator.ModifyArchitecture(random, tree, grammar, maxTreeSize, maxTreeHeight, maxFunctionDefiningBranches, maxFunctionArguments, out success);
    }
  }
}
