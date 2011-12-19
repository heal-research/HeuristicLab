using System;
using System.Collections;
using System.ComponentModel;
using System.ServiceProcess;


namespace HeuristicLab.Clients.Hive.SlaveCore.WindowsService {
  [RunInstaller(true)]
  public partial class ProjectInstaller : System.Configuration.Install.Installer {
    public ProjectInstaller() {
      InitializeComponent();
    }

    protected override void OnBeforeUninstall(IDictionary savedState) {
      base.OnBeforeUninstall(savedState);

      //try to shutdown the service before uninstalling it
      using (var serviceController = new ServiceController(this.serviceInstaller1.ServiceName, Environment.MachineName)) {
        try {
          serviceController.Stop();
        }
        catch { }
      }
    }

    protected override void OnAfterInstall(IDictionary savedState) {
      base.OnAfterInstall(savedState);

      //try to create an event log      
      try {
        if (!System.Diagnostics.EventLog.SourceExists("HLHive")) {
          System.Diagnostics.EventLog.CreateEventSource("HLHive", "HiveSlave");
        }
      }
      catch { }

      //try to start the service after installation
      using (var serviceController = new ServiceController(this.serviceInstaller1.ServiceName, Environment.MachineName)) {
        try {
          serviceController.Start();
        }
        catch { }
      }
    }
  }
}
