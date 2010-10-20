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

using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Analysis.Views {
  [View("AlleleFrequencyArray View")]
  [Content(typeof(AlleleFrequencyArray), true)]
  public partial class AlleleFrequencyArrayView : ItemView {
    public new AlleleFrequencyArray Content {
      get { return (AlleleFrequencyArray)base.Content; }
      set { base.Content = value; }
    }

    public AlleleFrequencyArrayView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      // ...
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      // ...
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      chart.Titles.Clear();
      chart.Series.Clear();
      chart.DataSource = null;
      if (Content != null)
        PopulateChart();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      chart.Enabled = Content != null;
    }

    private void PopulateChart() {
      chart.Titles.Add(new Title("Allele Frequencies", Docking.Top));
      chart.DataSource = Content;
      Series frequencies = new Series("Frequencies");
      frequencies.ChartType = SeriesChartType.Column;
      frequencies.XValueMember = "Id";
      frequencies.XValueType = ChartValueType.String;
      frequencies.YValueMembers = "Frequency";
      frequencies.YValueType = ChartValueType.Double;
      frequencies.YAxisType = AxisType.Primary;
      frequencies.ToolTip = "X = #INDEX, Y = #VAL";
      chart.Series.Add(frequencies);
      Series qualities = new Series("Average Solution Qualities");
      qualities.ChartType = SeriesChartType.FastLine;
      qualities.XValueMember = "Id";
      qualities.XValueType = ChartValueType.String;
      qualities.YValueMembers = "AverageSolutionQuality";
      qualities.YValueType = ChartValueType.Double;
      qualities.YAxisType = AxisType.Secondary;
      qualities.ToolTip = "X = #INDEX, Y = #VAL";
      chart.Series.Add(qualities);
    }
  }
}
