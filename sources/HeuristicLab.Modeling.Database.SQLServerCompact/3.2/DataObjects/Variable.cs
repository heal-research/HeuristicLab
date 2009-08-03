using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Text;

namespace HeuristicLab.Modeling.Database.SQLServerCompact {
  [Table(Name = "Variable")]
  public class Variable : IVariable{
    public Variable() {
    }

    public Variable(string name)
      : this() {
      this.name = name;
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
      private set { this.name = value; }
    }
  }
}
