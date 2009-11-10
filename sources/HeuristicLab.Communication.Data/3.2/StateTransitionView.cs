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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Communication.Data {
  public partial class StateTransitionView : ViewBase {

    public StateTransition StateTransition {
      get { return (StateTransition)Item; }
      set { base.Item = value; }
    }

    public StateTransitionView() {
      InitializeComponent();
    }

    public StateTransitionView(StateTransition stateTransition)
      : this() {
      StateTransition = stateTransition;
    }

    protected override void RemoveItemEvents() {
      StateTransition.Changed -= new EventHandler(StateTransition_Changed);
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      StateTransition.Changed += new EventHandler(StateTransition_Changed);
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (StateTransition == null) {
        transitionFunctionProgrammableOperatorView.Enabled = false;
        targetStateComboBox.Enabled = false;
        targetStateComboBox.DataSource = null;
        targetStateComboBox.Items.Clear();
      } else {
        targetStateComboBox.Enabled = true;
        transitionFunctionProgrammableOperatorView.ProgrammableOperator = StateTransition.TransitionCondition;
        transitionFunctionProgrammableOperatorView.Enabled = true;
        BuildTargetStateComboBox();
        BuildVariablesListBox();
      }
    }

    void StateTransition_Changed(object sender, EventArgs e) {
      Refresh();
    }

    void VariablesList_Changed(object sender, EventArgs e) {
      BuildVariablesListBox();
    }

    #region Events to synchronize changes in request/response variables to state transition
    void Parameter_Changed(object sender, EventArgs e) {
      BuildVariablesListBox();
    }
    void Parameter_Added(object sender, EventArgs<IItem, int> e) {
      e.Value.Changed += new EventHandler(Parameter_Changed);
    }
    void Parameter_Removed(object sender, EventArgs<IItem, int> e) {
      e.Value.Changed -= new EventHandler(Parameter_Changed);
    }
    #endregion

    private void BuildTargetStateComboBox() {
      ComboBox comboBox = targetStateComboBox;
      ItemList<ProtocolState> states = null;
      /*if (StateTransition.SourceState == null || StateTransition.SourceState.Protocol == null)
        states = null;
      else
        states = StateTransition.SourceState.Protocol.States;*/
      ProtocolState selected = StateTransition.TargetState;
      ProtocolState forbidden = StateTransition.SourceState;

      if (states != null && states.Count > 0) {
        IList<ProtocolState> sourceList = new List<ProtocolState>(states.Count);
        int selectedIndex = -1, counter = -1;
        for (int i = 0 ; i < states.Count ; i++) {
          if (forbidden == null || !states[i].Guid.Equals(forbidden.Guid)) {
            sourceList.Add((ProtocolState)states[i]);
            counter++;
          }
          if (selected != null && states[i].Guid.Equals(selected.Guid))
            selectedIndex = counter;
        }
        comboBox.SelectedIndexChanged -= new EventHandler(targetStateComboBox_SelectedIndexChanged);
        BindingList<ProtocolState> bl = new BindingList<ProtocolState>(sourceList);
        comboBox.DataSource = bl;
        comboBox.DisplayMember = "Name";
        comboBox.ValueMember = "Guid";
        comboBox.SelectedIndex = selectedIndex;
        comboBox.SelectedIndexChanged += new EventHandler(targetStateComboBox_SelectedIndexChanged);
      }
    }

    private void BuildVariablesListBox() {
      ListBox variables = variablesListBox;
      /*ConstrainedItemList request = StateTransition.SourceState.SendingData;
      ConstrainedItemList response = StateTransition.SourceState.ReceivingData;*/
      variables.Items.Clear();
      variables.Items.Add("==Sending==");
      /*for (int i = 0 ; i < request.Count ; i++) {
        variables.Items.Add(request[i]);
      }*/
      variables.Items.Add("==Receiving==");
      /*for (int i = 0 ; i < response.Count ; i++) {
        variables.Items.Add(response[i]);
      }*/
    }

    private void targetStateComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (targetStateComboBox.SelectedIndex >= 0) {
        StateTransition.TargetState = (ProtocolState)targetStateComboBox.SelectedItem;
      }
    }
  }
}
