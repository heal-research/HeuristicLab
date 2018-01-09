#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;

namespace HeuristicLab.Clients.Hive.SlaveCore.WindowsService {
  static class Program {
    private static void Main(string[] args) {
      // Install as service, see http://stackoverflow.com/a/12703878
      bool installDone = false;
      try {
        string parameter = string.Concat(args);
        switch (parameter) {
          case "--install":
            installDone = true;
            ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
            break;
          case "--uninstall":
            installDone = true;
            ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
            break;
        }
      }
      catch (Exception ex) {
        Console.WriteLine("Error on (un)install of Hive Slave service: " + Environment.NewLine + ex);
      }

      if (!installDone) {
        ServiceBase[] ServicesToRun;
        ServicesToRun = new ServiceBase[]
        {
          new SlaveWindowsService()
        };
        ServiceBase.Run(ServicesToRun);
      }
    }
  }
}
