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

using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Optimization.Views {
  [View("Single-Objective Evaluation Result View")]
  [Content(typeof(SingleObjectiveEvaluationResult), IsDefaultView = true)]
  public partial class SingleObjectiveEvaluationResultView : ItemView {
    
    public new SingleObjectiveEvaluationResult Content {
      get => (SingleObjectiveEvaluationResult)base.Content;
      set => base.Content = value;
    }

    public SingleObjectiveEvaluationResultView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        qualityTextBox.Text = string.Empty;
      } else {
        qualityTextBox.Text = Content.Quality.ToString();
      }
      dataViewHost.Content = FetchAdditionalData();
    }

    private ReadOnlyItemCollection<IItem> FetchAdditionalData() {
      if (Content == null) return null;

      var list = new ItemCollection<IItem>();
      foreach (var kvp in Content.AdditionalData) {
        if (kvp.Value is INamedItem namedItem) {
          list.Add(namedItem);
        } else if (kvp.Value is IItem item) {
          list.Add(new Variable(kvp.Key, item));
        } else {
          list.Add(new Variable(kvp.Key, new StringValue(kvp.Value?.ToString() ?? "(null)", @readonly: true)));
        }
      }
      return list.AsReadOnly();
    }
  }
}
