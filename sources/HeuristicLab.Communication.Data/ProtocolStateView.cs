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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Communication.Data {
  public partial class ProtocolStateView : ViewBase {
    public ProtocolState ProtocolState {
      get { return (ProtocolState)Item; }
      set { base.Item = value; }
    }

    public ProtocolStateView() {
      InitializeComponent();
    }
    public ProtocolStateView(ProtocolState protocolState)
      : this() {
      ProtocolState = protocolState;
    }

    protected override void RemoveItemEvents() {
      nameTextBox.DataBindings.Clear();
      giveBatchCheckBox.DataBindings.Clear();
      expectBatchCheckBox.DataBindings.Clear();
      ProtocolState.Changed -= new EventHandler(ProtocolState_Changed);
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      ProtocolState.Changed += new EventHandler(ProtocolState_Changed);
      expectBatchCheckBox.DataBindings.Add("Checked", ProtocolState, "ExpectBatch");
      giveBatchCheckBox.DataBindings.Add("Checked", ProtocolState, "GiveBatch");
      nameTextBox.DataBindings.Add("Text", ProtocolState, "Name");
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (ProtocolState == null) {
        nameTextBox.Enabled = false;
        communicationDataTabControl.Enabled = false;
        giveItemListView.ItemList = null;
        expectItemListView.ItemList = null;
      } else {
        nameTextBox.Text = ProtocolState.Name;
        giveBatchCheckBox.Checked = ProtocolState.GiveBatch;
        expectBatchCheckBox.Checked = ProtocolState.ExpectBatch;

        giveItemListView.ItemList = ProtocolState.Give;
        expectItemListView.ItemList = ProtocolState.Expect;

        communicationDataTabControl.Enabled = true;
        nameTextBox.Enabled = true;
      }
    }

    #region ProtocolState change handling (main underlying object)
    void ProtocolState_Changed(object sender, EventArgs e) {
      Refresh();
    }
    #endregion

  }
}
