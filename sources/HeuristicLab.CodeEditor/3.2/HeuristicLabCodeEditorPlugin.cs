using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.CodeEditor {

  [Plugin("HeuristicLab.CodeEditor-3.2")]
  [PluginFile("HeuristicLab.CodeEditor-3.2.dll", PluginFileType.Assembly)]
  [PluginDependency("HeuristicLab.Common.Resources","3.2.0.0")]
  [PluginDependency("HeuristicLab.SharpDevelop-3.1.1.5327")]
  public class HeuristicLabCodeEditorPlugin : PluginBase {
  }
}
