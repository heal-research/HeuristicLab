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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Creators {
  /// <summary>
  /// A base class for operators creating symbolic expression trees.
  /// </summary>
  [Item("SymbolicExpressionTreeCreator", "A base class for operators creating symbolic expression trees.")]
  [StorableClass]
  public abstract class SymbolicExpressionTreeCreator : SymbolicExpressionTreeOperator, ISymbolicExpressionTreeCreator {
    private const string MaxFunctionDefinitionsParameterName = "MaxFunctionDefinitions";
    private const string MaxFunctionArgumentsParameterName = "MaxFunctionArguments";
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    #region Parameter Properties
    public IValueLookupParameter<IntValue> MaxFunctionDefinitionsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaxFunctionDefinitionsParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaxFunctionArgumentsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaxFunctionArgumentsParameterName]; }
    }
    public ILookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    #endregion

    #region Propeties
    public IntValue MaxFunctionDefinitions {
      get { return MaxFunctionDefinitionsParameter.ActualValue; }
    }
    public IntValue MaxFunctionArguments {
      get { return MaxFunctionArgumentsParameter.ActualValue; }
    }
    public SymbolicExpressionTree SymbolicExpressionTree {
      get { return SymbolicExpressionTreeParameter.ActualValue; }
      set { SymbolicExpressionTreeParameter.ActualValue = value; }
    }

    #endregion
    [StorableConstructor]
    protected SymbolicExpressionTreeCreator(bool deserializing) : base(deserializing) { }
    protected SymbolicExpressionTreeCreator(SymbolicExpressionTreeCreator original, Cloner cloner) : base(original, cloner) { }
    protected SymbolicExpressionTreeCreator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>(MaxFunctionDefinitionsParameterName, "Maximal number of function definitions in the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaxFunctionArgumentsParameterName, "Maximal number of arguments of automatically defined functions in the symbolic expression tree."));
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression tree that should be created."));
    }

    public sealed override IOperation Apply() {
      SymbolicExpressionTree = Create(Random, SymbolicExpressionGrammar,
        MaxTreeSize, MaxTreeHeight, MaxFunctionDefinitions, MaxFunctionArguments);
      return base.Apply();
    }

    protected abstract SymbolicExpressionTree Create(
      IRandom random,
      ISymbolicExpressionGrammar grammar,
      IntValue maxTreeSize, IntValue maxTreeHeight,
      IntValue maxFunctionDefinitions, IntValue maxFunctionArguments
      );
  }
}
