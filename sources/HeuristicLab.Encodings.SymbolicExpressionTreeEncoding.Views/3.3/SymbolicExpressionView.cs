using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  [View("SymbolicExpression View")]
  [Content(typeof(SymbolicExpressionTree), false)]
  public partial class SymbolicExpressionView : AsynchronousContentView {
    public new SymbolicExpressionTree Content {
      get { return (SymbolicExpressionTree)base.Content; }
      set { base.Content = value; }
    }

    public SymbolicExpressionView() {
      InitializeComponent();
      Caption = "SymbolicExpression View";
    }

    public SymbolicExpressionView(SymbolicExpressionTree content)
      : this() {
      Content = content;
    }


    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        Caption = "SymbolicExpression View";
        textBox.Text = string.Empty;
        textBox.Enabled = false;
      } else {
        textBox.Text = SymbolicExpression(Content.Root, 0);
        textBox.Enabled = true;
      }
    }

    private static string SymbolicExpression(SymbolicExpressionTreeNode node, int indentLength) {
      StringBuilder strBuilder = new StringBuilder();
      strBuilder.Append(' ', indentLength); strBuilder.Append("(");
      // internal nodes or leaf nodes?
      if (node.SubTrees.Count > 0) {
        // symbol on same line as '('
        strBuilder.AppendLine(node.ToString());
        // each subtree expression on a new line
        // and closing ')' also on new line
        foreach (var subtree in node.SubTrees) {
          strBuilder.AppendLine(SymbolicExpression(subtree, indentLength + 2));
        }
        strBuilder.Append(' ', indentLength); strBuilder.Append(")");
      } else {
        // symbol in the same line with as '(' and ')'
        strBuilder.Append(node.ToString());
        strBuilder.Append(")");
      }
      return strBuilder.ToString();
    }
  }
}
