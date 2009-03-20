using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.Hive.Server
{
    public partial class MainForm : Form {
        public MainForm(Dictionary<string, Uri> addresses) {
          InitializeComponent();
          Uri uri;
          addresses.TryGetValue(HiveServerApplication.STR_ClientCommunicator, out uri);
          if(uri!=null)
            this.lblAddress1.Text = uri.ToString();
          addresses.TryGetValue(HiveServerApplication.STR_ServerConsoleFacade, out uri);
          if (uri != null)
            this.lblAddress2.Text = uri.ToString();
          addresses.TryGetValue(HiveServerApplication.STR_ExecutionEngineFacade, out uri);
          if (uri != null)
            this.lblAddress3.Text = uri.ToString();
        }

    }
}
