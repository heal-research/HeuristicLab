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
  public sealed class OperatorGraphVisualizationInfo : Item {
    private BidirectionalLookup<IOperator, IShapeInfo> shapeInfoMapping;

    public OperatorGraphVisualizationInfo(OperatorGraph operatorGraph) {
      this.operatorGraph = operatorGraph;
      this.shapeInfoMapping = new BidirectionalLookup<IOperator, IShapeInfo>();
      this.shapeInfos = new ObservableSet<IShapeInfo>();

      foreach (IOperator op in operatorGraph.Operators)
        this.AddOperator(op);
      this.initialOperator = operatorGraph.InitialOperator;

      operatorGraph.Operators.ItemsAdded += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IOperator>(Operators_ItemsAdded);
      operatorGraph.Operators.ItemsRemoved += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IOperator>(Operators_ItemsRemoved);
      operatorGraph.Operators.CollectionReset += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IOperator>(Operators_CollectionReset);
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

    private IOperator initialOperator;
    public IOperator InitialOperator {
      get { return this.initialOperator; }
      set {
        if (this.initialOperator != value) {
          if (!this.shapeInfoMapping.ContainsFirst(value))
            throw new ArgumentException("Could not set initial operator in graph visualization information, because the operator " +
              value.ToString() + " is not contained in the operator set.");
          this.initialOperator = value;
          this.OnChanged();
        }
      }
    }

    internal void AddShapeInfo(IOperator op, IShapeInfo shapeInfo) {
      this.RegisterOperatorEvents(op);
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
      if (!shapeInfoMapping.ContainsFirst(op)) {
        this.RegisterOperatorEvents(op);

        IShapeInfo shapeInfo = ShapeInfoFactory.CreateShapeInfo(op);
        this.shapeInfoMapping.Add(op, shapeInfo);
        this.shapeInfos.Add(shapeInfo);
      }
    }

    private void RemoveOperator(IOperator op) {
      this.DeregisterOperatorEvents(op);

      IShapeInfo shapeInfo = this.shapeInfoMapping.GetByFirst(op);
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
    private void AddParameter(IParameter param) {
      IValueParameter<IOperator> opParam = param as IValueParameter<IOperator>;
      if (opParam != null) {
        RegisterOperatorParameterEvents(opParam);
        //TODO add connector
      }
    }
    private void RemoveParameter(IParameter param) {
      IValueParameter<IOperator> opParam = param as IValueParameter<IOperator>;
      if (opParam != null) {
        DeregisterOperatorParameterEvents(opParam);
        //TODO remove connector
      }
    }

    private void opParam_ValueChanged(object sender, EventArgs e) {
      IValueParameter<IOperator> opParam = (IValueParameter<IOperator>)sender;
      //TODO update connections
    }

    private void Parameters_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      foreach (IParameter param in e.Items)
        AddParameter(param);
    }
    private void Parameters_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      foreach (IParameter param in e.Items)
        RemoveParameter(param);
    }
    private void Parameters_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      foreach (IParameter param in e.OldItems)
        RemoveParameter(param);
      foreach (IParameter param in e.Items)
        AddParameter(param);
    }
    private void Parameters_CollectionReset(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      foreach (IParameter param in e.OldItems)
        RemoveParameter(param);
      foreach (IParameter param in e.Items)
        AddParameter(param);
    }

    private void RegisterOperatorParameterEvents(IValueParameter<IOperator> opParam) {
      opParam.ValueChanged += new EventHandler(opParam_ValueChanged);
    }
    private void DeregisterOperatorParameterEvents(IValueParameter<IOperator> opParam) {
      opParam.ValueChanged -= new EventHandler(opParam_ValueChanged);
    }
    #endregion








    private static IConnection CreateConnection(IShape from, IShape to) {
      IConnector parentConnector = from.Connectors.Where(c => c.Name == "Bottom connector").First();
      IConnector operatorConnector = to.Connectors.Where(c => c.Name == "Top connector").First();

      IConnection connection = new Connection(parentConnector.Point, operatorConnector.Point);

      parentConnector.AttachConnector(connection.From);
      operatorConnector.AttachConnector(connection.To);

      return connection;
    }
  }
}
