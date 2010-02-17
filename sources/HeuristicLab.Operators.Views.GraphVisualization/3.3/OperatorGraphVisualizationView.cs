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

namespace HeuristicLab.Operators.Views.GraphVisualization {
  [Content(typeof(IOperator), false)]
  public partial class OperatorGraphVisualizationView : ItemView {
    private Dictionary<IOperator, IShape> operatorShapeMapping;
    private Dictionary<KeyValuePair<IOperator, IValueParameter<IOperator>>, IConnection> connectionMapping;
    private HashSet<IValueParameter<IOperator>> parameters;
    /// <summary>
    /// Initializes a new instance of <see cref="OperatorGraphVisualizationView"/> with caption "Operator Graph".
    /// </summary>
    public OperatorGraphVisualizationView() {
      InitializeComponent();
      Caption = "Operator Graph";
      this.operatorShapeMapping = new Dictionary<IOperator, IShape>();
      this.connectionMapping = new Dictionary<KeyValuePair<IOperator, IValueParameter<IOperator>>, IConnection>();
      this.parameters = new HashSet<IValueParameter<IOperator>>();

      this.graphVisualization.Controller.Model.OnEntityRemoved += new EventHandler<EntityEventArgs>(Model_OnEntityRemoved);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OperatorGraphVisualizationView"/> 
    /// with the given <paramref name="operatorGraph"/>.
    /// </summary>
    /// <remarks>Calls <see cref="OperatorGraphView()"/>.</remarks>
    /// <param name="operatorGraph">The operator graph to represent visually.</param>
    public OperatorGraphVisualizationView(IOperator content)
      : this() {
      this.Content = content;
    }

    /// <summary>
    /// Gets or sets the operator graph to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public new IOperator Content {
      get { return (IOperator)base.Content; }
      set { base.Content = value; }
    }

    private IModel GraphModel {
      get { return this.graphVisualization.Controller.Model; }
    }


    protected override void OnContentChanged() {
      base.OnContentChanged();
      this.ClearGraph();

      this.CreateGraph(null, new OperatorParameter(string.Empty, this.Content));
      foreach (IShape shape in this.operatorShapeMapping.Values)
        this.GraphModel.AddShape(shape);

      foreach (IConnection connection in this.connectionMapping.Values)
        this.GraphModel.AddConnection(connection);

      if (this.Content == null)
        this.graphVisualization.Controller.Model.LayoutRoot = null;
      else
        this.graphVisualization.Controller.Model.LayoutRoot = this.operatorShapeMapping[this.Content];
      this.RelayoutOperatorGraph();
    }

    private void opParam_ValueChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(opParam_ValueChanged), sender, e);
      else {
        this.OnContentChanged();
      }
    }

    private void ClearGraph() {
      this.GraphModel.Clear();
      foreach (IValueParameter<IOperator> opParam in this.parameters)
        opParam.ValueChanged -= opParam_ValueChanged;

      this.operatorShapeMapping.Clear();
      this.parameters.Clear();
      this.connectionMapping.Clear();
    }



    private void CreateGraph(IOperator parent, IValueParameter<IOperator> op) {
      if (op == null || op.Value == null)
        return;

      IShape shape;
      if (!this.operatorShapeMapping.ContainsKey(op.Value)) {
        shape = CreateOperatorShape(op.Value);
        this.operatorShapeMapping[op.Value] = shape;

        foreach (IParameter param in op.Value.Parameters) {
          IValueParameter<IOperator> opParam = param as IValueParameter<IOperator>;
          if (opParam != null) {
            HandleOperatorParameter(opParam);
            this.CreateGraph(op.Value, opParam);
          }
        }
      }

      if (parent != null)
        ConnectShapes(parent, op);
    }

    private void HandleOperatorParameter(IValueParameter<IOperator> opParam) {
      if (opParam == null)
        return;
      opParam.ValueChanged += new EventHandler(opParam_ValueChanged);
      parameters.Add(opParam);
    }

    private IConnection ConnectShapes(IOperator parent, IValueParameter<IOperator> opParam) {
      IShape operatorShape = this.operatorShapeMapping[opParam.Value];
      IShape parentShape = this.operatorShapeMapping[parent];
      IConnector operatorConnector = parentShape.Connectors.Where(c => c.Name == "Bottom connector").First();
      IConnector parentConnector = operatorShape.Connectors.Where(c => c.Name == "Top connector").First();

      IConnection connection = new Connection(parentConnector.Point, operatorConnector.Point);
      parentConnector.AttachConnector(connection.From);
      operatorConnector.AttachConnector(connection.To);

      this.connectionMapping[new KeyValuePair<IOperator, IValueParameter<IOperator>>(parent, opParam)] = connection;
      return connection;
    }

    private IShape CreateOperatorShape(IOperator op) {
      ClassShape shape = new ClassShape();
      shape.Name = op.Name;
      shape.Text = op.Name;
      shape.Title = op.Name;
      shape.SubTitle = op.GetType().ToString();

      return shape;
    }

    private void Model_OnEntityRemoved(object sender, EntityEventArgs e) {
      IShape shape = e.Entity as IShape;
      if (shape != null) {
        IOperator op = operatorShapeMapping.Where(os => os.Value == shape).First().Key;
        if (op == this.Content)
          this.Content = null;
        else {
          //clear all connections to the removed operator
          IEnumerable<IValueParameter<IOperator>> parentOperator = this.connectionMapping.Where(cs => cs.Key.Value.Value == op).Select(x => x.Key.Value);
          foreach(IValueParameter<IOperator> opParam in parentOperator.ToArray())
            opParam.Value = null;

          //remove connections from graph view
          IEnumerable<IConnection> connections = this.connectionMapping.Where(cs => cs.Key.Value.Value == op).Select(x => x.Value);
          foreach (IConnection connection in connections)
            this.GraphModel.Remove(connection);

          this.graphVisualization.Invalidate();
        }


      }
    }

    #region methods for toolbar items

    internal void RelayoutOperatorGraph() {
      if (this.operatorShapeMapping.Count > 0 && this.connectionMapping.Count > 0) { //otherwise the layout does not work
        this.graphVisualization.Invalidate();
        this.graphVisualization.Controller.RunActivity("Standard TreeLayout");
      }
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
  }
}
