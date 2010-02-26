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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using Netron.Diagramming.Core;
using HeuristicLab.Parameters;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Collections;

namespace HeuristicLab.Operators.Views.GraphVisualization {
  [View("Operator Graph Visualization")]
  [Content(typeof(OperatorGraph), false)]
  public partial class OperatorGraphView : ContentView {
    private BidirectionalLookup<IShapeInfo, IShape> shapeInfoShapeMapping;
    private BidirectionalLookup<IShapeInfo, INotifyObservableDictionaryItemsChanged<string, IShapeInfo>> shapeInfoConnectionsMapping;
    private BidirectionalLookup<IConnection, KeyValuePair<IConnector, IConnector>> connectionConnectorsMapping;

    private bool causedUpdateOfShapeInfo;
    public OperatorGraphView() {
      InitializeComponent();
      this.causedUpdateOfShapeInfo = false;
      Caption = "Operator Graph";
      this.shapeInfoShapeMapping = new BidirectionalLookup<IShapeInfo, IShape>();
      this.shapeInfoConnectionsMapping = new BidirectionalLookup<IShapeInfo, INotifyObservableDictionaryItemsChanged<string, IShapeInfo>>();
      this.connectionConnectorsMapping = new BidirectionalLookup<IConnection, KeyValuePair<IConnector, IConnector>>();
    }

    public OperatorGraphView(OperatorGraph content)
      : this() {
      this.Content = content;
    }

