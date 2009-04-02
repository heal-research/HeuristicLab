using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Server.ServerConsole {
  public enum Change { Create, Update, Delete };
  public enum Type { Client, ClientGroup, Job };
  public class Changes {

    public Type Types { get; set; }

    public Guid ID { get; set; }

    public Change ChangeType { get; set; }

    public int Position { get; set; }
  }
}
