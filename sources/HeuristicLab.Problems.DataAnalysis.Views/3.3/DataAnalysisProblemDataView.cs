using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Data-Analysis Problem View")]
  [Content(typeof(DataAnalysisProblemData), true)]
  public partial class DataAnalysisProblemDataView : ParameterizedNamedItemView {
    private OpenFileDialog openFileDialog;
    public new DataAnalysisProblemData Content {
      get { return (DataAnalysisProblemData)base.Content; }
      set {
        base.Content = value;
      }
    }

    public DataAnalysisProblemDataView() {
      InitializeComponent();
    }

    private void importButton_Click(object sender, EventArgs e) {
      if (openFileDialog == null) openFileDialog = new OpenFileDialog();

      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          Content.ImportFromFile(openFileDialog.FileName);
        }
        catch (Exception ex) {
          Auxiliary.ShowErrorMessageBox(ex);
        }
      }
    }
  }
}
