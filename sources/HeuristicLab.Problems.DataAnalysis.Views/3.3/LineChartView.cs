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
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis {
  [View("Line Chart View")]
  [Content(typeof(DataAnalysisSolution))]
  public partial class LineChartView : ContentView {
    
    public LineChartView()
      : base() {
      InitializeComponent();
      this.Caption = "Line Chart View";
      //configure axis
      this.chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
      this.chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
      this.chart.ChartAreas[0].CursorX.Interval = 0;

      this.chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
      this.chart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
      this.chart.ChartAreas[0].CursorY.Interval = 0;      
    }

    public LineChartView(DataAnalysisSolution dataAnalysisSolution)
      : this() {
      
    }

    private void model_Changed(object sender, EventArgs e) {
      if (InvokeRequired) {
        Action<object, EventArgs> action = new Action<object, EventArgs>(model_Changed);
        this.Invoke(action, sender, e);
      } else {
        IVisualModel model = (IVisualModel)sender;
        Series s = this.chart.Series.Single(x => x.Tag == model);
        s.Points.DataBindY(model.PredictedValues.ToArray());
        s.LegendText = model.ModelName;
        this.UpdateStripLines();
      }
    }

    protected override void ModelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
      base.ModelsCollectionChanged(sender, e);
      if (InvokeRequired) {
        Action<object, NotifyCollectionChangedEventArgs> action = new Action<object, NotifyCollectionChangedEventArgs>(ModelsCollectionChanged);
        this.Invoke(action, sender, e);
      } else {
        if (e.Action == NotifyCollectionChangedAction.Remove) {
          foreach (IVisualModel model in e.OldItems) {
            if (this.chart.Series.Any(x => x.Tag == model))
              this.RemoveModel(model);
          }
          this.UpdateStripLines();
        } else if (e.Action == NotifyCollectionChangedAction.Reset)
          this.RemoveAllModels();
      }
    }

    private void AddModel(IVisualModel model) {
      if (this.targetVariableName != model.TargetVariableName) {
        this.RemoveAllModels();
        this.chart.Series.Clear();
        this.chart.Series.Add(TARGETVARIABLE);
        this.chart.Series[TARGETVARIABLE].LegendText = model.TargetVariableName;
        this.chart.Series[TARGETVARIABLE].ChartType = SeriesChartType.FastLine;
        this.chart.Series[TARGETVARIABLE].Points.DataBindY(model.Dataset.GetVariableValues(model.TargetVariableName));
        this.targetVariableName = model.TargetVariableName;
        this.Caption = this.targetVariableName + " Model Line Chart";
      }
      string seriesName = model.GetHashCode().ToString();
      this.chart.Series.Add(seriesName);
      this.chart.Series[seriesName].Tag = model;
      this.chart.Series[seriesName].LegendText = model.ModelName;
      this.chart.Series[seriesName].ChartType = SeriesChartType.FastLine;
      this.chart.Series[seriesName].Points.DataBindY(model.PredictedValues.ToArray());
      model.Changed += new EventHandler(model_Changed);
      this.UpdateStripLines();
    }

    private void RemoveModel(IVisualModel model) {
      Series s = this.chart.Series.Single(x => x.Tag == model);
      this.chart.Series.Remove(s);
      model.Changed -= new EventHandler(model_Changed);
    }

    private void RemoveAllModels() {
      var models = (from s in chart.Series
                    let m = s.Tag as IVisualModel
                    where m != null
                    select m).ToArray(); // select and copy currently displayed models
      foreach (var m in models) {
        this.RemoveModel(m);
      }
      this.UpdateStripLines();
    }

    private void chart_MouseDown(object sender, MouseEventArgs e) {
      HitTestResult result = this.chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem) {
        if (result.Series.Name != TARGETVARIABLE) {
          this.RemoveModel(result.Series.Tag as IVisualModel);
          this.UpdateStripLines();
        }
      }
    }

    private void chart_MouseMove(object sender, MouseEventArgs e) {
      HitTestResult result = this.chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem) {
        if (result.Series.Name != TARGETVARIABLE)
          this.Cursor = Cursors.Hand;
      } else
        this.Cursor = Cursors.Default;
    }

    private void UpdateStripLines() {
      this.chart.ChartAreas[0].AxisX.StripLines.Clear();
      IEnumerable<IVisualModel> visualModels = from Series s in this.chart.Series
                                               where s.Tag is IVisualModel
                                               select (IVisualModel)s.Tag;
      if (visualModels.Count() > 0) {
        IVisualModel model = visualModels.ElementAt(0);
        if (visualModels.All(x => x.TrainingSamplesStart == model.TrainingSamplesStart && x.TrainingSamplesEnd == model.TrainingSamplesEnd))
          this.CreateAndAddStripLine("Training", Color.FromArgb(20, Color.Green), model.TrainingSamplesStart, model.TrainingSamplesEnd);
        if (visualModels.All(x => x.ValidationSamplesStart == model.ValidationSamplesStart && x.ValidationSamplesEnd == model.ValidationSamplesEnd))
          this.CreateAndAddStripLine("Validation", Color.FromArgb(20, Color.Yellow), model.ValidationSamplesStart, model.ValidationSamplesEnd);
        if (visualModels.All(x => x.TestSamplesStart == model.TestSamplesStart && x.TestSamplesEnd == model.TestSamplesEnd))
          this.CreateAndAddStripLine("Test", Color.FromArgb(20, Color.Red), model.TestSamplesStart, model.TestSamplesEnd);
      }
    }

    private void CreateAndAddStripLine(string title, Color c, int start, int end) {
      StripLine stripLine = new StripLine();
      stripLine.BackColor = c;
      stripLine.Text = title;
      stripLine.Font = new Font("Times New Roman", 12, FontStyle.Bold);
      stripLine.StripWidth = end - start;
      stripLine.IntervalOffset = start;
      this.chart.ChartAreas[0].AxisX.StripLines.Add(stripLine);
    }
  }
}
