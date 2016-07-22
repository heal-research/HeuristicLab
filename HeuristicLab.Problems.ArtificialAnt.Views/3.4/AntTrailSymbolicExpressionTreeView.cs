#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.ArtificialAnt.Views {
  [View("Artificial Ant Symbolic Expression Tree View")]
  [Content(typeof(AntTrail), false)]
  public sealed partial class AntTrailSymbolicExpressionTreeView : ItemView {
    private GraphicalSymbolicExpressionTreeView treeView;

    public new AntTrail Content {
      get { return (AntTrail)base.Content; }
      set { base.Content = value; }
    }

    public AntTrailSymbolicExpressionTreeView() {
      InitializeComponent();
      treeView = new GraphicalSymbolicExpressionTreeView();
      treeView.Dock = DockStyle.Fill;
      symExpressionGroupBox.Controls.Add(treeView);
    }

    protected override void DeregisterContentEvents() {
      Content.SymbolicExpressionTreeChanged -= new EventHandler(Content_SymbolicExpressionTreeChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.SymbolicExpressionTreeChanged += new EventHandler(Content_SymbolicExpressionTreeChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        treeView.Content = null;
      } else {
        treeView.Content = Content.SymbolicExpressionTree;
      }
    }

    void Content_SymbolicExpressionTreeChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_SymbolicExpressionTreeChanged), sender, e);
      else
        OnContentChanged();
    }
  }
}
