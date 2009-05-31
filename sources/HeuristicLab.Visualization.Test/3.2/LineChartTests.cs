using System;
using System.Drawing;
using System.Threading;
using NUnit.Framework;

namespace HeuristicLab.Visualization.Test {
  [TestFixture]
  public class LineChartTests {
    private ChartDataRowsModel model;

    [SetUp]
    public void SetUp() {
      model = new ChartDataRowsModel();
    }

    [Test]
    public void TestLineChartWithManyDataPoints() {
      LineChartTestForm f = new LineChartTestForm(model);

      IDataRow row1 = new DataRow();
      IDataRow row2 = new DataRow();
      IDataRow row3 = new DataRow();

      row1.RowSettings.Color = Color.Red;
      row2.RowSettings.Color = Color.Green;
      row3.RowSettings.Color = Color.Blue;

      row1.RowSettings.Thickness = 3;
      row2.RowSettings.Thickness = 4;
      row3.RowSettings.Thickness = 5;

      row1.RowSettings.Label = "Simon";
      row2.RowSettings.Label = "Gertschi";
      row3.RowSettings.Label = "Maxi";

      row1.RowSettings.Style = DrawingStyle.Solid;
      row2.RowSettings.Style = DrawingStyle.Solid;
      row3.RowSettings.Style = DrawingStyle.Dashed;


      model.AddDataRow(row1);
      model.AddDataRow(row2);
      model.AddDataRow(row3);

      row1.AddValue(10);
      row1.AddValue(5);
      row1.AddValue(7);
      row1.AddValue(3);
      row1.AddValue(10);
      row1.AddValue(2);

      row2.AddValue(5);
      row2.AddValue(6);
      row2.AddValue(5);

      row3.AddValue(2);
      row3.AddValue(2);
      row3.AddValue(2);
      row3.AddValue(2);
      row3.AddValue(2);

      Random rand = new Random();

      for (int i = 0; i < 10000; i++) {
        row1.AddValue(rand.NextDouble() * 10);
        row2.AddValue(rand.NextDouble() * 10);
        row3.AddValue(rand.NextDouble() * 10);
      }

      f.ShowDialog();
    }

    [Test]
    public void TestGrid() {
      LineChartTestForm f = new LineChartTestForm(model);

      model.XAxis.ShowGrid = true;
      model.XAxis.GridColor = Color.Red;

      model.DefaultYAxis.ShowGrid = true;
      model.DefaultYAxis.GridColor = Color.Blue;

      IDataRow row1 = new DataRow();
      row1.RowSettings.Label = "row1";

      model.AddDataRow(row1);

      row1.AddValue(0);
      row1.AddValue(10);

      f.ShowDialog();
    }

    [Test]
    public void TestShowMarkers() {
      LineChartTestForm f = new LineChartTestForm(model);

      IDataRow row1 = new DataRow();
      row1.RowSettings.Label = "row1";
      row1.RowSettings.Color = Color.Red;
      row1.RowSettings.ShowMarkers = true;

      IDataRow row2 = new DataRow();
      row2.RowSettings.Label = "row2";
      row2.RowSettings.Color = Color.Blue;
      row2.RowSettings.ShowMarkers = false;

      model.AddDataRow(row1);
      model.AddDataRow(row2);

      for (int i = 0; i < 10; i++) {
        row1.AddValue(i);
        row2.AddValue(i*2);
      }

      f.ShowDialog();
    }

    [Test]
    public void TestShowLabelOnAxes() {
      LineChartTestForm f = new LineChartTestForm(model);

      IDataRow row1 = new DataRow();
      IDataRow row2 = new DataRow();

      model.XAxis.Label = "X-Axis";
      model.XAxis.ShowLabel = true;

      row1.YAxis = model.DefaultYAxis;
      row1.YAxis.Label = "Y-Axis row1";
      row1.RowSettings.Color = Color.Blue;
      row1.YAxis.ShowYAxisLabel = false;

      row2.YAxis = new YAxisDescriptor();
      row2.YAxis.Label = "Y-Axis row2";
      row2.RowSettings.Color = Color.Red;
      row2.YAxis.ShowYAxisLabel = true;

      model.AddDataRow(row1);
      model.AddDataRow(row2);

      row1.AddValue(0);
      row1.AddValue(10);
      row1.AddValue(15);
      row1.AddValue(16);

      row2.AddValue(0);
      row2.AddValue(20);
      row2.AddValue(25);
      row2.AddValue(26);

      f.ShowDialog();
    }

