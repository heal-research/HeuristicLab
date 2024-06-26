﻿#region License Information
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// A base class for operators creating symbolic expression trees.
  /// </summary>
  [Item("SymbolicExpressionTreeCreator", "A base class for operators creating symbolic expression trees.")]
  [StorableType("88CC0F00-5674-4598-A3C4-756B3B7E80CB")]
  public abstract class SymbolicExpressionTreeCreator : SymbolicExpressionTreeOperator, ISymbolicExpressionTreeCreator {
    private const string MaximumSymbolicExpressionTreeLengthParameterName = "MaximumSymbolicExpressionTreeLength";
    private const string MaximumSymbolicExpressionTreeDepthParameterName = "MaximumSymbolicExpressionTreeDepth";

    private const string SymbolicExpressionTreeGrammarParameterName = "SymbolicExpressionTreeGrammar";

    #region Parameter Properties
    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeLengthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeLengthParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeDepthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeDepthParameterName]; }
    }
    public IValueLookupParameter<ISymbolicExpressionGrammar> SymbolicExpressionTreeGrammarParameter {
      get { return (IValueLookupParameter<ISymbolicExpressionGrammar>)Parameters[SymbolicExpressionTreeGrammarParameterName]; }
    }
    #endregion

    [StorableConstructor]
    protected SymbolicExpressionTreeCreator(StorableConstructorFlag _) : base(_) { }
    protected SymbolicExpressionTreeCreator(SymbolicExpressionTreeCreator original, Cloner cloner) : base(original, cloner) { }
    protected SymbolicExpressionTreeCreator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName, "The maximal length (number of nodes) of the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeDepthParameterName, "The maximal depth of the symbolic expression tree (a tree with one node has depth = 0)."));
      Parameters.Add(new ValueLookupParameter<ISymbolicExpressionGrammar>(SymbolicExpressionTreeGrammarParameterName, "The tree grammar that defines the correct syntax of symbolic expression trees that should be created."));
    }

    public override IOperation InstrumentedApply() {
      var rand = RandomParameter.ActualValue;
      var grammar = SymbolicExpressionTreeGrammarParameter.ActualValue;
      var maxTreeLength = MaximumSymbolicExpressionTreeLengthParameter.ActualValue.Value;
      var maxTreeDepth = MaximumSymbolicExpressionTreeDepthParameter.ActualValue.Value;

      SymbolicExpressionTreeParameter.ActualValue = CreateTree(rand, grammar, maxTreeLength, maxTreeDepth);
      return base.InstrumentedApply();
    }
    public abstract ISymbolicExpressionTree CreateTree(IRandom random, ISymbolicExpressionGrammar grammar, int maxTreeLength, int maxTreeDepth);
  }
}
