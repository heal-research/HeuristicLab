using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {

  [View("StructureTemplate View")]
  [Content(typeof(StructureTemplate), true)]
  public partial class StructureTemplateView : AsynchronousContentView {
    public new StructureTemplate Content {
      get => (StructureTemplate)base.Content;
      set => base.Content = value;
    }

    public StructureTemplateView() {
      InitializeComponent();
      errorLabel.Text = "";
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) return;

      expressionInput.Text = Content.Template;
      symRegTreeChart.Content = Content.Tree;
      subFunctionListView.Content = new ItemList<SubFunction>(Content.SubFunctions.Values).AsReadOnly();
      
      errorLabel.Text = "";
      
    }

    private void parseButton_Click(object sender, EventArgs e) {
      if(!string.IsNullOrEmpty(expressionInput.Text)) {
        try {
          Content.Template = expressionInput.Text;
          symRegTreeChart.Content = Content.Tree;

          var subFunctionList = new ItemList<SubFunction>();
          foreach (var func in Content.SubFunctions.Values)
            subFunctionList.Add(func);
          subFunctionListView.Content = subFunctionList.AsReadOnly();

          errorLabel.Text = "Template structure successfully parsed.";
          errorLabel.ForeColor = Color.DarkGreen;
        } catch (Exception ex) {
          errorLabel.Text = ex.Message;
          errorLabel.ForeColor = Color.DarkRed;
        }
      }
    }

    private void expressionInput_TextChanged(object sender, EventArgs e) {
      errorLabel.Text = "Unparsed changes! Press parse button to save changes.";
      errorLabel.ForeColor = Color.DarkOrange;
    }

  }
}
