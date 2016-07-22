#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.MainForm;

namespace HeuristicLab.Parameters.Views {
  [View("ConstrainedValueParameter Configuration View")]
  [Content(typeof(OptionalConstrainedValueParameter<>), false)]
  [Content(typeof(ConstrainedValueParameter<>), false)]
  public partial class ConstrainedValueParameterConfigurationView<T> : ConstrainedValueParameterView<T> where T : class, IItem {
    public ConstrainedValueParameterConfigurationView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      configureButton.Checked = false;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      configureButton.Enabled = Content != null && !ReadOnly;
    }

    #region Content Events
    protected override void Content_ValueChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ValueChanged), sender, e);
      else {
        SetDataTypeTextBoxText();
        SetComboBoxSelectedIndex();
        if (!configureButton.Checked) {
          viewHost.ViewType = null;
          viewHost.Content = Content != null ? Content.Value : null;
        }
      }
    }
    #endregion

    protected virtual void configureButton_CheckedChanged(object sender, EventArgs e) {
      if (Content == null) return;
      var newContent = configureButton.Checked ? (IContent)Content.ValidValues : (IContent)Content.Value;
      if (viewHost.Content != newContent) {
        viewHost.ViewType = null;
        viewHost.Content = newContent;
      }
    }
  }
}
