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
  [View("Multi-Objective Evaluation Result View")]
  [Content(typeof(MultiObjectiveEvaluationResult), IsDefaultView = true)]
  public partial class MultiObjectiveEvaluationResultView : ItemView {
    
    public new MultiObjectiveEvaluationResult Content {
      get => (MultiObjectiveEvaluationResult)base.Content;
      set => base.Content = value;
    }

    public MultiObjectiveEvaluationResultView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        qualitiesTextBox.Text = string.Empty;
      } else {
        qualitiesTextBox.Text = string.Join(" ; ", Content.Quality);
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
