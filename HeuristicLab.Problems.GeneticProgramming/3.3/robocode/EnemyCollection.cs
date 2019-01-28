#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HEAL.Attic;

namespace HeuristicLab.Problems.GeneticProgramming.Robocode {
  [Item("EnemyCollection", "A collection of enemy robots for the Robocode genetic programming problem.")]
  [StorableType("78324566-09C2-4D2F-A54F-79613934D7F1")]
  public class EnemyCollection : CheckedItemList<StringValue> {
    private const string sampleRobotToSelect = "sample.Crazy";

    [Storable]
    public string RobocodePath { get; set; }

    [StorableConstructor]
    protected EnemyCollection(StorableConstructorFlag _) : base(_) { }
    protected EnemyCollection(EnemyCollection original, Cloner cloner)
      : base(original, cloner) {
      RobocodePath = original.RobocodePath;
    }
    public EnemyCollection() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new EnemyCollection(this, cloner);
    }

    public static EnemyCollection ReloadEnemies(string robocodeDir) {
      EnemyCollection robotList = new EnemyCollection();

      try {
        var robotsDir = new DirectoryInfo(Path.Combine(robocodeDir, "robots"));
        var robotFiles = robotsDir.GetFiles("*", SearchOption.AllDirectories)
          .Where(x => x.Extension == ".class" || x.Extension == ".jar");
        var robotSet = new Dictionary<string, StringValue>();
        foreach (var robot in robotFiles) {
          string robotName = Path.Combine(robot.DirectoryName, Path.GetFileNameWithoutExtension(robot.FullName));
          robotName = robotName.Remove(0, robotsDir.FullName.Length);
          string[] nameParts = robotName.Split(new[] { Path.DirectorySeparatorChar },
            StringSplitOptions.RemoveEmptyEntries);
          robotName = string.Join(".", nameParts);
          robotSet[robot.FullName] = new StringValue(robotName);
        }
        robotList.AddRange(robotSet.Values);

        foreach (var robot in robotList) {
          robotList.SetItemCheckedState(robot, false);
        }
      }
      catch { }

      //select one robot so that if a user tries out the Robocode problem it works with the default settings
      if (robotList.Exists(x => x.Value == sampleRobotToSelect)) {
        StringValue sampleRobot = robotList.Single(x => x.Value == sampleRobotToSelect);
        robotList.SetItemCheckedState(sampleRobot, true);
      }

      return robotList;
    }
  }
}