    public new OperatorGraph Content {
      get { return (OperatorGraph)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      if (this.VisualizationInfo == null)
        this.VisualizationInfo = new OperatorGraphVisualizationInfo(this.Content);
      this.UpdateVisualizationInfo();
    }

    private OperatorGraphVisualizationInfo VisualizationInfo {
      get { return Content.VisualizationInfo as OperatorGraphVisualizationInfo; }
      set { this.Content.VisualizationInfo = value; }
    }

    private void UpdateVisualizationInfo() {
      foreach (IShapeInfo shapeInfo in this.shapeInfoShapeMapping.FirstValues)
        this.RemoveShapeInfo(shapeInfo);

      this.shapeInfoShapeMapping.Clear();
      this.shapeInfoConnectionsMapping.Clear();
      this.connectionConnectorsMapping.Clear();

      foreach (IShapeInfo shapeInfo in this.VisualizationInfo.ShapeInfos)
        if (!this.shapeInfoShapeMapping.ContainsFirst(shapeInfo))
          this.AddShapeInfo(shapeInfo);

      foreach (KeyValuePair<KeyValuePair<IShapeInfo, string>, IShapeInfo> connection in this.VisualizationInfo.Connections)
        this.AddConnection(connection.Key.Key, connection.Key.Value, connection.Value);

      this.UpdateLayoutRoot();
    }

    private void UpdateLayoutRoot() {
      IShapeInfo shapeInfo = this.VisualizationInfo.InitialShape;
      if (shapeInfo != null)
        this.graphVisualization.Controller.Model.LayoutRoot = this.shapeInfoShapeMapping.GetByFirst(shapeInfo);
       else
        this.graphVisualization.Controller.Model.LayoutRoot = null;
    }

    private void VisualizationInfo_InitialShapeChanged(object sender, EventArgs e) {
      this.UpdateLayoutRoot();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      this.VisualizationInfo.InitialShapeChanged += new EventHandler(VisualizationInfo_InitialShapeChanged);
      this.VisualizationInfo.ObserveableShapeInfos.ItemsAdded += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IShapeInfo>(ShapeInfos_ItemsAdded);
      this.VisualizationInfo.ObserveableShapeInfos.ItemsRemoved += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IShapeInfo>(ShapeInfos_ItemsRemoved);
      this.VisualizationInfo.ObserveableShapeInfos.CollectionReset += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IShapeInfo>(ShapeInfos_CollectionReset);

      this.VisualizationInfo.ObservableConnections.ItemsAdded += new CollectionItemsChangedEventHandler<KeyValuePair<KeyValuePair<IShapeInfo, string>, IShapeInfo>>(Connections_ItemsAdded);
      this.VisualizationInfo.ObservableConnections.ItemsRemoved += new CollectionItemsChangedEventHandler<KeyValuePair<KeyValuePair<IShapeInfo, string>, IShapeInfo>>(Connections_ItemsRemoved);
      this.VisualizationInfo.ObservableConnections.ItemsReplaced += new CollectionItemsChangedEventHandler<KeyValuePair<KeyValuePair<IShapeInfo, string>, IShapeInfo>>(Connections_ItemsReplaced);
      this.VisualizationInfo.ObservableConnections.CollectionReset += new CollectionItemsChangedEventHandler<KeyValuePair<KeyValuePair<IShapeInfo, string>, IShapeInfo>>(Connections_CollectionReset);
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      this.VisualizationInfo.InitialShapeChanged -= new EventHandler(VisualizationInfo_InitialShapeChanged);
      this.VisualizationInfo.ObserveableShapeInfos.ItemsAdded -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IShapeInfo>(ShapeInfos_ItemsAdded);
      this.VisualizationInfo.ObserveableShapeInfos.ItemsRemoved -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IShapeInfo>(ShapeInfos_ItemsRemoved);
      this.VisualizationInfo.ObserveableShapeInfos.CollectionReset -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IShapeInfo>(ShapeInfos_CollectionReset);

      this.VisualizationInfo.ObservableConnections.ItemsAdded -= new CollectionItemsChangedEventHandler<KeyValuePair<KeyValuePair<IShapeInfo, string>, IShapeInfo>>(Connections_ItemsAdded);
      this.VisualizationInfo.ObservableConnections.ItemsRemoved -= new CollectionItemsChangedEventHandler<KeyValuePair<KeyValuePair<IShapeInfo, string>, IShapeInfo>>(Connections_ItemsRemoved);
      this.VisualizationInfo.ObservableConnections.ItemsReplaced -= new CollectionItemsChangedEventHandler<KeyValuePair<KeyValuePair<IShapeInfo, string>, IShapeInfo>>(Connections_ItemsReplaced);
      this.VisualizationInfo.ObservableConnections.CollectionReset -= new CollectionItemsChangedEventHandler<KeyValuePair<KeyValuePair<IShapeInfo, string>, IShapeInfo>>(Connections_CollectionReset);
    }

    private void ShapeInfos_CollectionReset(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IShapeInfo> e) {
      foreach (IShapeInfo shapeInfo in e.OldItems)
        this.RemoveShapeInfo(shapeInfo);
      foreach (IShapeInfo shapeInfo in e.Items)
        this.AddShapeInfo(shapeInfo);
    }

    private void ShapeInfos_ItemsAdded(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IShapeInfo> e) {
      foreach (IShapeInfo shapeInfo in e.Items)
        this.AddShapeInfo(shapeInfo);
    }

    private void ShapeInfos_ItemsRemoved(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IShapeInfo> e) {
      foreach (IShapeInfo shapeInfo in e.Items)
        this.RemoveShapeInfo(shapeInfo);
    }

    private void AddShapeInfo(IShapeInfo shapeInfo) {
      this.RegisterShapeInfoEvents(shapeInfo);

      IShape shape = shapeInfo.CreateShape();
      shape.OnEntityChange += new EventHandler<EntityEventArgs>(shape_OnEntityChange);
      this.shapeInfoShapeMapping.Add(shapeInfo, shape);

      this.graphVisualization.Controller.Model.AddShape(shape);
      this.graphVisualization.Invalidate();
    }

    private void RemoveShapeInfo(IShapeInfo shapeInfo) {
      this.DeregisterShapeInfoEvents(shapeInfo);
      IShape shape = this.shapeInfoShapeMapping.GetByFirst(shapeInfo);
      shape.OnEntityChange -= new EventHandler<EntityEventArgs>(shape_OnEntityChange);
      shapeInfo.Changed -= new ChangedEventHandler(shapeInfo_Changed);

      IConnection connection;
      foreach (IConnector connector in shape.Connectors) {
        connection = this.GetConnection(shapeInfo, connector.Name);
        this.RemoveConnection(connection);
      }

      this.shapeInfoShapeMapping.RemoveByFirst(shapeInfo);
      this.shapeInfoConnectionsMapping.RemoveByFirst(shapeInfo);

      if (this.graphVisualization.Controller.Model.Shapes.Contains(shape)) {
        this.graphVisualization.Controller.Model.RemoveShape(shape);
      }
      this.graphVisualization.Invalidate();
    }

    private void RegisterShapeInfoEvents(IShapeInfo shapeInfo) {
      shapeInfo.Changed += new ChangedEventHandler(shapeInfo_Changed);
    }

    private void DeregisterShapeInfoEvents(IShapeInfo shapeInfo) {
      shapeInfo.Changed -= new ChangedEventHandler(shapeInfo_Changed);
    }

    private void Connections_CollectionReset(object sender, CollectionItemsChangedEventArgs<KeyValuePair<KeyValuePair<IShapeInfo, string>, IShapeInfo>> e) {
      IConnection connection;
      foreach (KeyValuePair<KeyValuePair<IShapeInfo, string>, IShapeInfo> pair in e.Items) {
        connection = this.GetConnection(pair.Key.Key, pair.Key.Value);
        this.RemoveConnection(connection);
      }
      foreach (KeyValuePair<KeyValuePair<IShapeInfo, string>, IShapeInfo> pair in e.Items)
        this.AddConnection(pair.Key.Key, pair.Key.Value, pair.Value);
    }

    private void Connections_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<KeyValuePair<KeyValuePair<IShapeInfo, string>, IShapeInfo>> e) {
      IConnection connection;
      foreach (KeyValuePair<KeyValuePair<IShapeInfo, string>, IShapeInfo> pair in e.Items) {
        connection = this.GetConnection(pair.Key.Key, pair.Key.Value);
        this.RemoveConnection(connection);
      }
      foreach (KeyValuePair<KeyValuePair<IShapeInfo, string>, IShapeInfo> pair in e.Items)
        this.AddConnection(pair.Key.Key, pair.Key.Value, pair.Value);
    }

