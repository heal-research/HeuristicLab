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
