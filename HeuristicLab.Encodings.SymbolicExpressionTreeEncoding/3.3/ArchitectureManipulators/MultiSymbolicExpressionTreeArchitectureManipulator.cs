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

using System;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Interfaces;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.ArchitectureManipulators {
  [Item("MultiSymbolicExpressionTreeArchitectureManipulator", "Randomly selects and applies one of its architecture manipulators every time it is called.")]
  [StorableClass]
  public sealed class MultiSymbolicExpressionTreeArchitectureManipulator : StochasticMultiBranch<ISymbolicExpressionTreeArchitectureManipulator>, ISymbolicExpressionTreeArchitectureManipulator, IStochasticOperator {
    private const string MaxTreeSizeParameterName = "MaxTreeSize";
    private const string MaxTreeHeightParameterName = "MaxTreeHeight";
    private const string SymbolicExpressionGrammarParameterName = "SymbolicExpressionGrammar";
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string MaxFunctionArgumentsParameterName = "MaxFunctionArguments";
    private const string MaxFunctionDefiningBranchesParameterName = "MaxFunctionDefiningBranches";

    public override bool CanChangeName {
      get { return false; }
    }
    protected override bool CreateChildOperation {
      get { return true; }
    }
    #region ISymbolicExpressionTreeArchitectureManipulator Members
    public IValueLookupParameter<IntValue> MaxFunctionDefinitionsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaxFunctionDefiningBranchesParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaxFunctionArgumentsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaxFunctionArgumentsParameterName]; }
    }
    #endregion

    #region ISymbolicExpressionTreeManipulator Members
    public ILookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    #endregion

    #region ISymbolicExpressionTreeOperator Members
    public IValueLookupParameter<IntValue> MaxTreeSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaxTreeSizeParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaxTreeHeightParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaxTreeHeightParameterName]; }
    }
    public ILookupParameter<ISymbolicExpressionGrammar> SymbolicExpressionGrammarParameter {
      get { return (ILookupParameter<ISymbolicExpressionGrammar>)Parameters[SymbolicExpressionGrammarParameterName]; }
    }
    #endregion


    [StorableConstructor]
    private MultiSymbolicExpressionTreeArchitectureManipulator(bool deserializing) : base(deserializing) { }
    private MultiSymbolicExpressionTreeArchitectureManipulator(MultiSymbolicExpressionTreeArchitectureManipulator original, Cloner cloner) : base(original, cloner) { }
    public MultiSymbolicExpressionTreeArchitectureManipulator()
      : base() {
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression tree on which the operator should be applied."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaxFunctionDefiningBranchesParameterName, "The maximal allowed number of function defining branches."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaxFunctionArgumentsParameterName, "The maximal allowed number of arguments of a newly created function."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaxTreeSizeParameterName, "The maximal size (number of nodes) of the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaxTreeHeightParameterName, "The maximal height of the symbolic expression tree (a tree with one node has height = 0)."));
      Parameters.Add(new LookupParameter<ISymbolicExpressionGrammar>(SymbolicExpressionGrammarParameterName, "The grammar that defines the allowed symbols and syntax of the symbolic expression trees."));

      foreach (Type type in ApplicationManager.Manager.GetTypes(typeof(ISymbolicExpressionTreeArchitectureManipulator))) {
        if (!typeof(MultiOperator<ISymbolicExpressionTreeArchitectureManipulator>).IsAssignableFrom(type))
          Operators.Add((ISymbolicExpressionTreeArchitectureManipulator)Activator.CreateInstance(type), true);
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiSymbolicExpressionTreeArchitectureManipulator(this, cloner);
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<ISymbolicExpressionTreeArchitectureManipulator>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeManipulators();
    }

    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<ISymbolicExpressionTreeArchitectureManipulator>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeManipulators();
    }

    private void ParameterizeManipulators() {
      foreach (ISymbolicExpressionTreeArchitectureManipulator manipulator in Operators.OfType<ISymbolicExpressionTreeArchitectureManipulator>()) {
        manipulator.MaxTreeSizeParameter.ActualName = MaxTreeSizeParameter.Name;
        manipulator.MaxTreeHeightParameter.ActualName = MaxTreeHeightParameter.Name;
        manipulator.SymbolicExpressionGrammarParameter.ActualName = SymbolicExpressionGrammarParameter.Name;
        manipulator.SymbolicExpressionTreeParameter.ActualName = SymbolicExpressionTreeParameter.Name;
        manipulator.MaxFunctionDefinitionsParameter.ActualName = MaxFunctionDefinitionsParameter.Name;
        manipulator.MaxFunctionArgumentsParameter.ActualName = MaxFunctionArgumentsParameter.Name;
      }

      foreach (IStochasticOperator manipulator in Operators.OfType<IStochasticOperator>()) {
        manipulator.RandomParameter.ActualName = RandomParameter.Name;
      }
    }

    #region ISymbolicExpressionTreeArchitectureManipulator Members
    public void ModifyArchitecture(IRandom random, SymbolicExpressionTree symbolicExpressionTree, ISymbolicExpressionGrammar grammar, IntValue maxTreeSize, IntValue maxTreeHeight, IntValue maxFunctionDefiningBranches, IntValue maxFunctionArguments, out bool success) {
      var op = Operators.SelectRandom(random);
      op.ModifyArchitecture(random, symbolicExpressionTree, grammar, maxTreeSize, maxTreeHeight, maxFunctionDefiningBranches, maxFunctionArguments, out success);
    }
    #endregion
  }
}
