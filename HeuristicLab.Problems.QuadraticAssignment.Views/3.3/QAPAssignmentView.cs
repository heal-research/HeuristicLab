#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.QuadraticAssignment.Views {
  /// <summary>
  /// The base class for visual representations of a path tour for a TSP.
  /// </summary>
  [View("QAPAssignment View")]
  [Content(typeof(QAPAssignment), true)]
  public sealed partial class QAPAssignmentView : ItemView {
    public new QAPAssignment Content {
      get { return (QAPAssignment)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="PathTSPTourView"/>.
    /// </summary>
    public QAPAssignmentView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.PropertyChanged -= new PropertyChangedEventHandler(Content_PropertyChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.PropertyChanged += new PropertyChangedEventHandler(Content_PropertyChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        qualityViewHost.Content = null;
        assignmentViewHost.Content = null;
        qapView.Distances = null;
        qapView.Weights = null;
        qapView.Assignment = null;
      } else {
        qualityViewHost.Content = Content.Quality;
        assignmentViewHost.Content = Content.Assignment;
        qapView.Distances = Content.Distances;
        qapView.Weights = Content.Weights;
        qapView.Assignment = Content.Assignment;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      qualityGroupBox.Enabled = Content != null;
      assignmentGroupBox.Enabled = Content != null;
      qapView.Enabled = Content != null;
    }

    private void Content_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (InvokeRequired)
        Invoke(new PropertyChangedEventHandler(Content_PropertyChanged), sender, e);
      else {
        switch (e.PropertyName) {
          case "Coordinates":
            break;
          case "Distances":
            qapView.Distances = Content.Distances;
            break;
          case "Weights":
            qapView.Weights = Content.Weights;
            break;
          case "Quality":
            break;
          case "Assignment":
            assignmentViewHost.Content = Content.Assignment;
            qapView.Assignment = Content.Assignment;
            break;
          default:
            break;
        }
      }
    }
  }
}
