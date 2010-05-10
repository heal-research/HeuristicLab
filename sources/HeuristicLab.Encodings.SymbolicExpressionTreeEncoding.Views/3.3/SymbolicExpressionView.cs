#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

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

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        Caption = "SymbolicExpression View";
        textBox.Text = string.Empty;
      } else {
        textBox.Text = SymbolicExpression(Content.Root, 0);
      }
      SetEnabledStateOfControls();
    }

    protected override void OnReadOnlyChanged() {
      base.OnReadOnlyChanged();
      SetEnabledStateOfControls();
    }

    private void SetEnabledStateOfControls() {
      textBox.Enabled = Content != null;
      textBox.ReadOnly = ReadOnly;
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
