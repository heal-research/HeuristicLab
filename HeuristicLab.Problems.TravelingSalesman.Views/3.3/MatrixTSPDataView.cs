#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.TravelingSalesman.Views {
  [View("Matrix TSP Data View")]
  [Content(typeof(MatrixTSPData), IsDefaultView = true)]
  public partial class MatrixTSPDataView : ItemView {

    public new MatrixTSPData Content {
      get { return (MatrixTSPData)base.Content; }
      set { base.Content = value; }
    }

    public MatrixTSPDataView() {
      InitializeComponent();
      setMatrixButton.Text = string.Empty;
      setMatrixButton.Image = VSImageLibrary.Edit;
      setCoordinatesButton.Text = string.Empty;
      setCoordinatesButton.Image = VSImageLibrary.Edit;
    }

    #region Content Events
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.PropertyChanged += ContentOnPropertyChanged;
      if (Content.DisplayCoordinates != null) {
        Content.DisplayCoordinates.Reset += ContentDisplayCoordinatesOnChanged;
        Content.DisplayCoordinates.ItemChanged += ContentDisplayCoordinatesOnChanged;
      }
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.PropertyChanged -= ContentOnPropertyChanged;
      if (Content.DisplayCoordinates != null) {
        Content.DisplayCoordinates.Reset -= ContentDisplayCoordinatesOnChanged;
        Content.DisplayCoordinates.ItemChanged -= ContentDisplayCoordinatesOnChanged;
      }
    }

    private void ContentOnPropertyChanged(object sender, PropertyChangedEventArgs e) {
      switch (e.PropertyName) {
        case nameof(Content.Matrix):
          distanceMatrixView.Content = Content.Matrix;
          break;
        case nameof(Content.DisplayCoordinates):
          if (coordinatesMatrixView.Content != null) {
            coordinatesMatrixView.Content.Reset -= ContentDisplayCoordinatesOnChanged;
            coordinatesMatrixView.Content.ItemChanged -= ContentDisplayCoordinatesOnChanged;
          }
          if (Content.DisplayCoordinates != null) {
            Content.DisplayCoordinates.Reset += ContentDisplayCoordinatesOnChanged;
            Content.DisplayCoordinates.ItemChanged += ContentDisplayCoordinatesOnChanged;
          }
          coordinatesMatrixView.Content = Content.DisplayCoordinates;
          GenerateImage();
          break;
      }
    }

    private void ContentDisplayCoordinatesOnChanged(object sender, EventArgs e) {
      GenerateImage();
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        distanceMatrixView.Content = null;
        coordinatesMatrixView.Content = null;
        coordinatesPictureBox.Image = null;
      } else {
        distanceMatrixView.Content = Content.Matrix;
        coordinatesMatrixView.Content = Content.DisplayCoordinates;
        GenerateImage();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      setMatrixButton.Enabled = !ReadOnly && !Locked && Content != null;
      setCoordinatesButton.Enabled = !ReadOnly && !Locked && Content != null;
      distanceMatrixView.Enabled = !ReadOnly && !Locked && Content != null;
      coordinatesMatrixView.Enabled = !ReadOnly && !Locked && Content != null;
    }

    private void GenerateImage() {
      if (coordinatesPictureBox.Width > 0 && coordinatesPictureBox.Height > 0) {
        DoubleMatrix coordinates = Content?.DisplayCoordinates;
        var bitmap = new Bitmap(coordinatesPictureBox.Width, coordinatesPictureBox.Height);

        if ((coordinates != null) && (coordinates.Rows > 0) && (coordinates.Columns == 2)) {
          double xMin = double.MaxValue, yMin = double.MaxValue, xMax = double.MinValue, yMax = double.MinValue;
          for (int i = 0; i < coordinates.Rows; i++) {
            if (xMin > coordinates[i, 0]) xMin = coordinates[i, 0];
            if (yMin > coordinates[i, 1]) yMin = coordinates[i, 1];
            if (xMax < coordinates[i, 0]) xMax = coordinates[i, 0];
            if (yMax < coordinates[i, 1]) yMax = coordinates[i, 1];
          }

          int border = 20;
          double xStep = xMax != xMin ? (coordinatesPictureBox.Width - 2 * border) / (xMax - xMin) : 1;
          double yStep = yMax != yMin ? (coordinatesPictureBox.Height - 2 * border) / (yMax - yMin) : 1;

          Point[] points = new Point[coordinates.Rows];
          for (int i = 0; i < coordinates.Rows; i++)
            points[i] = new Point(border + ((int)((coordinates[i, 0] - xMin) * xStep)),
                                  bitmap.Height - (border + ((int)((coordinates[i, 1] - yMin) * yStep))));

          using (Graphics graphics = Graphics.FromImage(bitmap)) {
            for (int i = 0; i < points.Length; i++)
              graphics.FillRectangle(Brushes.Red, points[i].X - 2, points[i].Y - 2, 6, 6);
          }
        } else {
          using (Graphics graphics = Graphics.FromImage(bitmap)) {
            graphics.Clear(Color.White);
            Font font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular);
            string text = "No coordinates defined or in wrong format.";
            SizeF strSize = graphics.MeasureString(text, font);
            graphics.DrawString(text, font, Brushes.Black, (float)(coordinatesPictureBox.Width - strSize.Width) / 2.0f, (float)(coordinatesPictureBox.Height - strSize.Height) / 2.0f);
          }
        }
        coordinatesPictureBox.Image = bitmap;
      }
    }

    private void SetCoordinatesButton_Click(object sender, EventArgs e) {
      if (Content.DisplayCoordinates != null) {
        Content.DisplayCoordinates.Reset -= ContentDisplayCoordinatesOnChanged;
        Content.DisplayCoordinates.ItemChanged -= ContentDisplayCoordinatesOnChanged;
      }
      var items = Math.Max(Content.Matrix.Rows, 1);
      Content.DisplayCoordinates = new DoubleMatrix(items, 2);
    }

    private void SetMatrixButton_Click(object sender, EventArgs e) {
      MessageBox.Show("Not yet implemented");
    }
  }
}
