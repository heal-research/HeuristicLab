using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Persistence.GUI {

  [Plugin("HeuristicLab.Persistence.GUI", "3.3.16.0")]
  [PluginFile("HeuristicLab.Persistence.GUI-3.3.dll", PluginFileType.Assembly)]
  [PluginDependency("HeuristicLab.Attic", "1.0")]
  [PluginDependency("HeuristicLab.Common.Resources", "3.3")]
  [PluginDependency("HeuristicLab.Persistence", "3.3")]
  public class HeuristicLabPersistenceGUIPlugin : PluginBase { }

}
