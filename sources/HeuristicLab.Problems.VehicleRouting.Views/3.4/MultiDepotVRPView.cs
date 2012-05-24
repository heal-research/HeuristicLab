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
  [View("MultiDepotVRPProblemInstance View")]
  [Content(typeof(MultiDepotVRPProblemInstance), true)]
  public partial class MultiDepotVRPView : VRPProblemInstanceView {
    public new MultiDepotVRPProblemInstance Content {
      get { return (MultiDepotVRPProblemInstance)base.Content; }
      set { base.Content = value; }
    }

    public MultiDepotVRPView() {
      InitializeComponent();
    }

    protected override void DrawVisualization(Bitmap bitmap) {
      DoubleMatrix coordinates = Content.Coordinates;
      DoubleMatrix distanceMatrix = Content.DistanceMatrix;
      BoolValue useDistanceMatrix = Content.UseDistanceMatrix;

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
            for (int i = 0; i < Content.Depots.Value; i++) {
              Point locationPoint = new Point(border + ((int)((coordinates[i, 0] - xMin) * xStep)),
                                bitmap.Height - (border + ((int)((coordinates[i, 1] - yMin) * yStep))));
              graphics.FillEllipse(Brushes.Blue, locationPoint.X - 5, locationPoint.Y - 5, 10, 10);
            }

            int currentTour = 0;

            List<Tour> tours = Solution.GetTours();
            List<Pen> pens = GetColors(tours.Count);

            foreach (Tour tour in tours) {
              Point[] tourPoints = new Point[tour.Stops.Count + 2];
              Brush[] customerBrushes = new Brush[tour.Stops.Count];
              int lastCustomer = 0;

              for (int i = -1; i <= tour.Stops.Count; i++) {
                int location = 0;

                if (i == -1 || i == tour.Stops.Count)
                  location = 0; //depot
                else
                  location = tour.Stops[i];

                Point locationPoint;

                if (location == 0) {
                  int tourIndex = Solution.GetTourIndex(tour);
                  int vehicle = Solution.GetVehicleAssignment(tourIndex);
                  int depot = Content.VehicleDepotAssignment[vehicle];

                  locationPoint = new Point(border + ((int)((Content.Coordinates[depot, 0] - xMin) * xStep)),
                                  bitmap.Height - (border + ((int)((Content.Coordinates[depot, 1] - yMin) * yStep))));
                } else {
                  locationPoint = new Point(border + ((int)((Content.GetCoordinates(location)[0] - xMin) * xStep)),
                                  bitmap.Height - (border + ((int)((Content.GetCoordinates(location)[1] - yMin) * yStep))));
                }
                tourPoints[i + 1] = locationPoint;

                if (i != -1 && i != tour.Stops.Count) {
                  Brush customerBrush = Brushes.Black;
                  customerBrushes[i] = customerBrush;
                }
                lastCustomer = location;
              }

              graphics.DrawPolygon(pens[currentTour], tourPoints);

              for (int i = 0; i < tour.Stops.Count; i++) {
                graphics.FillRectangle(customerBrushes[i], tourPoints[i + 1].X - 3, tourPoints[i + 1].Y - 3, 6, 6);
              }

              graphics.FillEllipse(Brushes.DarkBlue, tourPoints[0].X - 5, tourPoints[0].Y - 5, 10, 10);

              currentTour++;
            }

            for (int i = 0; i < pens.Count; i++)
              pens[i].Dispose();
          } else {
            Point locationPoint;
            //just draw customers
            for (int i = 1; i <= Content.Cities.Value; i++) {
              locationPoint = new Point(border + ((int)((Content.GetCoordinates(i)[0] - xMin) * xStep)),
                              bitmap.Height - (border + ((int)((Content.GetCoordinates(i)[1] - yMin) * yStep))));

              graphics.FillRectangle(Brushes.Black, locationPoint.X - 3, locationPoint.Y - 3, 6, 6);
            }

            for (int i = 0; i < Content.Depots.Value; i++) {
              locationPoint = new Point(border + ((int)((coordinates[i, 0] - xMin) * xStep)),
                                bitmap.Height - (border + ((int)((coordinates[i, 1] - yMin) * yStep))));
              graphics.FillEllipse(Brushes.Blue, locationPoint.X - 5, locationPoint.Y - 5, 10, 10);
            }
          }
        }
      }
    }
  }
}
