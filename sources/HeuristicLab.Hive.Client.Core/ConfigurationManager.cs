#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Client.Core {
  /// <summary>
  /// accesses the Server and sends his data (uuid, uptimes, hardware config) 
  /// </summary>
  public class ConfigurationManager {
    private static ConfigurationManager instance = null;

    private ClientInfo clientInfo;
    private Guid guid;

    public static ConfigurationManager GetInstance() {
      if (instance == null) {
        instance = new ConfigurationManager();
      }
      return instance;
    }

    /// <summary>
    /// Constructor for the singleton, must recover Guid, Calendar, ...
    /// </summary>
    private ConfigurationManager() {
      //retrive GUID from XML file, or burn in hell. as in hell. not heaven.
      //this won't work this way. We need a plugin for XML Handling.
      guid = Guid.NewGuid();
      clientInfo = new ClientInfo();
      clientInfo.ClientId = Guid.NewGuid();
      clientInfo.NrOfCores = Environment.ProcessorCount;
      clientInfo.Memory = 1024;
      clientInfo.Name = Environment.MachineName;
    }

    public ClientInfo GetClientInfo() {
      return clientInfo;          
    }

    public void Loggedin() {
      if (clientInfo == null) {
        clientInfo = new ClientInfo();
      }
      clientInfo.Login = DateTime.Now;
    }

    public void Connect(Guid guid) {
    }

  }
}
