using System.Drawing;
using System.Windows.Forms;

namespace HeuristicLab.Visualization.Test {
  public partial class MainForm : Form {
    private MouseEventListener dragDropListener;

    public MainForm() {
      InitializeComponent();

      CreateMouseEventListeners();

      canvasUI.MainCanvas.WorldShape = new WorldShape(new RectangleD(0, 0, 800, 600), new RectangleD(0, 0, 800, 600));

      CreateLeftWorldShape();
      CreateMiddleCompositeShape();
      CreateRightWorldShape();
      CreateSimpleRectangleShape();

      canvasUI.Invalidate();
    }

    private void CreateMouseEventListeners() {
      dragDropListener = new MouseEventListener();
      dragDropListener.OnMouseMove += DragDrop_OnMouseMove;
      dragDropListener.OnMouseUp += DragDrop_OnMouseUp;
    }

    private void CreateSimpleRectangleShape() {
      WorldShape mainWorld = canvasUI.MainCanvas.WorldShape;
      // simple rectangle shape
      RectangleShape rect7 = new RectangleShape(5, 5, 50, 50, Color.Black);
      mainWorld.AddShape(rect7);
    }

    private void CreateRightWorldShape() {
      WorldShape mainWorld = canvasUI.MainCanvas.WorldShape;
      // right world shape
      WorldShape rightWorld = new WorldShape(new RectangleD(-1, -1, 1, 1), new RectangleD(600, 10, 780, 600));

      double x1 = -3;
      double y1 = -3;

      for (int i = 0; i < 10000; i++) {
        RectangleShape rect = new RectangleShape(x1, y1, x1 + 0.3, y1 + 0.3, Color.Maroon);
        x1 += 0.4;
        y1 += 0.4;
        rightWorld.AddShape(rect);
      }

      mainWorld.AddShape(rightWorld);
    }

    private void CreateMiddleCompositeShape() {
      WorldShape mainWorld = canvasUI.MainCanvas.WorldShape;
      // middle composite shape
      CompositeShape middleComp = new CompositeShape();

      RectangleShape rect5 = new RectangleShape(400, 10, 500, 300, Color.Navy);
      RectangleShape rect6 = new RectangleShape(510, 310, 580, 590, Color.Magenta);

      middleComp.AddShape(rect5);
      middleComp.AddShape(rect6);

      mainWorld.AddShape(middleComp);
    }

    private void CreateLeftWorldShape() {
      WorldShape mainWorld = canvasUI.MainCanvas.WorldShape;
      // left world shape
      WorldShape leftWorld = new WorldShape(new RectangleD(0, 0, 1000, 1000), new RectangleD(10, 10, 380, 590));

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

      mainWorld.AddShape(leftWorld);
    }

    private void canvasUI_MouseDown(object sender, MouseEventArgs e) {
      mouseEventDemonstrationGraphics = canvasUI.CreateGraphics();

      dragDropListener.StartPoint = e.Location;
      lastActualPoint = e.Location;

      canvasUI.MouseEventListener = dragDropListener;
    }

    private Point lastActualPoint;
    private Graphics mouseEventDemonstrationGraphics;

    private void DragDrop_OnMouseUp(Point startPoint, Point actualPoint) {
      canvasUI.MouseEventListener = null;

      canvasUI.Invalidate();
      mouseEventDemonstrationGraphics.Dispose();
    }

    private void DragDrop_OnMouseMove(Point startPoint, Point actualPoint) {
      mouseEventDemonstrationGraphics.DrawLine(Pens.Blue, lastActualPoint, actualPoint);
      lastActualPoint = actualPoint;
    }

    private void legendButton_Click(object sender, System.EventArgs e) {
      LegendForm form = new LegendForm();
      form.Show();
    }
  }
}