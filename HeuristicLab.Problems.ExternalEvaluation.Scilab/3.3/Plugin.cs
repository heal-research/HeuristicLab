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
using HeuristicLab.PluginInfrastructure;
using Microsoft.Win32;

namespace HeuristicLab.Problems.ExternalEvaluation.Scilab {
  /// <summary>
  /// Plugin class for HeuristicLab.Problems.ExternalEvaluation.Scilab plugin.
  /// </summary>
  [Plugin("HeuristicLab.Problems.ExternalEvaluation.Scilab", "3.3.16.0")]
  [PluginFile("HeuristicLab.Problems.ExternalEvaluation.Scilab-3.3.dll", PluginFileType.Assembly)]
  [PluginDependency("HeuristicLab.DotNetScilab", "1.0")]
  [PluginDependency("HeuristicLab.Collections", "3.3")]
  [PluginDependency("HeuristicLab.Common", "3.3")]
  [PluginDependency("HeuristicLab.Core", "3.3")]
  [PluginDependency("HeuristicLab.Data", "3.3")]
  [PluginDependency("HeuristicLab.Encodings.RealVectorEncoding", "3.3")]
  [PluginDependency("HeuristicLab.Operators", "3.3")]
  [PluginDependency("HeuristicLab.Parameters", "3.3")]
  [PluginDependency("HeuristicLab.Attic", "1.0")]
  [PluginDependency("HeuristicLab.Problems.ParameterOptimization", "3.3")]
  public class HeuristicLabProblemsExternalEvaluationScilabPlugin : PluginBase {
    private const string registryKey = @"SOFTWARE\Scilab";
    private const string scilab_Install = @"LASTINSTALL";
    private const string scilab_PATH = @"SCIPATH";

    public override void OnLoad() {
      base.OnLoad();

      var currentPath = Environment.GetEnvironmentVariable("path");
      if (currentPath == null) return;

      var scilabPath = GetScilabInstallPath();
      if (string.IsNullOrEmpty(scilabPath)) return;

      Environment.SetEnvironmentVariable("path", scilabPath + ";" + currentPath);
    }

    private string GetScilabInstallPath() {
      using (var registryLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default)) {
        using (var registryScilab = registryLocalMachine.OpenSubKey(registryKey)) {
          if (registryScilab == null) return string.Empty;
          var scilabVersion = registryScilab.GetValue(scilab_Install);
          if (scilabVersion == null) return string.Empty;
          using (var registryScilabVersion = registryScilab.OpenSubKey(scilabVersion.ToString())) {
            if (registryScilabVersion == null) return string.Empty;
            var scilabPath = registryScilabVersion.GetValue(scilab_PATH);
            if (scilabPath == null) return string.Empty;
            string path = scilabPath.ToString();
            path += @"\bin";
            return path;
          }
        }
      }
    }
  }
}
