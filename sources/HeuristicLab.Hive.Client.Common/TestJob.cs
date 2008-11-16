using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Client.Common {
  public class TestJob: JobBase {
    public override void Run() {
      for (int x = 0; x < 10; x++) {
        for (int y = 0; y < Int32.MaxValue; y++) ;
        if (abort == true) {
          Logging.getInstance().Info(this.ToString(), "Job Processing aborted");
          //Console.WriteLine("Job Abort Processing");
          break;
        }
        Logging.getInstance().Info(this.ToString(), "Iteration " + x + " done");
        //Console.WriteLine("Iteration " + x + " done");
      }      
      OnJobStopped();
    }
  }
}
