using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Client.Common {
  public class TestJob: JobBase {
    public override void Run() {
      Console.WriteLine("The Job Thread is running!");
    }
  }
}
