using HeuristicLab.Core;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Encodings.ScheduleEncoding.JobSequenceMatrix;
using HeuristicLab.Encodings.ScheduleEncoding.PermutationWithRepetition;

namespace HeuristicLab.Encodings.ScheduleEncoding.Tests_33.Tests {
  public class TestUtils {
    public static JSMEncoding CreateTestJSM1() {
      JSMEncoding result = new JSMEncoding();
      ItemList<Permutation> jsm = new ItemList<Permutation>();
      for (int i = 0; i < 6; i++)
        jsm.Add(new Permutation(PermutationTypes.Absolute, new int[] { 0, 1, 2, 3, 4, 5 }));
      result.JobSequenceMatrix = jsm;
      return result;
    }

    public static JSMEncoding CreateTestJSM2() {
      JSMEncoding result = new JSMEncoding();
      ItemList<Permutation> jsm = new ItemList<Permutation>();
      for (int i = 0; i < 6; i++)
        jsm.Add(new Permutation(PermutationTypes.Absolute, new int[] { 5, 4, 3, 2, 1, 0 }));
      result.JobSequenceMatrix = jsm;
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
      j1.Tasks = new ItemList<Task> { 
        new Task (0, 0, j1.Index, 1),
        new Task (1, 1, j1.Index, 2),
        new Task (2, 2, j1.Index, 1)
      };

      Job j2 = new Job(1, 0);
      j2.Tasks = new ItemList<Task> { 
        new Task (3, 2, j2.Index, 2),
        new Task (4, 1, j2.Index, 1),
        new Task (5, 0, j2.Index, 2)
      };

      Job j3 = new Job(2, 0);
      j3.Tasks = new ItemList<Task> { 
        new Task (6, 0, j3.Index, 1),
        new Task (7, 2, j3.Index, 2),
        new Task (8, 1, j3.Index, 1)
      };

      return new ItemList<Job> { j1, j2, j3 };
    }

    public static Schedule CreateTestSchedule1() {
      Schedule result = DirectScheduleRandomCreator.Apply(3, 3, new PWREncoding(3, 3, new TestRandom(new int[] { 1, 0, 1, 1, 2, 0, 2, 2, 0 }, null)), CreateJobData());
      return result;
    }

    public static Schedule CreateTestSchedule2() {
      Schedule result = DirectScheduleRandomCreator.Apply(3, 3, new PWREncoding(3, 3, new TestRandom(new int[] { 0, 1, 1, 0, 2, 0, 1, 2, 2 }, null)), CreateJobData());
      return result;
    }
  }
}
