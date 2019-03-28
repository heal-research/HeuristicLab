#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Data.Views {
  [View("EnumValue View")]
  [Content(typeof(EnumValue<>), true)]
  public partial class EnumValueView<T> : ItemView where T : struct, IComparable {

    public new EnumValue<T> Content {
      get { return (EnumValue<T>)base.Content; }
      set { base.Content = value; }
    }

    public override bool ReadOnly {
      get {
        if ((Content != null) && Content.ReadOnly) return true;
        return base.ReadOnly;
      }
      set { base.ReadOnly = value; }
    }

    static EnumValueView() {
      if (!typeof(T).IsEnum)
        throw new InvalidOperationException("Generic type " + typeof(T).Name + " is not an enum.");
    }

    public EnumValueView() {
      InitializeComponent();
      this.Caption = typeof(T).Name + " View";

      valueComboBox.DataSource = Enum.GetValues(typeof(T));
      foreach (T flag in Enum.GetValues(typeof(T)))
        flagsListView.Items.Add(new ListViewItem(flag.ToString()) { Tag = flag });
      columnHeader.Width = -1;

      bool isFlags = Attribute.IsDefined(typeof(T), typeof(FlagsAttribute));
      if (isFlags) valueLabel.Text = "Flags:";
      valueComboBox.Visible = !isFlags;
      flagsListView.Visible = isFlags;
    }
    public EnumValueView(EnumValue<T> content)
      : this() {
      Content = content;
    }

    protected override void DeregisterContentEvents() {
      Content.ValueChanged -= new EventHandler(Content_ValueChanged);
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ValueChanged += new EventHandler(Content_ValueChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        valueComboBox.SelectedIndex = -1;
        foreach (ListViewItem item in flagsListView.Items)
          item.Checked = false;
      } else {
        valueComboBox.SelectedItem = Content.Value;
        foreach (ListViewItem item in flagsListView.Items) {
          var flag = (Enum)item.Tag;
          item.Checked = ((Enum)(object)Content.Value).HasFlag(flag);
        }
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      if (Content == null) {
        valueComboBox.Enabled = false;
        flagsListView.Enabled = false;
      } else {
        valueComboBox.Enabled = !ReadOnly;
        flagsListView.Enabled = !ReadOnly;
      }
    }

    private void Content_ValueChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ValueChanged), sender, e);
      else {
        valueComboBox.SelectedItem = Content.Value;
        foreach (ListViewItem item in flagsListView.Items) {
          var flag = (Enum)item.Tag;
          item.Checked = ((Enum)(object)Content.Value).HasFlag(flag);
        }
      }
    }

    private void valueComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if ((Content != null) && !Content.ReadOnly)
        Content.Value = (T)valueComboBox.SelectedItem;
    }

    private void flagsListView_ItemChecked(object sender, ItemCheckedEventArgs e) {
      var flag = (T)e.Item.Tag;
      if ((Content != null) && !Content.ReadOnly)
        Content.Value = ((Enum)(object)Content.Value).SetFlag(flag, e.Item.Checked);
    }
  }

  internal static class EnumHelper {
    //https://stackoverflow.com/a/21581418
    public static T SetFlag<T>(this Enum value, T flag, bool set) {
      var baseType = Enum.GetUnderlyingType(value.GetType());
      dynamic valueAsBase = Convert.ChangeType(value, baseType);
      dynamic flagAsBase = Convert.ChangeType(flag, baseType);
      if (set)
        valueAsBase |= flagAsBase;
      else
        valueAsBase &= ~flagAsBase;
      return (T)valueAsBase;
    }
  }
}
