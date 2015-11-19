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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.ScheduleEncoding.PermutationWithRepetition;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;


namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("DirectScheduleRandomCreator", "Creator class used to create schedule encoding objects.")]
  [StorableClass]
  public class DirectScheduleRandomCreator : ScheduleCreator, IStochasticOperator {

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public IValueLookupParameter<IntValue> JobsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["Jobs"]; }
    }
    public IValueLookupParameter<IntValue> ResourcesParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["Resources"]; }
    }
    public ILookupParameter<ItemList<Job>> JobDataParameter {
      get { return (LookupParameter<ItemList<Job>>)Parameters["JobData"]; }
    }


    [StorableConstructor]
    protected DirectScheduleRandomCreator(bool deserializing) : base(deserializing) { }
    protected DirectScheduleRandomCreator(DirectScheduleRandomCreator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new DirectScheduleRandomCreator(this, cloner);
    }
    public DirectScheduleRandomCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Jobs", "The number of jobs handled in this problem instance."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Resources", "The number of resources used in this problem instance."));
      Parameters.Add(new LookupParameter<ItemList<Job>>("JobData", "Job data taken from the JSSP - Instance."));
      ScheduleEncodingParameter.ActualName = "Schedule";
    }


    public static Schedule Apply(int jobs, int resources, PWREncoding pwr, ItemList<Job> jobData) {
      var resultingSchedule = new Schedule(jobData[0].Tasks.Count);
      foreach (int jobNr in pwr.PermutationWithRepetition) {
        int i = 0;
        while (jobData[jobNr].Tasks[i].IsScheduled) i++;
        Task currentTask = jobData[jobNr].Tasks[i];
        double startTime = GTAlgorithmUtils.ComputeEarliestStartTime(currentTask, resultingSchedule);
        currentTask.IsScheduled = true;
        resultingSchedule.ScheduleTask(currentTask.ResourceNr, startTime, currentTask.Duration, currentTask.JobNr);
      }
      return resultingSchedule;
    }



    protected override IScheduleEncoding CreateSolution() {
      try {
        var jobData = (ItemList<Job>)JobDataParameter.ActualValue.Clone();
        return Apply(JobsParameter.ActualValue.Value,
          ResourcesParameter.ActualValue.Value,
          new PWREncoding(JobsParameter.ActualValue.Value, ResourcesParameter.ActualValue.Value, RandomParameter.ActualValue),
          jobData);
      } catch {
        throw new Exception("ScheduleRandomCreator needs JobData parameter from a JSSP-Instance to create Schedule-Instances!");
      }
    }
  }
}
