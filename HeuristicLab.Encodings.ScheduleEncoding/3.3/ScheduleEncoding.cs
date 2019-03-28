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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [StorableType("D2FB1AF9-EF13-4ED2-B3E9-D5BE4E5772EA")]
  public abstract class ScheduleEncoding<TSchedule> : Encoding<TSchedule>, IScheduleEncoding
  where TSchedule : class, ISchedule {
    #region Encoding Parameters
    [Storable]
    private IFixedValueParameter<ItemList<Job>> jobDataParameter;
    public IFixedValueParameter<ItemList<Job>> JobDataParameter {
      get { return jobDataParameter; }
      set {
        if (value == null) throw new ArgumentNullException("JobData parameter must not be null.");
        if (value.Value == null) throw new ArgumentNullException("JobData parameter value must not be null.");
        if (jobDataParameter == value) return;

        if (jobDataParameter != null) Parameters.Remove(jobDataParameter);
        jobDataParameter = value;
        Parameters.Add(jobDataParameter);
        OnJobDataParameterChanged();
      }
    }

    [Storable]
    private IFixedValueParameter<IntValue> jobsParameter;
    public IFixedValueParameter<IntValue> JobsParameter {
      get { return jobsParameter; }
      set {
        if (value == null) throw new ArgumentNullException("Jobs parameter must not be null.");
        if (value.Value == null) throw new ArgumentNullException("Jobs parameter value must not be null.");
        if (jobsParameter == value) return;

        if (jobsParameter != null) Parameters.Remove(jobsParameter);
        jobsParameter = value;
        Parameters.Add(jobsParameter);
        OnJobsParameterChanged();
      }
    }
    [Storable]
    private IFixedValueParameter<IntValue> resourcesParameter;
    public IFixedValueParameter<IntValue> ResourcesParameter {
      get { return resourcesParameter; }
      set {
        if (value == null) throw new ArgumentNullException("Resources parameter must not be null.");
        if (value.Value == null) throw new ArgumentNullException("Resources parameter value must not be null.");
        if (resourcesParameter == value) return;

        if (resourcesParameter != null) Parameters.Remove(resourcesParameter);
        resourcesParameter = value;
        Parameters.Add(resourcesParameter);
        OnBoundsParameterChanged();
      }
    }

    [Storable]
    private IValueParameter<IScheduleDecoder<TSchedule>> decoderParameter;
    public IValueParameter<IScheduleDecoder<TSchedule>> DecoderParameter {
      get { return decoderParameter; }
      set {
        if (value == null) throw new ArgumentNullException("Decoder parameter must not be null.");
        if (value.Value == null) throw new ArgumentNullException("Decoder parameter value must not be null.");
        if (decoderParameter == value) return;

        if (decoderParameter != null) Parameters.Remove(decoderParameter);
        decoderParameter = value;
        Parameters.Add(decoderParameter);
        OnDecoderParameterChanged();
      }
    }
    #endregion

    public ItemList<Job> JobData {
      get { return JobDataParameter.Value; }
    }
    public int Jobs {
      get { return JobsParameter.Value.Value; }
      set { JobsParameter.Value.Value = value; }
    }
    public int Resources {
      get { return ResourcesParameter.Value.Value; }
      set { ResourcesParameter.Value.Value = value; }
    }

    public IScheduleDecoder<TSchedule> Decoder {
      get { return DecoderParameter.Value; }
      set { DecoderParameter.Value = value; }
    }

    [StorableConstructor]
    protected ScheduleEncoding(StorableConstructorFlag _) : base(_) { }
    protected ScheduleEncoding(ScheduleEncoding<TSchedule> original, Cloner cloner)
      : base(original, cloner) {
      jobDataParameter = cloner.Clone(original.JobDataParameter);
      jobsParameter = cloner.Clone(original.JobsParameter);
      resourcesParameter = cloner.Clone(original.ResourcesParameter);
      decoderParameter = cloner.Clone(original.DecoderParameter);
    }

    protected ScheduleEncoding() : this("Schedule") { }
    protected ScheduleEncoding(string name) : this(name, Enumerable.Empty<Job>()) { }
    protected ScheduleEncoding(string name, IEnumerable<Job> jobData)
      : base(name) {
      int jobs = jobData.Count();
      int resources = jobData.SelectMany(j => j.Tasks).Select(t => t.ResourceNr).Distinct().Count();

      jobDataParameter = new FixedValueParameter<ItemList<Job>>(Name + ".JobData", new ItemList<Job>(jobData));
      jobsParameter = new FixedValueParameter<IntValue>(Name + ".Jobs", new IntValue(jobs));
      resourcesParameter = new FixedValueParameter<IntValue>(Name + ".Resources", new IntValue(resources));
      decoderParameter = new ValueParameter<IScheduleDecoder<TSchedule>>(Name + ".Decoder");

      Parameters.Add(jobDataParameter);
      Parameters.Add(jobsParameter);
      Parameters.Add(resourcesParameter);
      Parameters.Add(decoderParameter);
    }

    public Schedule Decode(ISchedule schedule, ItemList<Job> jobData) {
      return Decoder.DecodeSchedule(schedule, jobData);
    }

    private void OnJobDataParameterChanged() {
      ConfigureOperators(Operators);
    }
    private void OnJobsParameterChanged() {
      ConfigureOperators(Operators);
    }
    private void OnBoundsParameterChanged() {
      ConfigureOperators(Operators);
    }

    private void OnDecoderParameterChanged() {
      ConfigureOperators(Operators);
    }


    public override void ConfigureOperators(IEnumerable<IItem> operators) {
      ConfigureCreators(operators.OfType<IScheduleCreator<TSchedule>>());
      ConfigureCrossovers(operators.OfType<IScheduleCrossover>());
      ConfigureManipulators(operators.OfType<IScheduleManipulator>());
    }

    private void ConfigureCreators(IEnumerable<IScheduleCreator<TSchedule>> creators) {
      foreach (var creator in creators) {
        creator.ScheduleParameter.ActualName = Name;
        creator.JobsParameter.ActualName = JobsParameter.Name;
        creator.ResourcesParameter.ActualName = ResourcesParameter.Name;
      }
    }

    private void ConfigureCrossovers(IEnumerable<IScheduleCrossover> crossovers) {
      foreach (var crossover in crossovers) {
        crossover.ChildParameter.ActualName = Name;
        crossover.ParentsParameter.ActualName = Name;
      }
    }

    private void ConfigureManipulators(IEnumerable<IScheduleManipulator> manipulators) {
      foreach (var manipulator in manipulators)
        manipulator.ScheduleParameter.ActualName = Name;
    }
  }
}
