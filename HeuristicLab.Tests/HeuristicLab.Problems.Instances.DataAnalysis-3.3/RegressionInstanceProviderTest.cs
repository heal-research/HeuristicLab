#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.Instances.DataAnalysis.Tests {
  [TestClass()]
  public class RegressionInstanceProviderTest {

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "medium")]
    public void GetKeijzerInstanceTest() {
      var target = new KeijzerInstanceProvider();
      StringBuilder erroneousInstances = new StringBuilder();
      int count = 0;
      foreach (var id in target.GetDataDescriptors()) {
        try {
          target.LoadData(id);
        } catch (Exception ex) {
          erroneousInstances.AppendLine(id.Name + ": " + ex.Message);
        }
        count++;
      }
      Assert.IsTrue(count > 0, "No problem instances were found.");
      Assert.IsTrue(erroneousInstances.Length == 0, "Some instances could not be parsed: " + Environment.NewLine + erroneousInstances.ToString());
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void GetKornInstanceTest() {
      var target = new KornsInstanceProvider();
      StringBuilder erroneousInstances = new StringBuilder();
      int count = 0;
      foreach (var id in target.GetDataDescriptors()) {
        try {
          target.LoadData(id);
        } catch (Exception ex) {
          erroneousInstances.AppendLine(id.Name + ": " + ex.Message);
        }
        count++;
      }
      Assert.IsTrue(count > 0, "No problem instances were found.");
      Assert.IsTrue(erroneousInstances.Length == 0, "Some instances could not be parsed: " + Environment.NewLine + erroneousInstances.ToString());
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void GetNguyenInstanceTest() {
      var target = new NguyenInstanceProvider();
      StringBuilder erroneousInstances = new StringBuilder();
      int count = 0;
      foreach (var id in target.GetDataDescriptors()) {
        try {
          target.LoadData(id);
        } catch (Exception ex) {
          erroneousInstances.AppendLine(id.Name + ": " + ex.Message);
        }
        count++;
      }
      Assert.IsTrue(count > 0, "No problem instances were found.");
      Assert.IsTrue(erroneousInstances.Length == 0, "Some instances could not be parsed: " + Environment.NewLine + erroneousInstances.ToString());
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void GetRealWorldInstanceTest() {
      var target = new RegressionRealWorldInstanceProvider();
      StringBuilder erroneousInstances = new StringBuilder();
      int count = 0;
      foreach (var id in target.GetDataDescriptors()) {
        try {
          target.LoadData(id);
        } catch (Exception ex) {
          erroneousInstances.AppendLine(id.Name + ": " + ex.Message);
        }
        count++;
      }
      Assert.IsTrue(count > 0, "No problem instances were found.");
      Assert.IsTrue(erroneousInstances.Length == 0, "Some instances could not be parsed: " + Environment.NewLine + erroneousInstances.ToString());
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void GetVariousInstanceTest() {
      var target = new VariousInstanceProvider();
      StringBuilder erroneousInstances = new StringBuilder();
      int count = 0;
      foreach (var id in target.GetDataDescriptors()) {
        try {
          target.LoadData(id);
        } catch (Exception ex) {
          erroneousInstances.AppendLine(id.Name + ": " + ex.Message);
        }
        count++;
      }
      Assert.IsTrue(count > 0, "No problem instances were found.");
      Assert.IsTrue(erroneousInstances.Length == 0, "Some instances could not be parsed: " + Environment.NewLine + erroneousInstances.ToString());
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void GetVladislavlevaInstanceTest() {
      var target = new VladislavlevaInstanceProvider();
      StringBuilder erroneousInstances = new StringBuilder();
      int count = 0;
      foreach (var id in target.GetDataDescriptors()) {
        try {
          target.LoadData(id);
        } catch (Exception ex) {
          erroneousInstances.AppendLine(id.Name + ": " + ex.Message);
        }
        count++;
      }
      Assert.IsTrue(count > 0, "No problem instances were found.");
      Assert.IsTrue(erroneousInstances.Length == 0, "Some instances could not be parsed: " + Environment.NewLine + erroneousInstances.ToString());
    }
  }
}
