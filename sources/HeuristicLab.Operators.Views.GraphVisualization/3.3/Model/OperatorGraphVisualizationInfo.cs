#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Core;
using Netron.Diagramming.Core;
using System.Drawing;
using HeuristicLab.Collections;

namespace HeuristicLab.Operators.Views.GraphVisualization {
  public sealed class OperatorGraphVisualizationInfo : DeepCloneable {
    private BidirectionalLookup<IOperator, IShapeInfo> shapeInfoMapping;
    private BidirectionalLookup<IOperator, IObservableKeyedCollection<string, IParameter>> operatorParameterCollectionMapping;
    private Dictionary<IValueParameter<IOperator>, IOperator> parameterOperatorMapping;

    private OperatorGraphVisualizationInfo() {
      this.shapeInfoMapping = new BidirectionalLookup<IOperator, IShapeInfo>();
      this.operatorParameterCollectionMapping = new BidirectionalLookup<IOperator, IObservableKeyedCollection<string, IParameter>>();
      this.parameterOperatorMapping = new Dictionary<IValueParameter<IOperator>, IOperator>();

      this.shapeInfos = new ObservableSet<IShapeInfo>();
    }

    public OperatorGraphVisualizationInfo(OperatorGraph operatorGraph)
      : this() {
      this.operatorGraph = operatorGraph;
      this.operatorGraph.InitialOperatorChanged += new EventHandler(operatorGraph_InitialOperatorChanged);
      foreach (IOperator op in operatorGraph.Operators)
        this.AddOperator(op);

      operatorGraph.Operators.ItemsAdded += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IOperator>(Operators_ItemsAdded);
      operatorGraph.Operators.ItemsRemoved += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IOperator>(Operators_ItemsRemoved);
      operatorGraph.Operators.CollectionReset += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IOperator>(Operators_CollectionReset);
    }

    public event EventHandler InitialShapeChanged;
    private void operatorGraph_InitialOperatorChanged(object sender, EventArgs e) {
      if (this.InitialShapeChanged != null)
        this.InitialShapeChanged(this, new EventArgs());
    }

    public IShapeInfo InitialShape {
      get {
        IOperator op = this.operatorGraph.InitialOperator;
        if (op == null)
          return null;
        return this.shapeInfoMapping.GetByFirst(op);
      }
    }

    private OperatorGraph operatorGraph;
    public OperatorGraph OperatorGraph {
      get { return this.operatorGraph; }
    }

    private ObservableSet<IShapeInfo> shapeInfos;
    public INotifyObservableCollectionItemsChanged<IShapeInfo> ObserveableShapeInfos {
      get { return this.shapeInfos; }
    }
    public IEnumerable<IShapeInfo> ShapeInfos {
      get { return this.shapeInfos; }
    }

    internal void AddShapeInfo(IOperator op, IShapeInfo shapeInfo) {
      this.RegisterOperatorEvents(op);
      this.operatorParameterCollectionMapping.Add(op, op.Parameters);
      this.shapeInfoMapping.Add(op, shapeInfo);
      this.shapeInfos.Add(shapeInfo);

      this.operatorGraph.Operators.Add(op);
    }

    internal void RemoveShapeInfo(IShapeInfo shapeInfo) {
      IOperator op = this.shapeInfoMapping.GetBySecond(shapeInfo);
      this.operatorGraph.Operators.Remove(op);
    }

    #region operator events
    private void AddOperator(IOperator op) {
      if (shapeInfoMapping.ContainsFirst(op))
        return;

      this.RegisterOperatorEvents(op);
      IShapeInfo shapeInfo = Factory.CreateShapeInfo(op);
      this.operatorParameterCollectionMapping.Add(op, op.Parameters);
      this.shapeInfoMapping.Add(op, shapeInfo);
      foreach (IParameter param in op.Parameters)
        this.AddParameter(op, param);

      this.shapeInfos.Add(shapeInfo);
    }

    private void RemoveOperator(IOperator op) {
      this.DeregisterOperatorEvents(op);
      foreach (IParameter param in op.Parameters)
        this.RemoveParameter(op, param);

      IShapeInfo shapeInfo = this.shapeInfoMapping.GetByFirst(op);
      this.operatorParameterCollectionMapping.RemoveByFirst(op);
      this.shapeInfoMapping.RemoveByFirst(op);
      this.shapeInfos.Remove(shapeInfo);
    }

    private void Operators_ItemsAdded(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IOperator> e) {
      foreach (IOperator op in e.Items)
        this.AddOperator(op);
    }

    private void Operators_ItemsRemoved(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IOperator> e) {
      foreach (IOperator op in e.Items)
        this.RemoveOperator(op);
    }

