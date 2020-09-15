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
using System.Drawing;
using System.Linq;
using System.Threading;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Encodings.ScheduleEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.Scheduling {
  public enum JSSPObjective { Makespan, Tardiness }

  [Item("Job Shop Scheduling Problem (JSSP)", "Represents a standard Job Shop Scheduling Problem")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 120)]
  [StorableType("d6c72bd1-c6cc-4efb-9cc5-b73bb3845799")]
  public sealed class JobShopSchedulingProblem : SingleObjectiveProblem<IScheduleEncoding, IScheduleSolution>, IProblemInstanceConsumer<JSSPData>, IProblemInstanceExporter<JSSPData>, IStorableContent {
    #region Default Instance
    private static readonly JSSPData DefaultInstance = new JSSPData() {
      Jobs = 10,
      Resources = 10,
      BestKnownQuality = 930,
      ProcessingTimes = new double[,] {
          { 29, 78,  9, 36, 49, 11, 62, 56, 44, 21 },
          { 43, 90, 75, 11, 69, 28, 46, 46, 72, 30 },
          { 91, 85, 39, 74, 90, 10, 12, 89, 45, 33 },
          { 81, 95, 71, 99,  9, 52, 85, 98, 22, 43 },
          { 14,  6, 22, 61, 26, 69, 21, 49, 72, 53 },
          { 84,  2, 52, 95, 48, 72, 47, 65,  6, 25 },
          { 46, 37, 61, 13, 32, 21, 32, 89, 30, 55 },
          { 31, 86, 46, 74, 32, 88, 19, 48, 36, 79 },
          { 76, 69, 76, 51, 85, 11, 40, 89, 26, 74 },
          { 85, 13, 61,  7, 64, 76, 47, 52, 90, 45 }
        },
      Demands = new int[,] {
          { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
          { 0, 2, 4, 9, 3, 1, 6, 5, 7, 8 },
          { 1, 0, 3, 2, 8, 5, 7, 6, 9, 4 },
          { 1, 2, 0, 4, 6, 8, 7, 3, 9, 5 },
          { 2, 0, 1, 5, 3, 4, 8, 7, 9, 6 },
          { 2, 1, 5, 3, 8, 9, 0, 6, 4, 7 },
          { 1, 0, 3, 2, 6, 5, 9, 8, 7, 4 },
          { 2, 0, 1, 5, 4, 6, 8, 9, 7, 3 },
          { 0, 1, 3, 5, 2, 9, 6, 7, 4, 8 },
          { 1, 0, 2, 6, 8, 9, 5, 3, 4, 7 }
        }
    };
    #endregion

    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Type; }
    }

    #region Parameter Properties
    [Storable] public ReferenceParameter<ItemList<Job>> JobDataParameter { get; private set; }
    [Storable] public OptionalValueParameter<Schedule> BestKnownSolutionParameter { get; private set; }
    [Storable] public ReferenceParameter<IntValue> JobsParameter { get; private set; }
    [Storable] public ReferenceParameter<IntValue> ResourcesParameter { get; private set; }
    [Storable] public IFixedValueParameter<EnumValue<JSSPObjective>> ObjectiveParameter { get; private set; }
    #endregion

    #region Properties
    public ItemList<Job> JobData {
      get { return JobDataParameter.Value; }
      set { JobDataParameter.Value = value; }
    }
    public Schedule BestKnownSolution {
      get { return BestKnownSolutionParameter.Value; }
      set { BestKnownSolutionParameter.Value = value; }
    }
    public int Jobs {
      get { return JobsParameter.Value.Value; }
      set { JobsParameter.Value.Value = value; }
    }
    public int Resources {
      get { return ResourcesParameter.Value.Value; }
      set { ResourcesParameter.Value.Value = value; }
    }
    public JSSPObjective Objective {
      get { return ObjectiveParameter.Value.Value; }
      set { ObjectiveParameter.Value.Value = value; }
    }

    public new IScheduleEncoding Encoding {
      get { return base.Encoding; }
      set { base.Encoding = value; }
    }
    #endregion

    [StorableConstructor]
    private JobShopSchedulingProblem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private JobShopSchedulingProblem(JobShopSchedulingProblem original, Cloner cloner)
      : base(original, cloner) {
      JobDataParameter = cloner.Clone(original.JobDataParameter);
      BestKnownSolutionParameter = cloner.Clone(original.BestKnownSolutionParameter);
      JobsParameter = cloner.Clone(original.JobsParameter);
      ResourcesParameter = cloner.Clone(original.ResourcesParameter);
      ObjectiveParameter = cloner.Clone(original.ObjectiveParameter);

      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new JobShopSchedulingProblem(this, cloner);
    }

    public JobShopSchedulingProblem()
      : base(new JobSequenceMatrixEncoding()) {
      Parameters.Add(JobDataParameter = new ReferenceParameter<ItemList<Job>>("JobData", "Jobdata defining the precedence relationships and the duration of the tasks in this JSSP-Instance.", Encoding.JobDataParameter));
      Parameters.Add(BestKnownSolutionParameter = new OptionalValueParameter<Schedule>("BestKnownSolution", "The best known solution of this JSSP instance."));
      Parameters.Add(JobsParameter = new ReferenceParameter<IntValue>("Jobs", "The number of jobs used in this JSSP instance.", Encoding.JobsParameter));
      Parameters.Add(ResourcesParameter = new ReferenceParameter<IntValue>("Resources", "The number of resources used in this JSSP instance.", Encoding.ResourcesParameter));
      Parameters.Add(ObjectiveParameter = new FixedValueParameter<EnumValue<JSSPObjective>>("Objective", "The objective to use in the evaluation of a schedule.", new EnumValue<JSSPObjective>(JSSPObjective.Makespan)));
      EncodingParameter.Hidden = false;

      RegisterEventHandlers();
      Load(DefaultInstance);
    }

    public override ISingleObjectiveEvaluationResult Evaluate(IScheduleSolution solution, IRandom random, CancellationToken cancellationToken) {
      var schedule = Encoding.Decode(solution, JobData);
      switch (Objective) {
        case JSSPObjective.Makespan:
          return new SingleObjectiveEvaluationResult(Makespan.Calculate(schedule));
        case JSSPObjective.Tardiness:
          return new SingleObjectiveEvaluationResult(MeanTardiness.Calculate(schedule, JobData));
        default:
          throw new InvalidOperationException("Objective " + Objective + " unknown");
      }
    }

    public override void Analyze(ISingleObjectiveSolutionContext<IScheduleSolution>[] solutionContexts, IRandom random) {
      base.Analyze(solutionContexts, random);

      //TODO: reimplement code below using results directly

      //bool max = Maximization;

      //int i = -1;
      //if (!max)
      //  i = qualities.Select((x, index) => new { index, x }).OrderBy(x => x.x).First().index;
      //else i = qualities.Select((x, index) => new { index, x }).OrderByDescending(x => x.x).First().index;

      //if (double.IsNaN(BestKnownQuality) ||
      //    max && qualities[i] > BestKnownQuality ||
      //    !max && qualities[i] < BestKnownQuality) {
      //  BestKnownQuality = qualities[i];
      //  BestKnownSolution = Encoding.Decode(solutions[i], JobData);
      //}
      //Schedule bestSolution;
      //if (results.TryGetValue("Best Scheduling Solution", out var result)) {
      //  bestSolution = result.Value as Schedule;
      //} else bestSolution = null;

      //if (bestSolution == null || IsBetter(bestSolution.Quality, qualities[i]))
      //  results.AddOrUpdateResult("Best Scheduling Solution", Encoding.Decode(solutions[i], JobData));
    }

    protected override void OnEncodingChanged() {
      base.OnEncodingChanged();
      var jobData = (ItemList<Job>)JobData.Clone();
      var jobs = Jobs;
      var resources = Resources;
      Parameters.Remove(JobDataParameter);
      Parameters.Add(JobDataParameter = new ReferenceParameter<ItemList<Job>>("JobData", "Jobdata defining the precedence relationships and the duration of the tasks in this JSSP-Instance.", Encoding.JobDataParameter));
      Parameters.Remove(JobsParameter);
      Parameters.Add(JobsParameter = new ReferenceParameter<IntValue>("Jobs", "The number of jobs used in this JSSP instance.", Encoding.JobsParameter));
      Parameters.Remove(ResourcesParameter);
      Parameters.Add(ResourcesParameter = new ReferenceParameter<IntValue>("Resources", "The number of resources used in this JSSP instance.", Encoding.ResourcesParameter));
      JobData = jobData;
      Jobs = jobs;
      Resources = resources;

      RegisterEventHandlers();
    }


    private void RegisterEventHandlers() {
      ItemListParameterChangeHandler<Job>.Create(JobDataParameter, JobDataOnChanged);
    }

    private void JobDataOnChanged() {
      Jobs = JobData.Count;
      Resources = JobData.SelectMany(x => x.Tasks).Select(x => x.ResourceNr).Distinct().Count();
    }


    #region Problem Instance Handling
    public void Load(JSSPData data) {
      var jobData = new ItemList<Job>(data.Jobs);
      for (int j = 0; j < data.Jobs; j++) {
        var job = new Job(j, data.DueDates != null ? data.DueDates[j] : double.MaxValue);
        for (int t = 0; t < data.Resources; t++) {
          job.Tasks.Add(new Task(t, data.Demands[j, t], j, data.ProcessingTimes[j, t]));
        }
        jobData.Add(job);
      }

      BestKnownQuality = data.BestKnownQuality ?? double.NaN;
      if (data.BestKnownSchedule != null) {
        var enc = new JSM(0);
        for (int i = 0; i < data.Resources; i++) {
          enc.JobSequenceMatrix.Add(new Permutation(PermutationTypes.Absolute, new int[data.Jobs]));
          for (int j = 0; j < data.Jobs; j++) {
            enc.JobSequenceMatrix[i][j] = data.BestKnownSchedule[i, j];
          }
        }
        BestKnownSolution = JSMDecoder.Decode(enc, jobData, JSMDecodingErrorPolicy.RandomPolicy, JSMForcingStrategy.SwapForcing);
        switch (Objective) {
          case JSSPObjective.Makespan:
            BestKnownQuality = Makespan.Calculate(BestKnownSolution);
            break;
          case JSSPObjective.Tardiness:
            BestKnownQuality = MeanTardiness.Calculate(BestKnownSolution, jobData);
            break;
        }
      }

      JobData = jobData;
      Resources = data.Resources;
    }

    public JSSPData Export() {
      var result = new JSSPData {
        Name = Name,
        Description = Description,
        Jobs = Jobs,
        Resources = Resources,
        ProcessingTimes = new double[Jobs, Resources],
        Demands = new int[Jobs, Resources],
        DueDates = new double[Jobs]
      };

      foreach (var job in JobData) {
        var counter = 0;
        result.DueDates[job.Index] = job.DueDate;
        foreach (var task in job.Tasks) {
          result.ProcessingTimes[task.JobNr, counter] = task.Duration;
          result.Demands[task.JobNr, counter] = task.ResourceNr;
          counter++;
        }
      }
      return result;
    }
    #endregion

  }
}
