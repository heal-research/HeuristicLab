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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using Netron.Diagramming.Core;

namespace HeuristicLab.Operators.Views.GraphVisualization.Views {
  [View("GraphVisualizationInfo View")]
  [Content(typeof(IGraphVisualizationInfo), true)]
  public partial class GraphVisualizationInfoView : AsynchronousContentView {
    private BidirectionalDictionary<IShapeInfo, IShape> shapeInfoShapeMapping;
    private BidirectionalDictionary<IConnectionInfo, IConnection> connectionInfoConnectionMapping;
    private LinePenStyle connectionPenStyle;

    public GraphVisualizationInfoView() {
      InitializeComponent();
      this.shapeInfoShapeMapping = new BidirectionalDictionary<IShapeInfo, IShape>();
      this.connectionInfoConnectionMapping = new BidirectionalDictionary<IConnectionInfo, IConnection>();
      this.connectionPenStyle = new LinePenStyle();
      this.connectionPenStyle.EndCap = LineCap.ArrowAnchor;

      PasteTool pasteTool = (PasteTool)this.Controller.Tools.Where(t => t.Name == ControllerBase.PasteToolName).FirstOrDefault();
      CopyTool copyTool = (CopyTool)this.Controller.Tools.Where(t => t.Name == ControllerBase.CopyToolName).FirstOrDefault();
      HeuristicLab.Netron.Controller controller = this.Controller as HeuristicLab.Netron.Controller;
      if (controller != null) {
        if (pasteTool != null) controller.RemoveTool(pasteTool);
        if (copyTool != null) controller.RemoveTool(copyTool);
      }
    }

    public IController Controller {
      get { return this.graphVisualization.Controller; }
    }

