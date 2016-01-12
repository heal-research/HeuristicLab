using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.DataPreprocessing.Views {
  [View("Scatter Plot Multi View")]
  [Content(typeof(ScatterPlotContent), false)]
  public partial class ScatterPlotMultiView : ItemView {

    private const int HEADER_WIDTH = 50;
    private const int HEADER_HEIGHT = 50;
    private const int MAX_AUTO_SIZE_ELEMENTS = 6;
    private const int FIXED_CHART_WIDTH = 250;
    private const int FIXED_CHART_HEIGHT = 150;

    public ScatterPlotMultiView() {
      InitializeComponent();
    }

    public new ScatterPlotContent Content {
      get { return (ScatterPlotContent)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        GenerateMultiLayout();
      }
    }

    //Add header elements to the table layout panel
    private void addHeaderToTableLayoutPanels() {

      List<string> variables = Content.PreprocessingData.GetDoubleVariableNames().ToList();

      for (int i = 1; i < variables.Count + 1; i++) {
        // Use buttons for header elements
        Button xButton = new Button();
        xButton.Enabled = false;
        xButton.BackColor = Color.Gainsboro;
        xButton.Text = variables[i - 1];
        xButton.Dock = DockStyle.Fill;
        tableLayoutPanel.Controls.Add(xButton, 0, i);
        Button yButton = new Button();
        yButton.Enabled = false;
        yButton.BackColor = Color.Gainsboro;
        yButton.Text = variables[i - 1];
        yButton.Dock = DockStyle.Fill;
        tableLayoutPanel.Controls.Add(yButton, i, 0);
      }
    }

    private void GenerateMultiLayout() {
      List<string> variables = Content.PreprocessingData.GetDoubleVariableNames().ToList();

      tableLayoutPanel.Controls.Clear();
      //Clear out the existing row and column styles
      tableLayoutPanel.ColumnStyles.Clear();
      tableLayoutPanel.RowStyles.Clear();

      //Set row and column count
      tableLayoutPanel.ColumnCount = variables.Count + 1;
      tableLayoutPanel.RowCount = variables.Count + 1;

      tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, HEADER_WIDTH));
      tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, HEADER_HEIGHT));
      // set column and row layout
      for (int x = 0; x < variables.Count; x++) {
        // auto size
        if (variables.Count <= MAX_AUTO_SIZE_ELEMENTS) {
          tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, (tableLayoutPanel.Width - HEADER_WIDTH) / variables.Count));
          tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, (tableLayoutPanel.Height - HEADER_HEIGHT) / variables.Count));
        }
        // fixed size
        else {
          tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, FIXED_CHART_WIDTH));
          tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, FIXED_CHART_HEIGHT));
        }
      }

      addHeaderToTableLayoutPanels();
      addChartsToTableLayoutPanel();

    }

    private void addChartsToTableLayoutPanel() {

      List<string> variables = Content.PreprocessingData.GetDoubleVariableNames().ToList();

      //set scatter plots and histograms
      for (int x = 1; x < variables.Count + 1; x++) {

        for (int y = 1; y < variables.Count + 1; y++) {
          // use historgram if x and y variable are equal
          if (x == y) {
            PreprocessingDataTable dataTable = new PreprocessingDataTable();
            DataRow dataRow = Content.CreateDataRow(variables[x - 1], DataRowVisualProperties.DataRowChartType.Histogram);
            dataTable.Rows.Add(dataRow);
            PreprocessingDataTableView pcv = new PreprocessingDataTableView();
            pcv.ChartDoubleClick += HistogramDoubleClick;
            pcv.Content = dataTable;
            tableLayoutPanel.Controls.Add(pcv, y, x);
          }
          //scatter plot
          else {
            ScatterPlot scatterPlot = Content.CreateScatterPlot(variables[x - 1], variables[y - 1]);
            PreprocessingScatterPlotView pspv = new PreprocessingScatterPlotView();
            pspv.ChartDoubleClick += ScatterPlotDoubleClick;
            pspv.Content = scatterPlot;
            pspv.Dock = DockStyle.Fill;
            tableLayoutPanel.Controls.Add(pspv, x, y);
          }
        }
      }
    }

    //Open scatter plot in new tab with new content when double clicked
    private void ScatterPlotDoubleClick(object sender, EventArgs e) {
      PreprocessingScatterPlotView pspv = (PreprocessingScatterPlotView)sender;
      ScatterPlotContent scatterContent = new ScatterPlotContent(Content, new Cloner());  // create new content
      ScatterPlot scatterPlot = pspv.Content;
      setVariablesInContentFromScatterPlot(scatterContent, scatterPlot);

      MainFormManager.MainForm.ShowContent(scatterContent, typeof(ScatterPlotSingleView));  // open in new tab
    }

    //Extract variable names from scatter plot and set them in content
    private void setVariablesInContentFromScatterPlot(ScatterPlotContent scatterContent, ScatterPlot scatterPlot) {

      // only one data row should be in scatter plot
      if (scatterPlot.Rows.Count == 1) {
        string[] variables = scatterPlot.Rows.ElementAt(0).Name.Split(new string[] { " - " }, StringSplitOptions.None); // extract variable names from string
        scatterContent.SelectedXVariable = variables[0];
        scatterContent.SelectedYVariable = variables[1];
      }
    }

    //Set variable item list from with variable from data table
    private void setVariableItemListFromDataTable(HistogramContent histoContent, PreprocessingDataTable dataTable) {

      // only one data row should be in data table 
      if (dataTable.Rows.Count == 1) {
        string variableName = dataTable.Rows.ElementAt(0).Name;

        // set only variable name checked
        foreach (var checkedItem in histoContent.VariableItemList) {
          if (checkedItem.Value == variableName)
            histoContent.VariableItemList.SetItemCheckedState(checkedItem, true);
          else
            histoContent.VariableItemList.SetItemCheckedState(checkedItem, false);

        }
      }
    }

    //open histogram in new tab with new content when double clicked
    private void HistogramDoubleClick(object sender, EventArgs e) {
      PreprocessingDataTableView pcv = (PreprocessingDataTableView)sender;
      HistogramContent histoContent = new HistogramContent(Content.PreprocessingData);  // create new content     
      histoContent.VariableItemList = Content.CreateVariableItemList();
      PreprocessingDataTable dataTable = pcv.Content;
      setVariableItemListFromDataTable(histoContent, dataTable);

      MainFormManager.MainForm.ShowContent(histoContent, typeof(HistogramView));  // open in new tab
    }


  }


}
