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

using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.HiveDrain {
  // Hive Drain HeuristicLab Application, based on the original code of apetrei's Hive Drain Console Application
  [Plugin("HeuristicLab.HiveDrain", "1.0.0.0")]
  [PluginFile("HeuristicLab.HiveDrain.dll", PluginFileType.Assembly)]
  public class HiveDrainPlugin : PluginBase { }

  [Application("Hive Drain", "Downloads and saves Hive Jobs")]
  public class HeuristicLabHiveDrainApplication : ApplicationBase {
    public static int MaxParallelDownloads = 2;

    public override void Run(ICommandLineArgument[] args) {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new HiveDrainMainWindow());
    }
  }
}
