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
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Text;

using HeuristicLab.DataAnalysis;
using HeuristicLab.Core;

namespace HeuristicLab.Modeling.Database.SQLServerCompact {
  [Table(Name = "Problem")]
  public class Problem : IProblem {
    public Problem() {
    }

    public Problem(Dataset dataset)
      : this() {
      this.Dataset = dataset;
    }

    private int id;
    [Column(Storage = "id", IsPrimaryKey = true, IsDbGenerated = true)]
    public int Id {
      get { return this.id; }
      private set { this.id = value; }
    }

    private byte[] data;
    [Column(Storage = "data", DbType = "image", CanBeNull = false)]
    public byte[] Data {
      get { return this.data; }
      private set { this.data = value; }
    }

    public Dataset Dataset {
      get { return (Dataset)PersistenceManager.RestoreFromGZip(this.Data); }
      set { this.Data = PersistenceManager.SaveToGZip(value); }
    }
  }
}