    private void Operators_CollectionReset(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IOperator> e) {
      foreach (IOperator op in e.OldItems)
        this.RemoveOperator(op);
      foreach (IOperator op in e.Items)
        this.AddOperator(op);
    }

    private void RegisterOperatorEvents(IOperator op) {
      op.Parameters.ItemsAdded += new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsAdded);
      op.Parameters.ItemsRemoved += new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsRemoved);
      op.Parameters.ItemsReplaced += new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsReplaced);
      op.Parameters.CollectionReset += new CollectionItemsChangedEventHandler<IParameter>(Parameters_CollectionReset);
    }
    private void DeregisterOperatorEvents(IOperator op) {
      op.Parameters.ItemsAdded -= new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsAdded);
      op.Parameters.ItemsRemoved -= new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsRemoved);
      op.Parameters.ItemsReplaced -= new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsReplaced);
      op.Parameters.CollectionReset -= new CollectionItemsChangedEventHandler<IParameter>(Parameters_CollectionReset);
    }
    #endregion

    #region parameter events
    private void AddParameter(IOperator op, IParameter param) {
      IValueParameter<IOperator> opParam = param as IValueParameter<IOperator>;
      if (opParam != null) {
        this.RegisterOperatorParameterEvents(opParam);
        this.parameterOperatorMapping.Add(opParam, op);
        IShapeInfo shapeInfo = this.shapeInfoMapping.GetByFirst(op);
        shapeInfo.AddConnector(param.Name);

        if (opParam.Value != null) {
          if (!this.shapeInfoMapping.ContainsFirst(opParam.Value))
            this.AddOperator(opParam.Value);
          shapeInfo.AddConnection(param.Name, this.shapeInfoMapping.GetByFirst(opParam.Value));
        }
      }
    }
    private void RemoveParameter(IOperator op, IParameter param) {
      IValueParameter<IOperator> opParam = param as IValueParameter<IOperator>;
      if (opParam != null) {
        this.DeregisterOperatorParameterEvents(opParam);
        this.parameterOperatorMapping.Remove(opParam);
        IShapeInfo shapeInfo = this.shapeInfoMapping.GetByFirst(op);
        shapeInfo.RemoveConnector(param.Name);
      }
    }

    private void opParam_ValueChanged(object sender, EventArgs e) {
      IValueParameter<IOperator> opParam = (IValueParameter<IOperator>)sender;
      if (opParam != null) {
        IOperator op = this.parameterOperatorMapping[opParam];
        IShapeInfo shapeInfo = this.shapeInfoMapping.GetByFirst(op);

        if (opParam.Value == null)
          shapeInfo.RemoveConnection(opParam.Name);
        else
          shapeInfo.ChangeConnection(opParam.Name, this.shapeInfoMapping.GetByFirst(opParam.Value));
      }
    }

    private void Parameters_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      IObservableKeyedCollection<string, IParameter> parameterCollection = sender as IObservableKeyedCollection<string, IParameter>;
      IOperator op = this.operatorParameterCollectionMapping.GetBySecond(parameterCollection);
      foreach (IParameter param in e.Items)
        AddParameter(op, param);
    }
    private void Parameters_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      IObservableKeyedCollection<string, IParameter> parameterCollection = sender as IObservableKeyedCollection<string, IParameter>;
      IOperator op = this.operatorParameterCollectionMapping.GetBySecond(parameterCollection);
      foreach (IParameter param in e.Items)
        RemoveParameter(op, param);
    }
    private void Parameters_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      IObservableKeyedCollection<string, IParameter> parameterCollection = sender as IObservableKeyedCollection<string, IParameter>;
      IOperator op = this.operatorParameterCollectionMapping.GetBySecond(parameterCollection);
      foreach (IParameter param in e.OldItems)
        RemoveParameter(op, param);
      foreach (IParameter param in e.Items)
        AddParameter(op, param);
    }
    private void Parameters_CollectionReset(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      IObservableKeyedCollection<string, IParameter> parameterCollection = sender as IObservableKeyedCollection<string, IParameter>;
      IOperator op = this.operatorParameterCollectionMapping.GetBySecond(parameterCollection);
      foreach (IParameter param in e.OldItems)
        RemoveParameter(op, param);
      foreach (IParameter param in e.Items)
        AddParameter(op, param);
    }


    private void RegisterOperatorParameterEvents(IValueParameter<IOperator> opParam) {
      opParam.ValueChanged += new EventHandler(opParam_ValueChanged);
    }
    private void DeregisterOperatorParameterEvents(IValueParameter<IOperator> opParam) {
      opParam.ValueChanged -= new EventHandler(opParam_ValueChanged);
    }
    #endregion
  }
}