    [Test]
    public void TestAxes() {
      LineChartTestForm f = new LineChartTestForm(model);

      IDataRow row1 = new DataRow();
      IDataRow row2 = new DataRow();
      IDataRow row3 = new DataRow();

      model.XAxis.Label = "X Axis";
      model.XAxis.ShowLabel = true;
      model.XAxis.ShowGrid = true;
      model.XAxis.GridColor = Color.Blue;

      YAxisDescriptor yaxis1 = model.DefaultYAxis;
      YAxisDescriptor yaxis2 = new YAxisDescriptor();

      yaxis1.Label = "Y-Axis 1";
      yaxis1.Position = AxisPosition.Left;

      yaxis2.Label = "Y-Axis 2";
      yaxis2.Position = AxisPosition.Right;

      row1.RowSettings.Color = Color.Red;
      row1.RowSettings.Label = "Die Rote";

      row2.RowSettings.Color = Color.Green;
      row2.RowSettings.Label = "Die Grüne";

      row3.RowSettings.Color = Color.Blue;
      row3.RowSettings.Label = "Die Blaue";
      row3.YAxis = yaxis2;

      model.AddDataRow(row1);
      model.AddDataRow(row2);
      model.AddDataRow(row3);

      Random rand = new Random(42);

      for (int i = 0; i < 10; i++) {
        row1.AddValue(rand.NextDouble() * 10);
        row2.AddValue(rand.NextDouble() * 10);
        row3.AddValue(rand.NextDouble() * 1);
      }

      f.AddValue += delegate {
        row1.AddValue(rand.NextDouble() * 10);
        row2.AddValue(rand.NextDouble() * 10);
        row3.AddValue(rand.NextDouble() * 1);
      };

      f.ShowDialog();
    }

    [Test]
    public void TestDataRowWithOnlyOneValueShouldntCauseZoomLevelTooHighError() {
      LineChartTestForm f = new LineChartTestForm(model);

      IDataRow row1 = new DataRow();
      row1.AddValue(10);

      model.AddDataRow(row1);

      f.ShowDialog();
    }

    [Test]
    public void TestAddValueToDataRow() {
      LineChartTestForm f = new LineChartTestForm(model);

      IDataRow row = new DataRow();
      row.AddValue(0);
      row.AddValue(10);
      row.AddValue(-5);

      model.AddDataRow(row);

      f.ShowDialog();
    }

    [Test]
    public void TestAddValuesToDataRow() {
      LineChartTestForm f = new LineChartTestForm(model);

      IDataRow row = new DataRow();
      row.AddValues(new double[] {0, 10, -5});

      model.AddDataRow(row);

      f.ShowDialog();
    }

    [Test]
    public void TestInsertValueInDataRow() {
      LineChartTestForm f = new LineChartTestForm(model);

      IDataRow row = new DataRow();
      row.AddValue(0);
      row.AddValue(5);

      row.AddValue(10, 1);
      row.AddValue(10, 2);

      model.AddDataRow(row);

      f.ShowDialog();
    }

    [Test]
    public void TestInsertValuesInDataRow() {
      LineChartTestForm f = new LineChartTestForm(model);

      IDataRow row = new DataRow();
      row.AddValue(0);
      row.AddValue(5);

      row.AddValues(new double[] {10, 10}, 1);

      model.AddDataRow(row);

      f.ShowDialog();
    }

    [Test]
    public void TestModifyValueInDataRow() {
      LineChartTestForm f = new LineChartTestForm(model);

      IDataRow row = new DataRow();
      row.AddValue(0);
      row.AddValue(100);
      row.AddValue(0);

      row.ModifyValue(5, 1);

      model.AddDataRow(row);

      f.ShowDialog();
    }

    [Test]
    public void TestModifyValuesInDataRow() {
      LineChartTestForm f = new LineChartTestForm(model);

      IDataRow row = new DataRow();
      row.AddValue(0);
      row.AddValue(100);
      row.AddValue(100);
      row.AddValue(0);

      row.ModifyValues(new double[] {5, 5}, 1);

      model.AddDataRow(row);

      f.ShowDialog();
    }

    [Test]
    public void TestRemoveValueFromDataRow() {
      LineChartTestForm f = new LineChartTestForm(model);

      IDataRow row = new DataRow();
      row.AddValue(0);
      row.AddValue(20);
      row.AddValue(100);
      row.AddValue(50);

      row.RemoveValue(2);

      model.AddDataRow(row);

      f.ShowDialog();
    }

