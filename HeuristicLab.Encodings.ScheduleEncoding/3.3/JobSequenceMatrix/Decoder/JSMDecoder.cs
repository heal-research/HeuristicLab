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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("JobSequenceMatrixDecoder", "Applies the GifflerThompson algorithm to create an active schedule from a JobSequence Matrix.")]
  [StorableClass]
  public class JSMDecoder : ScheduleDecoder {

    public IValueParameter<JSMDecodingErrorPolicy> DecodingErrorPolicyParameter {
      get { return (IValueParameter<JSMDecodingErrorPolicy>)Parameters["DecodingErrorPolicy"]; }
    }
    public IValueParameter<JSMForcingStrategy> ForcingStrategyParameter {
      get { return (IValueParameter<JSMForcingStrategy>)Parameters["ForcingStrategy"]; }
    }

    private JSMDecodingErrorPolicyTypes DecodingErrorPolicy {
      get { return DecodingErrorPolicyParameter.Value.Value; }
    }

    private JSMForcingStrategyTypes ForcingStrategy {
      get { return ForcingStrategyParameter.Value.Value; }
    }

    [StorableConstructor]
    protected JSMDecoder(bool deserializing) : base(deserializing) { }
    protected JSMDecoder(JSMDecoder original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new JSMDecoder(this, cloner);
    }

    public JSMDecoder()
      : base() {
      Parameters.Add(new ValueParameter<JSMDecodingErrorPolicy>("DecodingErrorPolicy", "Specify the policy that should be used to handle decoding errors.", new JSMDecodingErrorPolicy(JSMDecodingErrorPolicyTypes.RandomPolicy)));
      Parameters.Add(new ValueParameter<JSMForcingStrategy>("ForcingStrategy", "Specifies a forcing strategy.", new JSMForcingStrategy(JSMForcingStrategyTypes.SwapForcing)));
    }

    private static Task SelectTaskFromConflictSet(JSMEncoding solution, JSMDecodingErrorPolicyTypes decodingErrorPolicy, JSMForcingStrategyTypes forcingStrategy, int conflictedResourceNr, int progressOnConflictedResource, ItemList<Task> conflictSet, IRandom random) {
      if (conflictSet.Count == 1)
        return conflictSet[0];

      var jsm = solution.JobSequenceMatrix;

      //get solutionCandidate from jobSequencingMatrix
      int solutionCandidateJobNr = jsm[conflictedResourceNr][progressOnConflictedResource];

      //scan conflictSet for given solutionCandidate, and return if found
      foreach (Task t in conflictSet) {
        if (t.JobNr == solutionCandidateJobNr)
          return t;
      }

      //if solutionCandidate wasn't found in conflictSet apply DecodingErrorPolicy and ForcingPolicy
      Task result = ApplyDecodingErrorPolicy(decodingErrorPolicy, conflictSet, jsm[conflictedResourceNr], progressOnConflictedResource, random);
      int newResolutionIndex = 0;

      while (newResolutionIndex < jsm[conflictedResourceNr].Length && jsm[conflictedResourceNr][newResolutionIndex] != result.JobNr)
        newResolutionIndex++;
      ApplyForcingStrategy(forcingStrategy, solution, conflictedResourceNr, newResolutionIndex, progressOnConflictedResource, result.JobNr);

      return result;
    }

    private static Task ApplyDecodingErrorPolicy(JSMDecodingErrorPolicyTypes decodingErrorPolicy, ItemList<Task> conflictSet, Permutation resource, int progress, IRandom random) {
      if (decodingErrorPolicy == JSMDecodingErrorPolicyTypes.RandomPolicy) {
        //Random
        return conflictSet[random.Next(conflictSet.Count - 1)];
      } else {
        //Guided
        for (int i = progress; i < resource.Length; i++) {
          int j = 0;
          while (j < conflictSet.Count && conflictSet[j].JobNr != resource[i])
            j++;

          if (j < conflictSet.Count)
            return (conflictSet[j]);
        }
        return conflictSet[random.Next(conflictSet.Count - 1)];
      }
    }

    private static void ApplyForcingStrategy(JSMForcingStrategyTypes forcingStrategy, JSMEncoding solution, int conflictedResource, int newResolutionIndex, int progressOnResource, int newResolution) {
      var jsm = solution.JobSequenceMatrix;
      if (forcingStrategy == JSMForcingStrategyTypes.SwapForcing) {
        //SwapForcing
        jsm[conflictedResource][newResolutionIndex] = jsm[conflictedResource][progressOnResource];
        jsm[conflictedResource][progressOnResource] = newResolution;
      } else if (forcingStrategy == JSMForcingStrategyTypes.ShiftForcing) {
        //ShiftForcing
        List<int> asList = jsm[conflictedResource].ToList<int>();
        if (newResolutionIndex > progressOnResource) {
          asList.RemoveAt(newResolutionIndex);
          asList.Insert(progressOnResource, newResolution);
        } else {
          asList.Insert(progressOnResource, newResolution);
          asList.RemoveAt(newResolutionIndex);
        }
        jsm[conflictedResource] = new Permutation(PermutationTypes.Absolute, asList.ToArray<int>());
      } else {
        throw new InvalidOperationException(string.Format("JSMDecoder encountered unknown forcing strategy {0}", forcingStrategy));
      }
    }

    public override Schedule DecodeSchedule(ISchedule encoding, ItemList<Job> jobData) {
      var solution = encoding as JSMEncoding;
      if (solution == null) throw new InvalidOperationException("Encoding is not of type JSMEncoding");
      return DecodeSchedule(solution, jobData, DecodingErrorPolicy, ForcingStrategy);
    }

    public static Schedule DecodeSchedule(JSMEncoding solution, ItemList<Job> jobData, JSMDecodingErrorPolicyTypes decodingErrorPolicy, JSMForcingStrategyTypes forcingStrategy) {
      var random = new FastRandom(solution.RandomSeed);
      var jobs = (ItemList<Job>)jobData.Clone();
      var resultingSchedule = new Schedule(jobs[0].Tasks.Count);

      //Reset scheduled tasks in result
      foreach (Job j in jobs) {
        foreach (Task t in j.Tasks) {
          t.IsScheduled = false;
        }
      }

      //GT-Algorithm
      //STEP 0 - Compute a list of "earliest operations"
      ItemList<Task> earliestTasksList = GTAlgorithmUtils.GetEarliestNotScheduledTasks(jobs);
      while (earliestTasksList.Count > 0) {
        //STEP 1 - Get earliest not scheduled operation with minimal earliest completing time
        Task minimal = GTAlgorithmUtils.GetTaskWithMinimalEC(earliestTasksList, resultingSchedule);
        int conflictedResourceNr = minimal.ResourceNr;
        Resource conflictedResource = resultingSchedule.Resources[conflictedResourceNr];

        //STEP 2 - Compute a conflict set of all operations that can be scheduled on the conflicted resource
        ItemList<Task> conflictSet = GTAlgorithmUtils.GetConflictSetForTask(minimal, earliestTasksList, resultingSchedule);

        //STEP 3 - Select a task from the conflict set
        int progressOnResource = conflictedResource.Tasks.Count;
        Task selectedTask = SelectTaskFromConflictSet(solution, decodingErrorPolicy, forcingStrategy, conflictedResourceNr, progressOnResource, conflictSet, random);

        //STEP 4 - Add the selected task to the current schedule 
        selectedTask.IsScheduled = true;
        double startTime = GTAlgorithmUtils.ComputeEarliestStartTime(selectedTask, resultingSchedule);
        resultingSchedule.ScheduleTask(selectedTask.ResourceNr, startTime, selectedTask.Duration, selectedTask.JobNr);

        //STEP 5 - Back to STEP 1
        earliestTasksList = GTAlgorithmUtils.GetEarliestNotScheduledTasks(jobs);
      }

      return resultingSchedule;
    }
  }
}
