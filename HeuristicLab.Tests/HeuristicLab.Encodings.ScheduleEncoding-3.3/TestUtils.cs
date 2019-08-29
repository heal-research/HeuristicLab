#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Encodings.PermutationEncoding;

using HeuristicLab.Tests;

namespace HeuristicLab.Encodings.ScheduleEncoding.Tests {
  public class TestUtils {
    public static JSMEncoding CreateTestJSM1() {
      JSMEncoding result = new JSMEncoding(0);
      ItemList<Permutation> jsm = result.JobSequenceMatrix;
      for (int i = 0; i < 6; i++)
        jsm.Add(new Permutation(PermutationTypes.Absolute, new int[] { 0, 1, 2, 3, 4, 5 }));
      return result;
    }

    public static JSMEncoding CreateTestJSM2() {
      JSMEncoding result = new JSMEncoding(0);
      ItemList<Permutation> jsm = result.JobSequenceMatrix;
      for (int i = 0; i < 6; i++)
        jsm.Add(new Permutation(PermutationTypes.Absolute, new int[] { 5, 4, 3, 2, 1, 0 }));
      return result;
    }


    public static PWREncoding CreateTestPWR1() {
      PWREncoding result = new PWREncoding();
      IntegerVector pwr = new IntegerVector(new int[] { 1, 0, 1, 1, 2, 0, 2, 2, 0 });
      result.PermutationWithRepetition = pwr;
      return result;
    }

    public static PWREncoding CreateTestPWR2() {
      PWREncoding result = new PWREncoding();
      IntegerVector pwr = new IntegerVector(new int[] { 0, 1, 1, 0, 2, 0, 1, 2, 2 });
      result.PermutationWithRepetition = pwr;
      return result;
    }


    public static ItemList<Job> CreateJobData() {
      Job j1 = new Job(0, 0);
      j1.Tasks.AddRange(new[]{  
        new Task (0, 0, j1.Index, 1),
        new Task (1, 1, j1.Index, 2),
        new Task (2, 2, j1.Index, 1)
      });

      Job j2 = new Job(1, 0);
      j2.Tasks.AddRange(new[]{ 
        new Task (3, 2, j2.Index, 2),
        new Task (4, 1, j2.Index, 1),
        new Task (5, 0, j2.Index, 2)
      });

      Job j3 = new Job(2, 0);
      j3.Tasks.AddRange(new[]{ 
        new Task (6, 0, j3.Index, 1),
        new Task (7, 2, j3.Index, 2),
        new Task (8, 1, j3.Index, 1)
      });

      return new ItemList<Job> { j1, j2, j3 };
    }

    public static Schedule CreateTestSchedule1() {
      Schedule result = DirectScheduleRandomCreator.Apply(new PWREncoding(3, 3, new TestRandom(new int[] { 1, 0, 1, 1, 2, 0, 2, 2, 0 }, null)), CreateJobData());
      return result;
    }

    public static Schedule CreateTestSchedule2() {
      Schedule result = DirectScheduleRandomCreator.Apply(new PWREncoding(3, 3, new TestRandom(new int[] { 0, 1, 1, 0, 2, 0, 1, 2, 2 }, null)), CreateJobData());
      return result;
    }
    public static bool ScheduleEquals(Schedule actual, Schedule expected) {
      return actual.Resources.Count == expected.Resources.Count &&
             actual.Resources.Zip(expected.Resources, (a, e) => ResourceEquals(a, e)).All(_ => _);
    }

    public static bool ResourceEquals(Resource actual, Resource expected) {
      return actual.Index == expected.Index &&
             actual.TotalDuration == expected.TotalDuration &&
             actual.Tasks.Count == expected.Tasks.Count &&
             actual.Tasks.Zip(expected.Tasks, (a, e) => TaskEquals(a, e)).All(_ => _);
    }

    public static bool TaskEquals(ScheduledTask actual, ScheduledTask expected) {
      return
        actual.StartTime == expected.StartTime &&
        actual.EndTime == expected.EndTime &&
        actual.Duration == expected.Duration &&
        actual.ResourceNr == expected.ResourceNr &&
        actual.JobNr == expected.JobNr &&
        actual.TaskNr == expected.TaskNr;
    }

    public static bool JSMEncodingEquals(JSMEncoding expected, JSMEncoding actual) {
      if (expected.JobSequenceMatrix.Count != actual.JobSequenceMatrix.Count)
        return false;
      for (int i = 0; i < expected.JobSequenceMatrix.Count; i++) {
        if (!PermutationEquals(expected.JobSequenceMatrix[i], actual.JobSequenceMatrix[i]))
          return false;
      }
      return true;
    }
    private static bool PermutationEquals(Permutation p1, Permutation p2) {
      if (p1.Length != p2.Length)
        return false;
      for (int i = 0; i < p1.Length; i++) {
        if (p1[i] != p2[i])
          return false;
      }
      return true;
    }

    public static bool PRWEncodingEquals(PWREncoding expected, PWREncoding actual) {
      if (expected.PermutationWithRepetition.Length != actual.PermutationWithRepetition.Length)
        return false;
      for (int i = 0; i < expected.PermutationWithRepetition.Length; i++) {
        if (expected.PermutationWithRepetition[i] != actual.PermutationWithRepetition[i])
          return false;
      }
      return true;
    }
  }
}
