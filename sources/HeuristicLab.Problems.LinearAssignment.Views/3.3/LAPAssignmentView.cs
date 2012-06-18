#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.ComponentModel;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.LinearAssignment.Views {
  [View("LAPAssignmentView")]
  [Content(typeof(LAPAssignment), IsDefaultView = true)]
  public partial class LAPAssignmentView : ItemView {
    public new LAPAssignment Content {
      get { return (LAPAssignment)base.Content; }
      set { base.Content = value; }
    }

    public LAPAssignmentView() {
      InitializeComponent();
    }

    #region Register Content Events
    protected override void DeregisterContentEvents() {
      Content.PropertyChanged -= new PropertyChangedEventHandler(Content_PropertyChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.PropertyChanged += new PropertyChangedEventHandler(Content_PropertyChanged);
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        qualityView.Content = null;
        assignmentView.Content = null;
      } else {
        qualityView.Content = Content.Quality;
        assignmentView.Content = Content.Assignment;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
    }

    #region Event Handlers
    private void Content_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      switch (e.PropertyName) {
        case "Quality": qualityView.Content = Content.Quality;
          break;
        case "Assignment": assignmentView.Content = Content.Assignment;
          break;
        default: break;
      }
    }
    #endregion
  }
}
