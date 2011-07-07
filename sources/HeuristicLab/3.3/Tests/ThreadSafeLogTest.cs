using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Core;
using System.Threading.Tasks;
using System.Threading;

namespace HeuristicLab_33.Tests {
  [TestClass]
  public class ThreadSafeLogTest {

    [TestMethod]
    public void ThreadSafeLogThreadSavetyTest() {
      int count = 100000;
      if (Environment.ProcessorCount > 2) {
        Log l = new Log();
        AddMessagesInParallel(count, l);
        // with a normal log there are race conditions which should cause problems
        // actually we might geht lucky here but this should happen only seldomly
        Assert.AreNotEqual(count, l.Messages.Count());
      }

      ThreadSafeLog safeLog = new ThreadSafeLog();
      AddMessagesInParallel(count, safeLog);
      // the thread safe log should work like a charm
      Assert.AreEqual(count, safeLog.Messages.Count());
    }

    private void AddMessagesInParallel(int count, ILog l) {
      Parallel.For(0, count, (i) => {
        l.LogMessage("Message " + i); // write something
        l.Messages.Count(); // iterate over all messages
      });
    }
  }
}
