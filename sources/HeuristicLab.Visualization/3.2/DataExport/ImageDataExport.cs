using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace HeuristicLab.Visualization.DataExport {
  public class ImageDataExport : IExporter {
    public string Name {
      get { return "Image Export"; }
    }

    public void Export(IChartDataRowsModel model, LineChart chart) {
      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Filter = "Png Files|*.png|All Files|*.*";
      if (sfd.ShowDialog() != DialogResult.OK) {
        return;
      }

      string filename = sfd.FileName;

      using (Bitmap bmp = chart.Snapshot()) {
        bmp.Save(filename, ImageFormat.Png);
      }
    }
  }
}