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
  public abstract class ScheduleEncoding<TSchedule> : Encoding, IScheduleEncoding
  where TSchedule : class, IScheduleSolution {
    [Storable] public IValueParameter<ItemList<Job>> JobDataParameter { get; private set; }
    [Storable] public IFixedValueParameter<IntValue> JobsParameter { get; private set; }
    [Storable] public IFixedValueParameter<IntValue> ResourcesParameter { get; private set; }
    [Storable] public IValueParameter<IScheduleDecoder<TSchedule>> DecoderParameter { get; private set; }

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
      JobDataParameter = cloner.Clone(original.JobDataParameter);
      JobsParameter = cloner.Clone(original.JobsParameter);
      ResourcesParameter = cloner.Clone(original.ResourcesParameter);
      DecoderParameter = cloner.Clone(original.DecoderParameter);

      RegisterParameterEvents();
    }

    protected ScheduleEncoding(string name) : this(name, Enumerable.Empty<Job>()) { }
    protected ScheduleEncoding(string name, IEnumerable<Job> jobData)
      : base(name) {
      int jobs = jobData.Count();
      int resources = jobData.SelectMany(j => j.Tasks).Select(t => t.ResourceNr).Distinct().Count();

      Parameters.Add(JobDataParameter = new ValueParameter<ItemList<Job>>(Name + ".JobData", new ItemList<Job>(jobData)));
      Parameters.Add(JobsParameter = new FixedValueParameter<IntValue>(Name + ".Jobs", new IntValue(jobs)));
      Parameters.Add(ResourcesParameter = new FixedValueParameter<IntValue>(Name + ".Resources", new IntValue(resources)));
      Parameters.Add(DecoderParameter = new ValueParameter<IScheduleDecoder<TSchedule>>(Name + ".Decoder"));

      RegisterParameterEvents();
    }

    private void RegisterParameterEvents() {
      ItemListParameterChangeHandler<Job>.Create(JobDataParameter, () => {
        ConfigureOperators(Operators);
        OnJobDataChanged();
      });
      IntValueParameterChangeHandler.Create(JobsParameter, () => {
        ConfigureOperators(Operators);
        OnJobsChanged();
      });
      IntValueParameterChangeHandler.Create(ResourcesParameter, () => {
        ConfigureOperators(Operators);
        OnResourcesChanged();
      });
      ParameterChangeHandler<IScheduleDecoder<TSchedule>>.Create(DecoderParameter, () => {
        ConfigureOperators(Operators);
        OnDecoderChanged();
      });
    }

    public Schedule Decode(IScheduleSolution schedule, ItemList<Job> jobData) {
      return Decoder.DecodeSchedule(schedule, jobData);
    }

    public override void ConfigureOperators(IEnumerable<IItem> operators) {
      base.ConfigureOperators(operators);
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

    public event EventHandler JobDataChanged;
    protected virtual void OnJobDataChanged() => JobDataChanged?.Invoke(this, EventArgs.Empty);

    public event EventHandler JobsChanged;
    protected virtual void OnJobsChanged() => JobsChanged?.Invoke(this, EventArgs.Empty);
    
    public event EventHandler ResourcesChanged;
    protected virtual void OnResourcesChanged() => ResourcesChanged?.Invoke(this, EventArgs.Empty);

    public event EventHandler DecoderChanged;
    protected virtual void OnDecoderChanged() => DecoderChanged?.Invoke(this, EventArgs.Empty);
  }
}