    public new IGraphVisualizationInfo Content {
      get { return (IGraphVisualizationInfo)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      this.UpdateContent();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      DeleteTool deleteTool = (DeleteTool)this.Controller.Tools.Where(t => t.Name == ControllerBase.DeleteToolName).FirstOrDefault();
      HeuristicLab.Netron.Controller controller = this.Controller as HeuristicLab.Netron.Controller;
      if (Content == null && deleteTool != null && controller != null)
        controller.RemoveTool(deleteTool);
      else {
        if ((ReadOnly || Locked) && deleteTool != null && controller != null)
          controller.RemoveTool(deleteTool);
        else if ((!ReadOnly && !Locked) && deleteTool == null)
          this.Controller.AddTool(new DeleteTool(ControllerBase.DeleteToolName));
      }
    }

    private void UpdateContent() {
      foreach (IConnectionInfo connectionInfo in this.connectionInfoConnectionMapping.FirstKeys.ToList())
        this.RemoveConnectionInfo(connectionInfo);
      this.connectionInfoConnectionMapping.Clear();
      foreach (IShapeInfo shapeInfo in this.shapeInfoShapeMapping.FirstKeys.ToList())
        this.RemoveShapeInfo(shapeInfo);
      this.shapeInfoShapeMapping.Clear();

      if (Content != null) {
        foreach (IShapeInfo shapeInfo in this.Content.ShapeInfos)
          this.AddShapeInfo(shapeInfo);
        foreach (IConnectionInfo connectionInfo in this.Content.ConnectionInfos)
          this.AddConnectionInfo(connectionInfo);
        this.UpdateLayoutRoot();
      }
    }
    private void UpdateLayoutRoot() {
      IShapeInfo shapeInfo = this.Content.InitialShape;
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
      this.Content.InitialShapeChanged += new EventHandler(VisualizationInfo_InitialShapeChanged);

      this.Content.ObserveableShapeInfos.ItemsAdded += new CollectionItemsChangedEventHandler<IShapeInfo>(ShapeInfos_ItemsAdded);
      this.Content.ObserveableShapeInfos.ItemsRemoved += new CollectionItemsChangedEventHandler<IShapeInfo>(ShapeInfos_ItemsRemoved);
      this.Content.ObserveableShapeInfos.CollectionReset += new CollectionItemsChangedEventHandler<IShapeInfo>(ShapeInfos_CollectionReset);

      this.Content.ObservableConnectionInfos.ItemsAdded += new CollectionItemsChangedEventHandler<IConnectionInfo>(ConnectionInfos_ItemsAdded);
      this.Content.ObservableConnectionInfos.ItemsRemoved += new CollectionItemsChangedEventHandler<IConnectionInfo>(ConnectionInfos_ItemsRemoved);
      this.Content.ObservableConnectionInfos.CollectionReset += new CollectionItemsChangedEventHandler<IConnectionInfo>(ConnectionInfos_CollectionReset);
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      this.Content.InitialShapeChanged -= new EventHandler(VisualizationInfo_InitialShapeChanged);

      this.Content.ObserveableShapeInfos.ItemsAdded -= new CollectionItemsChangedEventHandler<IShapeInfo>(ShapeInfos_ItemsAdded);
      this.Content.ObserveableShapeInfos.ItemsRemoved -= new CollectionItemsChangedEventHandler<IShapeInfo>(ShapeInfos_ItemsRemoved);
      this.Content.ObserveableShapeInfos.CollectionReset -= new CollectionItemsChangedEventHandler<IShapeInfo>(ShapeInfos_CollectionReset);

      this.Content.ObservableConnectionInfos.ItemsAdded -= new CollectionItemsChangedEventHandler<IConnectionInfo>(ConnectionInfos_ItemsAdded);
      this.Content.ObservableConnectionInfos.ItemsRemoved -= new CollectionItemsChangedEventHandler<IConnectionInfo>(ConnectionInfos_ItemsRemoved);
      this.Content.ObservableConnectionInfos.CollectionReset -= new CollectionItemsChangedEventHandler<IConnectionInfo>(ConnectionInfos_CollectionReset);
    }

    #region ShapeInfos
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
      this.RegisterShapeEvents(shape);
      this.shapeInfoShapeMapping.Add(shapeInfo, shape);

      this.graphVisualization.Controller.Model.AddShape(shape);
      this.graphVisualization.Invalidate();
    }
    private void RemoveShapeInfo(IShapeInfo shapeInfo) {
      this.DeregisterShapeInfoEvents(shapeInfo);
      IShape shape = this.shapeInfoShapeMapping.GetByFirst(shapeInfo);
      this.DeregisterShapeEvents(shape);
      this.shapeInfoShapeMapping.RemoveByFirst(shapeInfo);

      if (this.graphVisualization.Controller.Model.Shapes.Contains(shape)) {
        this.graphVisualization.Controller.Model.RemoveShape(shape);
        this.graphVisualization.Controller.Model.Selection.Clear();
        this.graphVisualization.Invalidate();
      }
    }

    private void RegisterShapeInfoEvents(IShapeInfo shapeInfo) {
      shapeInfo.Changed += new EventHandler(shapeInfo_Changed);
    }
    private void DeregisterShapeInfoEvents(IShapeInfo shapeInfo) {
      shapeInfo.Changed -= new EventHandler(shapeInfo_Changed);
    }

    private void shapeInfo_Changed(object sender, EventArgs e) {
      IShapeInfo shapeInfo = (IShapeInfo)sender;
      IShape shape = this.shapeInfoShapeMapping.GetByFirst(shapeInfo);
      this.DeregisterShapeEvents(shape);
      shapeInfo.UpdateShape(shape);
      shape.Invalidate();
      this.RegisterShapeEvents(shape);
    }
    #endregion

