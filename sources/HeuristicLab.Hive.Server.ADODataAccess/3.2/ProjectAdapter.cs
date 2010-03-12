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
using System.Linq.Expressions;
using HeuristicLab.DataAccess.Interfaces;
using HeuristicLab.DataAccess.ADOHelper;
using HeuristicLab.Hive.Server.ADODataAccess.dsHiveServerTableAdapters;
using System.Data.Common;
using System.Data.SqlClient;
using HeuristicLab.Hive.Server.ADODataAccess.TableAdapterWrapper;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class ProjectAdapter: 
    DataAdapterBase<
      dsHiveServerTableAdapters.ProjectTableAdapter, 
      ProjectDto, 
      dsHiveServer.ProjectRow>,
    IProjectAdapter{
    public ProjectAdapter(): 
      base(new ProjectAdapterWrapper()) {
    }

    #region Overrides
    protected override ProjectDto ConvertRow(dsHiveServer.ProjectRow row, 
      ProjectDto project) {
      if (row != null && project != null) {
        project.Id = row.ProjectId;

        if (!row.IsNameNull())
          project.Name = row.Name;
        else
          project.Name = null;

        return project;
      }
      else
        return null;
    }

    protected override dsHiveServer.ProjectRow ConvertObj(ProjectDto project,
      dsHiveServer.ProjectRow row) {
      if (project != null && row != null) {
        row.ProjectId = project.Id;

        if (project.Name != null)
          row.Name = project.Name;
        else
          row.SetNameNull();
      }

      return row;
    }

    #endregion

  }
}
