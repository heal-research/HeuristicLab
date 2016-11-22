using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Analysis;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.DataPreprocessing.Views {
  [View("Scatter Plot Multi View")]
  [Content(typeof(ScatterPlotContent), false)]
  public partial class ScatterPlotMultiView : PreprocessingCheckedVariablesView {
    private readonly IDictionary<string, Label> columnHeaderCache;
    private readonly IDictionary<string, Label> rowHeaderCache;
    private readonly IDictionary<Tuple<string/*col*/, string/*row*/>, ItemView> bodyCache;

    public ScatterPlotMultiView() {
      InitializeComponent();

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

      columnHeaderCache = new Dictionary<string, Label>();
      rowHeaderCache = new Dictionary<string, Label>();
      bodyCache = new Dictionary<Tuple<string, string>, ItemView>();

      bodyScrollPanel.MouseWheel += bodyScrollPanel_MouseWheel;
    }

    public new ScatterPlotContent Content {
      get { return (ScatterPlotContent)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        GenerateCharts();
      }
    }

    protected override void CheckedItemsChanged(object sender, CollectionItemsChangedEventArgs<IndexedItem<StringValue>> checkedItems) {
      base.CheckedItemsChanged(sender, checkedItems);
      foreach (var variable in checkedItems.Items.Select(i => i.Value.Value)) {
        if (IsVariableChecked(variable))
          AddChartToTable(variable);
        else
          RemoveChartFromTable(variable);
      }
    }

    #region Add and remove charts
    private void AddChartToTable(string variable) {
      frameTableLayoutPanel.SuspendLayout();

      // find index to insert 
      var variables = checkedItemList.Content.Select(v => v.Value).ToList();
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
      for (int c = tlp.ColumnCount; c >  idx - 1; c--) {
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

    #region Add/Remove/Update Variable
    protected override void AddVariable(string name) {
      base.AddVariable(name);
      if (IsVariableChecked(name))
        AddChartToTable(name);
    }
    protected override void RemoveVariable(string name) {
      base.RemoveVariable(name);

      // clear caches
      columnHeaderCache.Remove(name);
      rowHeaderCache.Remove(name);
      var keys = bodyCache.Keys.Where(t => t.Item1 == name || t.Item2 == name).ToList();
      foreach (var key in keys)
        bodyCache.Remove(key);

      if (IsVariableChecked(name)) {
        RemoveChartFromTable(name);
      }
    }
    protected override void UpdateVariable(string name) {
      base.UpdateVariable(name);
      RemoveVariable(name);
      AddVariable(name);
    }
    protected override void ResetAllVariables() {
      GenerateCharts();
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
        rowHeaderCache.Add(variable, new Label() {
          Text = variable,
          TextAlign = ContentAlignment.MiddleCenter,
          Name = variable,
          Width = rowHeaderTableLayoutPanel.Width,
          Dock = DockStyle.Fill,
          Margin = new Padding(3)
        });
      }
      return rowHeaderCache[variable];
    }
    private ItemView GetBody(string colVariable, string rowVariable) {
      var key = Tuple.Create(colVariable, rowVariable);
      if (!bodyCache.ContainsKey(key)) {
        if (rowVariable == colVariable) { // use historgram if x and y variable are equal
          PreprocessingDataTable dataTable = new PreprocessingDataTable();
          DataRow dataRow = Content.CreateDataRow(rowVariable, DataRowVisualProperties.DataRowChartType.Histogram);
          dataTable.Rows.Add(dataRow);
          PreprocessingDataTableView pcv = new PreprocessingDataTableView {
            Name = key.ToString(),
            Content = dataTable,
            Dock = DockStyle.Fill,
            ShowLegend = false,
            XAxisFormat = "G3"
          };
          pcv.ChartDoubleClick += HistogramDoubleClick;
          bodyCache.Add(key, pcv);
        } else { //scatter plot
          ScatterPlot scatterPlot = Content.CreateScatterPlot(colVariable, rowVariable);
          PreprocessingScatterPlotView pspv = new PreprocessingScatterPlotView {
            Name = key.ToString(),
            Content = scatterPlot,
            Dock = DockStyle.Fill,
            ShowLegend = false,
            XAxisFormat = "G3"
          };
          pspv.ChartDoubleClick += ScatterPlotDoubleClick;
          bodyCache.Add(key, pspv);
        }
      }
      return bodyCache[key];
    }
    #endregion

    #region Generate Charts
    private void GenerateCharts() {
      var variables = GetCheckedVariables();

      // Clear old layouts and cache
      foreach (var tableLayoutPanel in new[] { columnHeaderTableLayoutPanel, rowHeaderTableLayoutPanel, bodyTableLayoutPanel }) {
        tableLayoutPanel.Controls.Clear();
        tableLayoutPanel.ColumnStyles.Clear();
        tableLayoutPanel.RowStyles.Clear();
      }
      columnHeaderCache.Clear();
      rowHeaderCache.Clear();
      bodyCache.Clear();

      // Set row and column count
      columnHeaderTableLayoutPanel.ColumnCount = variables.Count;
      rowHeaderTableLayoutPanel.RowCount = variables.Count;
      bodyTableLayoutPanel.ColumnCount = variables.Count;
      bodyTableLayoutPanel.RowCount = variables.Count;

      // Set column and row layout
      for (int i = 0; i < variables.Count; i++) {
        columnHeaderTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, GetColumnWidth()));
        rowHeaderTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, GetRowHeight()));
        bodyTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, GetColumnWidth()));
        bodyTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, GetRowHeight()));
      }

      frameTableLayoutPanel.SuspendLayout();
      AddHeaderToTableLayoutPanels();
      AddChartsToTableLayoutPanel();
      UpdateHeaderMargin();
      frameTableLayoutPanel.ResumeLayout(true);
    }

    private void AddHeaderToTableLayoutPanels() {
      int i = 0;
      foreach (var variable in GetCheckedVariables()) {
        columnHeaderTableLayoutPanel.Controls.Add(GetColumnHeader(variable), i, 0);
        rowHeaderTableLayoutPanel.Controls.Add(GetRowHeader(variable), 0, i);
        i++;
      }
    }
    private void AddChartsToTableLayoutPanel() {
      int c = 0;
      foreach (var colVar in GetCheckedVariables()) {
        if (!IsVariableChecked(colVar)) continue;
        int r = 0;
        foreach (var rowVar in GetCheckedVariables()) {
          if (!IsVariableChecked(rowVar)) continue;
          bodyTableLayoutPanel.Controls.Add(GetBody(colVar, rowVar), c, r);
          r++;
        }
        c++;
      }
    }

    #endregion

    #region DoubleClick Events
    //Open scatter plot in new tab with new content when double clicked
    private void ScatterPlotDoubleClick(object sender, EventArgs e) {
      PreprocessingScatterPlotView pspv = (PreprocessingScatterPlotView)sender;
      ScatterPlotContent scatterContent = new ScatterPlotContent(Content, new Cloner());  // create new content
      ScatterPlot scatterPlot = pspv.Content;

      //Extract variable names from scatter plot and set them in content
      if (scatterPlot.Rows.Count == 1) {
        string[] variables = scatterPlot.Rows.ElementAt(0).Name.Split(new string[] { " - " }, StringSplitOptions.None); // extract variable names from string
        scatterContent.SelectedXVariable = variables[0];
        scatterContent.SelectedYVariable = variables[1];
      }

      MainFormManager.MainForm.ShowContent(scatterContent, typeof(ScatterPlotSingleView));  // open in new tab
    }

    //open histogram in new tab with new content when double clicked
    private void HistogramDoubleClick(object sender, EventArgs e) {
      PreprocessingDataTableView pcv = (PreprocessingDataTableView)sender;
      HistogramContent histoContent = new HistogramContent(Content.PreprocessingData);  // create new content     
      histoContent.VariableItemList = Content.CreateVariableItemList();
      PreprocessingDataTable dataTable = pcv.Content;

      //Set variable item list from with variable from data table
      if (dataTable.Rows.Count == 1) { // only one data row should be in data table 
        string variableName = dataTable.Rows.ElementAt(0).Name;

        // set only variable name checked
        foreach (var checkedItem in histoContent.VariableItemList) {
          histoContent.VariableItemList.SetItemCheckedState(checkedItem, checkedItem.Value == variableName);
        }
      }
      MainFormManager.MainForm.ShowContent(histoContent, typeof(HistogramView));  // open in new tab
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
    private int GetColumnWidth() { return (int)(bodyScrollPanel.Width * ((float)widthTrackBar.Value / 100)); }
    private int GetRowHeight() { return (int)(bodyScrollPanel.Height * ((float)heightTrackBar.Value / 100)); }
    private void widthTrackBar_ValueChanged(object sender, EventArgs e) {
      frameTableLayoutPanel.SuspendRepaint();
      for (int i = 0; i < columnHeaderTableLayoutPanel.ColumnCount; i++) {
        columnHeaderTableLayoutPanel.ColumnStyles[i].Width = GetColumnWidth();
        bodyTableLayoutPanel.ColumnStyles[i].Width = GetColumnWidth();
      }
      frameTableLayoutPanel.ResumeRepaint(true);
    }
    private void heightTrackBar_ValueChanged(object sender, EventArgs e) {
      frameTableLayoutPanel.SuspendRepaint();

      for (int i = 0; i < rowHeaderTableLayoutPanel.RowCount; i++) {
        rowHeaderTableLayoutPanel.RowStyles[i].Height = GetRowHeight();
        bodyTableLayoutPanel.RowStyles[i].Height = GetRowHeight();
      }
      frameTableLayoutPanel.ResumeRepaint(true);
    }
    #endregion
  }
}
