#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [Item("MultiSymbolicExpressionTreeArchitectureManipulator", "Randomly selects and applies one of its architecture manipulators every time it is called.")]
  [StorableClass]
  public sealed class MultiSymbolicExpressionTreeArchitectureManipulator : StochasticMultiBranch<ISymbolicExpressionTreeManipulator>,
    ISymbolicExpressionTreeArchitectureManipulator,
    ISymbolicExpressionTreeSizeConstraintOperator,
    IStochasticOperator {
    private const string MaximumSymbolicExpressionTreeLengthParameterName = "MaximumSymbolicExpressionTreeLength";
    private const string MaximumSymbolicExpressionTreeDepthParameterName = "MaximumSymbolicExpressionTreeDepth";
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string MaximumFunctionArgumentsParameterName = "MaximumFunctionArguments";
    private const string MaximumFunctionDefinitionsParameterName = "MaximumFunctionDefinitions";

    public override bool CanChangeName {
      get { return false; }
    }
    protected override bool CreateChildOperation {
      get { return true; }
    }
    #region Parameter properties
    public ILookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<ISymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaximumFunctionDefinitionsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumFunctionDefinitionsParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaximumFunctionArgumentsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumFunctionArgumentsParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeLengthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeLengthParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeDepthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeDepthParameterName]; }
    }
    #endregion
    #region Parameter Properties
    public IntValue MaximumFunctionDefinitions {
      get { return MaximumFunctionDefinitionsParameter.ActualValue; }
    }
    public IntValue MaximumFunctionArguments {
      get { return MaximumFunctionArgumentsParameter.ActualValue; }
    }
    public IntValue MaximumSymbolicExpressionTreeLength {
      get { return MaximumSymbolicExpressionTreeLengthParameter.ActualValue; }
    }
    public IntValue MaximumSymbolicExpressionTreeDepth {
      get { return MaximumSymbolicExpressionTreeDepthParameter.ActualValue; }
    }
    #endregion


    [StorableConstructor]
    private MultiSymbolicExpressionTreeArchitectureManipulator(bool deserializing) : base(deserializing) { }
    private MultiSymbolicExpressionTreeArchitectureManipulator(MultiSymbolicExpressionTreeArchitectureManipulator original, Cloner cloner) : base(original, cloner) { }
    public MultiSymbolicExpressionTreeArchitectureManipulator()
      : base() {
      Parameters.Add(new LookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression tree on which the operator should be applied."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumFunctionDefinitionsParameterName, "The maximal allowed number of automatically defined functions."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumFunctionArgumentsParameterName, "The maximal allowed number of arguments of a automatically defined functions."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName, "The maximal length (number of nodes) of the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeDepthParameterName, "The maximal depth of the symbolic expression tree (a tree with one node has depth = 0)."));

      List<ISymbolicExpressionTreeManipulator> list = new List<ISymbolicExpressionTreeManipulator>();
      foreach (Type type in ApplicationManager.Manager.GetTypes(typeof(ISymbolicExpressionTreeManipulator))) {
        if (this.GetType().Assembly != type.Assembly) continue;
        if (typeof(IMultiOperator<ISymbolicExpressionTreeManipulator>).IsAssignableFrom(type)) continue;
        list.Add((ISymbolicExpressionTreeManipulator)Activator.CreateInstance(type));
      }
      CheckedItemList<ISymbolicExpressionTreeManipulator> checkedItemList = new CheckedItemList<ISymbolicExpressionTreeManipulator>();
      checkedItemList.AddRange(list.OrderBy(op => op.Name));
      Operators = checkedItemList.AsReadOnly();
      Operators_ItemsAdded(this, new CollectionItemsChangedEventArgs<IndexedItem<ISymbolicExpressionTreeManipulator>>(Operators.CheckedItems));

      SelectedOperatorParameter.ActualName = "SelectedManipulationOperator";
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiSymbolicExpressionTreeArchitectureManipulator(this, cloner);
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<ISymbolicExpressionTreeManipulator>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeManipulators();
    }

    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<ISymbolicExpressionTreeManipulator>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeManipulators();
    }

    private void ParameterizeManipulators() {
      foreach (ISymbolicExpressionTreeArchitectureManipulator manipulator in Operators.OfType<ISymbolicExpressionTreeArchitectureManipulator>()) {
        manipulator.MaximumFunctionArgumentsParameter.ActualName = MaximumFunctionArgumentsParameter.Name;
        manipulator.MaximumFunctionDefinitionsParameter.ActualName = MaximumFunctionDefinitionsParameter.Name;
      }
      foreach (ISymbolicExpressionTreeSizeConstraintOperator manipulator in Operators.OfType<ISymbolicExpressionTreeSizeConstraintOperator>()) {
        manipulator.MaximumSymbolicExpressionTreeDepthParameter.ActualName = MaximumSymbolicExpressionTreeDepthParameter.Name;
        manipulator.MaximumSymbolicExpressionTreeLengthParameter.ActualName = MaximumSymbolicExpressionTreeLengthParameter.Name;
      }
      foreach (ISymbolicExpressionTreeManipulator manipulator in Operators.OfType<ISymbolicExpressionTreeManipulator>()) {
        manipulator.SymbolicExpressionTreeParameter.ActualName = SymbolicExpressionTreeParameter.Name;
      }
      foreach (IStochasticOperator manipulator in Operators.OfType<IStochasticOperator>()) {
        manipulator.RandomParameter.ActualName = RandomParameter.Name;
      }
    }
  }
}
