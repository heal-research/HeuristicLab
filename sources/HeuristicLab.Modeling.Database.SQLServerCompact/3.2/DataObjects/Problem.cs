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
