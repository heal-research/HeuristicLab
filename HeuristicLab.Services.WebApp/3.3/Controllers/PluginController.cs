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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Services.Hive;
using DTO = HeuristicLab.Services.WebApp.Controllers.DataTransfer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeuristicLab.Services.WebApp.Controllers {

  [Authorize(Roles = HiveRoles.Administrator)]
  public class PluginController : ControllerBase {

    private readonly PluginManager pluginManager = PluginManager.Instance;

    public IEnumerable<DTO.Plugin> GetPlugins() {
      var plugins = pluginManager.Plugins;
      return plugins.Select(plugin => new DTO.Plugin {
        Name = plugin.Name,
        AssemblyName = plugin.AssemblyName,
        LastReload = plugin.LastReload,
        Exception = plugin.Exception
      });
    }

    public bool ReloadPlugin(string name) {
      var plugin = pluginManager.GetPlugin(name);
      if (plugin == null)
        return false;
      plugin.ReloadControllers();
      return true;
    }
  }
}
