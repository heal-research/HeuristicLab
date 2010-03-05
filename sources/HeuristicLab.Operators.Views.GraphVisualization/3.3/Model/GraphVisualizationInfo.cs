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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators.Views.GraphVisualization {
  public sealed class GraphVisualizationInfo : DeepCloneable {
    private BidirectionalLookup<IOperator, IOperatorShapeInfo> operatorShapeInfoMapping;
    [Storable]
    private BidirectionalLookup<IOperator, IOperatorShapeInfo> OperatorShapeInfoMappingStore {
      get { return this.operatorShapeInfoMapping; }
      set {
        IOperator op;
        IOperatorShapeInfo shapeInfo;
        foreach (KeyValuePair<IOperator, IOperatorShapeInfo> pair in value.FirstEnumerable) {
          op = pair.Key;
          shapeInfo = pair.Value;
          this.RegisterOperatorEvents(op);
          this.operatorParameterCollectionMapping.Add(op, pair.Key.Parameters);
          this.operatorShapeInfoMapping.Add(op, shapeInfo);
          this.shapeInfos.Add(shapeInfo);
        }

        foreach(IOperator oper in value.FirstValues) {
          foreach (IParameter param in oper.Parameters) {
            this.parameterOperatorMapping.Add(param, oper);
            IValueParameter<IOperator> opParam = param as IValueParameter<IOperator>;
            if (opParam != null) {
              this.RegisterOperatorParameterEvents(opParam);
              shapeInfo = this.operatorShapeInfoMapping.GetByFirst(oper);
              if (opParam.Value != null) {
                this.connections.Add(new KeyValuePair<IOperatorShapeInfo, string>(shapeInfo, param.Name), this.operatorShapeInfoMapping.GetByFirst(opParam.Value));
              }
            } else
              this.RegisterParameterEvents(param);
          }
        }
      }
    }

    private BidirectionalLookup<IOperator, IObservableKeyedCollection<string, IParameter>> operatorParameterCollectionMapping;
    private Dictionary<IParameter, IOperator> parameterOperatorMapping;

    private GraphVisualizationInfo() {
      this.operatorShapeInfoMapping = new BidirectionalLookup<IOperator, IOperatorShapeInfo>();
      this.operatorParameterCollectionMapping = new BidirectionalLookup<IOperator, IObservableKeyedCollection<string, IParameter>>();
      this.parameterOperatorMapping = new Dictionary<IParameter, IOperator>();

      this.shapeInfos = new ObservableSet<IOperatorShapeInfo>();
      this.connections = new ObservableDictionary<KeyValuePair<IOperatorShapeInfo, string>, IOperatorShapeInfo>();
    }

    public GraphVisualizationInfo(OperatorGraph operatorGraph)
      : this() {
      this.OperatorGraph = operatorGraph;

      foreach (IOperator op in operatorGraph.Operators)
        if (!this.operatorShapeInfoMapping.ContainsFirst(op))
          this.AddOperator(op);

      this.UpdateInitialShape();
    }

    public event EventHandler InitialShapeChanged;
    private void operatorGraph_InitialOperatorChanged(object sender, EventArgs e) {
      this.UpdateInitialShape();
    }

    private void UpdateInitialShape() {
      IOperatorShapeInfo old = this.oldInitialShape as OperatorShapeInfo;
      if (old != null)
        old.Color = oldInitialShapeColor;

      OperatorShapeInfo newInitialShapeInfo = this.InitialShape as OperatorShapeInfo;
      if (newInitialShapeInfo != null) {
        oldInitialShapeColor = newInitialShapeInfo.Color;
        newInitialShapeInfo.Color = Color.LightGreen;
      }

      oldInitialShape = this.InitialShape;
      if (this.InitialShapeChanged != null)
        this.InitialShapeChanged(this, new EventArgs());
    }

    [Storable]
    private IOperatorShapeInfo oldInitialShape;
    [Storable]
    private Color oldInitialShapeColor;
    public IOperatorShapeInfo InitialShape {
      get {
        IOperator op = this.operatorGraph.InitialOperator;
        if (op == null)
          return null;
        return this.operatorShapeInfoMapping.GetByFirst(op);
      }
      set {
        if (value == null)
          this.OperatorGraph.InitialOperator = null;
        else
          this.OperatorGraph.InitialOperator = this.operatorShapeInfoMapping.GetBySecond(value);
      }
    }

    
    private OperatorGraph operatorGraph;
    [Storable]
    public OperatorGraph OperatorGraph {
      get { return this.operatorGraph; }
      private set {
        if (this.operatorGraph != null || value == null)
          throw new InvalidOperationException("Could not set OperatorGraph");

        this.operatorGraph = value;
        this.operatorGraph.InitialOperatorChanged += new EventHandler(operatorGraph_InitialOperatorChanged);
        this.operatorGraph.Operators.ItemsAdded += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IOperator>(Operators_ItemsAdded);
        this.operatorGraph.Operators.ItemsRemoved += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IOperator>(Operators_ItemsRemoved);
        this.operatorGraph.Operators.CollectionReset += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IOperator>(Operators_CollectionReset);
      }
    }

    private ObservableSet<IOperatorShapeInfo> shapeInfos;
    public INotifyObservableCollectionItemsChanged<IOperatorShapeInfo> ObserveableShapeInfos {
      get { return this.shapeInfos; }
    }
    public IEnumerable<IOperatorShapeInfo> OperatorShapeInfos {
      get { return this.shapeInfos; }
    }
    public IOperator GetOperatorForShapeInfo(IOperatorShapeInfo shapeInfo) {
      return this.operatorShapeInfoMapping.GetBySecond(shapeInfo);
    }

    private ObservableDictionary<KeyValuePair<IOperatorShapeInfo, string>, IOperatorShapeInfo> connections;
    public INotifyObservableDictionaryItemsChanged<KeyValuePair<IOperatorShapeInfo, string>, IOperatorShapeInfo> ObservableConnections {
      get { return this.connections; }
    }
    public IEnumerable<KeyValuePair<KeyValuePair<IOperatorShapeInfo, string>, IOperatorShapeInfo>> Connections {
      get { return this.connections; }
    }

    #region methods to manipulate operatorgraph by the shape info
    internal void AddShapeInfo(IOperator op, IOperatorShapeInfo shapeInfo) {
      this.RegisterOperatorEvents(op);
      this.operatorParameterCollectionMapping.Add(op, op.Parameters);
      this.operatorShapeInfoMapping.Add(op, shapeInfo);
      this.shapeInfos.Add(shapeInfo);

      foreach (IParameter param in op.Parameters)
        this.AddParameter(op, param);

      this.operatorGraph.Operators.Add(op);
    }

    internal void RemoveShapeInfo(IOperatorShapeInfo shapeInfo) {
      IOperator op = this.operatorShapeInfoMapping.GetBySecond(shapeInfo);
      this.operatorGraph.Operators.Remove(op);
    }

    internal void AddConnection(IOperatorShapeInfo shapeInfoFrom, string connectorName, IOperatorShapeInfo shapeInfoTo) {
      IOperator opFrom = this.operatorShapeInfoMapping.GetBySecond(shapeInfoFrom);
      IOperator opTo = this.operatorShapeInfoMapping.GetBySecond(shapeInfoTo);

      IValueParameter<IOperator> param = (IValueParameter<IOperator>)opFrom.Parameters[connectorName];
      param.Value = opTo;
    }

    internal void ChangeConnection(IOperatorShapeInfo shapeInfoFrom, string connectorName, IOperatorShapeInfo shapeInfoTo) {
      IOperator opFrom = this.operatorShapeInfoMapping.GetBySecond(shapeInfoFrom);
      IOperator opTo = this.operatorShapeInfoMapping.GetBySecond(shapeInfoTo);

      IValueParameter<IOperator> param = (IValueParameter<IOperator>)opFrom.Parameters[connectorName];
      param.Value = opTo;
    }

    internal void RemoveConnection(IOperatorShapeInfo shapeInfoFrom, string connectorName) {
      IOperator opFrom = this.operatorShapeInfoMapping.GetBySecond(shapeInfoFrom);
      IValueParameter<IOperator> param = (IValueParameter<IOperator>)opFrom.Parameters[connectorName];
      param.Value = null;
    }
    #endregion

    #region operator events
    private void AddOperator(IOperator op) {
      if (!this.operatorShapeInfoMapping.ContainsFirst(op)) {
        this.RegisterOperatorEvents(op);
        IOperatorShapeInfo shapeInfo = Factory.CreateOperatorShapeInfo(op);
        this.operatorParameterCollectionMapping.Add(op, op.Parameters);
        this.operatorShapeInfoMapping.Add(op, shapeInfo);
        this.shapeInfos.Add(shapeInfo);
        foreach (IParameter param in op.Parameters)
          this.AddParameter(op, param);
      }
    }

    private void RemoveOperator(IOperator op) {
      this.DeregisterOperatorEvents(op);
      foreach (IParameter param in op.Parameters)
        this.RemoveParameter(op, param);

      IOperatorShapeInfo shapeInfo = this.operatorShapeInfoMapping.GetByFirst(op);
      this.operatorParameterCollectionMapping.RemoveByFirst(op);
      this.operatorShapeInfoMapping.RemoveByFirst(op);
      this.shapeInfos.Remove(shapeInfo);
    }

    private void OperatorBreakpointChanged(object sender, EventArgs e) {
      IOperator op = (IOperator)sender;
      IOperatorShapeInfo operatorShapeInfo = this.operatorShapeInfoMapping.GetByFirst(op);
      if (op.Breakpoint) {
        operatorShapeInfo.LineColor = Color.Red;
        operatorShapeInfo.LineWidth = 2;
      } else {
        operatorShapeInfo.LineColor = Color.Black;
        operatorShapeInfo.LineWidth = 1;
      }
    }

    private void OperatorNameChanged(object sender, EventArgs e) {
      IOperator op = (IOperator)sender;
      IOperatorShapeInfo operatorShapeInfo = this.operatorShapeInfoMapping.GetByFirst(op);
      operatorShapeInfo.Title = op.Name;
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
      op.NameChanged += new EventHandler(OperatorNameChanged);
      op.BreakpointChanged += new EventHandler(OperatorBreakpointChanged);
    }

    private void DeregisterOperatorEvents(IOperator op) {
      op.Parameters.ItemsAdded -= new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsAdded);
      op.Parameters.ItemsRemoved -= new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsRemoved);
      op.Parameters.ItemsReplaced -= new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsReplaced);
      op.Parameters.CollectionReset -= new CollectionItemsChangedEventHandler<IParameter>(Parameters_CollectionReset);
      op.NameChanged -= new EventHandler(OperatorNameChanged);
      op.BreakpointChanged -= new EventHandler(OperatorBreakpointChanged);
    }
    #endregion

    #region parameter events
    private void AddParameter(IOperator op, IParameter param) {
      this.parameterOperatorMapping.Add(param, op);
      IValueParameter<IOperator> opParam = param as IValueParameter<IOperator>;
      if (opParam != null) {
        this.RegisterOperatorParameterEvents(opParam);
        IOperatorShapeInfo shapeInfo = this.operatorShapeInfoMapping.GetByFirst(op);
        shapeInfo.AddConnector(param.Name);

        if (opParam.Value != null) {
          if (!this.operatorShapeInfoMapping.ContainsFirst(opParam.Value))
            this.AddOperator(opParam.Value);
          this.connections.Add(new KeyValuePair<IOperatorShapeInfo, string>(shapeInfo, param.Name), this.operatorShapeInfoMapping.GetByFirst(opParam.Value));
        }
      } else
        this.RegisterParameterEvents(param);
    }

    private void RemoveParameter(IOperator op, IParameter param) {
      IValueParameter<IOperator> opParam = param as IValueParameter<IOperator>;
      if (opParam != null) {
        this.DeregisterOperatorParameterEvents(opParam);
        IOperatorShapeInfo shapeInfo = this.operatorShapeInfoMapping.GetByFirst(op);
        this.connections.Remove(new KeyValuePair<IOperatorShapeInfo, string>(shapeInfo, param.Name));
        shapeInfo.RemoveConnector(param.Name);
      } else
        this.DeregisterParameterEvents(param);

      this.parameterOperatorMapping.Remove(param);
    }

    private void opParam_ValueChanged(object sender, EventArgs e) {
      IValueParameter<IOperator> opParam = (IValueParameter<IOperator>)sender;
      if (opParam != null) {
        IOperator op = this.parameterOperatorMapping[opParam];
        IOperatorShapeInfo shapeInfo = this.operatorShapeInfoMapping.GetByFirst(op);
        KeyValuePair<IOperatorShapeInfo, string> connectionFrom = new KeyValuePair<IOperatorShapeInfo, string>(shapeInfo, opParam.Name);

        if (opParam.Value == null)
          this.connections.Remove(connectionFrom);
        else {
          if (!this.operatorShapeInfoMapping.ContainsFirst(opParam.Value))
            this.AddOperator(opParam.Value);
          this.connections[connectionFrom] = this.operatorShapeInfoMapping.GetByFirst(opParam.Value);
        }
      }
    }

    private void Parameters_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      IObservableKeyedCollection<string, IParameter> parameterCollection = sender as IObservableKeyedCollection<string, IParameter>;
      IOperator op = this.operatorParameterCollectionMapping.GetBySecond(parameterCollection);
      foreach (IParameter param in e.Items)
        AddParameter(op, param);
      this.UpdateParameterLabels(op);
    }
    private void Parameters_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      IObservableKeyedCollection<string, IParameter> parameterCollection = sender as IObservableKeyedCollection<string, IParameter>;
      IOperator op = this.operatorParameterCollectionMapping.GetBySecond(parameterCollection);
      foreach (IParameter param in e.Items)
        RemoveParameter(op, param);
      this.UpdateParameterLabels(op);
    }
    private void Parameters_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      IObservableKeyedCollection<string, IParameter> parameterCollection = sender as IObservableKeyedCollection<string, IParameter>;
      IOperator op = this.operatorParameterCollectionMapping.GetBySecond(parameterCollection);
      foreach (IParameter param in e.OldItems)
        RemoveParameter(op, param);
      foreach (IParameter param in e.Items)
        AddParameter(op, param);
      this.UpdateParameterLabels(op);
    }
    private void Parameters_CollectionReset(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      IObservableKeyedCollection<string, IParameter> parameterCollection = sender as IObservableKeyedCollection<string, IParameter>;
      IOperator op = this.operatorParameterCollectionMapping.GetBySecond(parameterCollection);
      foreach (IParameter param in e.OldItems)
        RemoveParameter(op, param);
      foreach (IParameter param in e.Items)
        AddParameter(op, param);
      this.UpdateParameterLabels(op);
    }

    private void RegisterOperatorParameterEvents(IValueParameter<IOperator> opParam) {
      opParam.ValueChanged += new EventHandler(opParam_ValueChanged);
    }
    private void DeregisterOperatorParameterEvents(IValueParameter<IOperator> opParam) {
      opParam.ValueChanged -= new EventHandler(opParam_ValueChanged);
    }
    private void RegisterParameterEvents(IParameter param) {
      param.ToStringChanged += new EventHandler(param_ToStringChanged);
      param.NameChanged += new EventHandler(param_NameChanged);
    }
    private void DeregisterParameterEvents(IParameter param) {
      param.ToStringChanged -= new EventHandler(param_ToStringChanged);
      param.NameChanged -= new EventHandler(param_NameChanged);
    }

    private void param_NameChanged(object sender, EventArgs e) {
      IParameter param = (IParameter)sender;
      IOperator op = this.parameterOperatorMapping[param];
      this.UpdateParameterLabels(op);
    }
    private void param_ToStringChanged(object sender, EventArgs e) {
      IParameter param = (IParameter)sender;
      IOperator op = this.parameterOperatorMapping[param];
      this.UpdateParameterLabels(op);
    }

    private void UpdateParameterLabels(IOperator op) {
      IEnumerable<IParameter> parameters = op.Parameters.Where(p => !(p is IValueParameter<IOperator>));
      IOperatorShapeInfo operatorShapeInfo = this.operatorShapeInfoMapping.GetByFirst(op);
      if (parameters.Count() > 0)
        operatorShapeInfo.UpdateLabels(parameters.Select(p => p.ToString()));
      else
        operatorShapeInfo.UpdateLabels(new List<string>());
    }
    #endregion
  }
}