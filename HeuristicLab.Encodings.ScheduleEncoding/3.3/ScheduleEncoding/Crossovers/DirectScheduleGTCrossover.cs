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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.ScheduleEncoding.ScheduleEncoding {
  [Item("DirectScheduleGTCrossover", "Represents a crossover using the GT-Algorithm to cross two direct schedule representations.")]
  [StorableClass]
  public class DirectScheduleGTCrossover : DirectScheduleCrossover {

    public IValueLookupParameter<DoubleValue> MutationProbabilityParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["MutationProbability"]; }
    }

    [StorableConstructor]
    protected DirectScheduleGTCrossover(bool deserializing) : base(deserializing) { }
    protected DirectScheduleGTCrossover(DirectScheduleGTCrossover original, Cloner cloner) : base(original, cloner) { }
    public DirectScheduleGTCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MutationProbability", "The probability that a task from the conflict set is chosen randomly instead of from one of the parents."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DirectScheduleGTCrossover(this, cloner);
    }

    public static Schedule Apply(IRandom random, Schedule parent1, Schedule parent2, ItemList<Job> jobData, double mutProp) {
      var child = new Schedule(parent1.Resources.Count);

      //Reset scheduled tasks in result
      foreach (Job j in jobData) {
        foreach (Task t in j.Tasks) {
          t.IsScheduled = false;
        }
      }

      //GT-Algorithm
      //STEP 0 - Compute a list of "earliest operations"
      ItemList<Task> earliestTasksList = GTAlgorithmUtils.GetEarliestNotScheduledTasks(jobData);
      while (earliestTasksList.Count > 0) {
        //STEP 1 - Get earliest not scheduled operation with minimal earliest completing time
        Task minimal = GTAlgorithmUtils.GetTaskWithMinimalEC(earliestTasksList, child);
        int conflictedResourceNr = minimal.ResourceNr;

        //STEP 2 - Compute a conflict set of all operations that can be scheduled on the conflicted resource
        ItemList<Task> conflictSet = GTAlgorithmUtils.GetConflictSetForTask(minimal, earliestTasksList, child);

        //STEP 3 - Select a task from the conflict set
        Task selectedTask = null;
        if (random.NextDouble() < mutProp) {
          //Mutation
          selectedTask = conflictSet[random.Next(conflictSet.Count)];
        } else {
          //Crossover
          selectedTask = SelectTaskFromConflictSet(conflictSet, ((random.Next(2) == 0) ? parent1 : parent2), conflictedResourceNr);
        }

        //STEP 4 - Add the selected task to the current schedule 
        selectedTask.IsScheduled = true;
        double startTime = GTAlgorithmUtils.ComputeEarliestStartTime(selectedTask, child);
        child.ScheduleTask(selectedTask.ResourceNr, startTime, selectedTask.Duration, selectedTask.JobNr);

        //STEP 5 - Back to STEP 1
        earliestTasksList = GTAlgorithmUtils.GetEarliestNotScheduledTasks(jobData);
      }

      return child;
    }

    private static Task SelectTaskFromConflictSet(ItemList<Task> conflictSet, Schedule usedParent, int conflictedResourceNr) {
      //Apply Crossover
      foreach (ScheduledTask st in usedParent.Resources[conflictedResourceNr].Tasks) {
        foreach (Task t in conflictSet) {
          if (st.JobNr == t.JobNr)
            return t;
        }
      }
      return conflictSet[0];
    }


    public override Schedule Cross(IRandom random, Schedule parent1, Schedule parent2) {
      var jobData = (ItemList<Job>)JobDataParameter.ActualValue.Clone();
      var mutProp = MutationProbabilityParameter.ActualValue;
      return Apply(random, parent1, parent2, jobData, mutProp.Value);
    }
  }
}
