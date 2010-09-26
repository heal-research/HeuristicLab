#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.SharpDevelop {
  [Plugin("HeuristicLab.SharpDevelop", "3.1.1.5327")]
  [PluginFile("ICSharpCode.TextEditor.dll", PluginFileType.Assembly)]
  [PluginFile("ICSharpCode.SharpDevelop.Dom.dll", PluginFileType.Assembly)]
  [PluginFile("ICSharpCode.NRefactory.dll", PluginFileType.Assembly)]
  [PluginFile("Mono.Cecil.dll", PluginFileType.Assembly)]
  [PluginFile("Mono License.txt", PluginFileType.License)]
  [PluginFile("SharpDevelop License.txt", PluginFileType.License)]
  [PluginDependency("HeuristicLab.log4net", "1.2.10.0")]
  public class HeuristicLabSharpDevelopPlugin : PluginBase {
  }
}
