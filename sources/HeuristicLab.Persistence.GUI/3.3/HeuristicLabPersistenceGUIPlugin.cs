using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Persistence.GUI {

  [ClassInfo(Name = "HeuristicLab.Persistence.GUI-3.3")]
  [PluginFile(Filename = "HeuristicLab.Persistence.GUI-3.3.dll", Filetype = PluginFileType.Assembly)]
  [Dependency(Dependency = "HeuristicLab.Persistence-3.3")]
  public class HeuristicLabPersistencePlugin : PluginBase { }


  [ClassInfo(Name = "Persistence Configuration")]
  public class HeuristicLabPersistenceApplication : ApplicationBase {
    public override void Run() {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new PersistenceConfigurationForm());
    }
  }

}