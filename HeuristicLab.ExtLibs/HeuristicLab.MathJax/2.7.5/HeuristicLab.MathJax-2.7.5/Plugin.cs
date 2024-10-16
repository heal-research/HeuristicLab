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
using System.IO;
using System.IO.Compression;
using System.Linq;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.MathJax {
  [Plugin("HeuristicLab.MathJax", "HeuristicLab transport plugin for MathJax (www.mathjax.org) , an open source JavaScript display engine for mathematics that works in all browsers", "1.2.0")]
  [PluginFile("HeuristicLab.MathJax-2.7.5.dll", PluginFileType.Assembly)]
  [PluginFile("MathJax license.txt", PluginFileType.License)]
  [PluginFile("mathjax.zip", PluginFileType.Data)]

  public class HeuristicLabMathJaxPlugin : PluginBase {
    public override void OnLoad() {
      base.OnLoad();
      if (!Directory.EnumerateDirectories(AppDomain.CurrentDomain.BaseDirectory, "MathJax", SearchOption.TopDirectoryOnly).Any()) {
        ZipFile.ExtractToDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mathjax.zip"), AppDomain.CurrentDomain.BaseDirectory);
      }
    }
  }
}
