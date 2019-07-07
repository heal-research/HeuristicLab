#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Security.Permissions;

namespace HeuristicLab.Services.Deployment {
  public class AdminService : IAdminService {
    #region IAdminService Members
    [PrincipalPermission(SecurityAction.Demand, Role = "Deployment Administrator")]
    public void DeployProduct(ProductDescription product) {
      var store = new PluginStore();
      store.Persist(product);
    }
    [PrincipalPermission(SecurityAction.Demand, Role = "Deployment Administrator")]
    public void DeleteProduct(ProductDescription product) {
      var store = new PluginStore();
      store.Delete(product);
    }
    [PrincipalPermission(SecurityAction.Demand, Role = "Deployment Administrator")]
    public void DeployPlugin(PluginDescription plugin, byte[] zipFile) {
      var store = new PluginStore();
      store.Persist(plugin, zipFile);
    }

    #endregion
  }
}
