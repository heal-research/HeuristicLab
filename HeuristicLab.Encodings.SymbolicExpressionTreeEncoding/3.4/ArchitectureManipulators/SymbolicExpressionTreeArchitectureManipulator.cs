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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// Base class for architecture altering operators for symbolic expression trees.
  /// </summary>
  [StorableClass]
  public abstract class SymbolicExpressionTreeArchitectureManipulator : SymbolicExpressionTreeManipulator, ISymbolicExpressionTreeArchitectureManipulator {
    private const string MaximumFunctionArgumentsParameterName = "MaximumFunctionArguments";
    private const string MaximumFunctionDefinitionsParameterName = "MaximumFunctionDefinitions";
    public override bool CanChangeName {
      get { return false; }
    }
    #region parameter properties
    public IValueLookupParameter<IntValue> MaximumFunctionDefinitionsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumFunctionDefinitionsParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaximumFunctionArgumentsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumFunctionArgumentsParameterName]; }
    }
    #endregion
    #region properties
    public IntValue MaximumFunctionDefinitions {
      get { return MaximumFunctionDefinitionsParameter.ActualValue; }
    }
    public IntValue MaximumFunctionArguments {
      get { return MaximumFunctionArgumentsParameter.ActualValue; }
    }
    #endregion
    [StorableConstructor]
    protected SymbolicExpressionTreeArchitectureManipulator(bool deserializing) : base(deserializing) { }
    protected SymbolicExpressionTreeArchitectureManipulator(SymbolicExpressionTreeArchitectureManipulator original, Cloner cloner) : base(original, cloner) { }
    public SymbolicExpressionTreeArchitectureManipulator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumFunctionDefinitionsParameterName, "The maximal allowed number of automatically defined functions."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumFunctionArgumentsParameterName, "The maximal allowed number of arguments of a automatically defined functions."));
    }

    protected override sealed void Manipulate(IRandom random, ISymbolicExpressionTree symbolicExpressionTree) {
      ModifyArchitecture(random, symbolicExpressionTree, MaximumFunctionDefinitions, MaximumFunctionArguments);
    }

    public abstract void ModifyArchitecture(
      IRandom random,
      ISymbolicExpressionTree tree,
      IntValue maxFunctionDefinitions,
      IntValue maxFunctionArguments);
  }
}