    [Test]
    public void TestRemoveValuesFromDataRow() {
      LineChartTestForm f = new LineChartTestForm(model);

      IDataRow row = new DataRow();
      row.AddValue(0);
      row.AddValue(20);
      row.AddValue(100);
      row.AddValue(50);

      row.RemoveValues(1, 2);

      model.AddDataRow(row);

      f.ShowDialog();
    }

    [Test]
    public void SimpleTestAggregator() {
      LineChartTestForm f = new LineChartTestForm(model);

      IDataRow row1 = new DataRow();
      row1.RowSettings.Label = "row";
      row1.RowSettings.Color = Color.Red;
      row1.RowSettings.Thickness = 3;
      row1.RowSettings.Style = DrawingStyle.Solid;

      model.AddDataRow(row1);


      MaxAggregator aggregator = new MaxAggregator();
      aggregator.RowSettings.Label = "MinAggregator";
      aggregator.RowSettings.Color = Color.Pink;
      aggregator.RowSettings.Thickness = 5;
      aggregator.RowSettings.Style = DrawingStyle.Solid;
      aggregator.RowSettings.LineType = DataRowType.SingleValue;
      aggregator.AddWatch(row1);

      model.AddDataRow(aggregator);

      row1.AddValue(10);
      row1.AddValue(5);
      row1.AddValue(7);
      row1.AddValue(3);
      row1.AddValue(10);
      row1.AddValue(2);

      f.ShowDialog();
    }


    public class Worker {
      // This method will be called when the thread is started.
      private readonly ChartDataRowsModel model;

      public Worker(ChartDataRowsModel model) {
        this.model = model;
      }

      public void DoWorkMultiLine() {
        IDataRow row1 = new DataRow();
        row1.RowSettings.Color = Color.Red;
        row1.RowSettings.Thickness = 2;
        row1.RowSettings.Label = "Sinus";
        row1.RowSettings.Style = DrawingStyle.Solid;
        row1.RowSettings.ShowMarkers = false;
        model.AddDataRow(row1);

        IDataRow row2 = new DataRow();
        row2.RowSettings.Color = Color.Red;
        row2.RowSettings.Thickness = 3;
        row2.RowSettings.Label = "Growing";
        row2.RowSettings.Style = DrawingStyle.Solid;
        row2.RowSettings.ShowMarkers = false;
        model.AddDataRow(row2);

        AvgAggregator multiAvgAggregator = new AvgAggregator();
        multiAvgAggregator.RowSettings.Label = "MultiAvgAggregator";
        multiAvgAggregator.RowSettings.Color = Color.DarkOliveGreen;
        multiAvgAggregator.RowSettings.Thickness = 3;
        multiAvgAggregator.RowSettings.Style = DrawingStyle.Solid;
        multiAvgAggregator.RowSettings.LineType = DataRowType.SingleValue;
        multiAvgAggregator.RowSettings.ShowMarkers = false;
        multiAvgAggregator.AddWatch(row1);
        multiAvgAggregator.AddWatch(row2);
        model.AddDataRow(multiAvgAggregator);

        MaxAggregator multiMaxAggregator = new MaxAggregator();
        multiMaxAggregator.RowSettings.Label = "MultiMaxAggregator";
        multiMaxAggregator.RowSettings.Color = Color.DarkKhaki;
        multiMaxAggregator.RowSettings.Thickness = 3;
        multiMaxAggregator.RowSettings.Style = DrawingStyle.Solid;
        multiMaxAggregator.RowSettings.LineType = DataRowType.SingleValue;
        multiMaxAggregator.RowSettings.ShowMarkers = false;
        multiMaxAggregator.AddWatch(row1);
        multiMaxAggregator.AddWatch(row2);
        model.AddDataRow(multiMaxAggregator);

        MinAggregator multiMinAggregator = new MinAggregator();
        multiMinAggregator.RowSettings.Label = "MultiMinAggregator";
        multiMinAggregator.RowSettings.Color = Color.DarkRed;
        multiMinAggregator.RowSettings.Thickness = 3;
        multiMinAggregator.RowSettings.Style = DrawingStyle.Solid;
        multiMinAggregator.RowSettings.LineType = DataRowType.SingleValue;
        multiMinAggregator.RowSettings.ShowMarkers = false;
        multiMinAggregator.AddWatch(row1);
        multiMinAggregator.AddWatch(row2);
        model.AddDataRow(multiMinAggregator);

        //        AvgLineAggregator multiLineAvgAggregator = new AvgLineAggregator {
        //                                                                           Label = "MultiLineAvgAggregator",
        //                                                                           Color = Color.Red,
        //                                                                           Thickness = 4,
        //                                                                           Style = DrawingStyle.Solid,
        //                                                                           LineType = DataRowType.Normal,
        //                                                                           ShowMarkers = false
        //                                                                         };
        //        multiLineAvgAggregator.AddWatch(row1);
        //        multiLineAvgAggregator.AddWatch(row2);
        //        multiLineAvgAggregator.AddValue(0);
        //        model.AddDataRow(multiLineAvgAggregator);

        double i = 0;


        while (!_shouldStop && i <= 24) {
          i += 0.2;
          double newY = Math.Sin(i);
          Console.WriteLine("working");
          //row1.AddValue(rand.NextDouble() * 10);
          row1.AddValue(newY * 10);
          row2.AddValue(i * 2 - 15);
          Thread.Sleep(100);
        }
        Console.WriteLine("worker thread: terminating gracefully.");
      }