    #region ConnectionInfos
    private void ConnectionInfos_CollectionReset(object sender, CollectionItemsChangedEventArgs<IConnectionInfo> e) {
      foreach (IConnectionInfo connectionInfo in e.Items)
        this.RemoveConnectionInfo(connectionInfo);
      foreach (IConnectionInfo connectionInfo in e.Items)
        this.AddConnectionInfo(connectionInfo);
    }
    private void ConnectionInfos_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IConnectionInfo> e) {
      foreach (IConnectionInfo connectionInfo in e.Items)
        this.AddConnectionInfo(connectionInfo);
    }
    private void ConnectionInfos_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IConnectionInfo> e) {
      foreach (IConnectionInfo connectionInfo in e.Items)
        this.RemoveConnectionInfo(connectionInfo);
    }

    private void AddConnectionInfo(IConnectionInfo connectionInfo) {
      this.RegisterConnectionInfoEvents(connectionInfo);
      IShape shapeFrom = this.shapeInfoShapeMapping.GetByFirst(connectionInfo.From);
      IShape shapeTo = this.shapeInfoShapeMapping.GetByFirst(connectionInfo.To);

      IConnector connectorFrom = shapeFrom.Connectors.Where(c => c.Name == connectionInfo.ConnectorFrom).FirstOrDefault();
      IConnector connectorTo = shapeTo.Connectors.Where(c => c.Name == connectionInfo.ConnectorTo).FirstOrDefault();
      if (connectorFrom != null && connectorTo != null) {
        Connection connection = new Connection(connectorFrom.Point, connectorTo.Point);
        connection.From.AllowMove = false;
        connection.To.AllowMove = false;
        connectorFrom.AttachConnector(connection.From);
        connectorTo.AttachConnector(connection.To);
        connection.PenStyle = this.connectionPenStyle;
        this.connectionInfoConnectionMapping.Add(connectionInfo, connection);
        this.graphVisualization.Controller.Model.AddConnection(connection);
        this.graphVisualization.Invalidate();
      }
    }

    private void RemoveConnectionInfo(IConnectionInfo connectionInfo) {
      DeregisterConnectionInfoEvents(connectionInfo);
      IConnection connection = this.connectionInfoConnectionMapping.GetByFirst(connectionInfo);
      this.connectionInfoConnectionMapping.RemoveByFirst(connectionInfo);
      this.RemoveConnection(connection);

    }
    private void RemoveConnection(IConnection connection) {
      if (connection.From.AttachedTo != null)
        connection.From.DetachFromParent();
      if (connection.To.AttachedTo != null)
        connection.To.DetachFromParent();
      if (this.Controller.Model.Connections.Contains(connection)) {
        this.graphVisualization.Controller.Model.Remove(connection);
        this.graphVisualization.Invalidate();
      }
    }

    private void RegisterConnectionInfoEvents(IConnectionInfo connectionInfo) {
      connectionInfo.Changed += new EventHandler(connectionInfo_Changed);
    }
    private void DeregisterConnectionInfoEvents(IConnectionInfo connectionInfo) {
      connectionInfo.Changed -= new EventHandler(connectionInfo_Changed);
    }
    private void connectionInfo_Changed(object sender, EventArgs e) {
      IConnectionInfo connectionInfo = (IConnectionInfo)sender;
      IConnection connection = this.connectionInfoConnectionMapping.GetByFirst(connectionInfo);
      this.RemoveConnectionInfo(connectionInfo);
      this.AddConnectionInfo(connectionInfo);
    }
    #endregion

    #region netron events - shapes, graphvisualization
    private void RegisterShapeEvents(IShape shape) {
      shape.OnEntityChange += new EventHandler<EntityEventArgs>(shape_OnEntityChange);
      shape.OnMouseEnter += new EventHandler<EntityMouseEventArgs>(shape_OnMouseEnter);
      shape.OnMouseLeave += new EventHandler<EntityMouseEventArgs>(shape_OnMouseLeave);
    }

    private void DeregisterShapeEvents(IShape shape) {
      shape.OnEntityChange -= new EventHandler<EntityEventArgs>(shape_OnEntityChange);
      shape.OnMouseEnter -= new EventHandler<EntityMouseEventArgs>(shape_OnMouseEnter);
      shape.OnMouseLeave -= new EventHandler<EntityMouseEventArgs>(shape_OnMouseLeave);
    }

    private Cursor oldCursor;
    private void shape_OnMouseEnter(object sender, EntityMouseEventArgs e) {
      this.oldCursor = this.Cursor;
      this.Controller.View.CurrentCursor = CursorPalette.Move;
    }

    private void shape_OnMouseLeave(object sender, EntityMouseEventArgs e) {
      this.Controller.View.CurrentCursor = this.oldCursor;
      this.oldCursor = null;
    }

    private void shape_OnEntityChange(object sender, EntityEventArgs e) {
      IShape shape = e.Entity as IShape;
      IShapeInfo shapeInfo = this.shapeInfoShapeMapping.GetBySecond(shape);
      this.DeregisterShapeInfoEvents(shapeInfo);
      shapeInfo.UpdateShapeInfo(shape);
      this.RegisterShapeInfoEvents(shapeInfo);
      this.graphVisualization.Invalidate();
    }


    private void graphVisualization_OnEntityAdded(object sender, EntityEventArgs e) {
      IConnection connection = e.Entity as IConnection;
      if (connection != null && !this.connectionInfoConnectionMapping.ContainsSecond(connection)) {
        IConnector connectorFrom = connection.From.AttachedTo;
        IConnector connectorTo = connection.To.AttachedTo;
        this.RemoveConnection(connection); //is added again by the model events

        if (connectorFrom != null && connectorTo != null) {
          IShape shapeFrom = (IShape)connectorFrom.Parent;
          IShape shapeTo = (IShape)connectorTo.Parent;
          IShapeInfo shapeInfoFrom = this.shapeInfoShapeMapping.GetBySecond(shapeFrom);
          IShapeInfo shapeInfoTo = this.shapeInfoShapeMapping.GetBySecond(shapeTo);
          string connectorFromName = connectorFrom.Name;
          string connectorToName = connectorTo.Name;

          if (shapeInfoFrom != shapeInfoTo) //avoid self references
            this.Content.AddConnectionInfo(new ConnectionInfo(shapeInfoFrom, connectorFromName, shapeInfoTo, connectorToName));
        }
      }
    }

    private void graphVisualization_OnEntityRemoved(object sender, EntityEventArgs e) {
      IShape shape = e.Entity as IShape;
      if (shape != null && this.shapeInfoShapeMapping.ContainsSecond(shape)) {
        IShapeInfo shapeInfo = this.shapeInfoShapeMapping.GetBySecond(shape);
        this.Content.RemoveShapeInfo(shapeInfo);
      }

      IConnection connection = e.Entity as IConnection;
      if (connection != null && this.connectionInfoConnectionMapping.ContainsSecond(connection)) {
        IConnectionInfo connectionInfo = connectionInfoConnectionMapping.GetBySecond(connection);
        this.Content.RemoveConnectionInfo(connectionInfo);
      }
    }
    #endregion

    public void RelayoutGraph() {
      if (this.shapeInfoShapeMapping.Count > 0
        && this.connectionInfoConnectionMapping.Count > 0
        && this.Content.InitialShape != null) { //otherwise the layout does not work
        string layoutName = "Standard TreeLayout";
        this.graphVisualization.Controller.RunActivity(layoutName);
        this.graphVisualization.Invalidate();

        //fix to avoid negative shape positions after layouting
        Thread.Sleep(300);
        int minX = this.graphVisualization.Controller.Model.Shapes.Min(s => s.Location.X);
        int shiftX = minX < 0 ? Math.Abs(minX) + 50 : 0;
        int minY = this.graphVisualization.Controller.Model.Shapes.Min(s => s.Location.Y);
        int shiftY = minY < 0 ? Math.Abs(minY) + 50 : 0;
        if (minX < 0 || minY < 0) {
          foreach (IShape s in this.Controller.Model.Shapes)
            s.MoveBy(new Point(shiftX, shiftY));
        }
      }
    }
  }
}