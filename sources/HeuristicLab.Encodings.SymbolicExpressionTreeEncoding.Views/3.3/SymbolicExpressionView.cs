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
    private SymbolicExpressionTreeStringFormatter treeFormatter;

    public new SymbolicExpressionTree Content {
      get { return (SymbolicExpressionTree)base.Content; }
      set { base.Content = value; }
    }

    public SymbolicExpressionView() {
      InitializeComponent();
      treeFormatter = new SymbolicExpressionTreeStringFormatter();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null)
        textBox.Text = string.Empty;
      else
        textBox.Text = treeFormatter.Format(Content);
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      textBox.Enabled = Content != null;
      textBox.ReadOnly = ReadOnly;
    }
  }
}
