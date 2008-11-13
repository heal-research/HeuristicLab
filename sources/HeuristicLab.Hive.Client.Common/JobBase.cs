using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HeuristicLab.Hive.Client.Common {
  abstract public class JobBase {

    private Thread thread = null;
    public int Progress { get; set; }
    

    abstract public void Run();

    public void Start() {
      thread = new Thread(new ThreadStart(Run));
      thread.Start();
    }
    public JobBase() {
    }
  }
}
