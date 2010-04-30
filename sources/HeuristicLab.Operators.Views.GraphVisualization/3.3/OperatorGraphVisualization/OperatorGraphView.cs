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
  [View("OperatorGraph View (Chart)")]
  [Content(typeof(OperatorGraph), true)]
  public partial class OperatorGraphView : AsynchronousContentView {
    public OperatorGraphView() {
      InitializeComponent();
      Caption = "Operator Graph Visualization";

      this.graphVisualizationInfoView.Controller.OnShowContextMenu += new EventHandler<EntityMenuEventArgs>(Controller_OnShowContextMenu);
      this.graphVisualizationInfoView.Controller.Model.Selection.OnNewSelection += new EventHandler(Controller_SelectionChanged);
      this.graphVisualizationInfoView.Controller.OnMouseDown += new EventHandler<MouseEventArgs>(Controller_OnMouseDown);
      foreach (ITool tool in this.graphVisualizationInfoView.Controller.Tools) {
        tool.OnToolActivate += new EventHandler<ToolEventArgs>(tool_OnToolActivate);
        tool.OnToolDeactivate += new EventHandler<ToolEventArgs>(tool_OnToolDeactivate);
      }
    }

    public new OperatorGraph Content {
      get { return (OperatorGraph)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      bool createdVisualizationInfo = false;
      if (this.VisualizationInfo == null) {
        this.VisualizationInfo = new OperatorGraphVisualizationInfo(this.Content);
        createdVisualizationInfo = true;
      }
      this.graphVisualizationInfoView.Content = this.VisualizationInfo;
      if (createdVisualizationInfo)
        this.graphVisualizationInfoView.RelayoutGraph();

      this.SetEnabledStateOfControls();
    }

    protected override void OnReadOnlyChanged() {
      base.OnReadOnlyChanged();
      this.SetEnabledStateOfControls();
    }

    protected override void OnLockedChanged() {
      base.OnLockedChanged();
      this.SetEnabledStateOfControls();
    }

    private void SetEnabledStateOfControls() {
      if (Content == null) {
        selectButton.Enabled = false;
        panButton.Enabled = false;
        relayoutButton.Enabled = false;
        zoomToFitButton.Enabled = false;
        zoomInButton.Enabled = false;
        zoomOutButton.Enabled = false;
        screenshotButton.Enabled = false;
        detailsViewHost.Enabled = false;
        connectButton.Enabled = false;
      } else {
        selectButton.Enabled = true;
        panButton.Enabled = true;
        relayoutButton.Enabled = true;
        zoomToFitButton.Enabled = true;
        zoomInButton.Enabled = true;
        zoomOutButton.Enabled = true;
        screenshotButton.Enabled = true;
        detailsViewHost.Enabled = true;
        connectButton.Enabled = !ReadOnly && !Locked;
      }
    }

    private OperatorGraphVisualizationInfo VisualizationInfo {
      get { return Content.VisualizationInfo as OperatorGraphVisualizationInfo; }
      set { this.Content.VisualizationInfo = value; }
    }

    private void Controller_SelectionChanged(object sender, EventArgs e) {
      this.detailsViewHost.ViewType = null;
      this.detailsViewHost.Content = null;

      CollectionBase<IDiagramEntity> selectedObjects = this.graphVisualizationInfoView.Controller.Model.Selection.SelectedItems;
      if (selectedObjects.Count == 1) {
        IShape shape = selectedObjects[0] as IShape;
        if (shape != null) {
          IOperatorShapeInfo shapeInfo = shape.Tag as IOperatorShapeInfo;
          IOperator op = this.VisualizationInfo.GetOperatorForShapeInfo(shapeInfo);
          this.detailsViewHost.Content = op;
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
          IOperatorShapeInfo shapeInfo = shape.Tag as IOperatorShapeInfo;
          IOperator op = this.VisualizationInfo.GetOperatorForShapeInfo(shapeInfo);
          if (connectorName != "Predecessor") {
            IParameter parameter = op.Parameters.Where(p => p.Name == connectorName).First();
            this.detailsViewHost.ViewType = null;
            this.detailsViewHost.Content = parameter;
          }
        }
      }

    }

    private void Controller_OnMouseDown(object sender, MouseEventArgs e) {
      if (e.Clicks >= 2) {
        IShape shape = this.graphVisualizationInfoView.Controller.Model.GetShapeAt(e.Location);
        if (shape != null) {
          IOperatorShapeInfo shapeInfo = shape.Tag as IOperatorShapeInfo;
          if (shapeInfo != null) {
            IOperator op = this.VisualizationInfo.GetOperatorForShapeInfo(shapeInfo);
            IContentView view = MainFormManager.MainForm.ShowContent(op);
            if (view != null) {
              view.ReadOnly = this.ReadOnly;
              view.Locked = this.Locked;
            }
            HandledMouseEventArgs eventArgs = e as HandledMouseEventArgs;
            if (eventArgs != null)
              eventArgs.Handled = true;
          }
        }
      }
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
      IOperatorShapeInfo shapeInfo = this.shapeContextMenu.Tag as IOperatorShapeInfo;
      if (shapeInfo != null) {
        IOperator op = this.VisualizationInfo.GetOperatorForShapeInfo(shapeInfo);
        this.initialToolStripMenuItem.Checked = this.Content.InitialOperator == op;
        this.initialToolStripMenuItem.Enabled = !ReadOnly && !Locked;
        this.breakPointToolStripMenuItem.Checked = op.Breakpoint;
        this.breakPointToolStripMenuItem.Enabled = !ReadOnly && !Locked;
      }
    }

    private void openViewToolStripMenuItem_Click(object sender, EventArgs e) {
      IOperatorShapeInfo shapeInfo = this.shapeContextMenu.Tag as IOperatorShapeInfo;
      if (shapeInfo != null) {
        IOperator op = this.VisualizationInfo.GetOperatorForShapeInfo(shapeInfo);
        IContentView view = MainFormManager.MainForm.ShowContent(op);
        if (view != null) {
          view.ReadOnly = this.ReadOnly;
          view.Locked = this.Locked;
        }
      }
    }

    private void initialOperatorToolStripMenuItem_Click(object sender, EventArgs e) {
      if (!ReadOnly) {
        IOperatorShapeInfo shapeInfo = this.shapeContextMenu.Tag as IOperatorShapeInfo;
        if (this.VisualizationInfo.InitialShape == shapeInfo)
          this.VisualizationInfo.InitialShape = null;
        else
          this.VisualizationInfo.InitialShape = shapeInfo;
      }
    }

    private void breakPointToolStripMenuItem_Click(object sender, EventArgs e) {
      if (!ReadOnly) {
        IOperatorShapeInfo shapeInfo = this.shapeContextMenu.Tag as IOperatorShapeInfo;
        if (shapeInfo != null) {
          IOperator op = this.VisualizationInfo.GetOperatorForShapeInfo(shapeInfo);
          op.Breakpoint = !op.Breakpoint;
        }
      }
    }
    #endregion

    #region drag and drop
    private void OperatorGraphView_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      Type type = e.Data.GetData("Type") as Type;
      if (!ReadOnly && (type != null) && (typeof(IOperator).IsAssignableFrom(type))) {
        if ((e.KeyState & 8) == 8) e.Effect = DragDropEffects.Link;  // CTRL key        
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) e.Effect = DragDropEffects.Copy;
        else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move) e.Effect = DragDropEffects.Move;
        else if ((e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link) e.Effect = DragDropEffects.Link;
      }
    }

    private void OperatorGraphView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        IOperator op = e.Data.GetData("Value") as IOperator;
        if ((e.Effect & DragDropEffects.Copy) == DragDropEffects.Copy) op = (IOperator)op.Clone();
        IOperatorShapeInfo shapeInfo = OperatorShapeInfoFactory.CreateOperatorShapeInfo(op);
        Point mouse = new Point(MousePosition.X, MousePosition.Y);
        Point screen = this.graphVisualizationInfoView.PointToScreen(new Point(0, 0));
        Point control = new Point(mouse.X - screen.X, mouse.Y - screen.Y);
        PointF worldPoint = this.graphVisualizationInfoView.Controller.View.ViewToWorld(control);

        if (worldPoint.X < 0)
          worldPoint.X = 0;
        if (worldPoint.Y < 0)
          worldPoint.Y = 0;

        shapeInfo.Location = Point.Round(worldPoint);
        this.VisualizationInfo.AddShapeInfo(op, shapeInfo);
      }
    }
    #endregion

    private void tool_OnToolActivate(object sender, ToolEventArgs e) {
      Button button = GetButtonForTool(e.Properties.Name);
      if (button != null)
        button.Enabled = false;
    }

    private void tool_OnToolDeactivate(object sender, ToolEventArgs e) {
      Button button = GetButtonForTool(e.Properties.Name);
      if (button != null)
        button.Enabled = true;
    }

    private Button GetButtonForTool(string toolName) {
      Button button = null;
      switch (toolName) {
        case ControllerBase.SelectionToolName:
          button = this.selectButton;
          break;
        case ControllerBase.PanToolName:
          button = this.panButton;
          break;
        case ControllerBase.ConnectionToolName:
          button = this.connectButton;
          break;
        case ControllerBase.ZoomAreaToolName:
          button = this.zoomToFitButton;
          break;
      }
      return button;
    }

    private void selectButton_Click(object sender, EventArgs e) {
      ITool tool = this.graphVisualizationInfoView.Controller.Tools.Where(t => t.Name == ControllerBase.SelectionToolName).First();
      this.graphVisualizationInfoView.Controller.DeactivateAllTools();
    }

    private void panButton_Click(object sender, EventArgs e) {
      this.graphVisualizationInfoView.Controller.ActivateTool(ControllerBase.PanToolName);
    }

    private void connectButton_Click(object sender, EventArgs e) {
      this.graphVisualizationInfoView.Controller.ActivateTool(ControllerBase.ConnectionToolName);
    }

    private void relayoutButton_Click(object sender, EventArgs e) {
      this.graphVisualizationInfoView.RelayoutGraph();
    }

    private void zoomAreaButton_Click(object sender, EventArgs e) {
      this.graphVisualizationInfoView.Controller.View.ZoomFit();
    }

    private void zoomInButton_Click(object sender, EventArgs e) {
      this.graphVisualizationInfoView.Controller.ActivateTool(ControllerBase.ZoomInToolName);
    }

    private void zoomOutButton_Click(object sender, EventArgs e) {
      this.graphVisualizationInfoView.Controller.ActivateTool(ControllerBase.ZoomOutToolName);
    }

    private void screenshotButton_Click(object sender, EventArgs e) {
      Bitmap bitmap = ImageExporter.FromBundle(new Bundle(this.graphVisualizationInfoView.Controller.Model.Paintables), this.graphVisualizationInfoView.Controller.View.Graphics);
      SaveFileDialog saveFileDialog = new SaveFileDialog();
      saveFileDialog.Title = "Save Screenshot";
      saveFileDialog.DefaultExt = "bmp";
      saveFileDialog.Filter = "Bitmap|*.bmp|All Files|*.*";
      saveFileDialog.FilterIndex = 1;
      saveFileDialog.AddExtension = true;

      if (saveFileDialog.ShowDialog() == DialogResult.OK) {
        bitmap.Save(saveFileDialog.FileName);
      }
    }
  }
}
