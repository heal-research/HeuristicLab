#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Discriminant Function Classification Model")]
  [Content(typeof(DiscriminantFunctionClassificationModel), true)]
  internal sealed partial class DiscriminantFunctionClassificationModelView : AsynchronousContentView {
    private ViewHost view;

    public DiscriminantFunctionClassificationModelView() {
      InitializeComponent();
      view = new ViewHost();
      view.Dock = DockStyle.Fill;
      Controls.Add(view);
    }

    public new DiscriminantFunctionClassificationModel Content {
      get { return (DiscriminantFunctionClassificationModel)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        view.Content = Content.Model;
      } else
        view.Content = null;
    }
  }
}
