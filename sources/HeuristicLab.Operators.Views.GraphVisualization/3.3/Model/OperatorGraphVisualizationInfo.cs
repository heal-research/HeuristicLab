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
      this.connections = new ObservableDictionary<KeyValuePair<IShapeInfo, string>, IShapeInfo>();
    }

    public OperatorGraphVisualizationInfo(OperatorGraph operatorGraph)
      : this() {
      this.operatorGraph = operatorGraph;
      this.operatorGraph.InitialOperatorChanged += new EventHandler(operatorGraph_InitialOperatorChanged);
      foreach (IOperator op in operatorGraph.Operators)
        if (!this.shapeInfoMapping.ContainsFirst(op))
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

    private ObservableDictionary<KeyValuePair<IShapeInfo, string>, IShapeInfo> connections;
    public INotifyObservableDictionaryItemsChanged<KeyValuePair<IShapeInfo, string>, IShapeInfo> ObservableConnections {
      get { return this.connections; }
    }
    public IEnumerable<KeyValuePair<KeyValuePair<IShapeInfo, string>, IShapeInfo>> Connections {
      get { return this.connections; }
    }

    #region methods to manipulate operatorgraph by the shape info
    internal void AddShapeInfo(IOperator op, IShapeInfo shapeInfo) {
      this.RegisterOperatorEvents(op);
      this.operatorParameterCollectionMapping.Add(op, op.Parameters);
      this.shapeInfoMapping.Add(op, shapeInfo);
      this.shapeInfos.Add(shapeInfo);

      foreach (IParameter param in op.Parameters)
        this.AddParameter(op, param);

      this.operatorGraph.Operators.Add(op);
    }

    internal void RemoveShapeInfo(IShapeInfo shapeInfo) {
      IOperator op = this.shapeInfoMapping.GetBySecond(shapeInfo);
      this.operatorGraph.Operators.Remove(op);
    }

    internal void AddConnection(IShapeInfo shapeInfoFrom, string connectorName, IShapeInfo shapeInfoTo) {
      IOperator opFrom = this.shapeInfoMapping.GetBySecond(shapeInfoFrom);
      IOperator opTo = this.shapeInfoMapping.GetBySecond(shapeInfoTo);

      IValueParameter<IOperator> param = (IValueParameter<IOperator>)opFrom.Parameters[connectorName];
      param.Value = opTo;
    }

    internal void ChangeConnection(IShapeInfo shapeInfoFrom, string connectorName, IShapeInfo shapeInfoTo) {
      IOperator opFrom = this.shapeInfoMapping.GetBySecond(shapeInfoFrom);
      IOperator opTo = this.shapeInfoMapping.GetBySecond(shapeInfoTo);

      IValueParameter<IOperator> param = (IValueParameter<IOperator>)opFrom.Parameters[connectorName];
      param.Value = opTo;
    }

    internal void RemoveConnection(IShapeInfo shapeInfoFrom, string connectorName) {
      IOperator opFrom = this.shapeInfoMapping.GetBySecond(shapeInfoFrom);
      IValueParameter<IOperator> param = (IValueParameter<IOperator>)opFrom.Parameters[connectorName];
      param.Value = null;
    }
    #endregion

    #region operator events
    private void AddOperator(IOperator op) {
      if (!this.shapeInfoMapping.ContainsFirst(op)) {
        this.RegisterOperatorEvents(op);
        IShapeInfo shapeInfo = Factory.CreateShapeInfo(op);
        this.operatorParameterCollectionMapping.Add(op, op.Parameters);
        this.shapeInfoMapping.Add(op, shapeInfo);
        foreach (IParameter param in op.Parameters)
          this.AddParameter(op, param);

        this.shapeInfos.Add(shapeInfo);
      }
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
          this.connections.Add(new KeyValuePair<IShapeInfo, string>(shapeInfo, param.Name), this.shapeInfoMapping.GetByFirst(opParam.Value));
        }
      }
    }

    private void RemoveParameter(IOperator op, IParameter param) {
      IValueParameter<IOperator> opParam = param as IValueParameter<IOperator>;
      if (opParam != null) {
        this.DeregisterOperatorParameterEvents(opParam);
        this.parameterOperatorMapping.Remove(opParam);
        IShapeInfo shapeInfo = this.shapeInfoMapping.GetByFirst(op);

        this.connections.Remove(new KeyValuePair<IShapeInfo, string>(shapeInfo, param.Name));
        shapeInfo.RemoveConnector(param.Name);
      }
    }

    private void opParam_ValueChanged(object sender, EventArgs e) {
      IValueParameter<IOperator> opParam = (IValueParameter<IOperator>)sender;
      if (opParam != null) {
        IOperator op = this.parameterOperatorMapping[opParam];
        IShapeInfo shapeInfo = this.shapeInfoMapping.GetByFirst(op);
        KeyValuePair<IShapeInfo, string> connectionFrom = new KeyValuePair<IShapeInfo, string>(shapeInfo, opParam.Name);

        if (opParam.Value == null)
          this.connections.Remove(connectionFrom);
        else {
          if (!this.shapeInfoMapping.ContainsFirst(opParam.Value))
            this.AddOperator(opParam.Value);
          this.connections[connectionFrom] = this.shapeInfoMapping.GetByFirst(opParam.Value);
        }
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
