#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.SimSharp {
  [Plugin("HeuristicLab.SimSharp", "Transport plugin for the simulation framework Sim#", "3.0.7")]
  [PluginFile("HeuristicLab.SimSharp-3.0.7.dll", PluginFileType.Assembly)]
  [PluginFile("SimSharp-3.0.7.dll", PluginFileType.Assembly)]
  [PluginFile("SimSharp-3.0.7 License.txt", PluginFileType.License)]
  public class Plugin : PluginBase {
  }
}
