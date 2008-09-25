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
      ProtocolState.Changed -= new EventHandler(ProtocolState_Changed);
      ProtocolState.Protocol.StatesChanged -= new EventHandler(States_Changed);
      if (ProtocolState.StateTransitions != null) {
        for (int i = 0 ; i < ProtocolState.StateTransitions.Count ; i++) {
          ProtocolState.StateTransitions[i].Changed -= new EventHandler(StateTransitions_Changed);
        }
      }
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      ProtocolState.Changed += new EventHandler(ProtocolState_Changed);
      ProtocolState.Protocol.StatesChanged += new EventHandler(States_Changed);
      if (ProtocolState.StateTransitions != null) {
        for (int i = 0 ; i < ProtocolState.StateTransitions.Count ; i++) {
          ProtocolState.StateTransitions[i].Changed += new EventHandler(StateTransitions_Changed);
        }
      }
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (ProtocolState == null) {
        nameStringDataView.Enabled = false;
        nameStringDataView.StringData = null;
        acceptingStateBoolDataView.Enabled = false;
        acceptingStateBoolDataView.BoolData = null;
        outboundCommunicationDataView.Enabled = false;
        outboundCommunicationDataView.ConstrainedItemList = null;
        inboundCommunicationDataView.Enabled = false;
        inboundCommunicationDataView.ConstrainedItemList = null;
        stateTransitionTabControl.Controls.Clear();
      } else {
        nameStringDataView.StringData = ProtocolState.Name;
        nameStringDataView.Enabled = true;
        acceptingStateBoolDataView.Enabled = true;
        acceptingStateBoolDataView.BoolData = ProtocolState.AcceptingState;
        outboundCommunicationDataView.ConstrainedItemList = ProtocolState.SendingData;
        outboundCommunicationDataView.Enabled = true;
        inboundCommunicationDataView.ConstrainedItemList = ProtocolState.ReceivingData;
        inboundCommunicationDataView.Enabled = true;

        addTransitionButton.Enabled = (ProtocolState.Protocol.States.Count > 1);
        removeTransitionButton.Enabled = (ProtocolState.StateTransitions != null && ProtocolState.StateTransitions.Count > 0);

        stateTransitionTabControl.Controls.Clear();
        ItemList<StateTransition> stateTransitions = ProtocolState.StateTransitions;
        if (stateTransitions != null) {
          for (int i = 0 ; i < stateTransitions.Count ; i++) {
            StateTransition stateTransition = (StateTransition)stateTransitions[i];
            TabPage newTransitionTabPage = new TabPage((stateTransition.TargetState == null) ? ("no target") : (stateTransition.TargetState.Name.ToString()));
            Control stateTransitionView = (Control)stateTransition.CreateView();
            stateTransitionTabControl.Controls.Add(newTransitionTabPage);

            newTransitionTabPage.Controls.Add(stateTransitionView);
            newTransitionTabPage.Location = new Point(4, 22);
            newTransitionTabPage.Padding = new Padding(3);
            newTransitionTabPage.Size = stateTransitionTabControl.ClientSize;
            newTransitionTabPage.UseVisualStyleBackColor = true;

            stateTransitionView.Location = new Point(0, 0);
            stateTransitionView.Size = newTransitionTabPage.ClientSize;
            stateTransitionView.Dock = DockStyle.Fill;
            stateTransitionView.Anchor = (AnchorStyles)(AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
          }
        }
      }
    }

    #region ProtocolState change handling (main underlying object)
    void ProtocolState_Changed(object sender, EventArgs e) {
      Refresh();
    }
    #endregion

    #region Protocol.States change handling (parent object)
    void States_Changed(object sender, EventArgs e) {
      for (int i = 0 ; i < stateTransitionTabControl.TabCount ; i++)
        stateTransitionTabControl.TabPages[i].Controls[0].Refresh();
      addTransitionButton.Enabled = (ProtocolState.Protocol.States.Count > 1);
    }
    #endregion

    #region StateTransitions change handling (child object)
    void StateTransitions_Changed(object sender, EventArgs e) {
      for (int i = 0 ; i < stateTransitionTabControl.TabCount ; i++) {
        if (((StateTransition)ProtocolState.StateTransitions[i]).TargetState != null)
          stateTransitionTabControl.TabPages[i].Text = ((StateTransition)ProtocolState.StateTransitions[i]).TargetState.Name.ToString();
      }
    }
    #endregion

    #region Button events
    private void addTransitionButton_Click(object sender, EventArgs e) {
      //ProtocolState.AcceptingState = new BoolData(false);
      StateTransition stateTransition = new StateTransition();
      stateTransition.Changed += new EventHandler(StateTransitions_Changed);
      stateTransition.SourceState = ProtocolState;

      if (ProtocolState.StateTransitions == null) {
        ItemList<StateTransition> temp = new ItemList<StateTransition>();
        temp.Add(stateTransition);
        ProtocolState.StateTransitions = temp;
      } else {
        ProtocolState.StateTransitions.Add(stateTransition);
        Refresh();
      }
      removeTransitionButton.Enabled = true;
    }

    private void removeTransitionButton_Click(object sender, EventArgs e) {
      if (stateTransitionTabControl.TabCount > 0) {
        int selIdx = stateTransitionTabControl.SelectedIndex;
        ProtocolState.StateTransitions[selIdx].Changed -= new EventHandler(StateTransitions_Changed);
        stateTransitionTabControl.Controls.RemoveAt(selIdx);
        ProtocolState.StateTransitions.RemoveAt(selIdx);
        if (ProtocolState.StateTransitions.Count == 0) {
          //ProtocolState.AcceptingState = new BoolData(true);
          ProtocolState.StateTransitions = null;
          removeTransitionButton.Enabled = false;
        }
      }
    }
    #endregion
  }
}
