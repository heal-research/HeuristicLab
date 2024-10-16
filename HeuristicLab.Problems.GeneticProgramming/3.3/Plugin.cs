#region License Information
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

namespace HeuristicLab.Problems.GeneticProgramming {
  [Plugin("HeuristicLab.Problems.GeneticProgramming","Provides implementations for genetic programming problems such as the artificial ant problem.", "3.3.16.0")]
  [PluginFile("HeuristicLab.Problems.GeneticProgramming-3.3.dll", PluginFileType.Assembly)]
  [PluginFile("Robocode/robocode.zip", PluginFileType.Data)]
  [PluginFile("Robocode/BattleRunner.class", PluginFileType.Data)]
  [PluginFile("Robocode/BattleObserver.class", PluginFileType.Data)]
  [PluginDependency("HeuristicLab.Collections", "3.3")]
  [PluginDependency("HeuristicLab.Common", "3.3")]
  [PluginDependency("HeuristicLab.Common.Resources", "3.3")]
  [PluginDependency("HeuristicLab.Core", "3.3")]
  [PluginDependency("HeuristicLab.Data", "3.3")]
  [PluginDependency("HeuristicLab.Encodings.SymbolicExpressionTreeEncoding","3.4")]
  [PluginDependency("HeuristicLab.Optimization", "3.3")]
  [PluginDependency("HeuristicLab.Parameters", "3.3")]
  [PluginDependency("HeuristicLab.Attic", "1.0")]
  [PluginDependency("HeuristicLab.Problems.DataAnalysis", "3.4")]
  [PluginDependency("HeuristicLab.Problems.Instances", "3.3")]
  [PluginDependency("HeuristicLab.Random", "3.3")]
  public class HeuristicLabProblemsGeneticProgrammingPlugin : PluginBase {
    public override void OnLoad() {
        base.OnLoad();
        if (!Directory.EnumerateDirectories(AppDomain.CurrentDomain.BaseDirectory, Path.Combine("Robocode", "libs"), SearchOption.TopDirectoryOnly).Any()) {
          ZipFile.ExtractToDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.Combine("Robocode", "robocode.zip")), AppDomain.CurrentDomain.BaseDirectory);
        }
      }
  }
}
