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
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.OneMax;
using HeuristicLab.Data;
using System.Collections.Generic;

namespace HeuristicLab.Problems.OneMax.Views {
  /// <summary>
  /// A view for a OneMax solution.
  /// </summary>
  [View("OneMax View")]
  [Content(typeof(OneMaxSolution), true)]
  public partial class OneMaxSolutionView : HeuristicLab.Core.Views.ItemView {
    public new OneMaxSolution Content {
      get { return (OneMaxSolution)base.Content; }
      set { base.Content = value; }
    }
    
    public OneMaxSolutionView() {
      InitializeComponent();

      qualityView.ReadOnly = true;
      binaryVectorView.ReadOnly = true;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();

      if (Content == null) {
        qualityView.Content = null;
        binaryVectorView.Content = null;
      } else {
        qualityView.ViewType = null;
        qualityView.Content = Content.Quality;

        binaryVectorView.ViewType = null;
        binaryVectorView.Content = Content.BinaryVector;
      }

      SetEnabledStateOfControls();
    }

    private void SetEnabledStateOfControls() {
      qualityView.Enabled = Content != null;
      binaryVectorView.Enabled = Content != null;
    }
  }
}
