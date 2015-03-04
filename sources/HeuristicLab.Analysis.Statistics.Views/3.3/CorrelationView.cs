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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;

namespace HeuristicLab.Analysis.Statistics.Views {
  [View("Correlations")]
  [Content(typeof(RunCollection), false)]
  public sealed partial class CorrelationView : ItemView {
    private const string PearsonName = "Pearson product-moment correlation coefficient";
    private const string SpearmanName = "Spearman's rank correlation coefficient";

    private enum ResultParameterType {
      Result,
      Parameter
    }

    private bool suppressUpdates = false;

    public new RunCollection Content {
      get { return (RunCollection)base.Content; }
      set { base.Content = value; }
    }

    public override bool ReadOnly {
      get { return true; }
      set { /*not needed because results are always readonly */}
    }

    public CorrelationView() {
      InitializeComponent();
      stringConvertibleMatrixView.Minimum = -1.0;
      stringConvertibleMatrixView.Maximum = 1.0;

      methodComboBox.Items.Add(PearsonName);
      methodComboBox.Items.Add(SpearmanName);
      methodComboBox.SelectedIndex = 0;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();

      if (Content != null) {
        RebuildCorrelationTable();
      }
      UpdateCaption();
    }

    private void UpdateCaption() {
      Caption = Content != null ? Content.OptimizerName + " Correlations" : ViewAttribute.GetViewName(GetType());
    }

    #region events
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ColumnsChanged += Content_ColumnsChanged;
      Content.RowsChanged += Content_RowsChanged;
      Content.CollectionReset += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
      Content.UpdateOfRunsInProgressChanged += Content_UpdateOfRunsInProgressChanged;
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ColumnsChanged -= Content_ColumnsChanged;
      Content.RowsChanged -= Content_RowsChanged;
      Content.CollectionReset -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
      Content.UpdateOfRunsInProgressChanged -= Content_UpdateOfRunsInProgressChanged;
    }

    void Content_RowsChanged(object sender, EventArgs e) {
      UpdateUI();
    }

    void Content_ColumnsChanged(object sender, EventArgs e) {
      if (!suppressUpdates) {
        UpdateUI();
      }
    }

    private void Content_CollectionReset(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IRun> e) {
      UpdateUI();
    }

    private void Content_UpdateOfRunsInProgressChanged(object sender, EventArgs e) {
      suppressUpdates = Content.UpdateOfRunsInProgress;
      UpdateUI();
    }
    #endregion

    private void UpdateUI() {
      if (!suppressUpdates) {
        RebuildCorrelationTable();
      }
    }

    private List<string> GetResultRowNames() {
      var results = (from run in Content
                     where run.Visible
                     from result in run.Results
                     where result.Value is DoubleValue || result.Value is IntValue
                     select result.Key).Distinct().OrderBy(x => x).ToList();

      return results;
    }

    private List<string> GetParameterRowNames() {
      var parameters = (from run in Content
                        where run.Visible
                        from parameter in run.Parameters
                        where parameter.Value is DoubleValue || parameter.Value is IntValue
                        select parameter.Key).Distinct().OrderBy(x => x).ToList();

      return parameters;
    }

    private Dictionary<string, ResultParameterType> GetRowNames() {
      Dictionary<string, ResultParameterType> ret = new Dictionary<string, ResultParameterType>();

      var results = GetResultRowNames();
      var parameters = GetParameterRowNames();

      foreach (var r in results) {
        ret.Add(r, ResultParameterType.Result);
      }
      foreach (var p in parameters) {
        if (!ret.ContainsKey(p)) {
          ret.Add(p, ResultParameterType.Parameter);
        }
      }

      return ret;
    }

    private List<double> GetDoublesFromResults(List<IRun> runs, string key) {
      List<double> res = new List<double>();

      foreach (var r in runs) {
        if (r.Results[key] is DoubleValue) {
          res.Add(((DoubleValue)r.Results[key]).Value);
        } else {
          res.Add(((IntValue)r.Results[key]).Value);
        }
      }
      return res;
    }

    private List<double> GetDoublesFromParameters(List<IRun> runs, string key) {
      List<double> res = new List<double>();

      foreach (var r in runs) {
        if (r.Parameters[key] is DoubleValue) {
          res.Add(((DoubleValue)r.Parameters[key]).Value);
        } else {
          res.Add(((IntValue)r.Parameters[key]).Value);
        }
      }
      return res;
    }

    private List<double> GetValuesFromResultsParameters(IEnumerable<IRun> runs, string name, ResultParameterType type) {
      if (type == ResultParameterType.Parameter) {
        return GetDoublesFromParameters(runs.Where(x => x.Parameters.ContainsKey(name)).ToList(), name);
      } else if (type == ResultParameterType.Result) {
        return GetDoublesFromResults(runs.Where(x => x.Results.ContainsKey(name)).ToList(), name);
      } else {
        return null;
      }
    }

    private void RebuildCorrelationTable() {
      Dictionary<string, ResultParameterType> resultsParameters = GetRowNames();
      string methodName = (string)methodComboBox.SelectedItem;
      var columnNames = resultsParameters.Keys.ToArray();

      var runs = Content.Where(x => x.Visible);

      DoubleMatrix dt = new DoubleMatrix(resultsParameters.Count(), columnNames.Count());
      dt.RowNames = columnNames;
      dt.ColumnNames = columnNames;

      int i = 0;
      foreach (var res in resultsParameters) {
        var rowValues =
          GetValuesFromResultsParameters(runs, res.Key, res.Value)
            .Where(x => !double.IsNaN(x) && !double.IsNegativeInfinity(x) && !double.IsPositiveInfinity(x));

        int j = 0;
        foreach (var cres in resultsParameters) {
          var columnValues = GetValuesFromResultsParameters(runs, cres.Key, cres.Value)
                .Where(x => !double.IsNaN(x) && !double.IsNegativeInfinity(x) && !double.IsPositiveInfinity(x));

          if (!rowValues.Any() || !columnValues.Any() || i == j || rowValues.Count() != columnValues.Count()) {
            dt[i, j] = double.NaN;
          } else {
            if (methodName == PearsonName) {
              dt[i, j] = alglib.pearsoncorr2(rowValues.ToArray(), columnValues.ToArray());
            } else {
              dt[i, j] = alglib.spearmancorr2(rowValues.ToArray(), columnValues.ToArray());
            }
          }
          j++;
        }
        i++;
      }

      dt.SortableView = true;
      stringConvertibleMatrixView.Content = dt;
    }

    private void methodComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (Content != null) {
        RebuildCorrelationTable();
      }
    }
  }
}
