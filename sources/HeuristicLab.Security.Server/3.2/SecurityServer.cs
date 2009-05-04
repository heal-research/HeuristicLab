using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.Security.Server {
  public partial class SecurityServer : Form {
    public SecurityServer(Dictionary<string, String> baseAddrDict) {
      InitializeComponent();
      String uri;
      baseAddrDict.TryGetValue(SecurityServerApplication.STR_PermissionManager, out uri);
      if (uri != null)
        this.labelPermission.Text = uri.ToString();
      baseAddrDict.TryGetValue(SecurityServerApplication.STR_SecurityManager, out uri);
      if (uri != null)
        this.labelSecurity.Text = uri.ToString();
    }
  }
}
