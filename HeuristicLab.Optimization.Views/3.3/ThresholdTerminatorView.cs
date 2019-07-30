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

namespace HeuristicLab.Optimization.Views {

  [View("ThresholdTerminator View")]
  [Content(typeof(ThresholdTerminator<>), true)]
  public partial class ThresholdTerminatorView<T> : ItemView where T : class, IItem, IStringConvertibleValue, new() {

    public new ThresholdTerminator<T> Content {
      get { return (ThresholdTerminator<T>)base.Content; }
      set { base.Content = value; }
    }

    public ThresholdTerminatorView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      viewHost.Content = null;
      if (Content != null) {
        viewHost.Content = Content.ThresholdParameter.ActualValue ?? Content.ThresholdParameter;
      }
    }
  }
}
