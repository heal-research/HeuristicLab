using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.VehicleRouting.Views {
  [View("CVRPPDTWProblemInstance View")]
  [Content(typeof(CVRPPDTWProblemInstance), true)]
  public partial class CVRPPDTWView : VRPProblemInstanceView {
    public new CVRPPDTWProblemInstance Content {
      get { return (CVRPPDTWProblemInstance)base.Content; }
      set { base.Content = value; }
    }

    public CVRPPDTWView() {
      InitializeComponent();
    }

    private bool drawFlow = false;

    protected override void pictureBox_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e) {
      if (e.Button == System.Windows.Forms.MouseButtons.Right) {
        drawFlow = !drawFlow;
        GenerateImage();
      }      
    }

    protected override void DrawVisualization(Bitmap bitmap) {
      DoubleMatrix coordinates = Content.Coordinates;
      DoubleMatrix distanceMatrix = Content.DistanceMatrix;
      BoolValue useDistanceMatrix = Content.UseDistanceMatrix;
      DoubleArray dueTime = Content.DueTime;
      DoubleArray serviceTime = Content.ServiceTime;
      DoubleArray readyTime = Content.ReadyTime;
      DoubleArray demand = Content.Demand;
      IntArray pickupDeliveryLocation = Content.PickupDeliveryLocation;

      Pen flowPen = new Pen(Brushes.Red, 3);
      flowPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
      flowPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

      if ((coordinates != null) && (coordinates.Rows > 0) && (coordinates.Columns == 2)) {
        double xMin = double.MaxValue, yMin = double.MaxValue, xMax = double.MinValue, yMax = double.MinValue;
        for (int i = 0; i < coordinates.Rows; i++) {
          if (xMin > coordinates[i, 0]) xMin = coordinates[i, 0];
          if (yMin > coordinates[i, 1]) yMin = coordinates[i, 1];
          if (xMax < coordinates[i, 0]) xMax = coordinates[i, 0];
          if (yMax < coordinates[i, 1]) yMax = coordinates[i, 1];
        }

        int border = 20;
        double xStep = xMax != xMin ? (bitmap.Width - 2 * border) / (xMax - xMin) : 1;
        double yStep = yMax != yMin ? (bitmap.Height - 2 * border) / (yMax - yMin) : 1;

        using (Graphics graphics = Graphics.FromImage(bitmap)) {
          if (Solution != null) {
            int currentTour = 0;

            List<Tour> tours = Solution.GetTours();
            List<Pen> pens = GetColors(tours.Count);

            foreach (Tour tour in tours) {
              double t = 0.0;
              Point[] tourPoints = new Point[tour.Stops.Count + 2];
              Brush[] customerBrushes = new Brush[tour.Stops.Count];
              Pen[] customerPens = new Pen[tour.Stops.Count];
              bool[] validPickupDelivery = new bool[tour.Stops.Count];
              int lastCustomer = 0;

              Dictionary<int, bool> stops = new Dictionary<int, bool>();
              for (int i = -1; i <= tour.Stops.Count; i++) {
                int location = 0;

                if (i == -1 || i == tour.Stops.Count)
                  location = 0; //depot
                else
                  location = tour.Stops[i];

                Point locationPoint = new Point(border + ((int)((coordinates[location, 0] - xMin) * xStep)),
                                bitmap.Height - (border + ((int)((coordinates[location, 1] - yMin) * yStep))));
                tourPoints[i + 1] = locationPoint;

                if (i != -1 && i != tour.Stops.Count) {
                  Brush customerBrush = Brushes.Black;
                  Pen customerPen = Pens.Black;

                  t += Content.GetDistance(
                    lastCustomer, location, Solution);

                  if (t < readyTime[location]) {
                    t = readyTime[location];
                    customerBrush = Brushes.Orange;
                    customerPen = Pens.Orange;

                  } else if (t > dueTime[location]) {
                    customerBrush = Brushes.Red;
                    customerPen = Pens.Red;
                  }

                  t += serviceTime[location];

                  validPickupDelivery[i] =
                    ((demand[location] >= 0) ||
                     (stops.ContainsKey(pickupDeliveryLocation[location])));

                  customerBrushes[i] = customerBrush;
                  customerPens[i] = customerPen;

                  stops.Add(location, true);
                }
                lastCustomer = location;
              }

              if (!drawFlow)
                graphics.DrawPolygon(pens[currentTour], tourPoints);

              for (int i = 0; i < tour.Stops.Count; i++) {
                if (validPickupDelivery[i]) {
                  graphics.FillRectangle(customerBrushes[i], tourPoints[i + 1].X - 3, tourPoints[i + 1].Y - 3, 6, 6);

                  if (demand[tour.Stops[i]] < 0 && drawFlow) {
                    int location = pickupDeliveryLocation[tour.Stops[i]];
                    int source = tour.Stops.IndexOf(location);

                    graphics.DrawLine(flowPen, tourPoints[source + 1], tourPoints[i + 1]);
                  }
                } else
                  graphics.DrawRectangle(customerPens[i], tourPoints[i + 1].X - 3, tourPoints[i + 1].Y - 3, 6, 6);
              }

              graphics.FillEllipse(Brushes.Blue, tourPoints[0].X - 5, tourPoints[0].Y - 5, 10, 10);

              currentTour++;
            }

            for (int i = 0; i < pens.Count; i++)
              pens[i].Dispose();
          } else {
            {
              Point[] locationPoints = new Point[coordinates.Rows];
              //just draw customers
              for (int i = 1; i < coordinates.Rows; i++) {
                locationPoints[i] = new Point(border + ((int)((coordinates[i, 0] - xMin) * xStep)),
                                bitmap.Height - (border + ((int)((coordinates[i, 1] - yMin) * yStep))));

                graphics.FillRectangle(Brushes.Black, locationPoints[i].X - 3, locationPoints[i].Y - 3, 6, 6);
              }

              if (drawFlow) {
                for (int i = 1; i < coordinates.Rows; i++) {
                  if (demand[i] < 0) {

                    graphics.DrawLine(flowPen, locationPoints[pickupDeliveryLocation[i]], locationPoints[i]);
                  }
                }
              }

              Point locationPoint = new Point(border + ((int)((coordinates[0, 0] - xMin) * xStep)),
                                bitmap.Height - (border + ((int)((coordinates[0, 1] - yMin) * yStep))));
              graphics.FillEllipse(Brushes.Blue, locationPoint.X - 5, locationPoint.Y - 5, 10, 10);
            }
          }
        }
      }

      flowPen.Dispose();
    }
  }
}
