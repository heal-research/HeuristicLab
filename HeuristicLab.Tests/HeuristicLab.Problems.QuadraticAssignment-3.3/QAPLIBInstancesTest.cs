#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Problems.Instances.QAPLIB;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.QuadraticAssignment.Tests {
  [TestClass]
  public class QAPLIBInstancesTest {
    #region instances
    private static Dictionary<string, double> qaplibInstances = new Dictionary<string, double>() {
      { "bur26a", 5426670 },
      { "bur26b", 3817852 },
      { "bur26c", 5426795 },
      { "bur26d", 3821225 },
      { "bur26e", 5386879 },
      { "bur26f", 3782044 },
      { "bur26g", 10117172 },
      { "bur26h", 7098658 },
      { "chr12a", 9552 },
      { "chr12b", 9742 },
      { "chr12c", 11156 },
      { "chr15a", 9896 },
      { "chr15b", 7990 },
      { "chr15c", 9504 },
      { "chr18a", 11098 },
      { "chr18b", 1534 },
      { "chr20a", 2192 },
      { "chr20b", 2298 },
      { "chr20c", 14142 },
      { "chr22a", 6156 },
      { "chr22b", 6194 },
      { "chr25a", 3796 },
      { "els19", 17212548 },
      { "esc16a", 68 },
      { "esc16b", 292 },
      { "esc16c", 160 },
      { "esc16d", 16 },
      { "esc16e", 28 },
      { "esc16f", 0 },
      { "esc16g", 26 },
      { "esc16h", 996 },
      { "esc16i", 14 },
      { "esc16j", 8 },
      { "esc32a", 130 },
      { "esc32b", 168 },
      { "esc32c", 642 },
      { "esc32d", 200 },
      { "esc32e", 2 },
      { "esc32f", 2 },
      { "esc32g", 6 },
      { "esc32h", 438 },
      { "esc64", 116 },
      { "esc128", 64 },
      { "had12", 1652 },
      { "had14", 2724 },
      { "had16", 3720 },
      { "had18", 5358 },
      { "had20", 6922 },
      { "kra30a", 88900 },
      { "kra30b", 91420 },
      { "kra32", 88700 },
      { "lipa20a", 3683 },
      { "lipa20b", 27076 },
      { "lipa30a", 13178 },
      { "lipa30b", 151426 },
      { "lipa40a", 31538 },
      { "lipa40b", 476581 },
      { "lipa50a", 62093 },
      { "lipa50b", 1210244 },
      { "lipa60a", 107218 },
      { "lipa60b", 2520135 },
      { "lipa70a", 169755 },
      { "lipa70b", 4603200 },
      { "lipa80a", 253195 },
      { "lipa80b", 7763962 },
      { "lipa90a", 360630 },
      { "lipa90b", 12490441 },
      { "nug12", 578 },
      { "nug14", 1014 },
      { "nug15", 1150 },
      { "nug16a", 1610 },
      { "nug16b", 1240 },
      { "nug17", 1732 },
      { "nug18", 1930 },
      { "nug20", 2570 },
      { "nug21", 2438 },
      { "nug22", 3596 },
      { "nug24", 3488 },
      { "nug25", 3744 },
      { "nug27", 5234 },
      { "nug28", 5166 },
      { "nug30", 6124 },
      { "rou12", 235528 },
      { "rou15", 354210 },
      { "rou20", 725522 },
      { "scr12", 31410 },
      { "scr15", 51140 },
      { "scr20", 110030 },
      { "sko42", 15812 },
      { "sko49", 23386 },
      { "sko56", 34458 },
      { "sko64", 48498 },
      { "sko72", 66256 },
      { "sko81", 90998 },
      { "sko90", 115534 },
      { "sko100a", 152002 },
      { "sko100b", 153890 },
      { "sko100c", 147862 },
      { "sko100d", 149576 },
      { "sko100e", 149150 },
      { "sko100f", 149036 },
      { "ste36a", 9526 },
      { "ste36b", 15852 },
      { "ste36c", 8239110 },
      { "tai12a", 224416 },
      { "tai12b", 39464925 },
      { "tai15a", 388214 },
      { "tai15b", 51765268 },
      { "tai17a", 491812 },
      { "tai20a", 703482 },
      { "tai20b", 122455319 },
      { "tai25a", 1167256 },
      { "tai25b", 344355646 },
      { "tai30a", 1818146 },
      { "tai30b", 637117113 },
      { "tai35a", 2422002 },
      { "tai35b", 283315445 },
      { "tai40a", 3139370 },
      { "tai40b", 637250948 },
      { "tai50a", 4938796 },
      { "tai50b", 458821517 },
      { "tai60a", 7208572  },
      { "tai60b", 608215054 },
      { "tai64c", 1855928 },
      { "tai80a", 13557864 },
      { "tai80b", 818415043 },
      { "tai100a", 21052466 },
      { "tai100b", 1185996137 },
      { "tai150b", 498896643 },
      { "tai256c", 44759294 },
      { "tho30", 149936 },
      { "tho40", 240516 },
      { "tho150", 8133398 },
      { "wil50", 48816 },
      { "wil100", 273038 }
    };
    private static Dictionary<string, double> lowerBounds = new Dictionary<string, double>() {
      { "bur26a", 5315200 },
      { "bur26f", 3706888 },
      { "chr25a", 2765 },
      { "els19", 11971949 },
      { "esc32a", 35 },
      { "esc32e", 0 },
      { "had20", 6166 },
      { "kra32", 67390 },
      { "lipa50a", 62020 },
      { "lipa50b", 1210244 },
      { "nug30", 4539 },
      { "scr20", 86766 },
      { "sko42", 11311 },
      { "tai35a", 1951207 },
      { "tai35b", 30866283 },
      { "tai100a", 15824355 }
    };
    #endregion

    [TestMethod]
    [TestCategory("Problems.Assignment")]
    [TestProperty("Time", "long")]
    public void TestQAPLIBInstances() {
      var provider = new QAPLIBInstanceProvider();
      var qap = new QuadraticAssignmentProblem();
      var failedInstances = new StringBuilder();

      var instances = provider.GetDataDescriptors();
      Assert.IsTrue(instances.Any(), "No instances could be found.");

      foreach (var instance in instances) {
        try {
          qap.Load(provider.LoadData(instance));
        } catch (Exception ex) {
          failedInstances.AppendLine(instance + ": " + ex.Message);
        }
      }
      Assert.IsTrue(failedInstances.Length == 0, "Following instances failed: " + Environment.NewLine + failedInstances.ToString());
    }

    [TestMethod]
    [TestCategory("Problems.Assignment")]
    [TestProperty("Time", "long")]
    public void TestQAPLIBSolutions() {
      var provider = new QAPLIBInstanceProvider();
      var qap = new QuadraticAssignmentProblem();
      var failedInstances = new StringBuilder();

      var instances = provider.GetDataDescriptors();
      Assert.IsTrue(instances.Any(), "No instances could be found.");

      foreach (var instance in instances) {
        qap.Load(provider.LoadData(instance));
        if (qaplibInstances.ContainsKey(instance.Name)
          && qap.BestKnownQuality != null && qap.BestKnownQuality.Value != qaplibInstances[instance.Name])
          failedInstances.AppendLine(instance.Name + ": " + qap.BestKnownQuality.Value.ToString() + " vs " + qaplibInstances[instance.Name]);
      }
      Assert.IsTrue(failedInstances.Length == 0, "Following instances/solutions have suspicious best quality: " + Environment.NewLine + failedInstances.ToString());
    }

    [TestMethod]
    [TestCategory("Problems.Assignment")]
    [TestProperty("Time", "short")]
    public void TestQAPLIBLowerBounds() {
      var provider = new QAPLIBInstanceProvider();
      var qap = new QuadraticAssignmentProblem();
      var failedInstances = new StringBuilder();

      var instances = provider.GetDataDescriptors();
      Assert.IsTrue(instances.Any(), "No instances could be found.");

      foreach (var instance in instances) {
        if (lowerBounds.ContainsKey(instance.Name)) {
          qap.Load(provider.LoadData(instance));
          if (qap.LowerBound.Value != lowerBounds[instance.Name])
            failedInstances.AppendLine(instance.Name + ": The Gilmore-Lawler lower bound is not valid.");
        }
      }
      Assert.IsTrue(failedInstances.Length == 0, "Following instances failed for the GLB calculation: " + Environment.NewLine + failedInstances.ToString());
    }
  }
}