      public void DoWorkSingleLine() {
        IDataRow row1 = new DataRow();
        row1.RowSettings.Color = Color.Red;
        row1.RowSettings.Thickness = 2;
        row1.RowSettings.Label = "Sinus";
        row1.RowSettings.Style = DrawingStyle.Solid;
        row1.RowSettings.ShowMarkers = false;
        model.AddDataRow(row1);

        IDataRow row2 = new DataRow();
        row2.RowSettings.Color = Color.Red;
        row2.RowSettings.Thickness = 3;
        row2.RowSettings.Label = "Growing";
        row2.RowSettings.Style = DrawingStyle.Solid;
        row2.RowSettings.ShowMarkers = false;
        model.AddDataRow(row2);

        MinAggregator aggregator = new MinAggregator();
        aggregator.RowSettings.Label = "MinAggregator";
        aggregator.RowSettings.Color = Color.Pink;
        aggregator.RowSettings.Thickness = 3;
        aggregator.RowSettings.Style = DrawingStyle.Solid;
        aggregator.RowSettings.LineType = DataRowType.SingleValue;
        aggregator.AddWatch(row1);
        model.AddDataRow(aggregator);

        MaxAggregator maxAggregator = new MaxAggregator();
        maxAggregator.RowSettings.Label = "MaxAggregator";
        maxAggregator.RowSettings.Color = Color.DeepSkyBlue;
        maxAggregator.RowSettings.Thickness = 3;
        maxAggregator.RowSettings.Style = DrawingStyle.Solid;
        maxAggregator.RowSettings.LineType = DataRowType.SingleValue;
        maxAggregator.AddWatch(row1);
        model.AddDataRow(maxAggregator);

        AvgAggregator avgAggregator = new AvgAggregator();
        avgAggregator.RowSettings.Label = "AvgAggregator";
        avgAggregator.RowSettings.Color = Color.Violet;
        avgAggregator.RowSettings.Thickness = 3;
        avgAggregator.RowSettings.Style = DrawingStyle.Solid;
        avgAggregator.RowSettings.LineType = DataRowType.SingleValue;
        avgAggregator.AddWatch(row1);
        model.AddDataRow(avgAggregator);

        double i = 0;


        while (!_shouldStop && i <= 240) {
          i += 0.2;
          double newY = Math.Sin(i);
          Console.WriteLine("working");
          //row1.AddValue(rand.NextDouble() * 10);
          row1.AddValue(newY * 10);
          row2.AddValue(i * 2 - 15);
          //System.Threading.Thread.Sleep(100);
        }
        Console.WriteLine("worker thread: terminating gracefully.");
      }

