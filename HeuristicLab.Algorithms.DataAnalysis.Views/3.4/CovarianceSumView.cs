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

using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  [View("Covariance Sum View")]
  [Content(typeof(CovarianceSum), true)]
  public partial class CovarianceSumView : AsynchronousContentView {

    public new CovarianceSum Content {
      get { return (CovarianceSum)base.Content; }
      set { base.Content = value; }
    }

    public CovarianceSumView()
      : base() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        // clear
        this.itemListView.Content = null;
      } else {
        // update
        this.itemListView.Content = Content.Terms;
      }
    }
  }
}
