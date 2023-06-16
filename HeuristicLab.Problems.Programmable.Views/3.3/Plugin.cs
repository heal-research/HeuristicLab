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

namespace HeuristicLab.Problems.Programmable.Views {
  [Plugin("HeuristicLab.Problems.Programmable.Views", "Views for programmable problem for defining custom representation and evaluation function.", "3.3.16.0")]
  [PluginFile("HeuristicLab.Problems.Programmable.Views-3.3.dll", PluginFileType.Assembly)]
  [PluginDependency("HeuristicLab.Common", "3.3")]
  [PluginDependency("HeuristicLab.CodeEditor", "3.4")]
  [PluginDependency("HeuristicLab.Core", "3.3")]
  [PluginDependency("HeuristicLab.Core.Views", "3.3")]
  [PluginDependency("HeuristicLab.MainForm", "3.3")]
  [PluginDependency("HeuristicLab.MainForm.WindowsForms", "3.3")]
  [PluginDependency("HeuristicLab.Problems.Programmable", "3.3")]
  [PluginDependency("HeuristicLab.Scripting", "3.3")]
  [PluginDependency("HeuristicLab.Scripting.Views", "3.3")]
  public class HeuristicLabProblemsProgrammableViewsPlugin : PluginBase { }
}
