#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2009 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.SpringNET {
  
  [Plugin("HeuristicLab.SpringNET", "1.3.0.2849")]
  [PluginFile("HeuristicLab.SpringNET-1.3.0.dll", PluginFileType.Assembly)]
  [PluginFile("Common.Logging.dll", PluginFileType.Assembly)]
  [PluginFile("Spring.Services.dll", PluginFileType.Assembly)]
  [PluginFile("Spring.Core.dll", PluginFileType.Assembly)]
  [PluginFile("Spring.Aop.dll", PluginFileType.Assembly)]  
  public class HeuristicLabSpringNetPlugin : PluginBase {
  }
}
