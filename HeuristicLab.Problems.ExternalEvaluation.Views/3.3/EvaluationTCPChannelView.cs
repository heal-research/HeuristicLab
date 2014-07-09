#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.ExternalEvaluation.Views {
  [View("TCP Channel View")]
  [Content(typeof(EvaluationTCPChannel), IsDefaultView = true)]
  public sealed partial class EvaluationTCPChannelView : NamedItemView {
    public new EvaluationTCPChannel Content {
      get { return (EvaluationTCPChannel)base.Content; }
      set { base.Content = value; }
    }

    public EvaluationTCPChannelView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.Connected -= new EventHandler(Content_Connected);
      Content.Disconnected -= new EventHandler(Content_Disconnected);
      Content.IpAddressChanged -= new EventHandler(Content_IpAddressChanged);
      Content.PortChanged -= new EventHandler(Content_PortChanged);
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Connected += new EventHandler(Content_Connected);
      Content.Disconnected += new EventHandler(Content_Disconnected);
      Content.IpAddressChanged += new EventHandler(Content_IpAddressChanged);
      Content.PortChanged += new EventHandler(Content_PortChanged);
    }

    #region Event Handlers (Content)
    private void Content_Connected(object sender, EventArgs e) {
      SetEnabledStateOfControls();
    }
    private void Content_Disconnected(object sender, EventArgs e) {
      SetEnabledStateOfControls();
    }
    private void Content_IpAddressChanged(object sender, EventArgs e) {
      ipAddressTextBox.Text = Content.IpAddress;
    }
    private void Content_PortChanged(object sender, EventArgs e) {
      portTextBox.Text = Content.Port.ToString();
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        ipAddressTextBox.Text = String.Empty;
        portTextBox.Text = String.Empty;
      } else {
        ipAddressTextBox.Text = Content.IpAddress;
        portTextBox.Text = Content.Port.ToString();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      bool readOnlyDriverNullOrStarted = ReadOnly || Content == null || Content.IsInitialized;
      ipAddressTextBox.Enabled = !readOnlyDriverNullOrStarted;
      portTextBox.Enabled = !readOnlyDriverNullOrStarted;
      connectButton.Enabled = !readOnlyDriverNullOrStarted;
      disconnectButton.Enabled = !ReadOnly && Content != null && Content.IsInitialized;
    }

    #region Event Handlers (child controls)
    private void ipAddressTextBox_Validating(object sender, CancelEventArgs e) {
      if (Content != null) {
        try {
          System.Net.IPAddress.Parse(ipAddressTextBox.Text);
        }
        catch (FormatException) {
          e.Cancel = true;
        }
        if (!e.Cancel) Content.IpAddress = ipAddressTextBox.Text;
      }
    }
    private void portTextBox_Validating(object sender, CancelEventArgs e) {
      if (Content != null) {
        int port;
        e.Cancel = !int.TryParse(portTextBox.Text, out port);
        if (!e.Cancel) Content.Port = port;
      }
    }
    private void connectButton_Click(object sender, EventArgs e) {
      try {
        Content.Open();
      }
      catch (Exception ex) {
        PluginInfrastructure.ErrorHandling.ShowErrorDialog(ex);
      }
    }
    private void disconnectButton_Click(object sender, EventArgs e) {
      try {
        Content.Close();
      }
      catch (Exception ex) {
        PluginInfrastructure.ErrorHandling.ShowErrorDialog(ex);
      }
    }
    #endregion
  }
}
