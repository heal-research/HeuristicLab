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

using System;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Optimization.Views {
  /// <summary>
  /// The base class for visual representations of items.
  /// </summary>
  [View("Problem View")]
  [Content(typeof(IProblem), true)]
  public partial class ProblemView : AsynchronousContentView {

    public new IProblem Content {
      get { return (IProblem)base.Content; }
      set { base.Content = value; }
    }



    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public ProblemView() {
      InitializeComponent();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.NameChanged += Content_NameChanged;
    }

    protected override void DeregisterContentEvents() {
      Content.NameChanged -= Content_NameChanged;
      base.DeregisterContentEvents();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      problemInstanceProvidersControl.Consumer = Content as IProblemInstanceConsumer;
      namedItemView.Content = Content;
      resultsProducingItemView.Content = Content;

      if (!problemInstanceProvidersControl.ProvidersAvailable) problemInstanceSplitContainer.Panel1Collapsed = true;
      if (Content == null) return;

      Caption = Content.Name;
    }

    protected virtual void Content_NameChanged(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_NameChanged), sender, e);
        return;
      }
      Caption = Content.Name;
    }

  }
}
