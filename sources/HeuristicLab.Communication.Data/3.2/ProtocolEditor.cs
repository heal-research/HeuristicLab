#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Communication.Data {
  public partial class ProtocolEditor : EditorBase {
    public Protocol Protocol {
      get { return (Protocol)Item; }
      set { base.Item = value; }
    }

    public ProtocolEditor() {
      InitializeComponent();
      statesListBox.DrawMode = DrawMode.OwnerDrawFixed;
      statesListBox.DrawItem += new DrawItemEventHandler(statesListBox_DrawItem);
    }

    void statesListBox_DrawItem(object sender, DrawItemEventArgs e) {
      if (e.Index >= 0) { // during Items.Clear() this method is called with index -1
        ListBox lb = (ListBox)sender;
        ProtocolState state = (ProtocolState)lb.Items[e.Index];
        e.DrawBackground();
        e.DrawFocusRectangle();
        SolidBrush textColor = new SolidBrush(Color.Black);
        if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) textColor.Color = Color.White;
        if (Protocol.InitialState.Equals(state))
          e.Graphics.DrawString(state.Name, new Font(e.Font.FontFamily, e.Font.Size, FontStyle.Bold), textColor, e.Bounds);
        else
          e.Graphics.DrawString(state.Name, e.Font, textColor, e.Bounds);
      }
    }

    public ProtocolEditor(Protocol protocol)
      : this() {
      Protocol = protocol;
    }

    protected override void RemoveItemEvents() {
      nameTextBox.DataBindings.Clear();
      Protocol.Changed -= new EventHandler(Protocol_Changed);
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      Protocol.Changed += new EventHandler(Protocol_Changed);
      nameTextBox.DataBindings.Add("Text", Protocol, "Name");
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (Protocol == null) {
        Caption = "Protocol";
        nameTextBox.Enabled = false;
        setAsInitialStateButton.Enabled = false;
        addStateButton.Enabled = false;
        removeStateButton.Enabled = false;
        statesListBox.Enabled = false;

        nameTextBox.Text = "";
        statesListBox.Items.Clear();
      } else {
        Caption = Protocol.Name;

        statesListBox.Items.Clear();
        foreach (ProtocolState state in Protocol.States)
          statesListBox.Items.Add(state);

        statesListBox.Enabled = true;
        addStateButton.Enabled = true;
        removeStateButton.Enabled = true;
        setAsInitialStateButton.Enabled = true;
        nameTextBox.Enabled = true;
      }
    }

    void Protocol_Changed(object sender, EventArgs e) {
      Refresh();
    }

    private void addStateButton_Click(object sender, EventArgs e) {
      ProtocolState tmp = new ProtocolState();
      int index = statesListBox.SelectedIndex;
      if (index < 0) {
        Protocol.States.Add(tmp);
      } else {
        Protocol.States.Insert(index, tmp);
      }
      Refresh();
      statesListBox.SelectedIndex = index;
    }

    private void removeStateButton_Click(object sender, EventArgs e) {
      if (statesListBox.SelectedIndex >= 0) {
        int index = statesListBox.SelectedIndex;
        Protocol.States.RemoveAt(statesListBox.SelectedIndex);
        Refresh();
        if (Protocol.States.Count > 0)
          statesListBox.SelectedIndex = ((index < Protocol.States.Count) ? (index) : (Protocol.States.Count - 1));
      }
    }

    private void setAsInitialStateButton_Click(object sender, EventArgs e) {
      if (statesListBox.SelectedIndex >= 0) {
        Protocol.InitialState = (ProtocolState)statesListBox.SelectedItem;
        statesListBox.Refresh();
      }
    }

    private void statesListBox_DoubleClick(object sender, EventArgs e) {
      if (lastDeselectedIndex >= 0 || lastSelectedIndex >= 0) {
        statesListBox.SelectedIndex = (lastDeselectedIndex >= 0) ? (lastDeselectedIndex) : (lastSelectedIndex);
        ProtocolState selected = (ProtocolState)statesListBox.Items[(lastDeselectedIndex >= 0) ? (lastDeselectedIndex) : (lastSelectedIndex)];
        bool editingInitial = (Protocol.InitialState == selected);
        ProtocolState selectedClone = (ProtocolState)selected.Clone(new Dictionary<Guid, object>());
        IView stateView = selectedClone.CreateView();
        using (WindowedView display = new WindowedView(stateView as UserControl)) {
          display.ShowDialog(this);
          if (display.DialogResult == DialogResult.OK) {
            Protocol.States[(lastDeselectedIndex >= 0) ? (lastDeselectedIndex) : (lastSelectedIndex)] = selectedClone;
            if (editingInitial) Protocol.InitialState = selectedClone;
            Refresh();
          }
        }
      }
    }

    private int lastSelectedIndex = -1;
    private int lastDeselectedIndex = -1;

    private void statesListBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (statesListBox.SelectedIndex >= 0) {
        if (lastSelectedIndex == statesListBox.SelectedIndex) {
          lastDeselectedIndex = statesListBox.SelectedIndex;
          statesListBox.SelectedIndex = -1;
          lastSelectedIndex = -1;
        } else {
          lastSelectedIndex = statesListBox.SelectedIndex;
          lastDeselectedIndex = -1;
        }
      }
    }
  }
}