      public void DoWorkAvgLine() {
        IDataRow row1 = new DataRow();
        row1.RowSettings.Color = Color.Red;
        row1.RowSettings.Thickness = 2;
        row1.RowSettings.Label = "Sinus";
        row1.RowSettings.Style = DrawingStyle.Solid;
        row1.RowSettings.ShowMarkers = false;
        model.AddDataRow(row1);

        IDataRow row2 = new DataRow();
        row2.RowSettings.Color = Color.Red;
        row2.RowSettings.Thickness = 3;
        row2.RowSettings.Label = "Growing";
        row2.RowSettings.Style = DrawingStyle.Solid;
        row2.RowSettings.ShowMarkers = false;
        model.AddDataRow(row2);

        AvgLineAggregator avgLineAggregator = new AvgLineAggregator();
        avgLineAggregator.RowSettings.Label = "AvgLineAggregator";
        avgLineAggregator.RowSettings.Color = Color.Violet;
        avgLineAggregator.RowSettings.Thickness = 3;
        avgLineAggregator.RowSettings.Style = DrawingStyle.Solid;
        avgLineAggregator.RowSettings.LineType = DataRowType.Normal;
        avgLineAggregator.RowSettings.ShowMarkers = false;
        avgLineAggregator.AddWatch(row1);
        avgLineAggregator.AddWatch(row2);
        model.AddDataRow(avgLineAggregator);

        double i = 0;

        while (!_shouldStop && i <= 240) {
          i += 0.2;
          double newY = Math.Sin(i);
          Console.WriteLine("working");
          //row1.AddValue(rand.NextDouble() * 10);
          row1.AddValue(newY * 10);
          row2.AddValue(i * 2 - 15);
          //Thread.Sleep(100);
        }
        Console.WriteLine("worker thread: terminating gracefully.");
      }

      public void DoWorkFloatingAvg() {
        IDataRow row1 = new DataRow();
        row1.RowSettings.Color = Color.Red;
        row1.RowSettings.Thickness = 2;
        row1.RowSettings.Label = "SinusHacked";
        row1.RowSettings.Style = DrawingStyle.Solid;
        row1.RowSettings.ShowMarkers = false;
        model.AddDataRow(row1);

        IDataRow row2 = new DataRow();
        row2.RowSettings.Color = Color.Red;
        row2.RowSettings.Thickness = 3;
        row2.RowSettings.Label = "GrowingHacked";
        row2.RowSettings.Style = DrawingStyle.Solid;
        row2.RowSettings.ShowMarkers = false;
        model.AddDataRow(row2);

        FloatingAvgAggregator avgAggregator = new FloatingAvgAggregator();
        avgAggregator.RowSettings.Thickness = 2;
        avgAggregator.RowSettings.Label = "floatingAvg";
        avgAggregator.RowSettings.Color = Color.Peru;
        avgAggregator.RowSettings.ShowMarkers = false;
        avgAggregator.RowSettings.Style = DrawingStyle.Solid;

        avgAggregator.AddWatch(row1);
        model.AddDataRow(avgAggregator);

        FloatingAvgAggregator avgAggregator2 = new FloatingAvgAggregator();
        avgAggregator2.RowSettings.Thickness = 2;
        avgAggregator2.RowSettings.Label = "floatingAvg";
        avgAggregator2.RowSettings.Color = Color.Aqua;
        avgAggregator2.RowSettings.ShowMarkers = false;
        avgAggregator2.RowSettings.Style = DrawingStyle.Solid;

        avgAggregator2.AddWatch(row2);
        model.AddDataRow(avgAggregator2);

       
        double i = 0;
        Random rnd = new Random();

        while (!_shouldStop && i <= 100) {
          i += 0.2;
          double newY = Math.Sin(i);

          double hack = rnd.NextDouble() * i / 10;
          row1.AddValue(newY * 10 + hack);

          hack = rnd.NextDouble() * i / 10;
          row2.AddValue(i * 2 - 15 + hack);
          //Thread.Sleep(100);
        }
        Console.WriteLine("worker thread: terminating gracefully.");
      }

      public void RequestStop() {
        _shouldStop = true;
      }

      // Volatile is used as hint to the compiler that this data
      // member will be accessed by multiple threads.
      private volatile bool _shouldStop;
    }


    [Test]
    public void TestAggregatorMultiLine() {
      LineChartTestForm f = new LineChartTestForm(model);

      // Create the thread object. This does not start the thread.
      Worker workerObject = new Worker(model);
      Thread workerThread = new Thread(workerObject.DoWorkMultiLine);

      // Start the worker thread.
      workerThread.Start();

      f.ShowDialog();
      workerObject.RequestStop();
    }

    [Test]
    public void TestAggregatorSingleLine() {
      LineChartTestForm f = new LineChartTestForm(model);
      model.Title = "SingleLineAggregator Tests";

      // Create the thread object. This does not start the thread.
      Worker workerObject = new Worker(model);
      Thread workerThread = new Thread(workerObject.DoWorkSingleLine);

      // Start the worker thread.
      workerThread.Start();

      f.ShowDialog();
      workerObject.RequestStop();
    }

    [Test]
    public void TestAggregatorAvgLine() {
      LineChartTestForm f = new LineChartTestForm(model);
      model.Title = "AvgLineTest";

      // Create the thread object. This does not start the thread.
      Worker workerObject = new Worker(model);
      Thread workerThread = new Thread(workerObject.DoWorkAvgLine);

      // Start the worker thread.
      workerThread.Start();

      f.ShowDialog();
      workerObject.RequestStop();
    }