    private void Connections_ItemsAdded(object sender, CollectionItemsChangedEventArgs<KeyValuePair<KeyValuePair<IShapeInfo, string>, IShapeInfo>> e) {
      foreach (KeyValuePair<KeyValuePair<IShapeInfo, string>, IShapeInfo> pair in e.Items)
        this.AddConnection(pair.Key.Key, pair.Key.Value, pair.Value);
    }

    private void Connections_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<KeyValuePair<KeyValuePair<IShapeInfo, string>, IShapeInfo>> e) {
      IConnection connection;
      foreach (KeyValuePair<KeyValuePair<IShapeInfo, string>, IShapeInfo> pair in e.Items) {
        connection = this.GetConnection(pair.Key.Key, pair.Key.Value);
        this.RemoveConnection(connection);
      }
    }

    private void AddConnection(IShapeInfo shapeInfoFrom, string connectorName, IShapeInfo shapeInfoTo) {
      IShape shapeFrom = this.shapeInfoShapeMapping.GetByFirst(shapeInfoFrom);
      IShape shapeTo = this.shapeInfoShapeMapping.GetByFirst(shapeInfoTo);

      IConnector connectorFrom = shapeFrom.Connectors.Where(c => c.Name == connectorName).First();
      IConnector connectorTo = shapeTo.Connectors.Where(c => c.Name == "Predecessor").FirstOrDefault();
      KeyValuePair<IConnector, IConnector> connectorPair = new KeyValuePair<IConnector, IConnector>(connectorFrom, connectorTo);
      if (!this.connectionConnectorsMapping.ContainsSecond(connectorPair)) {
        IConnection connection = Factory.CreateConnection(connectorFrom, connectorTo);
        this.connectionConnectorsMapping.Add(connection, connectorPair);
        this.graphVisualization.Controller.Model.AddConnection(connection);
        this.graphVisualization.Invalidate();
      }
    }

    private IConnection GetConnection(IShapeInfo shapeInfoFrom, string connectorName) {
      IShape shape = this.shapeInfoShapeMapping.GetByFirst(shapeInfoFrom);
      IConnector connector = shape.Connectors.Where(c => c.Name == connectorName).First();

      if (!this.connectionConnectorsMapping.SecondValues.Any(p => p.Key == connector))
        return null;

      KeyValuePair<IConnector, IConnector> connectorPair = this.connectionConnectorsMapping.SecondValues.Where(p => p.Key == connector).FirstOrDefault();
      return this.connectionConnectorsMapping.GetBySecond(connectorPair);
    }

    private void ChangeConnection(IShapeInfo shapeInfoFrom, string connectorName, IShapeInfo shapeInfoTo) {
      IConnection connection = this.GetConnection(shapeInfoFrom, connectorName);
      IShape shapeTo = this.shapeInfoShapeMapping.GetByFirst(shapeInfoFrom);
      IConnector connectorTo = shapeTo.Connectors.Where(c => c.Name == "Predecessor").First();

      connection.To.DetachFromParent();
      connection.To.AttachTo(connectorTo);
      this.graphVisualization.Invalidate();
    }

    private void RemoveConnection(IConnection connection) {
      if (connection == null)
        return;

      if (connection.From.AttachedTo != null)
        connection.From.DetachFromParent();
      if (connection.To.AttachedTo != null)
        connection.To.DetachFromParent();

      if (this.connectionConnectorsMapping.ContainsFirst(connection))
        this.connectionConnectorsMapping.RemoveByFirst(connection);
      if (this.graphVisualization.Controller.Model.Connections.Contains(connection))
        this.graphVisualization.Controller.Model.Remove(connection);
      this.graphVisualization.Invalidate();
    }


    private void shapeInfo_Changed(object sender, ChangedEventArgs e) {
      if (this.causedUpdateOfShapeInfo)
        return;

      IShapeInfo shapeInfo = (IShapeInfo)sender;
      IShape shape = this.shapeInfoShapeMapping.GetByFirst(shapeInfo);
      shapeInfo.UpdateShape(shape);
      shape.Invalidate();
    }


