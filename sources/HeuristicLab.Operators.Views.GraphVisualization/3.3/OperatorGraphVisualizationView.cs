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

namespace HeuristicLab.Operators.Views.GraphVisualization {
  [Content(typeof(IOperator), false)]
  public partial class OperatorGraphVisualizationView : ItemView {
    private Dictionary<IOperator, IShape> operatorShapeMapping;
    private Dictionary<KeyValuePair<IOperator, IOperator>, IConnection> connectionMapping;
    /// <summary>
    /// Initializes a new instance of <see cref="OperatorGraphVisualizationView"/> with caption "Operator Graph".
    /// </summary>
    public OperatorGraphVisualizationView() {
      InitializeComponent();
      Caption = "Operator Graph";
      this.operatorShapeMapping = new Dictionary<IOperator, IShape>();
      this.connectionMapping = new Dictionary<KeyValuePair<IOperator, IOperator>, IConnection>();
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
      this.GraphModel.Shapes.Clear();
      this.operatorShapeMapping.Clear();
      this.connectionMapping.Clear();

      this.CreateGraph(null, this.Content);
      foreach (IShape shape in this.operatorShapeMapping.Values)
        this.GraphModel.AddShape(shape);

      foreach (IConnection connection in this.connectionMapping.Values)
        this.GraphModel.AddConnection(connection);

      this.graphVisualization.Controller.View.Invalidate();
      this.graphVisualization.Controller.Model.LayoutRoot = this.operatorShapeMapping[this.Content];
      this.graphVisualization.Controller.RunActivity("Standard TreeLayout");
    }

    private void opParam_ValueChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(opParam_ValueChanged), sender, e);
      else {
        //IValueParameter<IOperator> opParam = (IValueParameter<IOperator>)sender;
        //foreach (TreeNode node in opParamNodeTable[opParam].ToArray())
        //  //remove nodes
        //foreach (TreeNode node in opParamNodeTable[opParam]) {
        //  //add nodes
        //}
      }
    } 

    private void CreateGraph(IOperator parent, IOperator op) {
      if (op == null)
        return;

      IShape shape;
      if (!this.operatorShapeMapping.ContainsKey(op)) {
        shape = CreateOperatorShape(op);
        this.operatorShapeMapping[op] = shape;

        foreach (IParameter param in op.Parameters) {
          IValueParameter<IOperator> opParam = param as IValueParameter<IOperator>;
          if (opParam != null) {
            opParam.ValueChanged += new EventHandler(opParam_ValueChanged);
            this.CreateGraph(op, opParam.Value);
          }
        }
      }

      if (parent != null)
        ConnectShapes(parent, op);
    }

    private void RemoveGraph(IOperator op) {
      if(op == null)
        return;
      IShape shape = this.operatorShapeMapping[op];
      this.GraphModel.RemoveShape(shape);
    }

    private IConnection ConnectShapes(IOperator parent, IOperator op) {
      IShape operatorShape = this.operatorShapeMapping[op];
      IShape parentShape = this.operatorShapeMapping[parent];
      IConnector operatorConnector = parentShape.Connectors.Where(c => c.Name == "Bottom connector").First();
      IConnector parentConnector = operatorShape.Connectors.Where(c => c.Name == "Top connector").First();

      IConnection connection = new Connection(parentConnector.Point, operatorConnector.Point);
      parentConnector.AttachConnector(connection.From);
      operatorConnector.AttachConnector(connection.To);

      this.connectionMapping[new KeyValuePair<IOperator, IOperator>(parent, op)] = connection;
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
  }
}
