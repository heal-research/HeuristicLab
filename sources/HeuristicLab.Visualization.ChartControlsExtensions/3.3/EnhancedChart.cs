#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
namespace HeuristicLab.Visualization.ChartControlsExtensions {
  public partial class EnhancedChart : Chart {
    public EnhancedChart()
      : base() {
    }

    private void saveImageToolStripMenuItem_Click(object sender, System.EventArgs e) {
      SaveFileDialog saveFileDialog = new SaveFileDialog();

      // Sets the current file name filter string, which determines 
      // the choices that appear in the "Save as file type" or 
      // "Files of type" box in the dialog box.
      saveFileDialog.Filter = "Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|EMF (*.emf)|*.emf|PNG (*.png)|*.png|SVG (*.svg)|*.svg|GIF (*.gif)|*.gif|TIFF (*.tif)|*.tif";
      saveFileDialog.FilterIndex = 2;
      saveFileDialog.RestoreDirectory = true;

      // Set image file format
      if (saveFileDialog.ShowDialog() == DialogResult.OK) {
        ChartImageFormat format = ChartImageFormat.Bmp;

        if (saveFileDialog.FileName.EndsWith("bmp")) {
          format = ChartImageFormat.Bmp;
        } else if (saveFileDialog.FileName.EndsWith("jpg")) {
          format = ChartImageFormat.Jpeg;
        } else if (saveFileDialog.FileName.EndsWith("emf")) {
          format = ChartImageFormat.EmfDual;
        } else if (saveFileDialog.FileName.EndsWith("gif")) {
          format = ChartImageFormat.Gif;
        } else if (saveFileDialog.FileName.EndsWith("png")) {
          format = ChartImageFormat.Png;
        } else if (saveFileDialog.FileName.EndsWith("tif")) {
          format = ChartImageFormat.Tiff;
        }

        // Save image
        SaveImage(saveFileDialog.FileName, format);
      }
    }

    private void copyImageToClipboardToolStripMenuItem_Click(object sender, System.EventArgs e) {
      System.IO.MemoryStream stream = new System.IO.MemoryStream();
      SaveImage(stream, System.Drawing.Imaging.ImageFormat.Bmp);
      Bitmap bmp = new Bitmap(stream);
      Clipboard.SetDataObject(bmp);
    }
  }
}
