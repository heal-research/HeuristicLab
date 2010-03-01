#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Permutation;
using HeuristicLab.Charting;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.TSP {
  /// <summary>
  /// Class for the visual representation of a <see cref="TSPTour"/>.
  /// </summary>
  [Content(typeof(TSPTour), true)]
  public partial class TSPTourView : ItemViewBase {
    /// <summary>
    /// Gets or sets the <see cref="TSPTour"/> to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public TSPTour TSPTour {
      get { return (TSPTour)base.Item; }
      set { base.Item = value; }
    }


    /// <summary>
    /// Initializes a new instance of <see cref="TSPTourView"/> with caption "TSP Tour View".
    /// </summary>
    public TSPTourView() {
      InitializeComponent();
      Caption = "TSP Tour View";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="TSPTourView"/> with the given <paramref name="tspTour"/>.
    /// </summary>
    /// <param name="tspTour">The tour to display.</param>
    public TSPTourView(TSPTour tspTour)
      : this() {
      TSPTour = tspTour;
    }


    /// <summary>
    /// Removes all event handlers from the underlying <see cref="TSPTour"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks> 
    protected override void RemoveItemEvents() {
      TSPTour.CoordinatesChanged -= new EventHandler(TSPTour_CoordinatesChanged);
      TSPTour.TourChanged -= new EventHandler(TSPTour_TourChanged);
      base.RemoveItemEvents();
    }
    /// <summary>
    /// Adds event handlers to the underlying <see cref="TSPTour"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      TSPTour.CoordinatesChanged += new EventHandler(TSPTour_CoordinatesChanged);
      TSPTour.TourChanged += new EventHandler(TSPTour_TourChanged);
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.UpdateControls"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void UpdateControls() {
      base.UpdateControls();
      Chart chart = new Chart(0, 0, 10, 10);
      chartControl.ScaleOnResize = false;

      if ((TSPTour.Coordinates != null) && (TSPTour.Tour != null)) {
        chart.UpdateEnabled = false;

        double[,] coords = TSPTour.Coordinates.Data;
        int[] tour = TSPTour.Tour.Data;

        for (int i = 0; i < tour.Length - 1; i++)
          chart.Group.Add(new Line(chart, coords[tour[i], 0], coords[tour[i], 1],
                                          coords[tour[i + 1], 0], coords[tour[i + 1], 1], Pens.Black));
        chart.Group.Add(new Line(chart, coords[tour[tour.Length - 1], 0], coords[tour[tour.Length - 1], 1],
                                        coords[tour[0], 0], coords[tour[0], 1], Pens.Black));

        for (int i = 0; i < coords.GetLength(0); i++) {
          FixedSizeCircle circle = new FixedSizeCircle(chart, coords[i, 0], coords[i, 1], 8, Pens.Black, Brushes.Blue);
          circle.ToolTipText = "(" + coords[i, 0].ToString() + " ; " + coords[i, 1].ToString() + ")";
          chart.Group.Add(circle);
        }

        if (coords.GetLength(0) > 0) {
          PointD min = new PointD(coords[0, 0], coords[0, 1]);
          PointD max = new PointD(coords[0, 0], coords[0, 1]);
          for (int i = 0; i < coords.GetLength(0); i++) {
            if (coords[i, 0] < min.X) min.X = coords[i, 0];
            if (coords[i, 1] < min.Y) min.Y = coords[i, 1];
            if (coords[i, 0] > max.X) max.X = coords[i, 0];
            if (coords[i, 1] > max.Y) max.Y = coords[i, 1];
          }
          Offset offset = new Offset((max.X - min.X) / 20, (max.Y - min.Y) / 20);
          min = min - offset;
          max = max + offset;
          chart.SetPosition(min, max);
        }
        chart.UpdateEnabled = true;
      }
      chartControl.Chart = chart;
    }

    #region TSPTour Events
    private void TSPTour_TourChanged(object sender, EventArgs e) {
      Refresh();
    }
    private void TSPTour_CoordinatesChanged(object sender, EventArgs e) {
      Refresh();
    }
    #endregion
  }
}
