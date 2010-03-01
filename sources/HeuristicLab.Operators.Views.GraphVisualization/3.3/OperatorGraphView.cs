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
    public OperatorGraphView() {
      InitializeComponent();
      Caption = "Operator Graph Visualization";

      this.graphVisualizationInfoView.Controller.OnShowContextMenu += new EventHandler<EntityMenuEventArgs>(Controller_OnShowContextMenu);
      this.graphVisualizationInfoView.Controller.Model.Selection.OnNewSelection += new EventHandler(Controller_SelectionChanged);
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
      bool createdVisualizationInfo = false;
      if (this.VisualizationInfo == null) {
        this.VisualizationInfo = new GraphVisualizationInfo(this.Content);
        createdVisualizationInfo = true;
      }
      this.graphVisualizationInfoView.Content = this.VisualizationInfo;
      if (createdVisualizationInfo)
        this.graphVisualizationInfoView.RelayoutGraph();
    }

    private GraphVisualizationInfo VisualizationInfo {
      get { return Content.VisualizationInfo as GraphVisualizationInfo; }
      set { this.Content.VisualizationInfo = value; }
    }


    #region connector tooltips

    #endregion

    private void Controller_SelectionChanged(object sender, EventArgs e) {
      CollectionBase<IDiagramEntity> selectedObjects = this.graphVisualizationInfoView.Controller.Model.Selection.SelectedItems;
      this.detailsViewHost.ViewType = null;
      if (selectedObjects.Count == 1) {
        IShape shape = selectedObjects[0] as IShape;
        if (shape != null) {
          IShapeInfo shapeInfo = shape.Tag as ShapeInfo;
          IOperator op = this.VisualizationInfo.GetOperatorForShapeInfo(shapeInfo);
          this.detailsViewHost.Content = op;
          return;
        }
      }
      IConnector connector = this.graphVisualizationInfoView.Controller.Model.Selection.Connector;
      if (connector != null) {
        IShape shape = connector.Parent as IShape;
        string connectorName = connector.Name;
        if (shape == null) {
          shape = connector.AttachedTo.Parent as IShape; //connection connector selected
          connectorName = connector.AttachedTo.Name;
        }
        if (shape != null) {
          IShapeInfo shapeInfo = shape.Tag as ShapeInfo;
          IOperator op = this.VisualizationInfo.GetOperatorForShapeInfo(shapeInfo);
          if (connectorName != "Predecessor") {
            IParameter parameter = op.Parameters.Where(p => p.Name == connectorName).First();
            this.detailsViewHost.Content = parameter;
            return;
          }
        }
      }
      this.detailsViewHost.ViewType = null;
      this.detailsViewHost.Content = null;
    }

    #region context menu
    private void Controller_OnShowContextMenu(object sender, EntityMenuEventArgs e) {
      IShape shape = this.graphVisualizationInfoView.Controller.Model.GetShapeAt(e.MouseEventArgs.Location);
      if (shape != null) {
        IShapeInfo shapeInfo = shape.Tag as IShapeInfo;
        this.shapeContextMenu.Tag = shapeInfo;
        PointF worldPoint = this.graphVisualizationInfoView.Controller.View.WorldToView(e.MouseEventArgs.Location);
        this.shapeContextMenu.Show(this, Point.Round(worldPoint));
      }
    }

    private void shapeContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
      IShapeInfo shapeInfo = this.shapeContextMenu.Tag as ShapeInfo;
      if (shapeInfo != null) {
        IOperator op = this.VisualizationInfo.GetOperatorForShapeInfo(shapeInfo);
        this.initialToolStripMenuItem.Checked = this.Content.InitialOperator == op;
        this.breakPointToolStripMenuItem.Checked = op.Breakpoint;
      }
    }

    private void openViewToolStripMenuItem_Click(object sender, EventArgs e) {
      IShapeInfo shapeInfo = this.shapeContextMenu.Tag as ShapeInfo;
      if (shapeInfo != null) {
        IOperator op = this.VisualizationInfo.GetOperatorForShapeInfo(shapeInfo);
        MainFormManager.CreateDefaultView(op).Show();
      }
    }

    private void initialOperatorToolStripMenuItem_Click(object sender, EventArgs e) {
      IShapeInfo shapeInfo = this.shapeContextMenu.Tag as ShapeInfo;
      if (this.VisualizationInfo.InitialShape == shapeInfo)
        this.VisualizationInfo.InitialShape = null;
      else
        this.VisualizationInfo.InitialShape = shapeInfo;
    }

    private void breakPointToolStripMenuItem_Click(object sender, EventArgs e) {
      IShapeInfo shapeInfo = this.shapeContextMenu.Tag as ShapeInfo;
      if (shapeInfo != null) {
        IOperator op = this.VisualizationInfo.GetOperatorForShapeInfo(shapeInfo);
        op.Breakpoint = !op.Breakpoint;
      }
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
        Point mouse = new Point(MousePosition.X, MousePosition.Y);
        Point p = new Point(e.X, e.Y);
        Point screen = this.PointToScreen(new Point(0, 0));
        PointF worldPoint = this.graphVisualizationInfoView.Controller.View.WorldToView(new Point(mouse.X - screen.X, mouse.Y - screen.Y));

        shapeInfo.Location = Point.Round(worldPoint);
        this.VisualizationInfo.AddShapeInfo(op, shapeInfo);
      }
    }

    #endregion

    private void selectButton_Click(object sender, EventArgs e) {
      ITool tool = this.graphVisualizationInfoView.Controller.Tools.Where(t => t.Name == ControllerBase.SelectionToolName).First();
      tool.IsSuspended = false;
      tool = this.graphVisualizationInfoView.Controller.Tools.Where(t => t.Name == ControllerBase.PanToolName).First();
      this.graphVisualizationInfoView.Controller.DeactivateTool(tool);
    }

    private void panButton_Click(object sender, EventArgs e) {
      ITool tool = this.graphVisualizationInfoView.Controller.Tools.Where(t => t.Name == ControllerBase.SelectionToolName).First();
      tool.IsSuspended = true;
      this.graphVisualizationInfoView.Controller.ActivateTool(ControllerBase.PanToolName);
    }

    private void connectButton_Click(object sender, EventArgs e) {
      this.graphVisualizationInfoView.Controller.ActivateTool(ControllerBase.ConnectionToolName);
    }

    private void relayoutButton_Click(object sender, EventArgs e) {
      this.graphVisualizationInfoView.RelayoutGraph();
    }

    private void zoomAreaButton_Click(object sender, EventArgs e) {
      this.graphVisualizationInfoView.Controller.ActivateTool(ControllerBase.ZoomAreaToolName);
    }

    private void zoomInButton_Click(object sender, EventArgs e) {
      this.graphVisualizationInfoView.Controller.ActivateTool(ControllerBase.ZoomInToolName);
    }

    private void zoomOutButton_Click(object sender, EventArgs e) {
      this.graphVisualizationInfoView.Controller.ActivateTool(ControllerBase.ZoomOutToolName);
    }
  }
}
