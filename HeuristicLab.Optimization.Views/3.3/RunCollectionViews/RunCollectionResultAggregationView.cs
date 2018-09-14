using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Optimization.Views.RunCollectionViews {
  [View("Result Aggregation")]
  [Content(typeof(RunCollection), false)]
  public partial class RunCollectionResultAggregationView : AsynchronousContentView {
    private enum AggregationMethod { Minimum, Maximum, Average, FirstQuartile, Median, ThirdQuartile, IQR, StdDev }

    private readonly Dictionary<AggregationMethod, Func<IEnumerable<double>, double>> aggregator = new Dictionary<AggregationMethod, Func<IEnumerable<double>, double>> {
      { AggregationMethod.Minimum, x => x.Min() },
      { AggregationMethod.Maximum, x => x.Max() },
      { AggregationMethod.Average, x => x.Average() },
      { AggregationMethod.FirstQuartile, x => x.Quantile(0.25) },
      { AggregationMethod.Median, x => x.Median() },
      { AggregationMethod.ThirdQuartile, x => x.Quantile(0.75) },
      { AggregationMethod.IQR, x => x.Quantile(0.75) - x.Quantile(0.25) },
      { AggregationMethod.StdDev, x => x.StandardDeviation() },
    };

    public RunCollectionResultAggregationView() {
      InitializeComponent();
    }

    public new RunCollection Content {
      get { return (RunCollection)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      var results = GetCompatibleResults(Content);

      statisticsCheckedList.Items.Clear();
      resultsCheckedList.Items.Clear();

      foreach (var name in Enum.GetNames(typeof(AggregationMethod))) {
        statisticsCheckedList.Items.Add(name, false);
      }

      foreach (var name in GetCompatibleResults(Content).OrderBy(x => x))
        resultsCheckedList.Items.Add(name, false);

      firstCriterionComboBox.Items.Clear();
      secondCriterionComboBox.Items.Clear();

      firstCriterionComboBox.Items.AddRange(Content.ParameterNames.OrderBy(x => x).ToArray());
      secondCriterionComboBox.Items.AddRange(Content.ParameterNames.OrderBy(x => x).ToArray());

      base.OnContentChanged();
    }

    private static IEnumerable<string> GetCompatibleResults(RunCollection runs) {
      var results = new HashSet<string>(runs.First().Results.Where(x => IsCompatible(x.Value)).Select(x => x.Key));

      foreach (var run in runs.Skip(1)) {
        results.IntersectWith(run.Results.Keys);
      }

      return results;
    }

    // return only results of a numeric type
    private static bool IsCompatible(IItem item) {
      return item is IntValue || item is DoubleValue || item is PercentValue || item is TimeSpanValue;
    }

    private void AggregateResults() {
      if (!(statisticsCheckedList.CheckedItems.Count > 0 && resultsCheckedList.CheckedItems.Count > 0 && !string.IsNullOrEmpty(firstCriterionComboBox.Text)))
        return;

      var groups = new Dictionary<string, IEnumerable<IRun>>();
      var first = Content.GroupBy(run => Content.GetValue(run, firstCriterionComboBox.Text).ToString());

      if (!string.IsNullOrEmpty(secondCriterionComboBox.Text) && secondCriterionComboBox.Text != firstCriterionComboBox.Text) {
        foreach (var f in first) {
          foreach (var g in f.GroupBy(run => Content.GetValue(run, secondCriterionComboBox.Text).ToString())) {
            var key = f.Key + " " + g.Key;
            groups.Add(key, g);
          }
        }
      } else {
        groups = first.ToDictionary(x => x.Key, x => x.AsEnumerable());
      }

      var aggregatedDict = new Dictionary<string, List<Tuple<string, double>>>();

      foreach (var result in resultsCheckedList.CheckedItems) {
        var resultName = result.ToString();
        foreach (var group in groups) {
          var values = GetValues(group.Value, resultName);

          var key = group.Key;
          if (!aggregatedDict.ContainsKey(key)) {
            aggregatedDict[key] = new List<Tuple<string, double>>();
          }

          foreach (var i in statisticsCheckedList.CheckedIndices) {
            var name = resultName + " " + (AggregationMethod)i;
            aggregatedDict[key].Add(Tuple.Create(name, aggregator[(AggregationMethod)i](values)));
          }
        }
      }
      var groupNames = aggregatedDict.Keys.ToList();
      var resultNames = aggregatedDict.First().Value.Select(x => x.Item1).ToList();

      if (!aggregatedDict.Values.Skip(1).All(x => resultNames.SequenceEqual(x.Select(y => y.Item1)))) {
        throw new Exception("Inconsistent results across groups.");
      }

      var rows = resultNames.Count;
      var cols = groups.Count;

      var matrix = new DoubleMatrix(rows, cols);

      var col = 0;
      foreach (var groupName in groupNames) {
        var row = 0;
        foreach (var resultPair in aggregatedDict[groupName]) {
          matrix[row, col] = resultPair.Item2;
          ++row;
        }
        ++col;
      }
      matrix.RowNames = resultNames;
      matrix.ColumnNames = groupNames;

      aggregatedResultsMatrixView.Content = transposeMatrixCheckBox.Checked ? Transpose(matrix) : matrix;
    }

    private IEnumerable<double> GetValues(IEnumerable<IRun> runs, string resultName) {
      var values = new List<double>();

      foreach (var run in runs) {
        var value = Content.GetValue(run, resultName);
        var doubleValue = value as DoubleValue;
        var intValue = value as IntValue;
        var percentValue = value as PercentValue;
        var timeSpanValue = value as TimeSpanValue;

        if (doubleValue != null) {
          values.Add(doubleValue.Value);
        } else if (intValue != null) {
          values.Add(intValue.Value);
        } else if (percentValue != null) {
          values.Add(percentValue.Value);
        } else if (timeSpanValue != null) {
          values.Add(timeSpanValue.Value.TotalSeconds);
        }
      }
      return values;
    }

    private static DoubleMatrix Transpose(DoubleMatrix matrix) {
      var transposed = new DoubleMatrix(matrix.Columns, matrix.Rows) {
        RowNames = matrix.ColumnNames,
        ColumnNames = matrix.RowNames
      };

      for (int i = 0; i < matrix.Rows; ++i) {
        for (int j = 0; j < matrix.Columns; ++j) {
          transposed[j, i] = matrix[i, j];
        }
      }

      return transposed;
    }

    #region events 
    private void aggregateByComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      AggregateResults();
    }

    private void resultsCheckedList_ItemCheck(object sender, ItemCheckEventArgs e) {
      BeginInvoke((MethodInvoker)AggregateResults); // delayed execution
    }

    private void statisticsToCalculateCheckBox_ItemCheck(object sender, ItemCheckEventArgs e) {
      BeginInvoke((MethodInvoker)AggregateResults); // delayed execution
    }

    private void criterionComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      AggregateResults();
    }

    private void transposeMatrixCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (aggregatedResultsMatrixView.Content == null)
        return;

      var matrix = (DoubleMatrix)aggregatedResultsMatrixView.Content;
      aggregatedResultsMatrixView.Content = Transpose(matrix);
    }

    private void orderByStatisticCheckbox_CheckedChanged(object sender, EventArgs e) {
      if (!orderByStatisticCheckbox.Checked) {
        AggregateResults();
        return;
      }

      // we reorder the matrix rows (or columns, if transposed) so that every n_th rows (column) become successive
      // the number of rows (columns, if transposed) in the matrix is a multiple of n
      var matrix = (DoubleMatrix)aggregatedResultsMatrixView.Content;
      var rowNames = matrix.RowNames.ToList();
      var columnNames = matrix.ColumnNames.ToList();

      var ordered = new DoubleMatrix(matrix.Rows, matrix.Columns);
      var orderedNames = new List<string>();

      var transposed = transposeMatrixCheckBox.Checked;
      var count = statisticsCheckedList.CheckedItems.Count;
      var outerCount = transposed ? matrix.Columns : matrix.Rows;
      var innerCount = transposed ? matrix.Rows : matrix.Columns;

      for (int i = 0; i < count; ++i) {
        for (int j = i; j < outerCount; j += count) {
          var c = orderedNames.Count;
          for (int k = 0; k < innerCount; ++k) {
            if (transposed)
              ordered[k, c] = matrix[k, j];
            else
              ordered[c, k] = matrix[j, k];
          }
          orderedNames.Add(transposed ? columnNames[j] : rowNames[j]);
        }
      }

      if (transposed) {
        ordered.RowNames = matrix.RowNames;
        ordered.ColumnNames = orderedNames;
      } else {
        ordered.ColumnNames = matrix.ColumnNames;
        ordered.RowNames = orderedNames;
      }

      aggregatedResultsMatrixView.Content = ordered;
    }
    #endregion
  }
}
