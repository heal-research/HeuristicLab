using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.SharpDevelop {

  [Plugin("HeuristicLab.SharpDevelop-3.1.1.5327")]
  [PluginFile("ICSharpCode.TextEditor.dll", PluginFileType.Assembly)]
  [PluginFile("ICSharpCode.SharpDevelop.Dom.dll", PluginFileType.Assembly)]
  [PluginFile("ICSharpCode.NRefactory.dll", PluginFileType.Assembly)]
  [PluginFile("Mono.Cecil.dll", PluginFileType.Assembly)]
  [PluginFile("Mono License.txt", PluginFileType.License)]
  [PluginFile("SharpDevelop License.txt", PluginFileType.License)]
  [PluginDependency("HeuristicLab.log4net","1.2.10.0")]
  public class HeuristicLabSharpDevelopPlugin : PluginBase {
  }
}
