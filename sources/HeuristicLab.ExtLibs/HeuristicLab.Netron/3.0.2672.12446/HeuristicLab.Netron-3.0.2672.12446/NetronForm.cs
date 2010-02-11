using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Netron.Diagramming.Core;

namespace HeuristicLab.Netron {
  public partial class NetronForm : Form {
    public NetronForm() {
      InitializeComponent();
      ClassShape shape1 = new ClassShape();
      shape1.Location = new Point(200, 200);
      shape1.Name = "Shape 1";
      IConnector c1b = shape1.Connectors.Where(c => c.Name == "Bottom connector").First();
      IConnector c1t = shape1.Connectors.Where(c => c.Name == "Top connector").First();

      ClassShape shape2 = new ClassShape();
      shape2.Location = new Point(300, 300);
      shape2.Name = "Shape 2";
      IConnector c2b = shape2.Connectors.Where(c => c.Name == "Bottom connector").First();
      IConnector c2t = shape2.Connectors.Where(c => c.Name == "Top connector").First();

      ClassShape shape3 = new ClassShape();
      shape3.Location = new Point(400, 400);
      shape3.Name = "Shape 3";
      IConnector c3b = shape3.Connectors.Where(c => c.Name == "Bottom connector").First();
      IConnector c3t = shape3.Connectors.Where(c => c.Name == "Top connector").First();

      ClassShape shape4 = new ClassShape();
      shape4.Location = new Point(500, 500);
      shape4.Name = "Shape 4";
      IConnector c4b = shape4.Connectors.Where(c => c.Name == "Bottom connector").First();
      IConnector c4t = shape4.Connectors.Where(c => c.Name == "Top connector").First();

      ClassShape shape5 = new ClassShape();
      shape5.Location = new Point(600, 600);
      shape5.Name = "Shape 5";
      IConnector c5b = shape5.Connectors.Where(c => c.Name == "Bottom connector").First();
      IConnector c5t = shape5.Connectors.Where(c => c.Name == "Top connector").First();

      Connection c1 = new Connection(c1b.Point, c2t.Point, netronVisualization.Controller.Model);
      c1b.AttachConnector(c1.From);
      c2t.AttachConnector(c1.To);

      Connection c2 = new Connection(c1b.Point, c3t.Point, netronVisualization.Controller.Model);
      c1b.AttachConnector(c2.From);
      c3t.AttachConnector(c2.To);

      Connection c3 = new Connection(c2b.Point, c4t.Point, netronVisualization.Controller.Model);
      c2b.AttachConnector(c3.From);
      c4t.AttachConnector(c3.To);

      Connection c4 = new Connection(c3b.Point, c4t.Point, netronVisualization.Controller.Model);
      c3b.AttachConnector(c4.From);
      c4t.AttachConnector(c4.To);

      Connection c5 = new Connection(c4b.Point, c5t.Point, netronVisualization.Controller.Model);
      c4b.AttachConnector(c5.From);
      c5t.AttachConnector(c5.To);

      netronVisualization.Controller.Model.AddShape(shape1);
      netronVisualization.Controller.Model.AddShape(shape2);
      netronVisualization.Controller.Model.AddShape(shape3);
      netronVisualization.Controller.Model.AddShape(shape4);
      netronVisualization.Controller.Model.AddShape(shape5);

      netronVisualization.Controller.Model.AddConnection(c1);
      netronVisualization.Controller.Model.AddConnection(c2);
      netronVisualization.Controller.Model.AddConnection(c3);
      netronVisualization.Controller.Model.AddConnection(c4);
      netronVisualization.Controller.Model.AddConnection(c5);

      netronVisualization.Controller.View.Invalidate();
      netronVisualization.Controller.Model.LayoutRoot = shape1;
      netronVisualization.Controller.RunActivity("Standard TreeLayout");
    }
  }
}
