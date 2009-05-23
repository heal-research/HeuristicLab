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
    public void TestLineChart() {
      LineChartTestForm f = new LineChartTestForm(model);

      IDataRow row1 = new DataRow();
      IDataRow row2 = new DataRow();
      IDataRow row3 = new DataRow();

      row1.Color = Color.Red;
      row2.Color = Color.Green;
      row3.Color = Color.Blue;

      row1.Thickness = 3;
      row2.Thickness = 4;
      row3.Thickness = 5;

      row1.Label = "Simon";
      row2.Label = "Gertschi";
      row3.Label = "Maxi";

      row1.Style = DrawingStyle.Solid;
      row2.Style = DrawingStyle.Solid;
      row3.Style = DrawingStyle.Dashed;


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
      yaxis1.ShowYAxisLabel = true;
      yaxis1.Position = AxisPosition.Left;
      yaxis1.ShowGrid = true;
      yaxis1.GridColor = Color.Gray;

      yaxis2.Label = "Y-Axis 2";
      yaxis2.ShowYAxisLabel = true;
      yaxis2.Position = AxisPosition.Right;
      yaxis2.ShowGrid = false;

      row1.Color = Color.Red;
      row1.Thickness = 3;
      row1.Style = DrawingStyle.Solid;
      row1.Label = "Die Rote";

      row2.Color = Color.Green;
      row2.Thickness = 3;
      row2.Style = DrawingStyle.Solid;
      row2.Label = "Die Grüne";

      row3.Color = Color.Blue;
      row3.Thickness = 3;
      row3.Style = DrawingStyle.Solid;
      row3.Label = "Die Blaue";
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
    public void SimpleTestAggregator() {
      LineChartTestForm f = new LineChartTestForm(model);

      IDataRow row1 = new DataRow { Label = "row", Color = Color.Red, Thickness = 3, Style = DrawingStyle.Solid };

      model.AddDataRow(row1);


      MaxAggregator aggregator = new MaxAggregator {
        Label = "MinAggregator",
        Color = Color.Pink,
        Thickness = 5,
        Style = DrawingStyle.Solid,
        LineType = DataRowType.SingleValue
      };
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
        IDataRow row1 = new DataRow { Color = Color.Red, Thickness = 2, Label = "Sinus", Style = DrawingStyle.Solid, ShowMarkers = false };
        model.AddDataRow(row1);

        IDataRow row2 = new DataRow { Color = Color.Red, Thickness = 3, Label = "Growing", Style = DrawingStyle.Solid, ShowMarkers = false };
        model.AddDataRow(row2);

        AvgAggregator multiAvgAggregator = new AvgAggregator {
          Label = "MultiAvgAggregator",
          Color = Color.DarkOliveGreen,
          Thickness = 3,
          Style = DrawingStyle.Solid,
          LineType = DataRowType.SingleValue,
          ShowMarkers = false
        };
        multiAvgAggregator.AddWatch(row1);
        multiAvgAggregator.AddWatch(row2);
        model.AddDataRow(multiAvgAggregator);

        MaxAggregator multiMaxAggregator = new MaxAggregator {
          Label = "MultiMaxAggregator",
          Color = Color.DarkKhaki,
          Thickness = 3,
          Style = DrawingStyle.Solid,
          LineType = DataRowType.SingleValue,
          ShowMarkers = false
        };
        multiMaxAggregator.AddWatch(row1);
        multiMaxAggregator.AddWatch(row2);
        model.AddDataRow(multiMaxAggregator);

        MinAggregator multiMinAggregator = new MinAggregator {
          Label = "MultiMinAggregator",
          Color = Color.DarkRed,
          Thickness = 3,
          Style = DrawingStyle.Solid,
          LineType = DataRowType.SingleValue,
          ShowMarkers = false
        };
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
        IDataRow row1 = new DataRow {
          Color = Color.Red,
          Thickness = 2,
          Label = "Sinus",
          Style = DrawingStyle.Solid,
          ShowMarkers = false
        };
        model.AddDataRow(row1);

        IDataRow row2 = new DataRow {
          Color = Color.Red,
          Thickness = 3,
          Label = "Growing",
          Style = DrawingStyle.Solid,
          ShowMarkers = false
        };
        model.AddDataRow(row2);

        MinAggregator aggregator = new MinAggregator {
          Label = "MinAggregator",
          Color = Color.Pink,
          Thickness = 3,
          Style = DrawingStyle.Solid,
          LineType = DataRowType.SingleValue
        };
        aggregator.AddWatch(row1);
        model.AddDataRow(aggregator);

        MaxAggregator maxAggregator = new MaxAggregator {
          Label = "MaxAggregator",
          Color = Color.DeepSkyBlue,
          Thickness = 3,
          Style = DrawingStyle.Solid,
          LineType = DataRowType.SingleValue
        };
        maxAggregator.AddWatch(row1);
        model.AddDataRow(maxAggregator);

        AvgAggregator avgAggregator = new AvgAggregator {
          Label = "AvgAggregator",
          Color = Color.Violet,
          Thickness = 3,
          Style = DrawingStyle.Solid,
          LineType = DataRowType.SingleValue
        };
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
        IDataRow row1 = new DataRow {
          Color = Color.Red,
          Thickness = 2,
          Label = "Sinus",
          Style = DrawingStyle.Solid,
          ShowMarkers = false
        };
        model.AddDataRow(row1);

        IDataRow row2 = new DataRow {
          Color = Color.Red,
          Thickness = 3,
          Label = "Growing",
          Style = DrawingStyle.Solid,
          ShowMarkers = false
        };
        model.AddDataRow(row2);

        AvgLineAggregator avgLineAggregator = new AvgLineAggregator {
          Label = "AvgLineAggregator",
          Color = Color.Violet,
          Thickness = 3,
          Style = DrawingStyle.Solid,
          LineType = DataRowType.Normal,
          ShowMarkers = false
        };
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
        IDataRow row1 = new DataRow {
          Color = Color.Red,
          Thickness = 2,
          Label = "SinusHacked",
          Style = DrawingStyle.Solid,
          ShowMarkers = false
        };
        model.AddDataRow(row1);

        IDataRow row2 = new DataRow {
          Color = Color.Red,
          Thickness = 3,
          Label = "GrowingHacked",
          Style = DrawingStyle.Solid,
          ShowMarkers = false
        };
        model.AddDataRow(row2);

        FloatingAvgAggregator avgAggregator = new FloatingAvgAggregator {
                                                                          Thickness = 2,
                                                                          Label = "floatingAvg",
                                                                          Color = Color.Peru,
                                                                          ShowMarkers = false,
                                                                          Style = DrawingStyle.Solid
                                                                        };

        avgAggregator.AddWatch(row1);
        model.AddDataRow(avgAggregator);

        FloatingAvgAggregator avgAggregator2 = new FloatingAvgAggregator {
          Thickness = 2,
          Label = "floatingAvg",
          Color = Color.Aqua,
          ShowMarkers = false,
          Style = DrawingStyle.Solid
        };

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
      IDataRow row1 = new DataRow { Color = Color.Red, Thickness = 3, Style = DrawingStyle.Solid };

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

      row1.Color = Color.Red;
      row2.Color = Color.Green;
      row3.Color = Color.Blue;

      row4.Color = Color.DeepPink;
      row5.Color = Color.Firebrick;
      row6.Color = Color.DarkSlateGray;

      row1.Thickness = 3;
      row2.Thickness = 4;
      row3.Thickness = 5;

      row4.Thickness = 3;
      row5.Thickness = 4;
      row6.Thickness = 5;

      row1.Label = "SingleValue";
      row2.Label = "Gertschi";
      row3.Label = "Maxi";

      row4.Label = "Simon";
      row5.Label = "klausmuellerwesternhagenunddierasperies";
      row6.Label = "anyways";

      row1.Style = DrawingStyle.Solid;
      row2.Style = DrawingStyle.Solid;
      row3.Style = DrawingStyle.Dashed;

      row4.Style = DrawingStyle.Solid;
      row5.Style = DrawingStyle.Solid;
      row6.Style = DrawingStyle.Dashed;

      row1.LineType = DataRowType.SingleValue;
      row2.LineType = DataRowType.SingleValue;
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
      IDataRow row1 = new DataRow { Color = Color.Red, Thickness = 3, Style = DrawingStyle.Dashed };

      row1.LineType = DataRowType.Points;
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