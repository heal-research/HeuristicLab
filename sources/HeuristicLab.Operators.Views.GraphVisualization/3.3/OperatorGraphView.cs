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

namespace HeuristicLab.Operators.Views.GraphVisualization {
  [Content(typeof(OperatorGraph), false)]
  public partial class OperatorGraphView : ContentView {
    private BidirectionalLookup<IShapeInfo, IShape> shapeMapping;
    private BidirectionalLookup<IConnectionInfo, IConnection> connectionMapping;

    private bool causedUpdateOfShapeInfo;
    public OperatorGraphView() {
      InitializeComponent();
      this.causedUpdateOfShapeInfo = false;
      Caption = "Operator Graph";
      this.shapeMapping = new BidirectionalLookup<IShapeInfo, IShape>();
      this.connectionMapping = new BidirectionalLookup<IConnectionInfo, IConnection>();
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
      foreach (IShapeInfo shapeInfo in this.shapeMapping.FirstValues)
        this.RemoveShapeInfo(shapeInfo);
      this.shapeMapping.Clear();

      foreach (IShapeInfo shapeInfo in this.VisualizationInfo.ShapeInfos)
        this.AddShapeInfo(shapeInfo);
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      this.VisualizationInfo.ObserveableShapeInfos.ItemsAdded += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IShapeInfo>(ShapeInfos_ItemsAdded);
      this.VisualizationInfo.ObserveableShapeInfos.ItemsRemoved += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IShapeInfo>(ShapeInfos_ItemsRemoved);
      this.VisualizationInfo.ObserveableShapeInfos.CollectionReset += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IShapeInfo>(ShapeInfos_CollectionReset);
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      this.VisualizationInfo.ObserveableShapeInfos.ItemsAdded -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IShapeInfo>(ShapeInfos_ItemsAdded);
      this.VisualizationInfo.ObserveableShapeInfos.ItemsRemoved -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IShapeInfo>(ShapeInfos_ItemsRemoved);
      this.VisualizationInfo.ObserveableShapeInfos.CollectionReset -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IShapeInfo>(ShapeInfos_CollectionReset);
    }


    private void ShapeInfos_CollectionReset(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IShapeInfo> e) {
      foreach (IShapeInfo shapeInfo in e.OldItems)
        this.RemoveShapeInfo(shapeInfo);
      foreach (IShapeInfo shapeInfo in e.Items)
        this.AddShapeInfo(shapeInfo);
      this.graphVisualization.Invalidate();
    }

    private void ShapeInfos_ItemsAdded(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IShapeInfo> e) {
      foreach (IShapeInfo shapeInfo in e.Items)
        this.AddShapeInfo(shapeInfo);
      this.graphVisualization.Invalidate();
    }

    private void ShapeInfos_ItemsRemoved(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IShapeInfo> e) {
      foreach (IShapeInfo shapeInfo in e.Items)
        this.RemoveShapeInfo(shapeInfo);
      this.graphVisualization.Invalidate();
    }

    private void AddShapeInfo(IShapeInfo shapeInfo) {
      IShape shape = shapeInfo.CreateShape();
      this.graphVisualization.Controller.Model.AddShape(shape);
      this.shapeMapping.Add(shapeInfo, shape);

      shape.OnEntityChange += new EventHandler<EntityEventArgs>(shape_OnEntityChange);
      shapeInfo.Changed += new ChangedEventHandler(shapeInfo_Changed);
    }

    private void RemoveShapeInfo(IShapeInfo shapeInfo) {
      IShape shape = this.shapeMapping.GetByFirst(shapeInfo);
      shape.OnEntityChange -= new EventHandler<EntityEventArgs>(shape_OnEntityChange);
      shapeInfo.Changed -= new ChangedEventHandler(shapeInfo_Changed);

      this.shapeMapping.RemoveByFirst(shapeInfo);
      if (this.graphVisualization.Controller.Model.Shapes.Contains(shape))
        this.graphVisualization.Controller.Model.RemoveShape(shape);
    }

    private void shapeInfo_Changed(object sender, ChangedEventArgs e) {
      IShapeInfo shapeInfo = (IShapeInfo)sender;
      this.UpdateShape(shapeInfo);
    }

    private void shape_OnEntityChange(object sender, EntityEventArgs e) {
      this.causedUpdateOfShapeInfo = true;
      IShape shape = e.Entity as IShape;
      IShapeInfo shapeInfo = this.shapeMapping.GetBySecond(shape);

      shapeInfo.Location = shape.Location;
      this.graphVisualization.Invalidate();
      this.causedUpdateOfShapeInfo = false;
    }

    private void UpdateShape(IShapeInfo shapeInfo) {
      if (!this.causedUpdateOfShapeInfo) {
        IShape shape = this.shapeMapping.GetByFirst(shapeInfo);
        shape.Location = shapeInfo.Location;
      }
    }

