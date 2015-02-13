using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Analysis;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.DataPreprocessing.Views {

  [View("Scatter Plot Single View")]
  [Content(typeof(ScatterPlotContent), true)]
  public partial class ScatterPlotSingleView : ItemView {

    public new ScatterPlotContent Content {
      get { return (ScatterPlotContent)base.Content; }
      set { base.Content = value; }
    }

    public ScatterPlotSingleView()
    {
      InitializeComponent();
    }

    public void InitData() {

      IEnumerable<string> variables = Content.PreprocessingData.GetDoubleVariableNames();

      // add variables to combo boxes
      comboBoxXVariable.Items.Clear();
      comboBoxYVariable.Items.Clear();
      comboBoxXVariable.Items.AddRange(variables.ToArray());
      comboBoxYVariable.Items.AddRange(variables.ToArray());

      // use x and y variable from content
      if (Content.SelectedXVariable != null && Content.SelectedYVariable != null) {
        comboBoxXVariable.SelectedItem = Content.SelectedXVariable;
        comboBoxYVariable.SelectedItem = Content.SelectedYVariable;
      } else {
        if (variables.Count() >= 2) {
          comboBoxXVariable.SelectedIndex = 0;
          comboBoxYVariable.SelectedIndex = 1;
          UpdateScatterPlot();

        }
      }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        InitData();
      }
    }

    private void comboBox_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateScatterPlot();
    }

    private void UpdateScatterPlot() {
      if (comboBoxXVariable.SelectedItem != null && comboBoxYVariable.SelectedItem != null) {
        //get scatter plot with selected x and y variable
        ScatterPlot scatterPlot = Content.CreateScatterPlot((string)comboBoxXVariable.SelectedItem, (string)comboBoxYVariable.SelectedItem);
        scatterPlotView.Content = scatterPlot;

        //save selected x and y variable in content
        this.Content.SelectedXVariable = (string)comboBoxXVariable.SelectedItem;
        this.Content.SelectedYVariable = (string)comboBoxYVariable.SelectedItem;
      }
    }

  }


}
