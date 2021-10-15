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
      infoLabel.Text = "";
      treeChart.SymbolicExpressionTreeNodeClicked += SymbolicExpressionTreeNodeClicked;
      
    }

    private void SymbolicExpressionTreeNodeClicked(object sender, MouseEventArgs e) {
      var visualTreeNode = sender as VisualTreeNode<ISymbolicExpressionTreeNode>;
      if(visualTreeNode != null) {
        var subFunctionTreeNode = visualTreeNode.Content as SubFunctionTreeNode;
        if(Content.SubFunctions.TryGetValue(subFunctionTreeNode.Name, out SubFunction subFunction))
          viewHost.Content = subFunction;
      }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) return;
      expressionInput.Text = Content.Template;
      PaintTree();
      infoLabel.Text = "";
    }

    private void ParseButtonClick(object sender, EventArgs e) {
      if(!string.IsNullOrEmpty(expressionInput.Text)) {
        try {
          Content.Template = expressionInput.Text;
          PaintTree();
          infoLabel.Text = "Template structure successfully parsed.";
          infoLabel.ForeColor = Color.DarkGreen;
        } catch (Exception ex) {
          infoLabel.Text = ex.Message;
          infoLabel.ForeColor = Color.DarkRed;
        }
      }
    }

    private void ExpressionInputTextChanged(object sender, EventArgs e) {
      infoLabel.Text = "Unparsed changes! Press parse button to save changes.";
      infoLabel.ForeColor = Color.DarkOrange;
    }


    private void PaintTree() {
      if(Content != null && Content.Tree != null) {
        treeChart.Tree = Content.Tree;
        foreach (var n in Content.Tree.IterateNodesPrefix()) {
          if (n.Symbol is SubFunctionSymbol) {
            var visualNode = treeChart.GetVisualSymbolicExpressionTreeNode(n);
            visualNode.FillColor = Color.LightCyan;
            visualNode.LineColor = Color.SlateGray;
          }
        }
        treeChart.RepaintNodes();
      }
    }
  }
}
