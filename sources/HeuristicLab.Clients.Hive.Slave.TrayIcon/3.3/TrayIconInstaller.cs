using System.Collections;
using System.ComponentModel;
using System.Diagnostics;


namespace HeuristicLab.Clients.Hive.SlaveCore.SlaveTrayIcon {
  [RunInstaller(true)]
  public partial class TrayIconInstaller : System.Configuration.Install.Installer {

    public TrayIconInstaller() {
      InitializeComponent();
    }

    public override void Commit(IDictionary savedState) {
      base.Commit(savedState);
      //TODO: disable on quiet install (for admins)?
      Process.Start(Context.Parameters["TARGETDIR"].ToString() + "HeuristicLab.Clients.Hive.Slave.SlaveTrayIcon.exe");
    }
  }
}
