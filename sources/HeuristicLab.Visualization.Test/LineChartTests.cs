using System;
using System.Drawing;
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

      f.ShowDialog();
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
  }
}