#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [View("Evaluation Service-Client View")]
  [Content(typeof(EvaluationServiceClient), IsDefaultView = true)]
  public sealed partial class EvaluationServiceClientView : ParameterizedNamedItemView {
    public new EvaluationServiceClient Content {
      get { return (EvaluationServiceClient)base.Content; }
      set { base.Content = value; }
    }

    public EvaluationServiceClientView() {
      InitializeComponent();
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
        // TODO: Add code when content has been changed and is null
      } else {
        // TODO: Add code when content has been changed and is not null
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      // TODO: Enable or disable controls based on whether the content is null or the view is set readonly
    }

    #region Event Handlers (child controls)
    // TODO: Put event handlers of child controls here.
    #endregion
  }
}