    private void graphVisualization_OnEntityRemoved(object sender, EntityEventArgs e) {
      IShape shape = (IShape)e.Entity;
      if (this.shapeMapping.ContainsSecond(shape)) {
        IShapeInfo shapeInfo = this.shapeMapping.GetBySecond(shape);
        this.VisualizationInfo.RemoveShapeInfo(shapeInfo);
      }
    }




    //protected override void OnContentChanged() {
    //  base.OnContentChanged();
    //  this.ClearGraph();

    //  this.CreateGraph(null, new OperatorParameter(string.Empty, this.Content));
    //  foreach (IShape shape in this.operatorShapeMapping.Values)
    //    this.GraphModel.AddShape(shape);

    //  foreach (IConnection connection in this.connectionMapping.Values)
    //    this.GraphModel.AddConnection(connection);

    //  if (this.Content == null)
    //    this.graphVisualization.Controller.Model.LayoutRoot = null;
    //  else
    //    this.graphVisualization.Controller.Model.LayoutRoot = this.operatorShapeMapping[this.Content];
    //  this.RelayoutOperatorGraph();
    //}

    //private void opParam_ValueChanged(object sender, EventArgs e) {
    //  if (InvokeRequired)
    //    Invoke(new EventHandler(opParam_ValueChanged), sender, e);
    //  else {
    //    this.OnContentChanged();
    //  }
    //}

    //private void ClearGraph() {
    //  this.GraphModel.Clear();
    //  foreach (IValueParameter<IOperator> opParam in this.parameters)
    //    opParam.ValueChanged -= opParam_ValueChanged;

    //  this.operatorShapeMapping.Clear();
    //  this.parameters.Clear();
    //  this.connectionMapping.Clear();
    //}



    //private void CreateGraph(IOperator parent, IValueParameter<IOperator> op) {
    //  if (op == null || op.Value == null)
    //    return;

    //  IShape shape;
    //  if (!this.operatorShapeMapping.ContainsKey(op.Value)) {
    //    //shape = GraphVisualizationInfo.CreateShape(op.Value);
    //    //this.operatorShapeMapping[op.Value] = shape;

    //    foreach (IParameter param in op.Value.Parameters) {
    //      IValueParameter<IOperator> opParam = param as IValueParameter<IOperator>;
    //      if (opParam != null) {
    //        HandleOperatorParameter(opParam);
    //        this.CreateGraph(op.Value, opParam);
    //      }
    //    }
    //  }

    //  if (parent != null) {
    //    IShape from = this.operatorShapeMapping[parent];
    //    IShape to = this.operatorShapeMapping[op.Value];
    //    //IConnection connection = GraphVisualizationInfo.CreateConnection(from,to);
    //    //this.connectionMapping[new KeyValuePair<IOperator, IValueParameter<IOperator>>(parent, op)] = connection;
    //  }
    //}

    //private void DeleteGraph(IOperator parent, IValueParameter<IOperator> op) {

    //}

    //private void HandleOperatorParameter(IValueParameter<IOperator> opParam) {
    //  if (opParam == null)
    //    return;
    //  opParam.ValueChanged += new EventHandler(opParam_ValueChanged);
    //  parameters.Add(opParam);
    //}

    //private void Model_OnEntityRemoved(object sender, EntityEventArgs e) {
    //  IShape shape = e.Entity as IShape;
    //  if (shape != null) {
    //    IOperator op = operatorShapeMapping.Where(os => os.Value == shape).First().Key;
    //    if (op == this.Content)
    //      this.Content = null;
    //    else {
    //      //clear all connections to the removed operator
    //      IEnumerable<IValueParameter<IOperator>> parentOperator = this.connectionMapping.Where(cs => cs.Key.Value.Value == op).Select(x => x.Key.Value);
    //      foreach (IValueParameter<IOperator> opParam in parentOperator.ToArray())
    //        opParam.Value = null;

    //      //remove connections from graph view
    //      IEnumerable<IConnection> connections = this.connectionMapping.Where(cs => cs.Key.Value.Value == op).Select(x => x.Value);
    //      foreach (IConnection connection in connections)
    //        this.GraphModel.Remove(connection);

    //      this.graphVisualization.Invalidate();
    //    }


    //  }
    //}

    #region methods for toolbar items

    internal void RelayoutOperatorGraph() {
      //if (this.operatorShapeMapping.Count > 0 && this.connectionMapping.Count > 0) { //otherwise the layout does not work
      this.graphVisualization.Controller.RunActivity("Standard TreeLayout");
      this.graphVisualization.Invalidate();
      //}
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
        IShapeInfo shapeInfo = ShapeInfoFactory.CreateShapeInfo(op);
        Point controlCoordinates = this.PointToClient(new Point(e.X, e.Y));
        PointF viewCoordinates = this.graphVisualization.Controller.View.DeviceToView(controlCoordinates);
        shapeInfo.Location = new Point((int)viewCoordinates.X, (int)viewCoordinates.Y);
        this.VisualizationInfo.AddShapeInfo(op, shapeInfo);
      }
    }

    #endregion
  }
}