    [Test]
    public void TestFloatingAvg() {
      LineChartTestForm f = new LineChartTestForm(model);
      model.Title = "FloatingAvg Test";
      model.ViewSettings.LegendPosition = Legend.LegendPosition.Top;

      // Create the thread object. This does not start the thread.
      Worker workerObject = new Worker(model);
      Thread workerThread = new Thread(workerObject.DoWorkFloatingAvg);

      // Start the worker thread.
      workerThread.Start();

      f.ShowDialog();
      workerObject.RequestStop();
    }


    [Test]
    public void TestAutoZoomInConstructor() {
      IDataRow row1 = new DataRow();
      row1.RowSettings.Color = Color.Red;
      row1.RowSettings.Thickness = 3;
      row1.RowSettings.Style = DrawingStyle.Solid;

      model.AddDataRow(row1);

      row1.AddValue(10);
      row1.AddValue(5);
      row1.AddValue(7);
      row1.AddValue(3);
      row1.AddValue(10);
      row1.AddValue(2);

      LineChartTestForm f = new LineChartTestForm(model);
      f.ShowDialog();
    }


    [Test]
    public void TestSingleValueDataRows() {
      LineChartTestForm f = new LineChartTestForm(model);

      IDataRow row1 = new DataRow();
      IDataRow row2 = new DataRow();
      IDataRow row3 = new DataRow();

      IDataRow row4 = new DataRow();
      IDataRow row5 = new DataRow();
      IDataRow row6 = new DataRow();

      row1.RowSettings.Color = Color.Red;
      row2.RowSettings.Color = Color.Green;
      row3.RowSettings.Color = Color.Blue;

      row4.RowSettings.Color = Color.DeepPink;
      row5.RowSettings.Color = Color.Firebrick;
      row6.RowSettings.Color = Color.DarkSlateGray;

      row1.RowSettings.Thickness = 3;
      row2.RowSettings.Thickness = 4;
      row3.RowSettings.Thickness = 5;

      row4.RowSettings.Thickness = 3;
      row5.RowSettings.Thickness = 4;
      row6.RowSettings.Thickness = 5;

      row1.RowSettings.Label = "SingleValue";
      row2.RowSettings.Label = "Gertschi";
      row3.RowSettings.Label = "Maxi";

      row4.RowSettings.Label = "Simon";
      row5.RowSettings.Label = "klausmuellerwesternhagenunddierasperies";
      row6.RowSettings.Label = "anyways";

      row1.RowSettings.Style = DrawingStyle.Solid;
      row2.RowSettings.Style = DrawingStyle.Solid;
      row3.RowSettings.Style = DrawingStyle.Dashed;

      row4.RowSettings.Style = DrawingStyle.Solid;
      row5.RowSettings.Style = DrawingStyle.Solid;
      row6.RowSettings.Style = DrawingStyle.Dashed;

      row1.RowSettings.LineType = DataRowType.SingleValue;
      row2.RowSettings.LineType = DataRowType.SingleValue;
      row1.AddValue(12);

      row2.AddValue(5);


      row3.AddValue(2);
      row3.AddValue(5);
      row3.AddValue(9);
      row3.AddValue(1);
      row3.AddValue(3);


      row4.AddValue(10);
      row5.AddValue(11);
      row6.AddValue(11);

      model.AddDataRow(row1);
      model.AddDataRow(row2);
      model.AddDataRow(row3);
      model.AddDataRow(row4);
      model.AddDataRow(row5);
      model.AddDataRow(row6);

      f.ShowDialog();
    }

    [Test]
    public void TestMainForm() {
      MainForm f = new MainForm();
      f.ShowDialog();
    }


    [Test]
    public void TestPointLines() {
      IDataRow row1 = new DataRow();
      row1.RowSettings.Color = Color.Red;
      row1.RowSettings.Thickness = 3;
      row1.RowSettings.Style = DrawingStyle.Dashed;

      row1.RowSettings.LineType = DataRowType.Points;
      model.AddDataRow(row1);

      row1.AddValue(10);
      row1.AddValue(5);
      row1.AddValue(7);
      row1.AddValue(3);
      row1.AddValue(10);
      row1.AddValue(2);

      LineChartTestForm f = new LineChartTestForm(model);
      f.ShowDialog();
    }


  }
}