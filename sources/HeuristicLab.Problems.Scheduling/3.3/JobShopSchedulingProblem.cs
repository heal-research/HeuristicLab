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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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

namespace HeuristicLab.Problems.Scheduling {
  [Item("JobShop Scheduling Problem", "Represents a standard JobShop Scheduling Problem")]
  [Creatable("Problems")]
  [StorableClass]
  public sealed class JobShopSchedulingProblem : SchedulingProblem, IStorableContent {
    #region Parameter Properties
    public ValueParameter<ItemList<Job>> JobDataParameter {
      get { return (ValueParameter<ItemList<Job>>)Parameters["JobData"]; }
    }
    public OptionalValueParameter<Schedule> BestKnownSolutionParameter {
      get { return (OptionalValueParameter<Schedule>)Parameters["BestKnownSolution"]; }
    }

    public ValueParameter<IntValue> JobsParameter {
      get { return (ValueParameter<IntValue>)Parameters["Jobs"]; }
    }
    public ValueParameter<IntValue> ResourcesParameter {
      get { return (ValueParameter<IntValue>)Parameters["Resources"]; }
    }
    public ValueParameter<SchedulingEvaluator> SolutionEvaluatorParameter {
      get { return (ValueParameter<SchedulingEvaluator>)Parameters["SolutionEvaluator"]; }
    }
    public ValueParameter<BoolValue> DueDatesParameter {
      get { return (ValueParameter<BoolValue>)Parameters["DueDates"]; }
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
    public IntValue Jobs {
      get { return JobsParameter.Value; }
      set { JobsParameter.Value = value; }
    }
    public IntValue Resources {
      get { return ResourcesParameter.Value; }
      set { ResourcesParameter.Value = value; }
    }
    public SchedulingEvaluator SolutionEvaluator {
      get { return SolutionEvaluatorParameter.Value; }
      set { SolutionEvaluatorParameter.Value = value; }
    }
    public BoolValue DueDates {
      get { return DueDatesParameter.Value; }
      set { DueDatesParameter.Value = value; }
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

      Parameters.Add(new ValueParameter<IntValue>("Jobs", "The number of jobs used in this JSSP instance.", new IntValue()));
      Parameters.Add(new ValueParameter<IntValue>("Resources", "The number of resources used in this JSSP instance.", new IntValue()));
      Parameters.Add(new ValueParameter<BoolValue>("DueDates", "Determines whether the problem instance uses due dates or not.", new BoolValue()));
      Parameters.Add(new ValueParameter<SchedulingEvaluator>("SolutionEvaluator", "The evaluator used to determine the quality of a solution.", new MakespanEvaluator()));

      InitializeOperators();
      InitializeProblemInstance();
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

    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
    }
    #endregion

    #region Helpers
    private void InitializeProblemInstance() {
      Jobs = new IntValue(10);
      Resources = new IntValue(10);
      BestKnownQuality = new DoubleValue(930);
      JobData = new ItemList<Job>();
      List<string> data = new List<string>
      {"0 29 1 78 2  9 3 36 4 49 5 11 6 62 7 56 8 44 9 21",
       "0 43 2 90 4 75 9 11 3 69 1 28 6 46 5 46 7 72 8 30",
       "1 91 0 85 3 39 2 74 8 90 5 10 7 12 6 89 9 45 4 33",
       "1 81 2 95 0 71 4 99 6  9 8 52 7 85 3 98 9 22 5 43",
       "2 14 0  6 1 22 5 61 3 26 4 69 8 21 7 49 9 72 6 53",
       "2 84 1  2 5 52 3 95 8 48 9 72 0 47 6 65 4  6 7 25",
       "1 46 0 37 3 61 2 13 6 32 5 21 9 32 8 89 7 30 4 55",
       "2 31 0 86 1 46 5 74 4 32 6 88 8 19 9 48 7 36 3 79",
       "0 76 1 69 3 76 5 51 2 85 9 11 6 40 7 89 4 26 8 74",
       "1 85 0 13 2 61 6  7 8 64 9 76 5 47 3 52 4 90 7 45" };

      int jobCount = 0;
      foreach (string s in data) {
        List<string> split = SplitString(s);
        JobData.Add(CreateJobFromData(split, jobCount++));
      }
    }

    private void InitializeOperators() {
      Operators.Clear();
      ApplyEncoding();
      Operators.Add(new BestSchedulingSolutionAnalyzer());
    }

    private void ApplyEncoding() {
      if (SolutionCreator.GetType().Equals(typeof(JSMRandomCreator))) {
        Operators.AddRange(ApplicationManager.Manager.GetInstances<IJSMOperator>());
        JSMDecoder decoder = new JSMDecoder();
        ((SchedulingEvaluationAlgorithm)this.EvaluatorParameter.ActualValue).InitializeOperatorGraph<JSMEncoding>(decoder);
      } else {
        if (SolutionCreator.GetType().Equals(typeof(PRVRandomCreator))) {
          Operators.AddRange(ApplicationManager.Manager.GetInstances<IPRVOperator>());
          PRVDecoder decoder = new PRVDecoder();
          ((SchedulingEvaluationAlgorithm)this.EvaluatorParameter.ActualValue).InitializeOperatorGraph<PRVEncoding>(decoder);
        } else {
          if (SolutionCreator.GetType().Equals(typeof(PWRRandomCreator))) {
            Operators.AddRange(ApplicationManager.Manager.GetInstances<IPWROperator>());
            PWRDecoder decoder = new PWRDecoder();
            ((SchedulingEvaluationAlgorithm)this.EvaluatorParameter.ActualValue).InitializeOperatorGraph<PWREncoding>(decoder);
          } else {
            if (SolutionCreator.GetType().Equals(typeof(DirectScheduleRandomCreator))) {
              Operators.AddRange(ApplicationManager.Manager.GetInstances<IDirectScheduleOperator>());
              ((SchedulingEvaluationAlgorithm)this.EvaluatorParameter.ActualValue).InitializeOperatorGraph<Schedule>();
            }
          }
        }
      }
    }
    #endregion

    #region Importmethods
    private List<string> SplitString(string line) {
      if (line == null)
        return null;
      List<string> data = new List<string>(line.Split(' '));
      List<string> result = new List<string>();

      foreach (string s in data) {
        if (!String.IsNullOrEmpty(s) && s != "" && s != " ")
          result.Add(s);
      }

      return result;
    }
    private int[] GetIntArray(List<string> data) {
      int[] arr = new int[data.Count];
      for (int i = 0; i < data.Count; i++) {
        arr[i] = Int32.Parse(data[i]);
      }
      return arr;
    }
    private Job CreateJobFromData(List<string> data, int jobCount) {
      double dueDate = 0;
      int dataCount = data.Count;
      if (DueDates.Value) {
        dueDate = Double.Parse(data[data.Count - 1]);
        dataCount--;
      }
      Job j = new Job(jobCount, dueDate);
      for (int i = 0; i < dataCount; i++) {
        Task t = new Task(i / 2, Int32.Parse(data[i]), jobCount, Double.Parse(data[i + 1]));
        j.Tasks.Add(t);
        i++;
      }//for
      return j;
    }

    public void ImportFromORLibrary(string fileName) {
      if (!File.Exists(fileName))
        return;
      StreamReader problemFile = new StreamReader(fileName);
      //assures that the parser only reads the first problem instance given in the file
      bool problemFound = false;

      JobData = new ItemList<Job>();

      while (!problemFile.EndOfStream && !problemFound) {
        string line = problemFile.ReadLine();
        List<string> data = SplitString(line);
        if (data.Count > 0 && ((int)data[0][0] >= 48 && (int)data[0][0] <= 57)) {
          int jobCount = 0;
          Jobs = new IntValue(Int32.Parse(data[0]));
          Resources = new IntValue(Int32.Parse(data[1]));
          //data[2] = bestKnownQuality (double)
          //data[3] = dueDates (0|1)
          DueDates.Value = false;
          if (data.Count > 2)
            BestKnownQualityParameter.ActualValue = new DoubleValue(Double.Parse(data[2]));
          if (data.Count > 3 && data[3] == "1")
            DueDates.Value = true;
          line = problemFile.ReadLine();
          data = SplitString(line);
          while (!problemFile.EndOfStream && data.Count > 0 && ((int)data[0][0] >= 48 && (int)data[0][0] <= 57)) {
            Job j = CreateJobFromData(data, jobCount);
            this.JobData.Add(j);
            jobCount++;
            line = problemFile.ReadLine();
            data = SplitString(line);
          }//while
          problemFound = true;
        }//if
      }//while
      problemFile.Close();
    }

    public void ImportJSMSolution(string fileName) {
      if (!File.Exists(fileName))
        return;
      StreamReader solutionFile = new StreamReader(fileName);
      //assures that the parser only reads the first solution instance given in the file
      bool solutionFound = false;
      JSMEncoding solution = new JSMEncoding();
      while (!solutionFile.EndOfStream && !solutionFound) {

        string line = solutionFile.ReadLine();
        List<string> data = SplitString(line);
        if (data.Count > 0 && ((int)data[0][0] >= 48 && (int)data[0][0] <= 57)) {
          if (data.Count > 2)
            BestKnownQualityParameter.ActualValue = new DoubleValue(Double.Parse(data[2]));
          line = solutionFile.ReadLine();
          data = SplitString(line);
          while (data != null && data.Count > 0 && ((int)data[0][0] >= 48 && (int)data[0][0] <= 57)) {
            Permutation p = new Permutation(PermutationTypes.Absolute, GetIntArray(data));
            solution.JobSequenceMatrix.Add(p);

            line = solutionFile.ReadLine();
            data = SplitString(line);
          }//while
          solutionFound = true;
        }//if
      }//while
      solutionFile.Close();

      JSMDecoder decoder = new JSMDecoder();
      Schedule result = decoder.CreateScheduleFromEncoding(solution, JobData);
      BestKnownSolution = result;
    }
    #endregion

  }
}
