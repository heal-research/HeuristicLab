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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Protobuf {
  [Plugin("HeuristicLab.Protobuf","HeuristicLab wrapper for Google.Protobuf nuget package","3.6.1.0")]  
  [PluginFile("HeuristicLab.Protobuf.dll", PluginFileType.Assembly)]
  [PluginFile("Google.Protobuf.dll", PluginFileType.Assembly)]
  [PluginFile("Google.Protobuf.License.txt", PluginFileType.License)]
  public class Plugin : PluginBase { }
}
