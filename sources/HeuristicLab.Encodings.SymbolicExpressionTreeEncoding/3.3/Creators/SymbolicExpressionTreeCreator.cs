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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Creators {
  /// <summary>
  /// A base class for operators creating symbolic expression trees.
  /// </summary>
  [Item("SymbolicExpressionTreeCreator", "A base class for operators creating symbolic expression trees.")]
  [StorableClass]
  public abstract class SymbolicExpressionTreeCreator : SymbolicExpressionTreeOperator, ISolutionCreator {
    private const string MaxFunctionDefinitionsParameterName = "MaxFunctionDefinitions";
    private const string MaxFunctionArgumentsParameterName = "MaxFunctionArguments";
    #region Parameter Properties
    public IValueLookupParameter<IntValue> MaxFunctionDefinitionsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaxFunctionDefinitionsParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaxFunctionArgumentsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaxFunctionArgumentsParameterName]; }
    }
    #endregion

    #region Propeties
    public IntValue MaxFunctionDefinitions {
      get { return MaxFunctionDefinitionsParameter.ActualValue; }
    }
    public IntValue MaxFunctionArguments {
      get { return MaxFunctionArgumentsParameter.ActualValue; }
    }

    #endregion
    protected SymbolicExpressionTreeCreator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>(MaxFunctionDefinitionsParameterName, "Maximal number of function definitions in the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaxFunctionArgumentsParameterName, "Maximal number of arguments of automatically defined functions in the symbolic expression tree."));
    }

    public sealed override IOperation Apply() {
      SymbolicExpressionTreeParameter.ActualValue = Create(Random, SymbolicExpressionGrammar,
        MaxTreeSize, MaxTreeHeight, MaxFunctionDefinitions, MaxFunctionArguments);
      return null;
    }

    protected abstract SymbolicExpressionTree Create(
      IRandom random,
      ISymbolicExpressionGrammar grammar,
      IntValue maxTreeSize, IntValue maxTreeHeight,
      IntValue maxFunctionDefinitions, IntValue maxFunctionArguments
      );
  }
}
