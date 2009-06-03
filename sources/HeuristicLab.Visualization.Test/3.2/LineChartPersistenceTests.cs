using System;
using System.Drawing;
using System.IO;
using HeuristicLab.Core;
using NUnit.Framework;

namespace HeuristicLab.Visualization.Test {
  [TestFixture]
  public class LineChartPersistenceTests {
    [Test]
    public void TestPersistence() {
      // setup
      ChartDataRowsModel model = new ChartDataRowsModel();

      model.Title = "model";

      model.XAxis.Label = "xaxis";
      model.XAxis.Color = Color.FromArgb(1, 2, 3, 4);
      model.XAxis.GridColor = Color.FromArgb(1, 2, 3, 4);
      model.XAxis.ShowGrid = true;
      model.XAxis.ShowLabel = true;

      IDataRow row1 = new DataRow();
      
      row1.RowSettings.Label = "row1";
      row1.RowSettings.Color = Color.FromArgb(1, 2, 3, 4);
      row1.RowSettings.LineType = DataRowType.Normal;
      row1.RowSettings.ShowMarkers = true;
      row1.RowSettings.Style = DrawingStyle.Dashed;
      row1.RowSettings.Thickness = 5;
      
      row1.AddValues(new double[] {0, 10.3, 5});

      row1.YAxis = model.DefaultYAxis;
      row1.YAxis.ClipChangeable = true;
      row1.YAxis.GridColor = Color.FromArgb(1, 2, 3, 4);
      row1.YAxis.Label = "yaxis label";
      row1.YAxis.Position = AxisPosition.Right;
      row1.YAxis.ShowGrid = true;
      row1.YAxis.ShowYAxis = true;
      row1.YAxis.ShowYAxisLabel = true;

      model.AddDataRow(row1);

      IDataRow row2 = new DataRow();
      
      row2.RowSettings.Label = "row2";
      row2.RowSettings.Color = Color.FromArgb(5, 6, 7, 8);
      row2.RowSettings.LineType = DataRowType.Points;
      row2.RowSettings.ShowMarkers = false;
      row2.RowSettings.Style = DrawingStyle.Solid;
      row2.RowSettings.Thickness = 6;

      row2.AddValues(new double[] { 1, 2, 3 });

      row2.YAxis = new YAxisDescriptor();
      row2.YAxis.ClipChangeable = false;
      row2.YAxis.GridColor = Color.FromArgb(4, 3, 2, 1);
      row2.YAxis.Label = "yaxis2 label";
      row2.YAxis.Position = AxisPosition.Left;
      row2.YAxis.ShowGrid = false;
      row2.YAxis.ShowYAxis = false;
      row2.YAxis.ShowYAxisLabel = false;

      model.AddDataRow(row2);

      // execute
      MemoryStream storage = new MemoryStream();
      PersistenceManager.Save(model, storage);
      
      Console.Out.WriteLine(new StreamReader(new MemoryStream(storage.ToArray())).ReadToEnd());

      storage.Position = 0;
      ChartDataRowsModel restoredModel = (ChartDataRowsModel)PersistenceManager.Load(new MemoryStream(storage.ToArray()));

      // verify
      Assert.AreEqual("model", restoredModel.Title);

      Assert.AreEqual(Color.FromArgb(1, 2, 3, 4), restoredModel.XAxis.Color);
      Assert.AreEqual(Color.FromArgb(1, 2, 3, 4), restoredModel.XAxis.GridColor);
      Assert.AreEqual(Color.FromArgb(1, 2, 3, 4), restoredModel.XAxis.GridColor);
      Assert.AreEqual("xaxis", restoredModel.XAxis.Label);
      Assert.AreEqual(true, restoredModel.XAxis.ShowGrid);
      Assert.AreEqual(true, restoredModel.XAxis.ShowLabel);

      Assert.AreEqual(2, restoredModel.Rows.Count);

      // row 1
      Assert.AreEqual("row1", restoredModel.Rows[0].RowSettings.Label);
      Assert.AreEqual(Color.FromArgb(1, 2, 3, 4), restoredModel.Rows[0].RowSettings.Color);
      Assert.AreEqual(DataRowType.Normal, restoredModel.Rows[0].RowSettings.LineType);
      Assert.AreEqual(true, restoredModel.Rows[0].RowSettings.ShowMarkers);
      Assert.AreEqual(DrawingStyle.Dashed, restoredModel.Rows[0].RowSettings.Style);
      Assert.AreEqual(5, restoredModel.Rows[0].RowSettings.Thickness);

      Assert.AreEqual(3, restoredModel.Rows[0].Count);
      Assert.AreEqual(0, restoredModel.Rows[0][0]);
      Assert.AreEqual(10.3, restoredModel.Rows[0][1]);
      Assert.AreEqual(5, restoredModel.Rows[0][2]);
      Assert.AreEqual(0, restoredModel.Rows[0].MinValue);
      Assert.AreEqual(10.3, restoredModel.Rows[0].MaxValue);

      Assert.AreEqual(true, restoredModel.Rows[0].YAxis.ClipChangeable);
      Assert.AreEqual(Color.FromArgb(1, 2, 3, 4), restoredModel.Rows[0].YAxis.GridColor);
      Assert.AreEqual("yaxis label", restoredModel.Rows[0].YAxis.Label);
      Assert.AreEqual(AxisPosition.Right, restoredModel.Rows[0].YAxis.Position);
      Assert.AreEqual(true, restoredModel.Rows[0].YAxis.ShowGrid);
      Assert.AreEqual(true, restoredModel.Rows[0].YAxis.ShowYAxis);
      Assert.AreEqual(true, restoredModel.Rows[0].YAxis.ShowYAxisLabel);

      // row 2
      Assert.AreEqual("row2", restoredModel.Rows[1].RowSettings.Label);
      Assert.AreEqual(Color.FromArgb(5, 6, 7, 8), restoredModel.Rows[1].RowSettings.Color);
      Assert.AreEqual(DataRowType.Points, restoredModel.Rows[1].RowSettings.LineType);
      Assert.AreEqual(false, restoredModel.Rows[1].RowSettings.ShowMarkers);
      Assert.AreEqual(DrawingStyle.Solid, restoredModel.Rows[1].RowSettings.Style);
      Assert.AreEqual(6, restoredModel.Rows[1].RowSettings.Thickness);

      Assert.AreEqual(3, restoredModel.Rows[1].Count);
      Assert.AreEqual(1, restoredModel.Rows[1][0]);
      Assert.AreEqual(2, restoredModel.Rows[1][1]);
      Assert.AreEqual(3, restoredModel.Rows[1][2]);
      Assert.AreEqual(1, restoredModel.Rows[1].MinValue);
      Assert.AreEqual(3, restoredModel.Rows[1].MaxValue);

      Assert.AreEqual(false, restoredModel.Rows[1].YAxis.ClipChangeable);
      Assert.AreEqual(Color.FromArgb(4, 3, 2, 1), restoredModel.Rows[1].YAxis.GridColor);
      Assert.AreEqual("yaxis2 label", restoredModel.Rows[1].YAxis.Label);
      Assert.AreEqual(AxisPosition.Left, restoredModel.Rows[1].YAxis.Position);
      Assert.AreEqual(false, restoredModel.Rows[1].YAxis.ShowGrid);
      Assert.AreEqual(false, restoredModel.Rows[1].YAxis.ShowYAxis);
      Assert.AreEqual(false, restoredModel.Rows[1].YAxis.ShowYAxisLabel);
    }
  }
}