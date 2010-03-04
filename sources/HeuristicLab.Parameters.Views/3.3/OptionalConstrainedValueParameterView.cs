#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Parameters.Views {
  /// <summary>
  /// The visual representation of a <see cref="Parameter"/>.
  /// </summary>
  [View("OptionalConstrainedValueParameter View")]
  [Content(typeof(OptionalConstrainedValueParameter<>), true)]
  public partial class OptionalConstrainedValueParameterView<T> : ParameterView where T : class, IItem {
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
    public OptionalConstrainedValueParameterView() {
      InitializeComponent();
      Caption = "OptionalConstrainedValueParameter";
      valueComboBoxItems = new List<T>();
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with the given <paramref name="variable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="VariableView()"/>.</remarks>
    /// <param name="variable">The variable to represent visually.</param>
    public OptionalConstrainedValueParameterView(OptionalConstrainedValueParameter<T> content)
      : this() {
      Content = content;
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="IVariable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterContentEvents() {
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
      Content.ValidValues.ItemsAdded += new CollectionItemsChangedEventHandler<T>(ValidValues_ItemsAdded);
      Content.ValidValues.ItemsRemoved += new CollectionItemsChangedEventHandler<T>(ValidValues_ItemsRemoved);
      Content.ValidValues.CollectionReset += new CollectionItemsChangedEventHandler<T>(ValidValues_CollectionReset);
      Content.ValueChanged += new EventHandler(Content_ValueChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        Caption = "OptionalConstrainedValueParameter";
        viewHost.Content = null;
        valueGroupBox.Enabled = false;
        FillValueComboBox();
      } else {
        Caption = Content.Name + " (" + Content.GetType().Name + ")";
        valueGroupBox.Enabled = true;
        FillValueComboBox();
        viewHost.Content = Content.Value;
      }
    }

    private void FillValueComboBox() {
      valueComboBox.SelectedIndexChanged -= new EventHandler(valueComboBox_SelectedIndexChanged);
      valueComboBoxItems.Clear();
      valueComboBoxItems.Add(null);
      valueComboBox.Items.Clear();
      valueComboBox.Items.Add("-");
      if (Content != null) {
        foreach (T item in Content.ValidValues) {
          valueComboBoxItems.Add(item);
          valueComboBox.Items.Add(item.ToString());
        }
        valueComboBox.SelectedIndex = valueComboBoxItems.IndexOf(Content.Value);
      }
      valueComboBox.SelectedIndexChanged += new EventHandler(valueComboBox_SelectedIndexChanged);
    }

    #region Content Events
    private void Content_ValueChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ValueChanged), sender, e);
      else {
        valueComboBox.SelectedIndex = valueComboBoxItems.IndexOf(Content.Value);
        viewHost.Content = Content.Value;
      }
    }
    private void ValidValues_ItemsAdded(object sender, CollectionItemsChangedEventArgs<T> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<T>(ValidValues_ItemsAdded), sender, e);
      else
        FillValueComboBox();
    }
    private void ValidValues_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<T> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<T>(ValidValues_ItemsRemoved), sender, e);
      else
        FillValueComboBox();
    }
    private void ValidValues_CollectionReset(object sender, CollectionItemsChangedEventArgs<T> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<T>(ValidValues_CollectionReset), sender, e);
      else
        FillValueComboBox();
    }
    #endregion

    private void valueComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      Content.Value = valueComboBoxItems[valueComboBox.SelectedIndex];
    }
  }
}
