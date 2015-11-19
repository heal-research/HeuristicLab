#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using System.ComponentModel;
using System.Drawing;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Data.Views {

  [View("BoolMatrixView")]
  [Content(typeof(BoolMatrix), IsDefaultView = false)]
  public sealed partial class BoolMatrixView : ItemView {
    private BackgroundWorker worker;

    public new BoolMatrix Content {
      get { return (BoolMatrix)base.Content; }
      set { base.Content = value; }
    }

    public BoolMatrixView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.ToStringChanged -= Content_ToStringChanged;
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ToStringChanged += Content_ToStringChanged;
    }

    private void Content_ToStringChanged(object sender, EventArgs e) {
      Rebuild();
    }

    private void Rebuild() {
      int width = pictureBox.Width;
      int height = pictureBox.Height;
      int tileWidth = width / Content.Columns;
      int tileHeight = height / Content.Rows;
      int border = (int)Math.Round((tileWidth + tileHeight) / 2.0 * 0.05);
      var bitmap = new Bitmap(width, height);

      if (worker != null)
        worker.CancelAsync();

      if (tileWidth == 0 || tileHeight == 0) {
        using (Graphics g = Graphics.FromImage(bitmap)) {
          g.DrawString("BoolMatrix is too big to draw.", DefaultFont, new SolidBrush(Color.Black), 10.0f, height / 2.0f);
          g.Flush();
        }
        pictureBox.Image = bitmap;
        return;
      }

      worker = new BackgroundWorker();
      worker.WorkerSupportsCancellation = true;
      worker.DoWork += (s, a) => {
        using (Graphics g = Graphics.FromImage(bitmap)) {
          for (int i = 0; i < Content.Rows; i++) {
            for (int j = 0; j < Content.Columns; j++) {
              if (worker.CancellationPending) {
                a.Cancel = true;
                break;
              }
              g.FillRectangle(Content[i, j] ? Brushes.Black : Brushes.White, i * tileWidth, j * tileHeight,
                                                                    tileWidth - border, tileHeight - border);
            }
          }
          g.Flush();
        }
      };
      worker.RunWorkerCompleted += (s, a) => {
        if (!a.Cancelled)
          pictureBox.Image = bitmap;
        if (!worker.CancellationPending)
          worker = null;
      };
      worker.RunWorkerAsync();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        pictureBox.Image = new Bitmap(1, 1);
      } else {
        Rebuild();
      }
    }
  }
}
