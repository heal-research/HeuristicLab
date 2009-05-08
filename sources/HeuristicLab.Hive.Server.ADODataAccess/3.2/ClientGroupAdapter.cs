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

using HeuristicLab.Hive.Server.DataAccess;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.Data;
using HeuristicLab.DataAccess.ADOHelper;
using HeuristicLab.Hive.Server.ADODataAccess.dsHiveServerTableAdapters;
using System.Data.Common;
using System.Data.SqlClient;
using HeuristicLab.Hive.Server.ADODataAccess.TableAdapterWrapper;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class ClientGroupAdapter : 
    DataAdapterBase<dsHiveServerTableAdapters.ClientGroupTableAdapter, 
    ClientGroup, 
    dsHiveServer.ClientGroupRow>, 
    IClientGroupAdapter {
    #region Fields
    private ManyToManyRelationHelper<
      dsHiveServerTableAdapters.ClientGroup_ResourceTableAdapter, 
      dsHiveServer.ClientGroup_ResourceRow> manyToManyRelationHelper = null;

    private ManyToManyRelationHelper<dsHiveServerTableAdapters.ClientGroup_ResourceTableAdapter, 
      dsHiveServer.ClientGroup_ResourceRow> ManyToManyRelationHelper {
      get {
        if (manyToManyRelationHelper == null) {
          manyToManyRelationHelper =
            new ManyToManyRelationHelper<dsHiveServerTableAdapters.ClientGroup_ResourceTableAdapter,
              dsHiveServer.ClientGroup_ResourceRow>(new ClientGroup_ResourceAdapterWrapper(), 1);
        }

        manyToManyRelationHelper.Session = Session as Session;

        return manyToManyRelationHelper;
      }
    }

    private IResourceAdapter resourceAdapter = null;

    private IResourceAdapter ResAdapter {
      get {
        if (resourceAdapter == null)
          resourceAdapter =
            this.Session.GetDataAdapter<Resource, IResourceAdapter>();

        return resourceAdapter;
      }
    }
    #endregion

    public ClientGroupAdapter(): 
      base(new ClientGroupAdapterWrapper()) {
    }

    #region Overrides
    protected override ClientGroup ConvertRow(dsHiveServer.ClientGroupRow row,
      ClientGroup clientGroup) {
      if (row != null && clientGroup != null) {
        /*Parent - Permission Owner*/
        clientGroup.Id = row.ResourceId;
        ResAdapter.GetById(clientGroup);

        ICollection<Guid> resources =
          ManyToManyRelationHelper.GetRelationships(clientGroup.Id);

        clientGroup.Resources.Clear();
        foreach(Guid resource in resources) {
          Resource res =
            ResAdapter.GetByIdPolymorphic(resource);

            clientGroup.Resources.Add(res);         
        }

        return clientGroup;
      } else
        return null;
    }

    protected override dsHiveServer.ClientGroupRow ConvertObj(ClientGroup clientGroup,
      dsHiveServer.ClientGroupRow row) {
      if (clientGroup != null && row != null) {
        row.ResourceId = clientGroup.Id;
      }

      return row;
    }
    #endregion

    #region IClientGroupAdapter Members
    protected override void doUpdate(ClientGroup group) {
      if (group != null) {
        ResAdapter.Update(group);

        base.doUpdate(group);

        List<Guid> relationships = 
          new List<Guid>();
        foreach(Resource res in group.Resources) {
          ResAdapter.UpdatePolymorphic(res);

          relationships.Add(res.Id);
        }

        ManyToManyRelationHelper.UpdateRelationships(group.Id,
          relationships, 1);
      }
    }

    public ClientGroup GetByName(string name) {
      ClientGroup group = new ClientGroup();
      Resource res =
        ResAdapter.GetByName(name);

      if (res != null) {
        return GetById(res.Id);
      }

      return null;
    }

    public ICollection<ClientGroup> MemberOf(Resource resource) {
      if (resource != null) {
        return base.FindMultiple(
           delegate() {
             return Adapter.GetDataByParentsOf(resource.Id);
           }
        );
      } 

      return null;
    }

    protected override bool doDelete(ClientGroup group) {
      if (group != null) {
        //delete all relationships
        ManyToManyRelationHelper.UpdateRelationships(group.Id,
          new List<Guid>(), 1);

        return base.doDelete(group) && 
          ResAdapter.Delete(group);
      }

      return false;
    }

    #endregion
  }
}
