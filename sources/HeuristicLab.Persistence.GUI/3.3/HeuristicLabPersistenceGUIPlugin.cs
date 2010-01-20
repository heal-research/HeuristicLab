using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Persistence.GUI {

  [Plugin("HeuristicLab.Persistence.GUI-3.3")]
  [PluginFile("HeuristicLab.Persistence.GUI-3.3.dll", PluginFileType.Assembly)]
  [PluginDependency("HeuristicLab.Persistence-3.3")]
  public class HeuristicLabPersistencePlugin : PluginBase { }


  [Application("Persistence Configuration")]
  public class HeuristicLabPersistenceApplication : ApplicationBase {
    public override void Run() {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new PersistenceConfigurationForm());
    }
  }

}