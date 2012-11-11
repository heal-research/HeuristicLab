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

using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Encodings.ScheduleEncoding;
using HeuristicLab.Encodings.ScheduleEncoding.JobSequenceMatrix;
using HeuristicLab.Encodings.ScheduleEncoding.PermutationWithRepetition;
using HeuristicLab.Encodings.ScheduleEncoding.PriorityRulesVector;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.Scheduling {
  [Item("Job Shop Scheduling Problem", "Represents a standard Job Shop Scheduling Problem")]
  [Creatable("Problems")]
  [StorableClass]
  public sealed class JobShopSchedulingProblem : SchedulingProblem, IProblemInstanceConsumer<JSSPData>, IProblemInstanceExporter<JSSPData>, IStorableContent {
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

    #region Parameter Properties
    public ValueParameter<ItemList<Job>> JobDataParameter {
      get { return (ValueParameter<ItemList<Job>>)Parameters["JobData"]; }
    }
    public OptionalValueParameter<Schedule> BestKnownSolutionParameter {
      get { return (OptionalValueParameter<Schedule>)Parameters["BestKnownSolution"]; }
    }

    public IFixedValueParameter<IntValue> JobsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Jobs"]; }
    }
    public IFixedValueParameter<IntValue> ResourcesParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Resources"]; }
    }
    public ValueParameter<SchedulingEvaluator> SolutionEvaluatorParameter {
      get { return (ValueParameter<SchedulingEvaluator>)Parameters["SolutionEvaluator"]; }
    }
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
    public SchedulingEvaluator SolutionEvaluator {
      get { return SolutionEvaluatorParameter.Value; }
      set { SolutionEvaluatorParameter.Value = value; }
    }
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Type; }
    }
    public string Filename { get; set; }
    #endregion

    public JobShopSchedulingProblem()
      : base(new SchedulingEvaluationAlgorithm(), new JSMRandomCreator()) {
      Parameters.Add(new ValueParameter<ItemList<Job>>("JobData", "Jobdata defining the precedence relationships and the duration of the tasks in this JSSP-Instance.", new ItemList<Job>()));
      Parameters.Add(new OptionalValueParameter<Schedule>("BestKnownSolution", "The best known solution of this JSSP instance."));

      Parameters.Add(new FixedValueParameter<IntValue>("Jobs", "The number of jobs used in this JSSP instance.", new IntValue()));
      Parameters.Add(new FixedValueParameter<IntValue>("Resources", "The number of resources used in this JSSP instance.", new IntValue()));
      Parameters.Add(new ValueParameter<SchedulingEvaluator>("SolutionEvaluator", "The evaluator used to determine the quality of a solution.", new MakespanEvaluator()));

      InitializeOperators();
      Load(DefaultInstance);
    }

    [StorableConstructor]
    private JobShopSchedulingProblem(bool deserializing) : base(deserializing) { }
    private JobShopSchedulingProblem(JobShopSchedulingProblem original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new JobShopSchedulingProblem(this, cloner);
    }

    #region Events
    protected override void OnSolutionCreatorChanged() {
      InitializeOperators();
    }
    #endregion

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

      if (data.BestKnownQuality.HasValue) BestKnownQuality = new DoubleValue(data.BestKnownQuality.Value);
      else BestKnownQuality = null;
      if (data.BestKnownSchedule != null) {
        var enc = new JSMEncoding();
        enc.JobSequenceMatrix = new ItemList<Permutation>(data.Resources);
        for (int i = 0; i < data.Resources; i++) {
          enc.JobSequenceMatrix[i] = new Permutation(PermutationTypes.Absolute, new int[data.Jobs]);
          for (int j = 0; j < data.Jobs; j++) {
            enc.JobSequenceMatrix[i][j] = data.BestKnownSchedule[i, j];
          }
        }
        BestKnownSolution = new JSMDecoder().CreateScheduleFromEncoding(enc, jobData);
        if (SolutionEvaluator is MeanTardinessEvaluator)
          BestKnownQuality = new DoubleValue(MeanTardinessEvaluator.GetMeanTardiness(BestKnownSolution, jobData));
        else if (SolutionEvaluator is MakespanEvaluator)
          BestKnownQuality = new DoubleValue(MakespanEvaluator.GetMakespan(BestKnownSolution));
      }

      JobData = jobData;
      Jobs = data.Jobs;
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

    #region Helpers
    private void InitializeOperators() {
      Operators.Clear();
      ApplyEncoding();
      Operators.Add(new BestSchedulingSolutionAnalyzer());
    }

    private void ApplyEncoding() {
      if (SolutionCreator.GetType() == typeof(JSMRandomCreator)) {
        Operators.AddRange(ApplicationManager.Manager.GetInstances<IJSMOperator>());
        var decoder = new JSMDecoder();
        ((SchedulingEvaluationAlgorithm)EvaluatorParameter.ActualValue).InitializeOperatorGraph(decoder);
      } else if (SolutionCreator.GetType() == typeof(PRVRandomCreator)) {
        Operators.AddRange(ApplicationManager.Manager.GetInstances<IPRVOperator>());
        var decoder = new PRVDecoder();
        ((SchedulingEvaluationAlgorithm)EvaluatorParameter.ActualValue).InitializeOperatorGraph(decoder);
      } else if (SolutionCreator.GetType() == typeof(PWRRandomCreator)) {
        Operators.AddRange(ApplicationManager.Manager.GetInstances<IPWROperator>());
        var decoder = new PWRDecoder();
        ((SchedulingEvaluationAlgorithm)EvaluatorParameter.ActualValue).InitializeOperatorGraph(decoder);
      } else if (SolutionCreator.GetType() == typeof(DirectScheduleRandomCreator)) {
        Operators.AddRange(ApplicationManager.Manager.GetInstances<IDirectScheduleOperator>());
        ((SchedulingEvaluationAlgorithm)EvaluatorParameter.ActualValue).InitializeOperatorGraph<Schedule>();
      }
    }
    #endregion

  }
}
