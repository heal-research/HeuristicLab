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
    }
    public ProtocolEditor(Protocol protocol)
      : this() {
      Protocol = protocol;
    }

    protected override void RemoveItemEvents() {
      Protocol.Changed -= new EventHandler(Protocol_Changed);
      Protocol.StatesChanged -= new EventHandler(States_Changed);
      Protocol.Name.Changed -= new EventHandler(ProtocolName_Changed);
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      Protocol.Changed += new EventHandler(Protocol_Changed);
      Protocol.StatesChanged += new EventHandler(States_Changed);
      Protocol.Name.Changed += new EventHandler(ProtocolName_Changed);
    }

    private void BuildInitialStateComboBox() {
      // Rebuild the two ComboBoxes depicting the initial and final state
      IList<ProtocolState> states = new List<ProtocolState>(Protocol.States.Count);
      int initialSelectedIndex = -1;
      for (int i = 0 ; i < Protocol.States.Count ; i++) {
        states.Add((ProtocolState)Protocol.States[i]);
        if (Protocol.States[i].Guid.Equals(Protocol.InitialState.Guid))
          initialSelectedIndex = i;
      }
      initialStateComboBox.SelectedIndexChanged -= new EventHandler(initialStateComboBox_SelectedIndexChanged);
      BindingList<ProtocolState> bl = new BindingList<ProtocolState>(states);
      initialStateComboBox.DataSource = bl;
      initialStateComboBox.DisplayMember = "Name";
      initialStateComboBox.ValueMember = "Guid";
      initialStateComboBox.SelectedIndex = initialSelectedIndex;
      initialStateComboBox.SelectedIndexChanged += new EventHandler(initialStateComboBox_SelectedIndexChanged);
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (Protocol == null) {
        Caption = "Protocol";
        nameViewControl.Enabled = false;
        nameViewControl.StringData = null;
        statesItemListView.Enabled = false;
        statesItemListView.ItemList = null;
        initialStateComboBox.Enabled = false;
        initialStateComboBox.DataSource = null;
        initialStateComboBox.Items.Clear();
      } else {
        Caption = Protocol.Name.Data;
        nameViewControl.StringData = Protocol.Name;
        nameViewControl.Enabled = true;
        statesItemListView.ItemList = Protocol.States;
        statesItemListView.Enabled = true;
        BuildInitialStateComboBox();
        initialStateComboBox.Enabled = true;
      }
    }

    #region Custom events
    void Protocol_Changed(object sender, EventArgs e) {
      Refresh();
    }

    void States_Changed(object sender, EventArgs e) {
      Refresh();
    }

    void ProtocolName_Changed(object sender, EventArgs e) {
      Caption = Protocol.Name.Data;
    }
    #endregion

    #region ComboBox events
    private void initialStateComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (initialStateComboBox.SelectedIndex >= 0)
        Protocol.InitialState = (ProtocolState)initialStateComboBox.SelectedItem;
    }
    #endregion

    private void invertButton_Click(object sender, EventArgs e) {
      for (int i = 0 ; i < Protocol.States.Count ; i++) {
        ConstrainedItemList tmp = ((ProtocolState)Protocol.States[i]).SendingData;
        ((ProtocolState)Protocol.States[i]).SendingData = ((ProtocolState)Protocol.States[i]).ReceivingData;
        ((ProtocolState)Protocol.States[i]).ReceivingData = tmp;
      }
      Refresh();
    }
  }
}