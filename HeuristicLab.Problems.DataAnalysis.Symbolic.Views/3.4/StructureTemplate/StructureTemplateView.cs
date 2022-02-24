 using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
      this.Resize += StructureTemplateViewResize;
      splitContainer.SplitterMoved += SplitContainerSplitterMoved;
      treeChart.SymbolicExpressionTreeNodeClicked += SymbolicExpressionTreeNodeClicked;
      
    }

    private void SplitContainerSplitterMoved(object sender, EventArgs e) {
      PaintTree();
    }

    private void StructureTemplateViewResize(object sender, EventArgs e) {
      PaintTree();
    }

    private void SymbolicExpressionTreeNodeClicked(object sender, MouseEventArgs e) {
      var visualTreeNode = sender as VisualTreeNode<ISymbolicExpressionTreeNode>;
      if(visualTreeNode != null && visualTreeNode.Content is SubFunctionTreeNode subFunctionTreeNode) {
        var selectedSubFunction = Content.SubFunctions.Where(x => x.Name == subFunctionTreeNode.Name).FirstOrDefault();
        if(subFunctionTreeNode != null && selectedSubFunction != null)
          viewHost.Content = selectedSubFunction;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      parseButton.Enabled = Content != null && !Locked && !ReadOnly;
      linearScalingCheckBox.Enabled = Content != null && !Locked && !ReadOnly;
      expressionInput.Enabled = Content != null && !Locked && !ReadOnly;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) return;
      expressionInput.Text = Content.Template;
      linearScalingCheckBox.Checked = Content.ApplyLinearScaling; 
      PaintTree();
      errorLabel.Text = "";
    }

    private void ParseButtonClick(object sender, EventArgs e) {
      Parse();
    }

    private void ExpressionInputTextChanged(object sender, EventArgs e) {
      errorLabel.Text = "Unparsed changes! Press parse button to save changes.";
      errorLabel.ForeColor = Color.DarkOrange;
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

    private void ExpressionInputKeyUp(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter)
        Parse();
    }

    private void Parse() {
      viewHost.Content = null; // reset active detail view
      if (!string.IsNullOrEmpty(expressionInput.Text)) {
        try {
          Content.Template = expressionInput.Text;
          PaintTree();
          errorLabel.Text = "Template structure successfully parsed.";
          errorLabel.ForeColor = Color.DarkGreen;
        } catch (AggregateException ex) {
          errorLabel.Text = string.Join("\n", ex.InnerExceptions.Select(x => x.Message));
          errorLabel.ForeColor = Color.DarkRed;
        } catch (Exception ex) {
          errorLabel.Text = ex.Message;
          errorLabel.ForeColor = Color.DarkRed;
        }
      }
    }

    private void LinearScalingCheckBoxCheckStateChanged(object sender, EventArgs e) {
      Content.ApplyLinearScaling = linearScalingCheckBox.Checked;
      PaintTree();
    }

    private void HelpButtonDoubleClick(object sender, EventArgs e) {
      using (InfoBox dialog = new InfoBox("Help for structure template",
        "HeuristicLab.Problems.DataAnalysis.Symbolic.Views.Resources.structureTemplateHelp.rtf",
        this)) {
        dialog.ShowDialog(this);
      }
    }
  }
}
