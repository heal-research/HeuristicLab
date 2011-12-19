#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.PluginInfrastructure;
using System.Collections.Generic;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Formatters;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  [View("SymbolicExpression View")]
  [Content(typeof(SymbolicExpressionTree), false)]
  public partial class SymbolicExpressionView : AsynchronousContentView {

    List<ISymbolicExpressionTreeStringFormatter> treeFormattersList = new List<ISymbolicExpressionTreeStringFormatter>();

    public new SymbolicExpressionTree Content {
      get { return (SymbolicExpressionTree)base.Content; }
      set { base.Content = value; }
    }

    public SymbolicExpressionView() {
      InitializeComponent();
      IEnumerable<ISymbolicExpressionTreeStringFormatter> formatters = ApplicationManager.Manager.GetInstances<ISymbolicExpressionTreeStringFormatter>();
      treeFormattersList = new List<ISymbolicExpressionTreeStringFormatter>();
      int selectedIndex = -1;
      foreach (ISymbolicExpressionTreeStringFormatter formatter in formatters) {
        if (formatter is SymbolicExpressionTreeStringFormatter)
          selectedIndex = treeFormattersList.Count;
        treeFormattersList.Add(formatter);
        formattersComboBox.Items.Add(formatter.Name);
      }
      formattersComboBox.SelectedIndex = selectedIndex;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateTextbox();
    }

    private void UpdateTextbox() {
      if (Content == null || formattersComboBox.SelectedIndex < 0)
        textBox.Text = string.Empty;
      else
        textBox.Text = treeFormattersList[formattersComboBox.SelectedIndex].Format(Content);
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      textBox.Enabled = Content != null;
      textBox.ReadOnly = ReadOnly;
    }

    private void formattersComboBox_SelectedIndexChanged(object sender, System.EventArgs e) {
      UpdateTextbox();
    }
  }
}
