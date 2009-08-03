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
using HeuristicLab.Core;
using System.Xml;
using System.ServiceModel;
using System.ServiceModel.Description;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Modeling.Database.SQLServerCompact;

namespace HeuristicLab.CEDMA.Core {
  public class Console : ItemBase, IEditable {
    private static readonly string sqlServerCompactFile = AppDomain.CurrentDomain.BaseDirectory + "HeuristicLab.Modeling.database.sdf";
    private static readonly string sqlServerCompactConnectionString = @"Data Source=" + sqlServerCompactFile;

    private Results results;
    public Results Results {
      get {
        if (results == null) ReloadResults();
        return results;
      }
    }
    public Console()
      : base() {
    }

    public IEditor CreateEditor() {
      return new ConsoleEditor(this);
    }

    public override IView CreateView() {
      return new ConsoleEditor(this);
    }

    private void ReloadResults() {
      results = new Results(new DatabaseService(sqlServerCompactConnectionString));
    }
  }
}
