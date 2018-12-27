#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("Job Shop Scheduling Problem (JSSP)", "Represents a standard Job Shop Scheduling Problem")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 120)]
  [StorableClass]
  public sealed class JobShopSchedulingProblem : SchedulingProblem, IProblemInstanceConsumer<JSSPData>, IProblemInstanceExporter<JSSPData>, IStorableContent {
    #region Default Instance
    private static readonly JSSPData DefaultInstance = new JSSPData() {
      Name = "Job Shop Scheduling Problem (JSSP)",
      Description = "The default instance of the JSSP problem in HeuristicLab",
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

    public string Filename { get; set; }
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Type; }
    }

    #region Parameter Properties
    public IValueParameter<ItemList<Job>> JobDataParameter {
      get { return (IValueParameter<ItemList<Job>>)Parameters["JobData"]; }
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
    public IValueParameter<IScheduleEvaluator> ScheduleEvaluatorParameter {
      get { return (IValueParameter<IScheduleEvaluator>)Parameters["ScheduleEvaluator"]; }
    }
    public OptionalValueParameter<IScheduleDecoder> ScheduleDecoderParameter {
      get { return (OptionalValueParameter<IScheduleDecoder>)Parameters["ScheduleDecoder"]; }
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
    public IScheduleEvaluator ScheduleEvaluator {
      get { return ScheduleEvaluatorParameter.Value; }
      set { ScheduleEvaluatorParameter.Value = value; }
    }
    public IScheduleDecoder ScheduleDecoder {
      get { return ScheduleDecoderParameter.Value; }
      set { ScheduleDecoderParameter.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private JobShopSchedulingProblem(bool deserializing) : base(deserializing) { }
    private JobShopSchedulingProblem(JobShopSchedulingProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    public JobShopSchedulingProblem()
      : base(new SchedulingEvaluator(), new JSMRandomCreator()) {
      Parameters.Add(new ValueParameter<ItemList<Job>>("JobData", "Jobdata defining the precedence relationships and the duration of the tasks in this JSSP-Instance.", new ItemList<Job>()));
      Parameters.Add(new OptionalValueParameter<Schedule>("BestKnownSolution", "The best known solution of this JSSP instance."));

      Parameters.Add(new FixedValueParameter<IntValue>("Jobs", "The number of jobs used in this JSSP instance.", new IntValue()));
      Parameters.Add(new FixedValueParameter<IntValue>("Resources", "The number of resources used in this JSSP instance.", new IntValue()));
      Parameters.Add(new ValueParameter<IScheduleEvaluator>("ScheduleEvaluator", "The evaluator used to determine the quality of a solution.", new MakespanEvaluator()));
      Parameters.Add(new OptionalValueParameter<IScheduleDecoder>("ScheduleDecoder", "The operator that decodes the representation and creates a schedule.", new JSMDecoder()));

      EvaluatorParameter.GetsCollected = false;
      EvaluatorParameter.Hidden = true;
      ScheduleDecoderParameter.Hidden = true;

      InitializeOperators();
      Load(DefaultInstance);
      RegisterEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new JobShopSchedulingProblem(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      ScheduleEvaluatorParameter.ValueChanged += ScheduleEvaluatorParameter_ValueChanged;
      ScheduleEvaluator.QualityParameter.ActualNameChanged += ScheduleEvaluator_QualityParameter_ActualNameChanged;
      SolutionCreator.ScheduleEncodingParameter.ActualNameChanged += SolutionCreator_SchedulingEncodingParameter_ActualNameChanged;
      ScheduleDecoderParameter.ValueChanged += ScheduleDecoderParameter_ValueChanged;
      if (ScheduleDecoder != null) ScheduleDecoder.ScheduleParameter.ActualNameChanged += ScheduleDecoder_ScheduleParameter_ActualNameChanged;
    }

    #region Events
    protected override void OnSolutionCreatorChanged() {
      base.OnSolutionCreatorChanged();
      SolutionCreator.ScheduleEncodingParameter.ActualNameChanged += SolutionCreator_SchedulingEncodingParameter_ActualNameChanged;
      InitializeOperators();
    }
    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      ParameterizeOperators();
    }
    private void ScheduleEvaluatorParameter_ValueChanged(object sender, EventArgs eventArgs) {
      ScheduleEvaluator.QualityParameter.ActualNameChanged += ScheduleEvaluator_QualityParameter_ActualNameChanged;
      ParameterizeOperators();
    }
    private void ScheduleEvaluator_QualityParameter_ActualNameChanged(object sender, EventArgs eventArgs) {
      ParameterizeOperators();
    }

    private void SolutionCreator_SchedulingEncodingParameter_ActualNameChanged(object sender, EventArgs eventArgs) {
      ParameterizeOperators();
    }
    private void ScheduleDecoderParameter_ValueChanged(object sender, EventArgs eventArgs) {
      if (ScheduleDecoder != null) ScheduleDecoder.ScheduleParameter.ActualNameChanged += ScheduleDecoder_ScheduleParameter_ActualNameChanged;
      ParameterizeOperators();
    }
    private void ScheduleDecoder_ScheduleParameter_ActualNameChanged(object sender, EventArgs eventArgs) {
      ParameterizeOperators();
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

      BestKnownQuality = data.BestKnownQuality.HasValue ? new DoubleValue(data.BestKnownQuality.Value) : null;
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
        if (ScheduleEvaluator is MeanTardinessEvaluator)
          BestKnownQuality = new DoubleValue(MeanTardinessEvaluator.GetMeanTardiness(BestKnownSolution, jobData));
        else if (ScheduleEvaluator is MakespanEvaluator)
          BestKnownQuality = new DoubleValue(MakespanEvaluator.GetMakespan(BestKnownSolution));
      }
      Name = data.Name;
      Description = data.Description;
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
      ParameterizeOperators();
    }

    private void ApplyEncoding() {
      if (SolutionCreator.GetType() == typeof(JSMRandomCreator)) {
        Operators.AddRange(ApplicationManager.Manager.GetInstances<IJSMOperator>());
        ScheduleDecoder = new JSMDecoder();
      } else if (SolutionCreator.GetType() == typeof(PRVRandomCreator)) {
        Operators.AddRange(ApplicationManager.Manager.GetInstances<IPRVOperator>());
        ScheduleDecoder = new PRVDecoder();
      } else if (SolutionCreator.GetType() == typeof(PWRRandomCreator)) {
        Operators.AddRange(ApplicationManager.Manager.GetInstances<IPWROperator>());
        ScheduleDecoder = new PWRDecoder();
      } else if (SolutionCreator.GetType() == typeof(DirectScheduleRandomCreator)) {
        Operators.AddRange(ApplicationManager.Manager.GetInstances<IDirectScheduleOperator>());
        ScheduleDecoder = null;
      }
    }

    private void ParameterizeOperators() {
      Evaluator.ScheduleDecoderParameter.ActualName = ScheduleDecoderParameter.Name;
      Evaluator.ScheduleDecoderParameter.Hidden = true;
      Evaluator.ScheduleEvaluatorParameter.ActualName = ScheduleEvaluatorParameter.Name;
      Evaluator.ScheduleEvaluatorParameter.Hidden = true;
      Evaluator.QualityParameter.ActualName = ScheduleEvaluator.QualityParameter.ActualName;
      Evaluator.QualityParameter.Hidden = true;

      if (ScheduleDecoder != null)
        ScheduleDecoder.ScheduleEncodingParameter.ActualName = SolutionCreator.ScheduleEncodingParameter.ActualName;

      if (ScheduleDecoder != null) {
        ScheduleEvaluator.ScheduleParameter.ActualName = ScheduleDecoder.ScheduleParameter.ActualName;
        ScheduleEvaluator.ScheduleParameter.Hidden = true;
      } else if (SolutionCreator is DirectScheduleRandomCreator) {
        var directEvaluator = (DirectScheduleRandomCreator)SolutionCreator;
        ScheduleEvaluator.ScheduleParameter.ActualName = directEvaluator.ScheduleEncodingParameter.ActualName;
        ScheduleEvaluator.ScheduleParameter.Hidden = true;
      } else {
        ScheduleEvaluator.ScheduleParameter.ActualName = ScheduleEvaluator.ScheduleParameter.Name;
        ScheduleEvaluator.ScheduleParameter.Hidden = false;
      }

      foreach (var op in Operators.OfType<IScheduleManipulator>()) {
        op.ScheduleEncodingParameter.ActualName = SolutionCreator.ScheduleEncodingParameter.ActualName;
        op.ScheduleEncodingParameter.Hidden = true;
      }

      foreach (var op in Operators.OfType<IScheduleCrossover>()) {
        op.ChildParameter.ActualName = SolutionCreator.ScheduleEncodingParameter.ActualName;
        op.ChildParameter.Hidden = true;
        op.ParentsParameter.ActualName = SolutionCreator.ScheduleEncodingParameter.ActualName;
        op.ParentsParameter.Hidden = true;
      }

      foreach (var op in Operators.OfType<BestSchedulingSolutionAnalyzer>()) {
        op.QualityParameter.ActualName = ScheduleEvaluator.QualityParameter.ActualName;
        if (ScheduleDecoder != null) {
          op.ScheduleParameter.ActualName = ScheduleDecoder.ScheduleParameter.ActualName;
          op.ScheduleParameter.Hidden = true;
        } else if (SolutionCreator is DirectScheduleRandomCreator) {
          op.ScheduleParameter.ActualName = ((DirectScheduleRandomCreator)SolutionCreator).ScheduleEncodingParameter.ActualName;
          op.ScheduleParameter.Hidden = true;
        } else {
          op.ScheduleParameter.ActualName = op.ScheduleParameter.Name;
          op.ScheduleParameter.Hidden = false;
        }
      }
    }
    #endregion

  }
}
