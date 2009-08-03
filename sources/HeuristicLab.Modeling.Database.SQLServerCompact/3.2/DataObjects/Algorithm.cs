using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Text;

namespace HeuristicLab.Modeling.Database.SQLServerCompact {
  [Table(Name = "Algorithm")]
  public class Algorithm : IAlgorithm  {
    public Algorithm() {
    }

    public Algorithm(string name)
      : this() {
      this.name = name;
    }

    public Algorithm(string name, string description)
      : this(name) {
      this.description = description;
    }

    private int id;
    [Column(Storage = "id", IsPrimaryKey = true, IsDbGenerated = true)]
    public int Id {
      get { return this.id; }
      private set { this.id = value; }
    }

    private string name;
    [Column(Storage = "name", CanBeNull = false)]
    public string Name {
      get { return this.name; }
      set { this.name = value; }
    }

    private string description;
    [Column(Storage = "description", CanBeNull = true)]
    public string Description {
      get { return this.description; }
      set { this.description = value; }
    }
  }
}
