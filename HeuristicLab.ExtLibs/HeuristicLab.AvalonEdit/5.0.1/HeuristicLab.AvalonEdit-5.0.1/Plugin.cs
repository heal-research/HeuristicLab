﻿#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.AvalonEdit {
  [Plugin("HeuristicLab.AvalonEdit", "HeuristicLab wrapper for AvalonEdit", "5.0.1.0")]
  [PluginFile("HeuristicLab.AvalonEdit-5.0.1.dll", PluginFileType.Assembly)]
  [PluginFile("ICSharpCode.AvalonEdit.dll", PluginFileType.Assembly)]
  [PluginFile("ICSharpCode.AvalonEdit License-5.0.1.txt", PluginFileType.License)]
  [PluginDependency("HeuristicLab.NRefactory", "5.5")]
  public class HeuristicLabAvalonEditPlugin : PluginBase {
  }
}
