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

using System;
using System.Collections.Generic;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Parameters.Views {
  /// <summary>
  /// The visual representation of a <see cref="Parameter"/>.
  /// </summary>
  [View("ConstrainedValueParameter View")]
  [Content(typeof(OptionalConstrainedValueParameter<>), true)]
  [Content(typeof(ConstrainedValueParameter<>), true)]
  public partial class ConstrainedValueParameterView<T> : ParameterView where T : class, IItem {
    private List<T> valueComboBoxItems;

    /// <summary>
    /// Gets or sets the variable to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public new OptionalConstrainedValueParameter<T> Content {
      get { return (OptionalConstrainedValueParameter<T>)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with caption "Variable".
    /// </summary>
    public ConstrainedValueParameterView() {
      InitializeComponent();
      valueComboBoxItems = new List<T>();
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="IVariable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterContentEvents() {
      Content.GetsCollectedChanged -= new EventHandler(Content_GetsCollectedChanged);
      Content.ValidValues.ItemsAdded -= new CollectionItemsChangedEventHandler<T>(ValidValues_ItemsAdded);
      Content.ValidValues.ItemsRemoved -= new CollectionItemsChangedEventHandler<T>(ValidValues_ItemsRemoved);
      Content.ValidValues.CollectionReset -= new CollectionItemsChangedEventHandler<T>(ValidValues_CollectionReset);
      Content.ValueChanged -= new EventHandler(Content_ValueChanged);
      base.DeregisterContentEvents();
    }

    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="IVariable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.GetsCollectedChanged += new EventHandler(Content_GetsCollectedChanged);
      Content.ValidValues.ItemsAdded += new CollectionItemsChangedEventHandler<T>(ValidValues_ItemsAdded);
      Content.ValidValues.ItemsRemoved += new CollectionItemsChangedEventHandler<T>(ValidValues_ItemsRemoved);
      Content.ValidValues.CollectionReset += new CollectionItemsChangedEventHandler<T>(ValidValues_CollectionReset);
      Content.ValueChanged += new EventHandler(Content_ValueChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        showInRunCheckBox.Checked = false;
        viewHost.Content = null;
        FillValueComboBox();
      } else {
        SetDataTypeTextBoxText();
        FillValueComboBox();
        showInRunCheckBox.Checked = Content.GetsCollected;
        viewHost.ViewType = null;
        viewHost.Content = Content.Value;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      valueGroupBox.Enabled = Content != null;
      valueComboBox.Enabled = (valueComboBox.Items.Count > 0) && !ReadOnly;
      showInRunCheckBox.Enabled = Content != null && !ReadOnly;
    }

    protected virtual void FillValueComboBox() {
      valueComboBox.SelectedIndexChanged -= new EventHandler(valueComboBox_SelectedIndexChanged);
      valueComboBoxItems.Clear();
      valueComboBox.Items.Clear();
      if (!(Content is ConstrainedValueParameter<T>)) {
        valueComboBoxItems.Add(null);
        valueComboBox.Items.Add("-");
      }
      if (Content != null) {
        foreach (T item in Content.ValidValues) {
          valueComboBoxItems.Add(item);
          valueComboBox.Items.Add(item.ToString());
        }
        valueComboBox.Enabled = (valueComboBox.Items.Count > 0) && !ReadOnly;
        valueComboBox.SelectedIndex = valueComboBoxItems.IndexOf(Content.Value);
      }
      valueComboBox.SelectedIndexChanged += new EventHandler(valueComboBox_SelectedIndexChanged);
    }

    #region Content Events
    protected virtual void Content_ValueChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ValueChanged), sender, e);
      else {
        SetDataTypeTextBoxText();
        SetComboBoxSelectedIndex();
        viewHost.ViewType = null;
        viewHost.Content = Content != null ? Content.Value : null;
      }
    }

    protected virtual void ValidValues_ItemsAdded(object sender, CollectionItemsChangedEventArgs<T> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<T>(ValidValues_ItemsAdded), sender, e);
      else
        FillValueComboBox();
    }
    protected virtual void ValidValues_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<T> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<T>(ValidValues_ItemsRemoved), sender, e);
      else
        FillValueComboBox();
    }
    protected virtual void ValidValues_CollectionReset(object sender, CollectionItemsChangedEventArgs<T> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<T>(ValidValues_CollectionReset), sender, e);
      else
        FillValueComboBox();
    }
    protected virtual void Content_GetsCollectedChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_GetsCollectedChanged), sender, e);
      else
        showInRunCheckBox.Checked = Content != null && Content.GetsCollected;
    }
    #endregion

    protected virtual void valueComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (valueComboBox.SelectedIndex >= 0)
        Content.Value = valueComboBoxItems[valueComboBox.SelectedIndex];
    }
    protected virtual void showInRunCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (Content != null) Content.GetsCollected = showInRunCheckBox.Checked;
    }

    #region Helpers
    protected void SetDataTypeTextBoxText() {
      if (Content == null) {
        dataTypeTextBox.Text = "-";
      } else {
        if ((Content.Value != null) && (Content.Value.GetType() != Content.DataType))
          dataTypeTextBox.Text = Content.DataType.GetPrettyName() + " (" + Content.Value.GetType().GetPrettyName() + ")";
        else
          dataTypeTextBox.Text = Content.DataType.GetPrettyName();
      }
    }
    protected void SetComboBoxSelectedIndex() {
      valueComboBox.SelectedIndex = valueComboBoxItems.IndexOf(Content != null ? Content.Value : null);
    }
    #endregion
  }
}
