#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Analysis;
using HeuristicLab.Analysis.Views;
using HeuristicLab.Collections;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using AggregationType = HeuristicLab.Analysis.DataTableVisualProperties.DataTableHistogramAggregation;
using RegressionType = HeuristicLab.Analysis.ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType;

namespace HeuristicLab.DataPreprocessing.Views {
  [View("Scatter Plot Multi View")]
  [Content(typeof(MultiScatterPlotContent), true)]
  public sealed partial class ScatterPlotMultiView : PreprocessingCheckedVariablesView {
    private readonly IDictionary<string, Label> columnHeaderCache = new Dictionary<string, Label>();
    private readonly IDictionary<string, VerticalLabel> rowHeaderCache = new Dictionary<string, VerticalLabel>();
    private readonly IDictionary<Tuple<string/*col*/, string/*row*/>, Control> bodyCache = new Dictionary<Tuple<string, string>, Control>();

    public new MultiScatterPlotContent Content {
      get { return (MultiScatterPlotContent)base.Content; }
      set { base.Content = value; }
    }

    public ScatterPlotMultiView() {
      InitializeComponent();

      oldWidth = (int)widthNumericUpDown.Value;
      oldHeight = (int)heightNumericUpDown.Value;

      regressionTypeComboBox.DataSource = Enum.GetValues(typeof(RegressionType));
      regressionTypeComboBox.SelectedItem = RegressionType.None;

      aggregationComboBox.DataSource = Enum.GetValues(typeof(AggregationType));
      aggregationComboBox.SelectedItem = AggregationType.Overlapping;

      legendOrderComboBox.DataSource = Enum.GetValues(typeof(PreprocessingChartContent.LegendOrder));
      legendOrderComboBox.SelectedItem = PreprocessingChartContent.LegendOrder.Appearance;

      #region Initialize Scrollbars
      columnHeaderScrollPanel.HorizontalScroll.Enabled = true;
      columnHeaderScrollPanel.VerticalScroll.Enabled = false;
      columnHeaderScrollPanel.HorizontalScroll.Visible = false;
      columnHeaderScrollPanel.VerticalScroll.Visible = false;

      rowHeaderScrollPanel.HorizontalScroll.Enabled = false;
      rowHeaderScrollPanel.VerticalScroll.Enabled = true;
      rowHeaderScrollPanel.HorizontalScroll.Visible = false;
      rowHeaderScrollPanel.VerticalScroll.Visible = false;

      bodyScrollPanel.HorizontalScroll.Enabled = true;
      bodyScrollPanel.VerticalScroll.Enabled = true;
      bodyScrollPanel.HorizontalScroll.Visible = true;
      bodyScrollPanel.VerticalScroll.Visible = true;
      bodyScrollPanel.AutoScroll = true;
      #endregion

      bodyScrollPanel.MouseWheel += bodyScrollPanel_MouseWheel;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        groupingComboBox.Items.Add(string.Empty);
        foreach (string var in PreprocessingChartContent.GetVariableNamesForGrouping(Content.PreprocessingData)) {
          groupingComboBox.Items.Add(var);
        }
        SuppressCheckedChangedUpdate = true;
        groupingComboBox.SelectedItem = Content.GroupingVariable ?? string.Empty;
        SuppressCheckedChangedUpdate = false;

        // uncheck variables that max 20 vars are selected initially
        var variables = Content.VariableItemList;
        int numChecked = variables.CheckedItems.Count();
        if (numChecked > 20) {
          string message = string.Format("Display all {0} input variables ({1} charts)?" + Environment.NewLine +
                                         "Press No to reduce the number of checked variables to 20." + Environment.NewLine +
                                         "Press Cancel to uncheck all.",
            numChecked, numChecked * numChecked);
          var dialogResult = MessageBox.Show(this, message, "Display All Input Variables?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
          if (dialogResult == DialogResult.No || dialogResult == DialogResult.Cancel) {
            SuppressCheckedChangedUpdate = true;
            IEnumerable<StringValue> toUncheck = variables;
            if (dialogResult == DialogResult.No) // only show the first 20
              toUncheck = variables.CheckedItems.Reverse().Take(numChecked - 20).Select(x => x.Value);
            foreach (var var in toUncheck)
              Content.VariableItemList.SetItemCheckedState(var, false);
            SuppressCheckedChangedUpdate = false;
          }
        }
        GenerateCharts(true);
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      var regressionType = (RegressionType)regressionTypeComboBox.SelectedValue;
      polynomialRegressionOrderNumericUpDown.Enabled = regressionType == RegressionType.Polynomial;
      sizeGroupBox.Enabled = Content != null;
      pointsGroupBox.Enabled = Content != null;
      groupingComboBox.Enabled = Content != null;
      regressionGroupBox.Enabled = Content != null;
    }

    protected override void CheckedItemsChanged(object sender, CollectionItemsChangedEventArgs<IndexedItem<StringValue>> checkedItems) {
      base.CheckedItemsChanged(sender, checkedItems);
      if (SuppressCheckedChangedUpdate) return;

      foreach (var variable in checkedItems.Items) {
        if (Content.VariableItemList.ItemChecked(variable.Value))
          ShowChartOnTable(variable.Value.Value, variable.Index);
        else
          HideChartFromTable(variable.Value.Value, variable.Index);
      }
    }

    #region Show and Hide charts
    private void ShowChartOnTable(string variable, int idx) {
      frameTableLayoutPanel.SuspendLayout();

      // show column header
      var colH = columnHeaderTableLayoutPanel;
      colH.ColumnStyles[idx].Width = GetColumnWidth();
      if (colH.GetControlFromPosition(idx, 0) == null)
        colH.Controls.Add(GetColumnHeader(variable), idx, 0);
      else
        colH.GetControlFromPosition(idx, 0).Visible = true;

      // show row header
      var rowH = rowHeaderTableLayoutPanel;
      rowH.RowStyles[idx].Height = GetRowHeight();
      if (rowH.GetControlFromPosition(0, idx) == null)
        rowH.Controls.Add(GetRowHeader(variable), 0, idx);
      else
        rowH.GetControlFromPosition(0, idx).Visible = true;

      // show body
      var body = bodyTableLayoutPanel;
      ShowColumnHelper(body, idx, r => GetBody(variable, Content.VariableItemList[r].Value));
      ShowRowHelper(body, idx, c => GetBody(Content.VariableItemList[c].Value, variable));

      frameTableLayoutPanel.ResumeLayout(true);
    }
    private void ShowColumnHelper(TableLayoutPanel tlp, int idx, Func<int, Control> creatorFunc) {
      tlp.ColumnStyles[idx].Width = GetColumnWidth();
      for (int r = 0; r < tlp.RowCount; r++) {
        if (Content.VariableItemList.ItemChecked(Content.VariableItemList[r])) {
          var control = tlp.GetControlFromPosition(idx, r);
          if (control == null)
            tlp.Controls.Add(creatorFunc(r), idx, r);
          else
            control.Visible = true;
        }
      }
    }
    private void ShowRowHelper(TableLayoutPanel tlp, int idx, Func<int, Control> creatorFunc) {
      tlp.RowStyles[idx].Height = GetRowHeight();
      for (int c = 0; c < tlp.ColumnCount; c++) {
        if (Content.VariableItemList.ItemChecked(Content.VariableItemList[c])) {
          var control = tlp.GetControlFromPosition(c, idx);
          if (control == null)
            tlp.Controls.Add(creatorFunc(c), c, idx);
          else
            tlp.GetControlFromPosition(c, idx).Visible = true;
        }
      }
    }

    private void HideChartFromTable(string variable, int idx) {
      frameTableLayoutPanel.SuspendLayout();

      // hide column header
      var colH = columnHeaderTableLayoutPanel;
      HideColumnHelper(colH, idx);

      // hide row header
      var rowH = rowHeaderTableLayoutPanel;
      HideRowHelper(rowH, idx);

      // hide from body
      var body = bodyTableLayoutPanel;
      HideColumnHelper(body, idx);
      HideRowHelper(body, idx);

      frameTableLayoutPanel.ResumeLayout(true);
    }
    private void HideColumnHelper(TableLayoutPanel tlp, int idx) {
      tlp.ColumnStyles[idx].Width = 0;
      // hide controls
      for (int r = 0; r < tlp.RowCount; r++) {
        var control = tlp.GetControlFromPosition(idx, r);
        if (control != null)
          control.Visible = false;
      }
    }
    private void HideRowHelper(TableLayoutPanel tlp, int idx) {
      tlp.RowStyles[idx].Height = 0;
      // hide controls
      for (int c = 0; c < tlp.ColumnCount; c++) {
        var control = tlp.GetControlFromPosition(c, idx);
        if (control != null)
          control.Visible = false;
      }
    }
    #endregion

    #region Add/Remove/Update Variable
    protected override void AddVariable(string name) {
      base.AddVariable(name);
      if (IsVariableChecked(name))
        AddChartToTable(name);
    }
    protected override void RemoveVariable(string name) {
      base.RemoveVariable(name);

      if (IsVariableChecked(name)) {
        RemoveChartFromTable(name);
      }

      // clear caches
      columnHeaderCache[name].Dispose();
      columnHeaderCache.Remove(name);
      rowHeaderCache[name].Dispose();
      rowHeaderCache.Remove(name);
      var keys = bodyCache.Keys.Where(t => t.Item1 == name || t.Item2 == name).ToList();
      foreach (var key in keys) {
        bodyCache[key].Dispose();
        bodyCache.Remove(key);
      }
    }
    protected override void UpdateVariable(string name) {
      base.UpdateVariable(name);
      RemoveVariable(name);
      AddVariable(name);
    }
    protected override void ResetAllVariables() {
      GenerateCharts(true);
    }

    private void AddChartToTable(string variable) {
      frameTableLayoutPanel.SuspendLayout();

      // find index to insert 
      var variables = Content.VariableItemList.Select(v => v.Value).ToList();
      int idx = variables              // all variables
        .TakeWhile(t => t != variable) // ... until the variable that was checked
        .Count(IsVariableChecked);     // ... how many checked variables

      // add column header
      var colH = columnHeaderTableLayoutPanel;
      AddColumnHelper(colH, idx, _ => GetColumnHeader(variable));

      // add row header
      var rowH = rowHeaderTableLayoutPanel;
      AddRowHelper(rowH, idx, _ => GetRowHeader(variable));

      // add body
      var body = bodyTableLayoutPanel;
      var vars = GetCheckedVariables();
      var varsMinus = vars.Except(new[] { variable }).ToList();
      AddColumnHelper(body, idx, r => GetBody(variable, varsMinus[r])); // exclude "variable" because the row for it does not exist yet
      AddRowHelper(body, idx, c => GetBody(vars[c], variable));

      frameTableLayoutPanel.ResumeLayout(true);
    }
    private void AddColumnHelper(TableLayoutPanel tlp, int idx, Func<int, Control> creatorFunc) {
      // add column
      tlp.ColumnCount++;
      tlp.ColumnStyles.Insert(idx, new ColumnStyle(SizeType.Absolute, GetColumnWidth()));
      // shift right
      for (int c = tlp.ColumnCount; c > idx - 1; c--) {
        for (int r = 0; r < tlp.RowCount; r++) {
          var control = tlp.GetControlFromPosition(c, r);
          if (control != null) {
            tlp.SetColumn(control, c + 1);
          }
        }
      }
      // add controls
      for (int r = 0; r < tlp.RowCount; r++) {
        if (tlp.GetControlFromPosition(idx, r) == null)
          tlp.Controls.Add(creatorFunc(r), idx, r);
      }

    }
    private void AddRowHelper(TableLayoutPanel tlp, int idx, Func<int, Control> creatorFunc) {
      // add row
      tlp.RowCount++;
      tlp.RowStyles.Insert(idx, new RowStyle(SizeType.Absolute, GetRowHeight()));
      // shift right
      for (int r = tlp.RowCount; r > idx - 1; r--) {
        for (int c = 0; c < tlp.ColumnCount; c++) {
          var control = tlp.GetControlFromPosition(c, r);
          if (control != null) {
            tlp.SetRow(control, r + 1);
          }
        }
      }
      // add controls
      for (int c = 0; c < tlp.ColumnCount; c++)
        if (tlp.GetControlFromPosition(c, idx) == null)
          tlp.Controls.Add(creatorFunc(c), c, idx);
    }

    private void RemoveChartFromTable(string variable) {
      frameTableLayoutPanel.SuspendLayout();

      // remove column header
      var colH = columnHeaderTableLayoutPanel;
      int colIdx = colH.GetColumn(colH.Controls[variable]);
      RemoveColumnHelper(colH, colIdx);

      // remove row header
      var rowH = rowHeaderTableLayoutPanel;
      int rowIdx = rowH.GetRow(rowH.Controls[variable]);
      RemoveRowHelper(rowH, rowIdx);

      // remove from body
      var body = bodyTableLayoutPanel;
      RemoveColumnHelper(body, colIdx);
      RemoveRowHelper(body, rowIdx);

      frameTableLayoutPanel.ResumeLayout(true);
    }
    private void RemoveColumnHelper(TableLayoutPanel tlp, int idx) {
      // remove controls
      for (int r = 0; r < tlp.RowCount; r++)
        tlp.Controls.Remove(tlp.GetControlFromPosition(idx, r));
      // shift left
      for (int c = idx + 1; c < tlp.ColumnCount; c++) {
        for (int r = 0; r < tlp.RowCount; r++) {
          var control = tlp.GetControlFromPosition(c, r);
          if (control != null) {
            tlp.SetColumn(control, c - 1);
          }
        }
      }
      // delete column
      tlp.ColumnStyles.RemoveAt(tlp.ColumnCount - 1);
      tlp.ColumnCount--;
    }
    private void RemoveRowHelper(TableLayoutPanel tlp, int idx) {
      // remove controls
      for (int c = 0; c < tlp.ColumnCount; c++)
        tlp.Controls.Remove(tlp.GetControlFromPosition(c, idx));
      // shift left
      for (int r = idx + 1; r < tlp.RowCount; r++) {
        for (int c = 0; c < tlp.ColumnCount; c++) {
          var control = tlp.GetControlFromPosition(c, r);
          if (control != null) {
            tlp.SetRow(control, r - 1);
          }
        }
      }
      // delete rows
      tlp.RowStyles.RemoveAt(tlp.RowCount - 1);
      tlp.RowCount--;
    }
    #endregion

    #region Creating Headers and Body
    private Label GetColumnHeader(string variable) {
      if (!columnHeaderCache.ContainsKey(variable)) {
        columnHeaderCache.Add(variable, new Label() {
          Text = variable,
          TextAlign = ContentAlignment.MiddleCenter,
          Name = variable,
          Height = columnHeaderTableLayoutPanel.Height,
          Dock = DockStyle.Fill,
          Margin = new Padding(3)
        });
      }
      return columnHeaderCache[variable];
    }
    private Label GetRowHeader(string variable) {
      if (!rowHeaderCache.ContainsKey(variable)) {
        rowHeaderCache.Add(variable, new VerticalLabel() {
          Text = variable,
          TextAlign = ContentAlignment.MiddleCenter,
          Name = variable,
          Width = rowHeaderTableLayoutPanel.Width,
          Height = columnHeaderScrollPanel.Width,
          Dock = DockStyle.Fill,
          Margin = new Padding(3)
        });
      }
      return rowHeaderCache[variable];
    }
    private Control GetBody(string colVariable, string rowVariable) {
      var key = Tuple.Create(colVariable, rowVariable);
      if (!bodyCache.ContainsKey(key)) {
        if (rowVariable == colVariable) { // use historgram if x and y variable are equal
          var dataTable = HistogramContent.CreateHistogram(
            Content.PreprocessingData,
            rowVariable,
            (string)groupingComboBox.SelectedItem,
            (AggregationType)aggregationComboBox.SelectedItem,
            (PreprocessingChartContent.LegendOrder)legendOrderComboBox.SelectedItem);
          dataTable.VisualProperties.Title = string.Empty;
          foreach (var dataRow in dataTable.Rows) {
            dataRow.VisualProperties.IsVisibleInLegend = legendCheckbox.Checked && groupingComboBox.SelectedIndex > 0;
          }
          var pcv = new DataTableView {
            Name = key.ToString(),
            Content = dataTable,
            Dock = DockStyle.Fill,
            ShowChartOnly = true
          };
          //pcv.ChartDoubleClick += HistogramDoubleClick;  // ToDo: not working; double click is already handled by the chart
          bodyCache.Add(key, pcv);
        } else { //scatter plot
          var scatterPlot = ScatterPlotContent.CreateScatterPlot(Content.PreprocessingData,
            colVariable,
            rowVariable,
            (string)groupingComboBox.SelectedItem,
            (PreprocessingChartContent.LegendOrder)legendOrderComboBox.SelectedItem);
          var regressionType = (RegressionType)regressionTypeComboBox.SelectedValue;
          int order = (int)polynomialRegressionOrderNumericUpDown.Value;
          int i = 0;
          var colors = PreprocessingChartView.Colors;
          foreach (var row in scatterPlot.Rows) {
            row.VisualProperties.PointSize = (int)pointSizeNumericUpDown.Value;
            row.VisualProperties.Color = Color.FromArgb((int)(pointOpacityNumericUpDown.Value * 255),
              row.VisualProperties.Color.IsEmpty ? colors[i++ % colors.Length] : row.VisualProperties.Color);
            row.VisualProperties.IsVisibleInLegend = legendCheckbox.Checked && groupingComboBox.SelectedIndex > 0;
            row.VisualProperties.IsRegressionVisibleInLegend = false;
            row.VisualProperties.RegressionType = regressionType;
            row.VisualProperties.PolynomialRegressionOrder = order;
          }
          scatterPlot.VisualProperties.Title = string.Empty;
          var scatterPlotView = new ScatterPlotView {
            Name = key.ToString(),
            Content = scatterPlot,
            Dock = DockStyle.Fill,
            ShowName = false
            //ShowLegend = false,
            //XAxisFormat = "G3"
          };
          //scatterPlotView.DoubleClick += ScatterPlotDoubleClick; // ToDo: not working; double click is already handled by the chart
          bodyCache.Add(key, scatterPlotView);
        }
      }
      return bodyCache[key];
    }
    #endregion

    protected override void CheckedChangedUpdate() {
      GenerateCharts(false); // only checked-changes -> reuse cached values
    }

    #region Generate Charts
    private void GenerateCharts(bool clearCache) {
      if (Content == null || SuppressCheckedChangedUpdate) return;

      // Clear old layouts and cache
      foreach (var tableLayoutPanel in new[] { columnHeaderTableLayoutPanel, rowHeaderTableLayoutPanel, bodyTableLayoutPanel }) {
        tableLayoutPanel.Controls.Clear();
        tableLayoutPanel.ColumnStyles.Clear();
        tableLayoutPanel.RowStyles.Clear();
      }

      if (clearCache) {
        foreach (var control in bodyCache.Values.Concat(columnHeaderCache.Values).Concat(rowHeaderCache.Values)) {
          control.Dispose();
        }
        columnHeaderCache.Clear();
        rowHeaderCache.Clear();
        bodyCache.Clear();
      }

      var variables = Content.VariableItemList.Select(x => x.Value).ToList();

      // Set row and column count
      columnHeaderTableLayoutPanel.ColumnCount = variables.Count;
      rowHeaderTableLayoutPanel.RowCount = variables.Count;
      bodyTableLayoutPanel.ColumnCount = variables.Count;
      bodyTableLayoutPanel.RowCount = variables.Count;

      // Set column and row layout
      for (int i = 0; i < variables.Count; i++) {
        bool @checked = Content.VariableItemList.ItemChecked(Content.VariableItemList[i]);
        columnHeaderTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, @checked ? GetColumnWidth() : 0));
        rowHeaderTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, @checked ? GetRowHeight() : 0));
        bodyTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, @checked ? GetColumnWidth() : 0));
        bodyTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, @checked ? GetRowHeight() : 0));
      }

      frameTableLayoutPanel.SuspendLayout();
      AddHeaderToTableLayoutPanels();
      AddChartsToTableLayoutPanel();
      UpdateHeaderMargin();
      frameTableLayoutPanel.ResumeLayout(true);
    }

    private void AddHeaderToTableLayoutPanels() {
      for (int i = 0; i < Content.VariableItemList.Count; i++) {
        var variable = Content.VariableItemList[i];
        if (Content.VariableItemList.ItemChecked(variable)) {
          columnHeaderTableLayoutPanel.Controls.Add(GetColumnHeader(variable.Value), i, 0);
          rowHeaderTableLayoutPanel.Controls.Add(GetRowHeader(variable.Value), 0, i);
        }
      }
    }
    private void AddChartsToTableLayoutPanel() {
      for (int c = 0; c < Content.VariableItemList.Count; c++) {
        var colVar = Content.VariableItemList[c].Value;
        if (!IsVariableChecked(colVar)) continue;
        for (int r = 0; r < Content.VariableItemList.Count; r++) {
          var rowVar = Content.VariableItemList[r].Value;
          if (!IsVariableChecked(rowVar)) continue;
          bodyTableLayoutPanel.Controls.Add(GetBody(colVar, rowVar), c, r);
        }
      }
      UpdateRegressionLine();
    }

    #endregion

    #region DoubleClick Events
    //Open scatter plot in new tab with new content when double clicked
    private void ScatterPlotDoubleClick(object sender, EventArgs e) {
      var scatterPlotView = (ScatterPlotView)sender;
      var scatterContent = new SingleScatterPlotContent(Content.PreprocessingData);
      ScatterPlot scatterPlot = scatterPlotView.Content;

      //Extract variable names from scatter plot and set them in content
      if (scatterPlot.Rows.Count == 1) {
        string[] variables = scatterPlot.Rows.ElementAt(0).Name.Split(new string[] { " - " }, StringSplitOptions.None); // extract variable names from string
        scatterContent.SelectedXVariable = variables[0];
        scatterContent.SelectedYVariable = variables[1];
      }

      MainFormManager.MainForm.ShowContent(scatterContent, typeof(ScatterPlotSingleView)); // open in new tab
    }

    //open histogram in new tab with new content when double clicked
    private void HistogramDoubleClick(object sender, EventArgs e) {
      DataTableView pcv = (DataTableView)sender;
      HistogramContent histoContent = new HistogramContent(Content.PreprocessingData);  // create new content     
                                                                                        //ToDo: histoContent.VariableItemList = Content.CreateVariableItemList();
      var dataTable = pcv.Content;

      //Set variable item list from with variable from data table
      if (dataTable.Rows.Count == 1) { // only one data row should be in data table 
        string variableName = dataTable.Rows.ElementAt(0).Name;

        // set only variable name checked
        foreach (var checkedItem in histoContent.VariableItemList) {
          histoContent.VariableItemList.SetItemCheckedState(checkedItem, checkedItem.Value == variableName);
        }
      }
      MainFormManager.MainForm.ShowContent(histoContent, typeof(HistogramView)); // open in new tab
    }
    #endregion

    #region Scrolling
    private void bodyScrollPanel_Scroll(object sender, ScrollEventArgs e) {
      SyncScroll();

      UpdateHeaderMargin();
    }
    private void bodyScrollPanel_MouseWheel(object sender, MouseEventArgs e) {
      // Scrolling with the mouse wheel is not captured in the Scoll event
      SyncScroll();
    }
    private void SyncScroll() {
      frameTableLayoutPanel.SuspendRepaint();

      columnHeaderScrollPanel.HorizontalScroll.Minimum = bodyScrollPanel.HorizontalScroll.Minimum;
      columnHeaderScrollPanel.HorizontalScroll.Maximum = bodyScrollPanel.HorizontalScroll.Maximum;
      rowHeaderScrollPanel.VerticalScroll.Minimum = bodyScrollPanel.VerticalScroll.Minimum;
      rowHeaderScrollPanel.VerticalScroll.Maximum = bodyScrollPanel.VerticalScroll.Maximum;

      columnHeaderScrollPanel.HorizontalScroll.Value = Math.Max(bodyScrollPanel.HorizontalScroll.Value, 1);
      rowHeaderScrollPanel.VerticalScroll.Value = Math.Max(bodyScrollPanel.VerticalScroll.Value, 1);
      // minimum 1 is nececary  because of two factors:
      // - setting the Value-property of Horizontal/VerticalScroll updates the internal state but the Value-property stays 0
      // - setting the same number of the Value-property has no effect
      // since the Value-property is always 0, setting it to 0 would have no effect; so it is set to 1 instead

      frameTableLayoutPanel.ResumeRepaint(true);
    }
    // add a margin to the header table layouts if the scollbar is visible to account for the width/height of the scrollbar
    private void UpdateHeaderMargin() {
      columnHeaderScrollPanel.Margin = new Padding(0, 0, bodyScrollPanel.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0, 0);
      rowHeaderScrollPanel.Margin = new Padding(0, 0, 0, bodyScrollPanel.HorizontalScroll.Visible ? SystemInformation.HorizontalScrollBarHeight : 0);
    }
    #endregion

    #region Sizing of Charts
    private int oldWidth;
    private int oldHeight;
    private float AspectRatio {
      get {
        if (oldWidth == 0 || oldHeight == 0) return 1;
        return (float)oldWidth / oldHeight;
      }
    }
    private bool lockChange = false;

    private int GetColumnWidth() { return (int)widthNumericUpDown.Value; }
    private int GetRowHeight() { return (int)heightNumericUpDown.Value; }

    private void widthNumericUpDown_ValueChanged(object sender, EventArgs e) {
      frameTableLayoutPanel.SuspendRepaint();
      if (lockAspectCheckBox.Checked && !lockChange) {
        lockChange = true;
        heightNumericUpDown.Value = (int)((double)widthNumericUpDown.Value / AspectRatio);
        lockChange = false;
      }
      for (int i = 0; i < columnHeaderTableLayoutPanel.ColumnCount; i++) {
        if (Content.VariableItemList.ItemChecked(Content.VariableItemList[i])) {
          columnHeaderTableLayoutPanel.ColumnStyles[i].Width = GetColumnWidth();
          bodyTableLayoutPanel.ColumnStyles[i].Width = GetColumnWidth();
        }
      }
      oldWidth = GetColumnWidth();
      oldHeight = GetRowHeight();
      frameTableLayoutPanel.ResumeRepaint(true);
    }
    private void heightNumericUpDown_ValueChanged(object sender, EventArgs e) {
      frameTableLayoutPanel.SuspendRepaint();
      if (lockAspectCheckBox.Checked && !lockChange) {
        lockChange = true;
        widthNumericUpDown.Value = (int)((double)heightNumericUpDown.Value * AspectRatio);
        lockChange = false;
      }
      for (int i = 0; i < rowHeaderTableLayoutPanel.RowCount; i++) {
        if (Content.VariableItemList.ItemChecked(Content.VariableItemList[i])) {
          rowHeaderTableLayoutPanel.RowStyles[i].Height = GetRowHeight();
          bodyTableLayoutPanel.RowStyles[i].Height = GetRowHeight();
        }
      }
      oldWidth = GetColumnWidth();
      oldHeight = GetRowHeight();
      frameTableLayoutPanel.ResumeRepaint(true);
    }
    private void pointSizeNumericUpDown_ValueChanged(object sender, EventArgs e) {
      int pointSize = (int)pointSizeNumericUpDown.Value;
      foreach (var control in bodyCache.ToList()) {
        var scatterPlotView = control.Value as ScatterPlotView;
        if (scatterPlotView != null) {
          foreach (var row in scatterPlotView.Content.Rows) {
            row.VisualProperties.PointSize = pointSize;
          }
        }
      }
    }
    private void pointOpacityNumericUpDown_ValueChanged(object sender, EventArgs e) {
      float opacity = (float)pointOpacityNumericUpDown.Value;
      foreach (var control in bodyCache.ToList()) {
        var scatterPlotView = control.Value as ScatterPlotView;
        if (scatterPlotView != null) {
          foreach (var row in scatterPlotView.Content.Rows) {
            var color = row.VisualProperties.Color;
            if (color.IsEmpty)
              color = PreprocessingChartView.Colors.First();
            row.VisualProperties.Color = Color.FromArgb((int)(opacity * 255), color);
          }
        }
      }
    }
    #endregion

    #region Regression Line
    private void regressionTypeComboBox_SelectedValueChanged(object sender, EventArgs e) {
      var regressionType = (RegressionType)regressionTypeComboBox.SelectedValue;
      polynomialRegressionOrderNumericUpDown.Enabled = regressionType == RegressionType.Polynomial;
      UpdateRegressionLine();
    }

    private void polynomialRegressionOrderNumericUpDown_ValueChanged(object sender, EventArgs e) {
      UpdateRegressionLine();
    }

    private void UpdateRegressionLine() {
      var regressionType = (RegressionType)regressionTypeComboBox.SelectedValue;
      int order = (int)polynomialRegressionOrderNumericUpDown.Value;

      foreach (var control in bodyCache.ToList()) {
        // hidden chart => reset cache
        if (!bodyTableLayoutPanel.Controls.Contains(control.Value)) {
          bodyCache.Remove(control.Key);
        }

        var scatterPlotView = control.Value as ScatterPlotView;
        if (scatterPlotView != null) {
          foreach (var row in scatterPlotView.Content.Rows) {
            row.VisualProperties.IsRegressionVisibleInLegend = false;
            row.VisualProperties.RegressionType = regressionType;
            row.VisualProperties.PolynomialRegressionOrder = order;
          }
        }
      }
    }
    #endregion

    #region Grouping
    private void groupingComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      aggregationLabel.Enabled = groupingComboBox.SelectedIndex > 0;
      aggregationComboBox.Enabled = groupingComboBox.SelectedIndex > 0;
      legendGroupBox.Enabled = groupingComboBox.SelectedIndex > 0;
      GenerateCharts(true); // new series within charts -> clear cache
    }

    private void aggregationComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      var aggregation = (AggregationType)aggregationComboBox.SelectedValue;
      foreach (var control in bodyCache.ToList()) {
        // hidden chart => reset cache
        if (!bodyTableLayoutPanel.Controls.Contains(control.Value)) {
          bodyCache.Remove(control.Key);
        }

        var histogramView = control.Value as DataTableView;
        if (histogramView != null) {
          histogramView.Content.VisualProperties.HistogramAggregation = aggregation;
        }
      }
    }

    private void legendCheckbox_CheckedChanged(object sender, EventArgs e) {
      foreach (var control in bodyCache.ToList()) {
        var histogramControl = control.Value as DataTableView;
        if (histogramControl != null) {
          foreach (var row in histogramControl.Content.Rows) {
            row.VisualProperties.IsVisibleInLegend = legendCheckbox.Checked && groupingComboBox.SelectedIndex > 0;
          }
        }
        var scatterplotControl = control.Value as ScatterPlotView;
        if (scatterplotControl != null) {
          foreach (var row in scatterplotControl.Content.Rows) {
            row.VisualProperties.IsVisibleInLegend = legendCheckbox.Checked && groupingComboBox.SelectedIndex > 0;
          }
        }
      }
    }

    private void legendOrderComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      GenerateCharts(true);
    }
    #endregion
  }
}

