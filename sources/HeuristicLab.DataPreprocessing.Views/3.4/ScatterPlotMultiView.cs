using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Analysis;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.DataPreprocessing.Views {
  [View("Scatter Plot Multi View")]
  [Content(typeof(ScatterPlotContent), false)]
  public partial class ScatterPlotMultiView : PreprocessingCheckedVariablesView {
    private const int MAX_AUTO_SIZE_ELEMENTS = 6;
    private const int FIXED_CHART_WIDTH = 250;
    private const int FIXED_CHART_HEIGHT = 150;

    public ScatterPlotMultiView() {
      InitializeComponent();

      columnHeaderTableLayoutPanel.HorizontalScroll.Enabled = true;
      columnHeaderTableLayoutPanel.VerticalScroll.Enabled = false;
      columnHeaderTableLayoutPanel.HorizontalScroll.Visible = false;
      columnHeaderTableLayoutPanel.VerticalScroll.Visible = false;

      rowHeaderTableLayoutPanel.HorizontalScroll.Enabled = false;
      rowHeaderTableLayoutPanel.VerticalScroll.Enabled = true;
      rowHeaderTableLayoutPanel.HorizontalScroll.Visible = false;
      rowHeaderTableLayoutPanel.VerticalScroll.Visible = false;

      bodyTableLayoutPanel.HorizontalScroll.Enabled = true;
      bodyTableLayoutPanel.VerticalScroll.Enabled = true;
      bodyTableLayoutPanel.HorizontalScroll.Visible = true;
      bodyTableLayoutPanel.VerticalScroll.Visible = true;
      bodyTableLayoutPanel.AutoScroll = true;

      bodyTableLayoutPanel.MouseWheel += bodyTableLayoutPanel_MouseWheel;

      splitContainer.Panel1Collapsed = true; // ToDo: remove after correctly handling unchecks
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

    }

    #region Add/Remove/Update Variable, Reset
    protected override void AddVariable(string name) {
      base.AddVariable(name);
    }
    protected override void RemoveVariable(string name) {
      base.RemoveVariable(name);

    }
    protected override void UpdateVariable(string name) {
      base.UpdateVariable(name);

    }
    protected override void ResetAllVariables() {
      GenerateCharts();
    }
    #endregion

    #region Generate Charts
    private void GenerateCharts() {
      List<string> variables = Content.PreprocessingData.GetDoubleVariableNames().ToList();
      var contentTableLayoutPanels = new[] { columnHeaderTableLayoutPanel, rowHeaderTableLayoutPanel, bodyTableLayoutPanel };

      // Clear table layouts
      foreach (var tlp in contentTableLayoutPanels) {
        tlp.Controls.Clear();
        //Clear out the existing row and column styles
        tlp.ColumnStyles.Clear();
        tlp.RowStyles.Clear();
      }

      // Set row and column count
      columnHeaderTableLayoutPanel.ColumnCount = variables.Count + 1;
      rowHeaderTableLayoutPanel.RowCount = variables.Count + 1;
      bodyTableLayoutPanel.ColumnCount = variables.Count;
      bodyTableLayoutPanel.RowCount = variables.Count;

      // Set column and row layout
      int width = variables.Count <= MAX_AUTO_SIZE_ELEMENTS ? bodyTableLayoutPanel.Width / variables.Count : FIXED_CHART_WIDTH;
      int height = variables.Count <= MAX_AUTO_SIZE_ELEMENTS ? bodyTableLayoutPanel.Height / variables.Count : FIXED_CHART_HEIGHT;
      for (int i = 0; i < variables.Count; i++) {
        columnHeaderTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, width));
        rowHeaderTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
        bodyTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, width));
        bodyTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
      }

      //frameTableLayoutPanel.SuspendLayout();
      AddHeaderToTableLayoutPanels();
      AddChartsToTableLayoutPanel();
      //frameTableLayoutPanel.ResumeLayout(true);
    }

    private void AddHeaderToTableLayoutPanels() {
      List<string> variables = Content.PreprocessingData.GetDoubleVariableNames().ToList();
      int width = variables.Count <= MAX_AUTO_SIZE_ELEMENTS ? bodyTableLayoutPanel.Width / variables.Count : FIXED_CHART_WIDTH;
      int height = variables.Count <= MAX_AUTO_SIZE_ELEMENTS ? bodyTableLayoutPanel.Height / variables.Count : FIXED_CHART_HEIGHT;
      for (int i = 0; i < variables.Count; i++) {
        columnHeaderTableLayoutPanel.Controls.Add(new Label() {
          Text = variables[i],
          TextAlign = ContentAlignment.MiddleCenter,
          Width = width,
          Height = columnHeaderTableLayoutPanel.Height,
          Dock = DockStyle.Fill,
          Margin = new Padding(3)
        }, i, 0);
        rowHeaderTableLayoutPanel.Controls.Add(new Label() {
          Text = variables[i],
          TextAlign = ContentAlignment.MiddleCenter,
          Width = rowHeaderTableLayoutPanel.Width,
          Height = height,
          Dock = DockStyle.Fill,
          Margin = new Padding(3)
        }, 0, i);
      }

      // Add empty labels with fixed size to correct scollbar width/height in headers
      columnHeaderTableLayoutPanel.Controls.Add(new Panel() {
        Width = bodyTableLayoutPanel.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0,
        Height = columnHeaderTableLayoutPanel.Height,
        Dock = DockStyle.Fill,
        //BackColor = Color.DarkRed
      }, variables.Count, 0);
      rowHeaderTableLayoutPanel.Controls.Add(new Panel() {
        Width = rowHeaderTableLayoutPanel.Width,
        Height = bodyTableLayoutPanel.HorizontalScroll.Visible ? SystemInformation.HorizontalScrollBarHeight : 0,
        Dock = DockStyle.Fill,
        //BackColor = Color.DarkRed
      }, 0, variables.Count);
    }
    private void AddChartsToTableLayoutPanel() {
      List<string> variables = Content.PreprocessingData.GetDoubleVariableNames().ToList();

      //set scatter plots and histograms
      for (int x = 0; x < variables.Count; x++) {
        for (int y = 0; y < variables.Count; y++) {
          if (x == y) { // use historgram if x and y variable are equal
            PreprocessingDataTable dataTable = new PreprocessingDataTable();
            DataRow dataRow = Content.CreateDataRow(variables[x], DataRowVisualProperties.DataRowChartType.Histogram);
            dataTable.Rows.Add(dataRow);
            PreprocessingDataTableView pcv = new PreprocessingDataTableView {
              Content = dataTable,
              Dock = DockStyle.Fill,
              ShowLegend = false,
              XAxisFormat = "G3"
            };
            pcv.ChartDoubleClick += HistogramDoubleClick;
            bodyTableLayoutPanel.Controls.Add(pcv, y, x);
          } else { //scatter plot
            ScatterPlot scatterPlot = Content.CreateScatterPlot(variables[x], variables[y]);
            PreprocessingScatterPlotView pspv = new PreprocessingScatterPlotView {
              Content = scatterPlot,
              Dock = DockStyle.Fill,
              ShowLegend = false,
              XAxisFormat = "G3"
            };
            pspv.ChartDoubleClick += ScatterPlotDoubleClick;
            bodyTableLayoutPanel.Controls.Add(pspv, x, y);
          }
        }
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

    private void bodyTableLayoutPanel_Scroll(object sender, ScrollEventArgs e) {
      SyncScroll();

      // update "padding labels" after scrollbars are added or removed
      var columHeaderPadding = columnHeaderTableLayoutPanel.Controls[columnHeaderTableLayoutPanel.Controls.Count - 1];
      var rowHeaderPadding = rowHeaderTableLayoutPanel.Controls[rowHeaderTableLayoutPanel.ColumnCount - 1];
      columHeaderPadding.Width = bodyTableLayoutPanel.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0;
      rowHeaderPadding.Height = bodyTableLayoutPanel.HorizontalScroll.Visible ? SystemInformation.HorizontalScrollBarHeight : 0;
    }
    private void bodyTableLayoutPanel_MouseWheel(object sender, MouseEventArgs e) {
      // Scrolling with the mouse wheel is not captured in the Scoll event
      SyncScroll();
    }
    private void SyncScroll() {
      //Debug.WriteLine("H: {0} <- {1}", columnHeaderTableLayoutPanel.HorizontalScroll.Value, bodyTableLayoutPanel.HorizontalScroll.Value);
      //Debug.WriteLine("V: {0} <- {1}", rowHeaderTableLayoutPanel.VerticalScroll.Value, bodyTableLayoutPanel.VerticalScroll.Value);

      frameTableLayoutPanel.SuspendRepaint();

      columnHeaderTableLayoutPanel.HorizontalScroll.Minimum = bodyTableLayoutPanel.HorizontalScroll.Minimum;
      columnHeaderTableLayoutPanel.HorizontalScroll.Maximum = bodyTableLayoutPanel.HorizontalScroll.Maximum;
      rowHeaderTableLayoutPanel.VerticalScroll.Minimum = bodyTableLayoutPanel.VerticalScroll.Minimum;
      rowHeaderTableLayoutPanel.VerticalScroll.Maximum = bodyTableLayoutPanel.VerticalScroll.Maximum;

      columnHeaderTableLayoutPanel.HorizontalScroll.Value = Math.Max(bodyTableLayoutPanel.HorizontalScroll.Value, 1);
      rowHeaderTableLayoutPanel.VerticalScroll.Value = Math.Max(bodyTableLayoutPanel.VerticalScroll.Value, 1);
      // minimum 1 is nececary  because of two factors:
      // - setting the Value-property of Horizontal/VerticalScroll updates the internal state but the Value-property stays 0
      // - setting the same number of the Value-property has no effect
      // since the Value-property is always 0, setting it to 0 would have no effect; so it is set to 1 instead

      frameTableLayoutPanel.ResumeRepaint(true);
    }
  }
}
