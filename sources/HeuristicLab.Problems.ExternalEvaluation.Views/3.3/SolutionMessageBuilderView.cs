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

using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.ExternalEvaluation.Views {
  [View("Solution Message Builder View")]
  [Content(typeof(SolutionMessageBuilder), IsDefaultView = true)]
  public sealed partial class SolutionMessageBuilderView : NamedItemView {
    CheckedItemListView<IItemToSolutionMessageConverter> converterView;

    public new SolutionMessageBuilder Content {
      get { return (SolutionMessageBuilder)base.Content; }
      set { base.Content = value; }
    }

    public SolutionMessageBuilderView() {
      InitializeComponent();
      converterView = new CheckedItemListView<IItemToSolutionMessageConverter>();
      converterView.Dock = DockStyle.Fill;
      convertersGroupBox.Controls.Add(converterView);
    }

    protected override void DeregisterContentEvents() {
      // TODO: Deregister your event handlers here
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      // TODO: Register your event handlers here
    }

    #region Event Handlers (Content)
    // TODO: Put event handlers of the content here
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        converterView.Content = null;
      } else {
        converterView.Content = Content.Converters;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
    }

    #region Event Handlers (child controls)
    // TODO: Put event handlers of child controls here.
    #endregion
  }
}
