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

namespace HeuristicLab.LibSVM {
  [Plugin("HeuristicLab.LibSVM", "HeuristicLab transport plugin for libSVM version 3.12 by Chih-Chung Chang and Chih-Jen Lin", "3.12.0.0")]
  [PluginFile("HeuristicLab.LibSVM-3.12.dll", PluginFileType.Assembly)]
  [PluginFile("LibSVM-3.12.dll", PluginFileType.Assembly)]
  [PluginFile("LibSVM license.txt", PluginFileType.License)]
  public class HeuristicLabLibSVMPlugin : PluginBase {
  }
}
