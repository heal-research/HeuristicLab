#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// A base class for operators creating symbolic expression trees.
  /// </summary>
  [Item("SymbolicExpressionTreeCreator", "A base class for operators creating symbolic expression trees.")]
  [StorableClass]
  public abstract class SymbolicExpressionTreeCreator : SymbolicExpressionTreeOperator, ISymbolicExpressionTreeCreator {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string SymbolicExpressionTreeGrammarParameterName = "SymbolicExpressionTreeGrammar";
    private const string ClonedSymbolicExpressionTreeGrammarParameterName = "ClonedSymbolicExpressionTreeGrammar";

    #region Parameter Properties
    public ILookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<ISymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }

    public IValueLookupParameter<ISymbolicExpressionGrammar> SymbolicExpressionTreeGrammarParameter {
      get { return (IValueLookupParameter<ISymbolicExpressionGrammar>)Parameters[SymbolicExpressionTreeGrammarParameterName]; }
    }

    public ILookupParameter<ISymbolicExpressionGrammar> ClonedSymbolicExpressionTreeGrammarParameter {
      get { return (ILookupParameter<ISymbolicExpressionGrammar>)Parameters[ClonedSymbolicExpressionTreeGrammarParameterName]; }
    }
    #endregion

    [StorableConstructor]
    protected SymbolicExpressionTreeCreator(bool deserializing) : base(deserializing) { }
    protected SymbolicExpressionTreeCreator(SymbolicExpressionTreeCreator original, Cloner cloner) : base(original, cloner) { }
    protected SymbolicExpressionTreeCreator()
      : base() {
      Parameters.Add(new LookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression tree that should be created."));
      Parameters.Add(new ValueLookupParameter<ISymbolicExpressionGrammar>(SymbolicExpressionTreeGrammarParameterName,
        "The tree grammar that defines the correct syntax of symbolic expression trees that should be created."));
      Parameters.Add(new LookupParameter<ISymbolicExpressionGrammar>(ClonedSymbolicExpressionTreeGrammarParameterName,
        "An immutable clone of the concrete grammar that is actually used to create and manipulate trees."));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(ClonedSymbolicExpressionTreeGrammarParameterName))
        Parameters.Add(new LookupParameter<ISymbolicExpressionGrammar>(ClonedSymbolicExpressionTreeGrammarParameterName, "An immutable clone of the concrete grammar that is actually used to create and manipulate trees."));
    }

    public override IOperation InstrumentedApply() {
      if (ClonedSymbolicExpressionTreeGrammarParameter.ActualValue == null) {
        SymbolicExpressionTreeGrammarParameter.ActualValue.ReadOnly = true;
        IScope globalScope = ExecutionContext.Scope;
        while (globalScope.Parent != null)
          globalScope = globalScope.Parent;

        globalScope.Variables.Add(new Variable(ClonedSymbolicExpressionTreeGrammarParameterName,
          (ISymbolicExpressionGrammar)SymbolicExpressionTreeGrammarParameter.ActualValue.Clone()));
      }
      SymbolicExpressionTreeParameter.ActualValue = Create(Random);
      return base.InstrumentedApply();
    }

    protected abstract ISymbolicExpressionTree Create(IRandom random);
    public abstract ISymbolicExpressionTree CreateTree(IRandom random, ISymbolicExpressionGrammar grammar, int maxTreeLength, int maxTreeDepth);
  }
}
