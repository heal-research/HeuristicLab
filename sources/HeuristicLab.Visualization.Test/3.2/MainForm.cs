using System;
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Visualization.Drawing;

namespace HeuristicLab.Visualization.Test {
  public partial class MainForm : Form {
    private readonly Canvas canvas;

    public MainForm() {
      InitializeComponent();

      canvas = canvasUI.Canvas;

      CreateLeftWorldShape();
      CreateMiddleCompositeShape();
      CreateRightWorldShape();
      CreateSimpleRectangleShape();

      canvasUI.Invalidate();
    }

    private void CreateSimpleRectangleShape() {
      // simple rectangle shape
      RectangleShape rect7 = new RectangleShape(5, 5, 50, 50, Color.Black);
      canvas.AddShape(rect7);
    }

    private void CreateRightWorldShape() {
      // right world shape
      WorldShape rightWorld = new WorldShape();
      rightWorld.ClippingArea = new RectangleD(-1, -1, 1, 1);
      rightWorld.BoundingBox = new RectangleD(600, 10, 780, 600);

      double x1 = -3;
      double y1 = -3;

      for (int i = 0; i < 10000; i++) {
        RectangleShape rect = new RectangleShape(x1, y1, x1 + 0.3, y1 + 0.3, Color.Maroon);
        x1 += 0.4;
        y1 += 0.4;
        rightWorld.AddShape(rect);
      }

      canvas.AddShape(rightWorld);
    }

    private void CreateMiddleCompositeShape() {
      // middle composite shape
      CompositeShape middleComp = new CompositeShape();

      RectangleShape rect5 = new RectangleShape(400, 10, 500, 300, Color.Navy);
      RectangleShape rect6 = new RectangleShape(510, 310, 580, 590, Color.Magenta);

      middleComp.AddShape(rect5);
      middleComp.AddShape(rect6);

      canvas.AddShape(middleComp);
    }

    private void CreateLeftWorldShape() {
      // left world shape
      WorldShape leftWorld = new WorldShape();
      leftWorld.ClippingArea = new RectangleD(0, 0, 1000, 1000);
      leftWorld.BoundingBox = new RectangleD(10, 10, 380, 590);

      RectangleShape fillRect = new RectangleShape(0, 0, 1000, 1000, Color.LightBlue);

      RectangleShape rect1 = new RectangleShape(100, 100, 500, 500, Color.Red);
      RectangleShape rect2 = new RectangleShape(800, -200, 1200, 500, Color.Green);

      CompositeShape comp1 = new CompositeShape();

      RectangleShape rect3 = new RectangleShape(510, 580, 590, 700, Color.Blue);
      RectangleShape rect4 = new RectangleShape(600, 710, 800, 900, Color.Orange);

      comp1.AddShape(rect3);
      comp1.AddShape(rect4);

      leftWorld.AddShape(fillRect);
      leftWorld.AddShape(rect1);
      leftWorld.AddShape(rect2);
      leftWorld.AddShape(comp1);

      canvas.AddShape(leftWorld);
    }

    private void legendButton_Click(object sender, EventArgs e) {
      LegendForm form = new LegendForm();
      form.Show();
    }
  }
}