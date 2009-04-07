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
using System.Net.Sockets;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Communication.Data {
  public partial class TcpNetworkDriverConfigurationView : ViewBase {
    private TcpNetworkDriverConfiguration TcpNetworkDriverConfiguration {
      get { return (TcpNetworkDriverConfiguration)base.Item; }
      set { base.Item = value; }
    }

    public TcpNetworkDriverConfigurationView() {
      InitializeComponent();
    }

    public TcpNetworkDriverConfigurationView(TcpNetworkDriverConfiguration tcpNetworkDriverConfiguration)
      : this() {
      TcpNetworkDriverConfiguration = tcpNetworkDriverConfiguration;
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (TcpNetworkDriverConfiguration == null) {
        ipAddressStringDataView.Enabled = false;
        ipAddressStringDataView.StringData = null;
        targetPortIntDataView.Enabled = false;
        targetPortIntDataView.IntData = null;
        sourcePortIntDataView.Enabled = false;
        sourcePortIntDataView.IntData = null;
      } else {
        ipAddressStringDataView.StringData = TcpNetworkDriverConfiguration.TargetIPAddress;
        ipAddressStringDataView.Enabled = true;
        targetPortIntDataView.IntData = TcpNetworkDriverConfiguration.TargetPort;
        targetPortIntDataView.Enabled = true;
        sourcePortIntDataView.IntData = TcpNetworkDriverConfiguration.SourcePort;
        sourcePortIntDataView.Enabled = true;
      }
    }
  }
}
