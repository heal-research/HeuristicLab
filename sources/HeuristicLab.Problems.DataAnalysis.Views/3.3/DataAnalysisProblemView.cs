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
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.DataAnalysis {
  [View("Data-Analysis Problem View")]
  [Content(typeof(DataAnalysisProblem), true)]
  public partial class DataAnalysisProblemView : NamedItemView {
    public new DataAnalysisProblem Content {
      get { return (DataAnalysisProblem)base.Content; }
      set { base.Content = value; }
    }

    public DataAnalysisProblemView() {
      InitializeComponent();
    }

    public DataAnalysisProblemView(DataAnalysisProblem content)
      : this() {
      Content = content;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        parameterCollectionView.Content = null;
        parameterCollectionView.Enabled = false;
        importButton.Enabled = false;
      } else {
        parameterCollectionView.Content = ((IParameterizedNamedItem)Content).Parameters;
        parameterCollectionView.Enabled = true;
        importButton.Enabled = true;
      }
    }
  }
}