    private void shape_OnEntityChange(object sender, EntityEventArgs e) {
      this.causedUpdateOfShapeInfo = true;
      IShape shape = e.Entity as IShape;
      IShapeInfo shapeInfo = this.shapeInfoShapeMapping.GetBySecond(shape);

      shapeInfo.Location = shape.Location;
      shapeInfo.Size = new Size(shape.Width, shape.Height);

      this.graphVisualization.Invalidate();
      this.causedUpdateOfShapeInfo = false;
    }

    private void graphVisualization_OnEntityAdded(object sender, EntityEventArgs e) {
      IConnection connection = e.Entity as IConnection;
      if (connection != null && !this.connectionConnectorsMapping.ContainsFirst(connection)) {
        IConnector connectorFrom = connection.From.AttachedTo;
        IConnector connectorTo = connection.To.AttachedTo;
        this.RemoveConnection(connection); //is added again by the model events

        if (connectorFrom != null && connectorTo != null) {
          IShape shapeFrom = (IShape)connectorFrom.Parent;
          IShape shapeTo = (IShape)connectorTo.Parent;
          IShapeInfo shapeInfoFrom = this.shapeInfoShapeMapping.GetBySecond(shapeFrom);
          IShapeInfo shapeInfoTo = this.shapeInfoShapeMapping.GetBySecond(shapeTo);
          string connectorName = connectorFrom.Name;

          this.VisualizationInfo.AddConnection(shapeInfoFrom, connectorName, shapeInfoTo);
        }
      }
    }

    private void graphVisualization_OnEntityRemoved(object sender, EntityEventArgs e) {
      IShape shape = e.Entity as IShape;
      if (shape != null && this.shapeInfoShapeMapping.ContainsSecond(shape)) {
        IShapeInfo shapeInfo = this.shapeInfoShapeMapping.GetBySecond(shape);
        this.VisualizationInfo.RemoveShapeInfo(shapeInfo);
      }

      IConnection connection = e.Entity as IConnection;
      if (connection != null && this.connectionConnectorsMapping.ContainsFirst(connection)) {
        IShape parentShape = (IShape)connection.From.AttachedTo.Parent;
        IShapeInfo shapeInfo = this.shapeInfoShapeMapping.GetBySecond(parentShape);
        string connectorName = connection.From.AttachedTo.Name;

        this.VisualizationInfo.RemoveConnection(shapeInfo, connectorName);
      }
    }

    #region methods for toolbar items

    internal void RelayoutOperatorGraph() {
      if (this.shapeInfoShapeMapping.Count > 0 
        && this.connectionConnectorsMapping.Count > 0 
        && this.VisualizationInfo.InitialShape != null) { //otherwise the layout does not work
        string layoutName = "Standard TreeLayout";
        this.graphVisualization.Controller.RunActivity(layoutName);
        this.graphVisualization.Invalidate();
      }
    }

    internal void ActivateConnectionTool() {
      this.graphVisualization.Controller.ActivateTool(ControllerBase.ConnectionToolName);
    }

    internal void ActivateZoomAreaTool() {
      this.graphVisualization.Controller.ActivateTool(ControllerBase.ZoomAreaToolName);
    }

    internal void ActivateZoomInTool() {
      this.graphVisualization.Controller.ActivateTool(ControllerBase.ZoomInToolName);
    }

    internal void ActivateZoomOutTool() {
      this.graphVisualization.Controller.ActivateTool(ControllerBase.ZoomOutToolName);
    }

    internal void ActivatePanTool() {
      this.graphVisualization.Controller.ActivateTool(ControllerBase.PanToolName);
    }

    internal void ActivateSelectTool() {
      this.graphVisualization.Controller.ActivateTool(ControllerBase.SelectionToolName);
    }

    #endregion

    #region drag and drop
    private void OperatorGraphView_DragEnter(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      Type type = e.Data.GetData("Type") as Type;
      if ((type != null) && (typeof(IOperator).IsAssignableFrom(type))) {
        e.Effect = DragDropEffects.Copy;
      }
    }

    private void OperatorGraphView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        IOperator op = e.Data.GetData("Value") as IOperator;
        IShapeInfo shapeInfo = Factory.CreateShapeInfo(op);
        Point controlCoordinates = this.PointToClient(new Point(e.X, e.Y));
        PointF viewCoordinates = this.graphVisualization.Controller.View.DeviceToView(controlCoordinates);
        shapeInfo.Location = new Point((int)viewCoordinates.X, (int)viewCoordinates.Y);
        this.VisualizationInfo.AddShapeInfo(op, shapeInfo);
      }
    }

    #endregion
  }
}
