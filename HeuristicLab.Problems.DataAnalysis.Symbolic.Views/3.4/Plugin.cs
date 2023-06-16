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
using System.Collections.Generic;
using System.Text;
using HeuristicLab.PluginInfrastructure;
using System.Runtime.InteropServices;
using System.IO;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  [Plugin("HeuristicLab.Problems.DataAnalysis.Symbolic.Views","Provides views for symbolic data analysis problem classes.", "3.4.12.0")]
  [PluginFile("HeuristicLab.Problems.DataAnalysis.Symbolic.Views-3.4.dll", PluginFileType.Assembly)]
  [PluginFile("displayModelFrame.html", PluginFileType.Data)]
  [PluginDependency("HeuristicLab.ALGLIB", "3.17.0")]
  [PluginDependency("HeuristicLab.Collections", "3.3")]
  [PluginDependency("HeuristicLab.Common", "3.3")]
  [PluginDependency("HeuristicLab.Common.Resources", "3.3")]
  [PluginDependency("HeuristicLab.Core", "3.3")]
  [PluginDependency("HeuristicLab.Core.Views", "3.3")]
  [PluginDependency("HeuristicLab.Data", "3.3")]
  [PluginDependency("HeuristicLab.Data.Views", "3.3")]
  [PluginDependency("HeuristicLab.EPPlus", "4.0.3")]
  [PluginDependency("HeuristicLab.Encodings.SymbolicExpressionTreeEncoding", "3.4")]
  [PluginDependency("HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views", "3.4")]
  [PluginDependency("HeuristicLab.MainForm", "3.3")]
  [PluginDependency("HeuristicLab.MainForm.WindowsForms", "3.3")]
  [PluginDependency("HeuristicLab.MathJax", "1.2")]
  [PluginDependency("HeuristicLab.Optimization","3.3")]
  [PluginDependency("HeuristicLab.Problems.DataAnalysis", "3.4")]
  [PluginDependency("HeuristicLab.Problems.DataAnalysis.Symbolic", "3.4")]
  public class HeuristicLabProblemsDataAnalysisSymbolicViewsPlugin : PluginBase {
    // HeuristicLab is marked with zone identifier 'Internet' after download.
    // This propagates to all extracted files and can be checked in the file properties.
    // IE prevents execution of MathJax JavaScript inside of untrusted html
    // Therefore we need to programmatically unblock the displayModelFrame.html file before extracting.
    // see http://engram404.net/programmatically-unblocking-downloaded-files/

    // for removing the Zone.Identifier alternative NTFS stream (effectively unblocking the file)
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool DeleteFile(string lpFileName);

    public override void OnLoad() {
      base.OnLoad();

      // unblock mathjax.zip () 
      DeleteFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "displayModelFrame.html:Zone.Identifier"));
    }
  }
}
