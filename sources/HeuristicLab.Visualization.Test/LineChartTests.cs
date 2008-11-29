using NUnit.Framework;

namespace HeuristicLab.Visualization.Test {
  [TestFixture]
  public class LineChartTests {
    [Test]
    public void TestLineChart() {
      LineChartTestForm f = new LineChartTestForm();

      IDataRow row = new DataRow();
      f.Model.AddDataRow(row);

      row.AddValue(10);
      row.AddValue(5);
      row.AddValue(7);
      row.AddValue(3);
      row.AddValue(10);
      row.AddValue(2);

      f.ShowDialog();
    }
  }
}