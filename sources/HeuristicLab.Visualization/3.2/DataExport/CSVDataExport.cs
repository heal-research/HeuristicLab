using System;
using System.IO;
using System.Windows.Forms;

namespace HeuristicLab.Visualization.DataExport {
  public class CSVDataExport : IExporter {
    public string Name {
      get { return "CSV Export"; }
    }

    public void Export(IChartDataRowsModel model) {
      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Filter = "CSV Files|*.csv|All Files|*.*";
      if (sfd.ShowDialog() != DialogResult.OK) {
        return;
      }

      string filename = sfd.FileName;

      Export(model, filename);
    }

    private static void Export(IChartDataRowsModel model, string filename) {
      using (FileStream fs = new FileStream(filename, FileMode.Create))
      using (StreamWriter sw = new StreamWriter(fs)) {
        CSVWriter writer = new CSVWriter(sw);

        // write headers
        writer.AddString(model.XAxis.XAxisLabel);
        foreach (IDataRow row in model.Rows)
          writer.AddString(row.Label);
        writer.NewLine();

        // figure out max number of rows in all data rows
        int maxItems = 0;
        foreach (IDataRow row in model.Rows)
          maxItems = Math.Max(maxItems, row.Count);

        // write rows
        for (int i = 0; i < maxItems; i++) {
          writer.AddNumber(i);
          foreach (IDataRow row in model.Rows) {
            if (i < row.Count)
              writer.AddNumber(row[i]);
            else
              writer.AddEmpty();
          }
          writer.NewLine();
        }
      }
    }
  }
}