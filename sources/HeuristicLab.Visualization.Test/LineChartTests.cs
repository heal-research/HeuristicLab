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
        row1.AddValue(rand.NextDouble()*10);
        row2.AddValue(rand.NextDouble()*10);
        row3.AddValue(rand.NextDouble()*10);
      }

      f.ShowDialog();
    }

    [Test]
    public void TestAxes() {
      LineChartTestForm f = new LineChartTestForm(model);

      IDataRow row1 = new DataRow();
      IDataRow row2 = new DataRow();
      IDataRow row3 = new DataRow();

      YAxisDescriptor yaxis1 = model.DefaultYAxis;
      YAxisDescriptor yaxis2 = new YAxisDescriptor();

      yaxis1.Label = "Y-Axis 1";
      yaxis2.Label = "Y-Axis 2";

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
      row3.YAxis.Label = "Die Blaue";

      model.AddDataRow(row1);
      model.AddDataRow(row2);
      model.AddDataRow(row3);

      row1.YAxis.Label = "Rot und Grün";

      Random rand = new Random(42);

      for (int i = 0; i < 10; i++) {
        row1.AddValue(rand.NextDouble()*10);
        row2.AddValue(rand.NextDouble()*10);
        row3.AddValue(rand.NextDouble()*1);
      }

      f.AddValue += delegate {
        row1.AddValue(rand.NextDouble()*10);
        row2.AddValue(rand.NextDouble()*10);
        row3.AddValue(rand.NextDouble()*1);
      };

      f.ShowDialog();
    }

    [Test]
    public void TestAggregator() {
      LineChartTestForm f = new LineChartTestForm(model);

      IDataRow row1 = new DataRow();
      row1.Label = "row";
      row1.Color = Color.Red;
      row1.Thickness = 3;
      row1.Style = DrawingStyle.Solid;

      model.AddDataRow(row1);


      MinAggregator aggregator = new MinAggregator();
      aggregator.Label = "MinAggregator";
      aggregator.Color = Color.Pink;
      aggregator.Thickness = 5;
      aggregator.Style = DrawingStyle.Solid;
      aggregator.AddValue(2);
      aggregator.LineType = DataRowType.SingleValue;

      IDataRow lineTest = new DataRow("testline");
      lineTest.Color = Color.DarkSalmon;
      lineTest.Thickness = 2;
      lineTest.Style = DrawingStyle.Dashed;
      lineTest.LineType = DataRowType.SingleValue;
      model.AddDataRow(lineTest);
      lineTest.AddValue(9);
      lineTest.AddValue(2);
      lineTest.AddValue(3);
      lineTest.AddValue(4);

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
      private ChartDataRowsModel model;
      public Worker(ChartDataRowsModel model) {
        this.model = model;
      }
      
      public void DoWorkMultiLine() {

        IDataRow row1 = new DataRow();
        row1.Color = Color.Red;
        row1.Thickness = 2;
        row1.Label = "Sinus";
        row1.Style = DrawingStyle.Solid;
        model.AddDataRow(row1);

        IDataRow row2 = new DataRow();
        row2.Color = Color.Red;
        row2.Thickness = 3;
        row2.Label = "Growing";
        row2.Style = DrawingStyle.Solid;
        model.AddDataRow(row2);

        AvgAggregator multiAvgAggregator = new AvgAggregator();
        multiAvgAggregator.Label = "MultiAvgAggregator";
        multiAvgAggregator.Color = Color.DarkOliveGreen;
        multiAvgAggregator.Thickness = 3;
        multiAvgAggregator.Style = DrawingStyle.Solid;
        multiAvgAggregator.LineType = DataRowType.SingleValue;
        multiAvgAggregator.AddWatch(row1);
        multiAvgAggregator.AddWatch(row2);
        model.AddDataRow(multiAvgAggregator);

        MaxAggregator multiMaxAggregator = new MaxAggregator();
        multiMaxAggregator.Label = "MultiMaxAggregator";
        multiMaxAggregator.Color = Color.DarkKhaki;
        multiMaxAggregator.Thickness = 3;
        multiMaxAggregator.Style = DrawingStyle.Solid;
        multiMaxAggregator.LineType = DataRowType.SingleValue;
        multiMaxAggregator.AddWatch(row1);
        multiMaxAggregator.AddWatch(row2);
        model.AddDataRow(multiMaxAggregator);

        MinAggregator multiMinAggregator = new MinAggregator();
        multiMinAggregator.Label = "MultiMinAggregator";
        multiMinAggregator.Color = Color.DarkRed;
        multiMinAggregator.Thickness = 3;
        multiMinAggregator.Style = DrawingStyle.Solid;
        multiMinAggregator.LineType = DataRowType.SingleValue;
        multiMinAggregator.AddWatch(row1);
        multiMinAggregator.AddWatch(row2);
        model.AddDataRow(multiMinAggregator);

        AvgLineAggregator multiLineAvgAggregator = new AvgLineAggregator();
        multiLineAvgAggregator.Label = "MultiLineAvgAggregator";
        multiLineAvgAggregator.Color = Color.Red;
        multiLineAvgAggregator.Thickness = 4;
        multiLineAvgAggregator.Style = DrawingStyle.Solid;
        multiLineAvgAggregator.LineType = DataRowType.Normal;
        multiLineAvgAggregator.AddWatch(row1);
        multiLineAvgAggregator.AddWatch(row2);
        multiLineAvgAggregator.AddValue(0);
        model.AddDataRow(multiLineAvgAggregator);

        double i = 0;
        double newY;

        Random rand = new Random();
        while (!_shouldStop && i <= 24) {
          i += 0.2;
          newY = Math.Sin(i);
          System.Console.WriteLine("working");
          //row1.AddValue(rand.NextDouble() * 10);
          row1.AddValue(newY * 10);
          row2.AddValue(i*2-15);
          System.Threading.Thread.Sleep(100);
        }
        Console.WriteLine("worker thread: terminating gracefully.");
      }
      public void DoWorkSingleLine() {

        IDataRow row1 = new DataRow();
        row1.Color = Color.Red;
        row1.Thickness = 2;
        row1.Label = "Sinus";
        row1.Style = DrawingStyle.Solid;
        model.AddDataRow(row1);

        IDataRow row2 = new DataRow();
        row2.Color = Color.Red;
        row2.Thickness = 3;
        row2.Label = "Growing";
        row2.Style = DrawingStyle.Solid;
        model.AddDataRow(row2);

        MinAggregator aggregator = new MinAggregator();
        aggregator.Label = "MinAggregator";
        aggregator.Color = Color.Pink;
        aggregator.Thickness = 3;
        aggregator.Style = DrawingStyle.Solid;
        aggregator.LineType = DataRowType.SingleValue;
        aggregator.AddWatch(row1);
        model.AddDataRow(aggregator);

        MaxAggregator maxAggregator = new MaxAggregator();
        maxAggregator.Label = "MaxAggregator";
        maxAggregator.Color = Color.DeepSkyBlue;
        maxAggregator.Thickness = 3;
        maxAggregator.Style = DrawingStyle.Solid;
        maxAggregator.LineType = DataRowType.SingleValue;
        maxAggregator.AddWatch(row1);
        model.AddDataRow(maxAggregator);

        AvgAggregator avgAggregator = new AvgAggregator();
        avgAggregator.Label = "AvgAggregator";
        avgAggregator.Color = Color.Violet;
        avgAggregator.Thickness = 3;
        avgAggregator.Style = DrawingStyle.Solid;
        avgAggregator.LineType = DataRowType.SingleValue;
        avgAggregator.AddWatch(row1);
        model.AddDataRow(avgAggregator);

        double i = 0;
        double newY;

        Random rand = new Random();
        while (!_shouldStop && i <= 24) {
          i += 0.2;
          newY = Math.Sin(i);
          System.Console.WriteLine("working");
          //row1.AddValue(rand.NextDouble() * 10);
          row1.AddValue(newY * 10);
          row2.AddValue(i * 2 - 15);
          System.Threading.Thread.Sleep(100);
        }
        Console.WriteLine("worker thread: terminating gracefully.");
      }
      public void DoWorkAvgLine() {

        IDataRow row1 = new DataRow();
        row1.Color = Color.Red;
        row1.Thickness = 2;
        row1.Label = "Sinus";
        row1.Style = DrawingStyle.Solid;
        model.AddDataRow(row1);

        IDataRow row2 = new DataRow();
        row2.Color = Color.Red;
        row2.Thickness = 3;
        row2.Label = "Growing";
        row2.Style = DrawingStyle.Solid;
        model.AddDataRow(row2);

        AvgLineAggregator avgLineAggregator = new AvgLineAggregator();
        avgLineAggregator.Label = "AvgLineAggregator";
        avgLineAggregator.Color = Color.Violet;
        avgLineAggregator.Thickness = 3;
        avgLineAggregator.Style = DrawingStyle.Solid;
        avgLineAggregator.LineType = DataRowType.Normal;
        avgLineAggregator.AddWatch(row1);
        avgLineAggregator.AddWatch(row2);
        model.AddDataRow(avgLineAggregator);

        double i = 0;
        double newY;

        Random rand = new Random();
        while (!_shouldStop && i <= 24) {
          i += 0.2;
          newY = Math.Sin(i);
          System.Console.WriteLine("working");
          //row1.AddValue(rand.NextDouble() * 10);
          row1.AddValue(newY * 10);
          row2.AddValue(i * 2 - 15);
          System.Threading.Thread.Sleep(100);
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

      // Create the thread object. This does not start the thread.
      Worker workerObject = new Worker(model);
      Thread workerThread = new Thread(workerObject.DoWorkAvgLine);

      // Start the worker thread.
      workerThread.Start();

      f.ShowDialog();
      workerObject.RequestStop();
    }

    

    [Test]
    public void TestAutoZoomInConstructor() {
      IDataRow row1 = new DataRow();

      row1.Color = Color.Red;
      row1.Thickness = 3;
      row1.Style = DrawingStyle.Solid;

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
  }
}
