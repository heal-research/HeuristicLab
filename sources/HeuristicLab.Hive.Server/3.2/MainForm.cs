using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Hive.Server.Properties;

namespace HeuristicLab.Hive.Server
{
    public partial class MainForm : Form {
        public MainForm(Dictionary<string, Uri> addresses) {
          InitializeComponent();
          this.Visible = false;

          Uri uri;
          StringBuilder servicesTxt = new StringBuilder();
          addresses.TryGetValue(HiveServerApplication.STR_ClientCommunicator, out uri);
          if (uri != null)
            servicesTxt.AppendLine(String.Format("Server Client: {0}", uri));
          addresses.TryGetValue(HiveServerApplication.STR_ServerConsoleFacade, out uri);
          if (uri != null)
            servicesTxt.AppendLine(String.Format("Server Console: {0}", uri));
          addresses.TryGetValue(HiveServerApplication.STR_ExecutionEngineFacade, out uri);
          if (uri != null)
            servicesTxt.AppendLine(String.Format("Execution Engine: {0}", uri));

          rtfServices.AppendText(servicesTxt.ToString());

          ni.Icon = Resources.HeuristicLab;
          ni.BalloonTipTitle = "HL Hive Server Services";
          ni.BalloonTipText = servicesTxt.ToString();
          ni.BalloonTipIcon = ToolTipIcon.Info;
          ni.Text = "HL Hive Server Services";
          ni.ShowBalloonTip(10000);
        }

        private void CloseApp(object sender, EventArgs e) {
          Dispose();
        }

        private void ShowInfo(object sender, EventArgs e) {
          this.Visible = true;
        }

        private void btnClose_Click(object sender, EventArgs e) {
          this.Visible = false;
        }

    }
}
