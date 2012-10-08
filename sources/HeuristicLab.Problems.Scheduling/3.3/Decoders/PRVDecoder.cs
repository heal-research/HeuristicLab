#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.ScheduleEncoding;
using HeuristicLab.Encodings.ScheduleEncoding.PriorityRulesVector;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Scheduling {
  [Item("JobSequencingMatrixDecoder", "Applies the GifflerThompson algorithm to create an active schedule from a JobSequencing Matrix.")]
  [StorableClass]
  public class PRVDecoder : ScheduleDecoder<PRVEncoding>, IStochasticOperator, IJSSPOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<ItemList<Job>> JobDataParameter {
      get { return (LookupParameter<ItemList<Job>>)Parameters["JobData"]; }
    }

    #region Private Members
    [Storable]
    private Schedule resultingSchedule;

    [Storable]
    private ItemList<Job> jobs;
    #endregion

    #region Priority Rules
    //smallest number of remaining tasks
    private Task FILORule(ItemList<Task> tasks) {
      Task currentResult = tasks[tasks.Count - 1];
      return currentResult;
    }

    //earliest start time
    private Task ESTRule(ItemList<Task> tasks) {
      Task currentResult = RandomRule(tasks);
      double currentEST = double.MaxValue;
      foreach (Task t in tasks) {
        double est = GTAlgorithmUtils.ComputeEarliestStartTime(t, resultingSchedule);
        if (est < currentEST) {
          currentEST = est;
          currentResult = t;
        }
      }
      return currentResult;
    }

    //shortest processingtime
    private Task SPTRule(ItemList<Task> tasks) {
      Task currentResult = RandomRule(tasks);
      foreach (Task t in tasks) {
        if (t.Duration < currentResult.Duration)
          currentResult = t;
      }
      return currentResult;
    }

    //longest processing time    
    private Task LPTRule(ItemList<Task> tasks) {
      Task currentResult = RandomRule(tasks);
      foreach (Task t in tasks) {
        if (t.Duration > currentResult.Duration)
          currentResult = t;
      }
      return currentResult;
    }

    //most work remaining
    private Task MWRRule(ItemList<Task> tasks) {
      Task currentResult = RandomRule(tasks);
      double currentLargestRemainingProcessingTime = 0;
      foreach (Task t in tasks) {
        double remainingProcessingTime = 0;
        foreach (Task jt in jobs[t.JobNr].Tasks) {
          if (!jt.IsScheduled)
            remainingProcessingTime += jt.Duration;
        }
        if (remainingProcessingTime > currentLargestRemainingProcessingTime) {
          currentLargestRemainingProcessingTime = remainingProcessingTime;
          currentResult = t;
        }
      }
      return currentResult;
    }

    //least work remaining
    private Task LWRRule(ItemList<Task> tasks) {
      Task currentResult = RandomRule(tasks);
      double currentSmallestRemainingProcessingTime = double.MaxValue;
      foreach (Task t in tasks) {
        double remainingProcessingTime = 0;
        foreach (Task jt in jobs[t.JobNr].Tasks) {
          if (!jt.IsScheduled)
            remainingProcessingTime += jt.Duration;
        }
        if (remainingProcessingTime < currentSmallestRemainingProcessingTime) {
          currentSmallestRemainingProcessingTime = remainingProcessingTime;
          currentResult = t;
        }
      }
      return currentResult;
    }

    //most operations remaining
    private Task MORRule(ItemList<Task> tasks) {
      Task currentResult = RandomRule(tasks);
      int currentLargestNrOfRemainingTasks = 0;
      foreach (Task t in tasks) {
        int nrOfRemainingTasks = 0;
        foreach (Task jt in jobs[t.JobNr].Tasks) {
          if (!jt.IsScheduled)
            nrOfRemainingTasks++;
        }
        if (currentLargestNrOfRemainingTasks < nrOfRemainingTasks) {
          currentLargestNrOfRemainingTasks = nrOfRemainingTasks;
          currentResult = t;
        }
      }
      return currentResult;
    }

    //least operationsremaining
    private Task LORRule(ItemList<Task> tasks) {
      Task currentResult = RandomRule(tasks);
      int currentSmallestNrOfRemainingTasks = int.MaxValue;
      foreach (Task t in tasks) {
        int nrOfRemainingTasks = 0;
        foreach (Task jt in jobs[t.JobNr].Tasks) {
          if (!jt.IsScheduled)
            nrOfRemainingTasks++;
        }
        if (currentSmallestNrOfRemainingTasks > nrOfRemainingTasks) {
          currentSmallestNrOfRemainingTasks = nrOfRemainingTasks;
          currentResult = t;
        }
      }
      return currentResult;
    }

    //first operation in Queue
    private Task FIFORule(ItemList<Task> tasks) {
      Task currentResult = tasks[0];
      return currentResult;
    }

    //random
    private Task RandomRule(ItemList<Task> tasks) {
      Task currentResult = tasks[RandomParameter.ActualValue.Next(tasks.Count)];
      return currentResult;
    }

    #endregion

    [StorableConstructor]
    protected PRVDecoder(bool deserializing) : base(deserializing) { }
    protected PRVDecoder(PRVDecoder original, Cloner cloner)
      : base(original, cloner) {
      this.resultingSchedule = cloner.Clone(original.resultingSchedule);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PRVDecoder(this, cloner);
    }

    public PRVDecoder()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
      Parameters.Add(new LookupParameter<ItemList<Job>>("JobData", "Job data taken from the SchedulingProblem - Instance."));
      ScheduleEncodingParameter.ActualName = "PriorityRulesVector";
    }

    private Task SelectTaskFromConflictSet(ItemList<Task> conflictSet, int ruleIndex, int nrOfRules) {
      if (conflictSet.Count == 1)
        return conflictSet[0];

      ruleIndex = ruleIndex % nrOfRules;
      switch (ruleIndex) {
        case 0: return FILORule(conflictSet);
        case 1: return ESTRule(conflictSet);
        case 2: return SPTRule(conflictSet);
        case 3: return LPTRule(conflictSet);
        case 4: return MWRRule(conflictSet);
        case 5: return LWRRule(conflictSet);
        case 6: return MORRule(conflictSet);
        case 7: return LORRule(conflictSet);
        case 8: return FIFORule(conflictSet);
        case 9: return RandomRule(conflictSet);
        default: return RandomRule(conflictSet);
      }
    }

    public override Schedule CreateScheduleFromEncoding(PRVEncoding solution) {
      jobs = (ItemList<Job>)JobDataParameter.ActualValue.Clone();
      resultingSchedule = new Schedule(jobs[0].Tasks.Count);

      //Reset scheduled tasks in result
      foreach (Job j in jobs) {
        foreach (Task t in j.Tasks) {
          t.IsScheduled = false;
        }
      }

      //GT-Algorithm
      //STEP 0 - Compute a list of "earliest operations"
      ItemList<Task> earliestTasksList = GTAlgorithmUtils.GetEarliestNotScheduledTasks(jobs);
      //int currentDecisionIndex = 0;
      while (earliestTasksList.Count > 0) {
        //STEP 1 - Get earliest not scheduled operation with minimal earliest completing time
        Task minimal = GTAlgorithmUtils.GetTaskWithMinimalEC(earliestTasksList, resultingSchedule);

        //STEP 2 - Compute a conflict set of all operations that can be scheduled on the machine the previously selected operation runs on
        ItemList<Task> conflictSet = GTAlgorithmUtils.GetConflictSetForTask(minimal, earliestTasksList, jobs, resultingSchedule);

        //STEP 3 - Select an operation from the conflict set (various methods depending on how the algorithm should work..)
        //Task selectedTask = SelectTaskFromConflictSet(conflictSet, solution.PriorityRulesVector [currentDecisionIndex++], solution.NrOfRules.Value);
        Task selectedTask = SelectTaskFromConflictSet(conflictSet, solution.PriorityRulesVector[minimal.JobNr], solution.NrOfRules.Value);

        //STEP 4 - Adding the selected operation to the current schedule
        selectedTask.IsScheduled = true;
        double startTime = GTAlgorithmUtils.ComputeEarliestStartTime(selectedTask, resultingSchedule);
        resultingSchedule.ScheduleTask(selectedTask.ResourceNr, startTime, selectedTask.Duration, selectedTask.JobNr);

        //STEP 5 - Back to STEP 1
        earliestTasksList = GTAlgorithmUtils.GetEarliestNotScheduledTasks(jobs);
      }

      return resultingSchedule;
    }

    public override IOperation Apply() {
      return base.Apply();
    }
  }
}
