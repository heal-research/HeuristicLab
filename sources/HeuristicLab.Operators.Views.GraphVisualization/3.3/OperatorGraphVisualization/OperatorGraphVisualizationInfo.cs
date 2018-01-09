#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators.Views.GraphVisualization {
  [StorableClass]
  public sealed class OperatorGraphVisualizationInfo : GraphVisualizationInfo {
    [Storable]
    private BidirectionalLookup<IOperator, IOperatorShapeInfo> operatorShapeInfoMapping;
    private BidirectionalLookup<IOperator, IKeyedItemCollection<string, IParameter>> operatorParameterCollectionMapping;
    private Dictionary<IParameter, IOperator> parameterOperatorMapping;

    private OperatorGraphVisualizationInfo()
      : base() {
      this.operatorShapeInfoMapping = new BidirectionalLookup<IOperator, IOperatorShapeInfo>();
      this.operatorParameterCollectionMapping = new BidirectionalLookup<IOperator, IKeyedItemCollection<string, IParameter>>();
      this.parameterOperatorMapping = new Dictionary<IParameter, IOperator>();
    }

    [StorableConstructor]
    private OperatorGraphVisualizationInfo(bool deserializing)
      : base(deserializing) {
      this.operatorParameterCollectionMapping = new BidirectionalLookup<IOperator, IKeyedItemCollection<string, IParameter>>();
      this.parameterOperatorMapping = new Dictionary<IParameter, IOperator>();
    }
    private OperatorGraphVisualizationInfo(OperatorGraphVisualizationInfo original, Cloner cloner)
      : base(original, cloner) {
      operatorShapeInfoMapping = new BidirectionalLookup<IOperator, IOperatorShapeInfo>();
      operatorParameterCollectionMapping = new BidirectionalLookup<IOperator, IKeyedItemCollection<string, IParameter>>();
      parameterOperatorMapping = new Dictionary<IParameter, IOperator>();

      operatorGraph = cloner.Clone(original.operatorGraph);
      RegisterOperatorGraphEvents();
      oldInitialShape = cloner.Clone(original.oldInitialShape);
      oldInitialShapeColor = original.oldInitialShapeColor;

      foreach (KeyValuePair<IOperator, IOperatorShapeInfo> pair in original.operatorShapeInfoMapping.FirstEnumerable) {
        IOperator op = cloner.Clone(pair.Key);
        IOperatorShapeInfo shapeInfo = cloner.Clone(pair.Value);
        RegisterOperatorEvents(op);
        operatorParameterCollectionMapping.Add(op, op.Parameters);
        operatorShapeInfoMapping.Add(op, shapeInfo);
      }

      foreach (IOperator oper in operatorShapeInfoMapping.FirstValues) {
        foreach (IParameter param in oper.Parameters) {
          parameterOperatorMapping.Add(param, oper);
          IValueParameter opParam = param as IValueParameter;
          if (opParam != null && typeof(IOperator).IsAssignableFrom(param.DataType))
            RegisterOperatorParameterEvents(opParam);
          else
            RegisterParameterEvents(param);
        }
      }
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new OperatorGraphVisualizationInfo(this, cloner);
    }

    public OperatorGraphVisualizationInfo(OperatorGraph operatorGraph)
      : this() {
      this.operatorGraph = operatorGraph;
      this.RegisterOperatorGraphEvents();

      foreach (IOperator op in operatorGraph.Operators)
        if (!this.operatorShapeInfoMapping.ContainsFirst(op))  //could be added by referencing parameters
          this.AddOperator(op);

      this.UpdateInitialShape();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      this.operatorGraph.DeserializationFinished += new EventHandler(operatorGraph_DeserializationFinished);
      if (oldInitialShapeColor.IsEmpty) oldInitialShapeColor = Color.LightBlue;

      IOperator op;
      IOperatorShapeInfo shapeInfo;
      foreach (KeyValuePair<IOperator, IOperatorShapeInfo> pair in this.operatorShapeInfoMapping.FirstEnumerable) {
        op = pair.Key;
        shapeInfo = pair.Value;
        shapeInfo.Icon = new Bitmap(op.ItemImage);
        this.RegisterOperatorEvents(op);
        this.operatorParameterCollectionMapping.Add(op, op.Parameters);
      }

      foreach (IOperator oper in this.operatorShapeInfoMapping.FirstValues) {
        foreach (IParameter param in oper.Parameters) {
          IValueParameter opParam = param as IValueParameter;
          this.parameterOperatorMapping.Add(param, oper);
          if (opParam != null && typeof(IOperator).IsAssignableFrom(param.DataType))
            this.RegisterOperatorParameterEvents(opParam);
          else
            this.RegisterParameterEvents(param);
        }
      }

      foreach (IOperatorShapeInfo shapeInfo2 in this.operatorShapeInfoMapping.SecondValues)
        if (string.IsNullOrEmpty(shapeInfo2.TypeName)) shapeInfo2.TypeName = this.operatorShapeInfoMapping.GetBySecond(shapeInfo2).GetType().GetPrettyName();
    }

    private void operatorGraph_DeserializationFinished(object sender, EventArgs e) {
      this.RegisterOperatorGraphEvents();
      this.operatorGraph.DeserializationFinished -= new EventHandler(operatorGraph_DeserializationFinished);
    }

    public IOperator GetOperatorForShapeInfo(IOperatorShapeInfo shapeInfo) {
      return this.operatorShapeInfoMapping.GetBySecond(shapeInfo);
    }

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
      this.OnInitialShapeChanged();
    }

    private IShapeInfo oldInitialShape;
    [Storable]
    private Color oldInitialShapeColor;
    public override IShapeInfo InitialShape {
      get {
        IOperator op = this.operatorGraph.InitialOperator;
        if (op == null) return null;
        return this.operatorShapeInfoMapping.GetByFirst(op);
      }
      set {
        if (value == null)
          this.OperatorGraph.InitialOperator = null;
        else {
          this.oldInitialShape = InitialShape;
          IOperatorShapeInfo shapeInfo = (IOperatorShapeInfo)value;
          this.OperatorGraph.InitialOperator = this.operatorShapeInfoMapping.GetBySecond(shapeInfo);
        }
      }
    }

    [Storable]
    private OperatorGraph operatorGraph;
    public OperatorGraph OperatorGraph {
      get { return this.operatorGraph; }
    }

    private void RegisterOperatorGraphEvents() {
      this.operatorGraph.InitialOperatorChanged += new EventHandler(operatorGraph_InitialOperatorChanged);
      this.operatorGraph.Operators.ItemsAdded += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IOperator>(Operators_ItemsAdded);
      this.operatorGraph.Operators.ItemsRemoved += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IOperator>(Operators_ItemsRemoved);
      this.operatorGraph.Operators.CollectionReset += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IOperator>(Operators_CollectionReset);
    }

    private void DeregisterOperatorGraphEvents() {
      this.operatorGraph.InitialOperatorChanged -= new EventHandler(operatorGraph_InitialOperatorChanged);
      this.operatorGraph.Operators.ItemsAdded -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IOperator>(Operators_ItemsAdded);
      this.operatorGraph.Operators.ItemsRemoved -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IOperator>(Operators_ItemsRemoved);
      this.operatorGraph.Operators.CollectionReset -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IOperator>(Operators_CollectionReset);
    }

    #region methods to manipulate operatorgraph by the shape info
    public void AddShapeInfo(IOperator op, IOperatorShapeInfo shapeInfo) {
      this.RegisterOperatorEvents(op);
      this.operatorParameterCollectionMapping.Add(op, op.Parameters);
      this.operatorShapeInfoMapping.Add(op, shapeInfo);
      this.shapeInfos.Add(shapeInfo);

      foreach (IParameter param in op.Parameters)
        this.AddParameter(op, param);

      this.operatorGraph.Operators.Add(op);
    }

    public override void RemoveShapeInfo(IShapeInfo shapeInfo) {
      IOperatorShapeInfo opShapeInfo = (IOperatorShapeInfo)shapeInfo;
      if (this.operatorShapeInfoMapping.ContainsSecond(opShapeInfo)) {
        IOperator op = this.operatorShapeInfoMapping.GetBySecond(opShapeInfo);
        this.operatorGraph.Operators.Remove(op);
      }
    }

    public override void RemoveConnectionInfo(IConnectionInfo connectionInfo) {
      IOperatorShapeInfo shapeInfo = (IOperatorShapeInfo)connectionInfo.From;
      IOperator op = this.operatorShapeInfoMapping.GetBySecond(shapeInfo);
      IValueParameter param = (IValueParameter)op.Parameters[connectionInfo.ConnectorFrom];
      param.Value = null;
    }

    public override void AddConnectionInfo(IConnectionInfo connectionInfo) {
      IOperatorShapeInfo shapeInfo = (IOperatorShapeInfo)connectionInfo.From;
      IOperator op = this.operatorShapeInfoMapping.GetBySecond(shapeInfo);
      IOperatorShapeInfo shapeInfoTo = (IOperatorShapeInfo)connectionInfo.To;
      IOperator opTo = this.operatorShapeInfoMapping.GetBySecond(shapeInfoTo);
      IValueParameter param = (IValueParameter)op.Parameters.Where(p => p.Name == connectionInfo.ConnectorFrom).SingleOrDefault();
      if (param != null)
        param.Value = opTo;
    }
    #endregion

    #region operator events
    private void AddOperator(IOperator op) {
      if (!this.operatorShapeInfoMapping.ContainsFirst(op)) {
        this.RegisterOperatorEvents(op);
        IOperatorShapeInfo shapeInfo = OperatorShapeInfoFactory.CreateOperatorShapeInfo(op);
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

    private void OperatorItemImageChanged(object sender, EventArgs e) {
      IOperator op = (IOperator)sender;
      IOperatorShapeInfo operatorShapeInfo = this.operatorShapeInfoMapping.GetByFirst(op);
      operatorShapeInfo.Icon = new Bitmap(op.ItemImage);
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
      op.ItemImageChanged += new EventHandler(OperatorItemImageChanged);
      op.BreakpointChanged += new EventHandler(OperatorBreakpointChanged);
    }

    private void DeregisterOperatorEvents(IOperator op) {
      op.Parameters.ItemsAdded -= new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsAdded);
      op.Parameters.ItemsRemoved -= new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsRemoved);
      op.Parameters.ItemsReplaced -= new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsReplaced);
      op.Parameters.CollectionReset -= new CollectionItemsChangedEventHandler<IParameter>(Parameters_CollectionReset);
      op.NameChanged -= new EventHandler(OperatorNameChanged);
      op.ItemImageChanged -= new EventHandler(OperatorItemImageChanged);
      op.BreakpointChanged -= new EventHandler(OperatorBreakpointChanged);
    }
    #endregion

    #region parameter events
    private void AddParameter(IOperator op, IParameter param) {
      this.parameterOperatorMapping.Add(param, op);
      IValueParameter opParam = param as IValueParameter;
      if (opParam != null && typeof(IOperator).IsAssignableFrom(param.DataType)) {
        this.RegisterOperatorParameterEvents(opParam);
        IOperatorShapeInfo shapeInfoFrom = this.operatorShapeInfoMapping.GetByFirst(op);
        shapeInfoFrom.AddConnector(param.Name);

        if (opParam.Value != null) {
          if (!this.operatorShapeInfoMapping.ContainsFirst((IOperator)opParam.Value))
            this.AddOperator((IOperator)opParam.Value);
          IOperatorShapeInfo shapeInfoTo = this.operatorShapeInfoMapping.GetByFirst((IOperator)opParam.Value);
          this.connectionInfos.Add(new ConnectionInfo(shapeInfoFrom, param.Name, shapeInfoTo, OperatorShapeInfoFactory.PredecessorConnector));
        }
      } else
        this.RegisterParameterEvents(param);
    }

    private void RemoveParameter(IOperator op, IParameter param) {
      IValueParameter opParam = param as IValueParameter;
      if (opParam != null && typeof(IOperator).IsAssignableFrom(param.DataType)) {
        this.DeregisterOperatorParameterEvents(opParam);
        IOperatorShapeInfo shapeInfo = this.operatorShapeInfoMapping.GetByFirst(op);
        this.connectionInfos.RemoveWhere(c => c.From == shapeInfo && c.ConnectorFrom == param.Name);
        this.connectionInfos.RemoveWhere(c => c.To == shapeInfo && c.ConnectorTo == param.Name);
        shapeInfo.RemoveConnector(param.Name);
      } else
        this.DeregisterParameterEvents(param);

      this.parameterOperatorMapping.Remove(param);
    }

    private void opParam_ValueChanged(object sender, EventArgs e) {
      IValueParameter opParam = (IValueParameter)sender;
      if (this.parameterOperatorMapping.ContainsKey(opParam)) {
        IOperator op = this.parameterOperatorMapping[opParam];
        IOperatorShapeInfo shapeInfoFrom = this.operatorShapeInfoMapping.GetByFirst(op);
        KeyValuePair<IOperatorShapeInfo, string> connectionFrom = new KeyValuePair<IOperatorShapeInfo, string>(shapeInfoFrom, opParam.Name);

        this.connectionInfos.RemoveWhere(c => c.From == shapeInfoFrom && c.ConnectorFrom == opParam.Name);
        if (opParam.Value != null) {
          if (!this.operatorShapeInfoMapping.ContainsFirst((IOperator)opParam.Value))
            this.AddOperator((IOperator)opParam.Value);
          IOperatorShapeInfo shapeInfoTo = this.operatorShapeInfoMapping.GetByFirst((IOperator)opParam.Value);
          base.AddConnectionInfo(new ConnectionInfo(shapeInfoFrom, opParam.Name, shapeInfoTo, OperatorShapeInfoFactory.PredecessorConnector));
        }
      }
    }

    private void Parameters_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      IKeyedItemCollection<string, IParameter> parameterCollection = sender as IKeyedItemCollection<string, IParameter>;
      IOperator op = this.operatorParameterCollectionMapping.GetBySecond(parameterCollection);
      foreach (IParameter param in e.Items)
        AddParameter(op, param);
      this.UpdateParameterLabels(op);
    }
    private void Parameters_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      IKeyedItemCollection<string, IParameter> parameterCollection = sender as IKeyedItemCollection<string, IParameter>;
      IOperator op = this.operatorParameterCollectionMapping.GetBySecond(parameterCollection);
      foreach (IParameter param in e.Items)
        RemoveParameter(op, param);
      this.UpdateParameterLabels(op);
    }
    private void Parameters_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      IKeyedItemCollection<string, IParameter> parameterCollection = sender as IKeyedItemCollection<string, IParameter>;
      IOperator op = this.operatorParameterCollectionMapping.GetBySecond(parameterCollection);
      foreach (IParameter param in e.OldItems)
        RemoveParameter(op, param);
      foreach (IParameter param in e.Items)
        AddParameter(op, param);
      this.UpdateParameterLabels(op);
    }
    private void Parameters_CollectionReset(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      IKeyedItemCollection<string, IParameter> parameterCollection = sender as IKeyedItemCollection<string, IParameter>;
      IOperator op = this.operatorParameterCollectionMapping.GetBySecond(parameterCollection);
      foreach (IParameter param in e.OldItems)
        RemoveParameter(op, param);
      foreach (IParameter param in e.Items)
        AddParameter(op, param);
      this.UpdateParameterLabels(op);
    }

    private void RegisterOperatorParameterEvents(IValueParameter opParam) {
      opParam.ValueChanged += new EventHandler(opParam_ValueChanged);
    }
    private void DeregisterOperatorParameterEvents(IValueParameter opParam) {
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
      IEnumerable<IParameter> parameters = op.Parameters.Where(p => !(p is IValueParameter && typeof(IOperator).IsAssignableFrom(p.DataType)));
      IOperatorShapeInfo operatorShapeInfo = this.operatorShapeInfoMapping.GetByFirst(op);
      if (parameters.Count() > 0)
        operatorShapeInfo.UpdateLabels(parameters.Select(p => p.ToString()));
      else
        operatorShapeInfo.UpdateLabels(new List<string>());
    }
    #endregion
  }
}