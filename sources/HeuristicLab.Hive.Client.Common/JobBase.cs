using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HeuristicLab.Hive.Client.Common {

  public delegate void Callback();

  abstract public class JobBase {

    public event Callback JobAborted;

    private Thread thread = null;
    
    public int Progress { get; set; }
    private bool abort = false;
    private bool running = false;

    abstract public void Run();

    public void Start() {
      thread = new Thread(new ThreadStart(Run));
      thread.Start();
      running = true;
    }

    public void Stop() {
      abort = true;        
    }

    public JobBase() {    
    }
  }
}
