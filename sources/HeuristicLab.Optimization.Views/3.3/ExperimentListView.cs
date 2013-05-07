#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimization.Views {
  [View("Experiment List View")]
  [Content(typeof(Experiment), false)]
  public sealed partial class ExperimentListView : IOptimizerView {
    public ExperimentListView() {
      InitializeComponent();
    }

    public new Experiment Content {
      get { return (Experiment)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        optimizerListView.Content = null;
        runsViewHost.Content = null;
      } else {
        optimizerListView.Content = Content.Optimizers;
        runsViewHost.Content = Content.Runs;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      optimizerListView.Enabled = Content != null;
      runsViewHost.Enabled = Content != null;
    }

    protected override void OnClosed(FormClosedEventArgs e) {
      if ((Content != null) && (Content.ExecutionState == ExecutionState.Started)) {
        //The content must be stopped if no other view showing the content is available
        var optimizers = MainFormManager.MainForm.Views.OfType<IContentView>().Where(v => v != this).Select(v => v.Content).OfType<IOptimizer>();
        if (!optimizers.Contains(Content)) {
          var nestedOptimizers = optimizers.SelectMany(opt => opt.NestedOptimizers);
          if (!nestedOptimizers.Contains(Content)) Content.Stop();
        }
      }
      base.OnClosed(e);
    }
  }
}
