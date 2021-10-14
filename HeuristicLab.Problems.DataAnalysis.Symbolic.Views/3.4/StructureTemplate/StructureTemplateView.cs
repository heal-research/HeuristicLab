using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views;
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
      treeChart.SymbolicExpressionTreeNodeClicked += TreeChart_SymbolicExpressionTreeNodeClicked;
    }

    private void TreeChart_SymbolicExpressionTreeNodeClicked(object sender, MouseEventArgs e) {
      var visualTreeNode = sender as VisualTreeNode<ISymbolicExpressionTreeNode>;
      if(visualTreeNode != null) {
        var subFunctionTreeNode = visualTreeNode.Content as SubFunctionTreeNode;
        viewHost.Content = subFunctionTreeNode?.SubFunction;
      }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) return;

      expressionInput.Text = Content.Template;
      symRegTreeChart.Content = Content.Tree;

      treeChart.Tree = Content.Tree;

      errorLabel.Text = "";
      
    }

    private void parseButton_Click(object sender, EventArgs e) {
      if(!string.IsNullOrEmpty(expressionInput.Text)) {
        try {
          Content.Template = expressionInput.Text;
          symRegTreeChart.Content = Content.Tree;
          treeChart.Tree = Content.Tree;

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
