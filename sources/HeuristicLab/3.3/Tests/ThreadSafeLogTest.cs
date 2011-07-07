#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Threading.Tasks;
using HeuristicLab.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
