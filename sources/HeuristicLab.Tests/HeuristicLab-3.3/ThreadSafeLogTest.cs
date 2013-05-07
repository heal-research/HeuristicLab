#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using System.Threading.Tasks;
using HeuristicLab.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab_33.Tests {
  [TestClass]
  public class ThreadSafeLogTest {

    [TestMethod]
    public void ThreadSafeLogThreadSafetyTest() {
      int count = 10000;
      ThreadSafeLog log = new ThreadSafeLog();

      Parallel.For(0, count, (i) => {
        log.LogMessage("Message " + i); // write something
        log.Messages.Count(); // iterate over all messages
      });

      Assert.AreEqual(count, log.Messages.Count());
    }
  }
}
