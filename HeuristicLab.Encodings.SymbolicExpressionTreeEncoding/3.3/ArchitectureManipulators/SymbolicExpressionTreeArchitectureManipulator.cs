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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Interfaces;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Manipulators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.ArchitectureManipulators {
  /// <summary>
  /// Base class for architecture altering operators for symbolic expression trees.
  /// </summary>
  [StorableClass]
  public abstract class SymbolicExpressionTreeArchitectureManipulator : SymbolicExpressionTreeManipulator, ISymbolicExpressionTreeArchitectureManipulator {
    private const string MaxFunctionArgumentsParameterName = "MaxFunctionArguments";
    private const string MaxFunctionDefiningBranchesParameterName = "MaxFunctionDefiningBranches";
    public override bool CanChangeName {
      get { return false; }
    }

    public IValueLookupParameter<IntValue> MaxFunctionDefinitionsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaxFunctionDefiningBranchesParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaxFunctionArgumentsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaxFunctionArgumentsParameterName]; }
    }
    public IntValue MaxFunctionDefiningBranches {
      get { return MaxFunctionDefinitionsParameter.ActualValue; }
    }
    public IntValue MaxFunctionArguments {
      get { return MaxFunctionArgumentsParameter.ActualValue; }
    }
    [StorableConstructor]
    protected SymbolicExpressionTreeArchitectureManipulator(bool deserializing) : base(deserializing) { }
    protected SymbolicExpressionTreeArchitectureManipulator(SymbolicExpressionTreeArchitectureManipulator original, Cloner cloner) : base(original, cloner) { }
    public SymbolicExpressionTreeArchitectureManipulator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>(MaxFunctionDefiningBranchesParameterName, "The maximal allowed number of function defining branches."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaxFunctionArgumentsParameterName, "The maximal allowed number of arguments of a newly created function."));
    }

    protected override sealed void Manipulate(IRandom random, SymbolicExpressionTree symbolicExpressionTree, ISymbolicExpressionGrammar grammar, IntValue maxTreeSize, IntValue maxTreeHeight, out bool success) {
      ModifyArchitecture(random, symbolicExpressionTree, grammar, maxTreeSize, maxTreeHeight, MaxFunctionDefiningBranches, MaxFunctionArguments, out success);
    }

    public abstract void ModifyArchitecture(
      IRandom random,
      SymbolicExpressionTree tree,
      ISymbolicExpressionGrammar grammar,
      IntValue maxTreeSize,
      IntValue maxTreeHeight,
      IntValue maxFunctionDefinitions,
      IntValue maxFunctionArguments,
      out bool success
      );
  }
}
