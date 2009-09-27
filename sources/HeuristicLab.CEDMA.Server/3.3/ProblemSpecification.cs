#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Core;
using System.Xml;
using HeuristicLab.Operators;
using HeuristicLab.Modeling.Database;

namespace HeuristicLab.CEDMA.Server {
  /// <summary>
  /// ProblemSpecification describes the data mining task.
  /// </summary>
  public class ProblemSpecification {

    private HeuristicLab.DataAnalysis.Dataset dataset;
    public HeuristicLab.DataAnalysis.Dataset Dataset {
      get { return dataset; }
      set {
        if (value != dataset) {
          dataset = value;
        }
      }
    }

    public string TargetVariable { get; set; }

    private int trainingSamplesStart;
    public int TrainingSamplesStart {
      get { return trainingSamplesStart; }
      set { trainingSamplesStart = value; }
    }

    private int trainingSamplesEnd;
    public int TrainingSamplesEnd {
      get { return trainingSamplesEnd; }
      set { trainingSamplesEnd = value; }
    }

    private int validationSamplesStart;
    public int ValidationSamplesStart {
      get { return validationSamplesStart; }
      set { validationSamplesStart = value; }
    }

    private int validationSamplesEnd;
    public int ValidationSamplesEnd {
      get { return validationSamplesEnd; }
      set { validationSamplesEnd = value; }
    }

    private int testSamplesStart;
    public int TestSamplesStart {
      get { return testSamplesStart; }
      set { testSamplesStart = value; }
    }

    private int testSamplesEnd;
    public int TestSamplesEnd {
      get { return testSamplesEnd; }
      set { testSamplesEnd = value; }
    }

    public int MaxTimeOffset { get; set; }
    public int MinTimeOffset { get; set; }

    public bool AutoRegressive { get; set; }

    private LearningTask learningTask;
    public LearningTask LearningTask {
      get { return learningTask; }
      set { learningTask = value; }
    }

    private List<string> inputVariables;
    public IEnumerable<string> InputVariables {
      get { return inputVariables; }
    }

    public ProblemSpecification() {
      Dataset = new HeuristicLab.DataAnalysis.Dataset();
      inputVariables = new List<string>();
    }

    // copy ctr
    public ProblemSpecification(ProblemSpecification original) {
      LearningTask = original.LearningTask;
      TargetVariable = original.TargetVariable;
      TrainingSamplesStart = original.TrainingSamplesStart;
      TrainingSamplesEnd = original.TrainingSamplesEnd;
      ValidationSamplesStart = original.ValidationSamplesStart;
      ValidationSamplesEnd = original.ValidationSamplesEnd;
      TestSamplesStart = original.TestSamplesStart;
      TestSamplesEnd = original.TestSamplesEnd;
      inputVariables = new List<string>(original.InputVariables);
      Dataset = original.Dataset;
    }

    internal void AddInputVariable(string name) {
      if (!inputVariables.Contains(name)) inputVariables.Add(name);
    }

    internal void RemoveInputVariable(string name) {
      inputVariables.Remove(name);
    }

    public override bool Equals(object obj) {
      ProblemSpecification other = (obj as ProblemSpecification);
      if (other == null) return false;
      return
        other.LearningTask == LearningTask &&
        other.MinTimeOffset == MinTimeOffset &&
        other.MaxTimeOffset == MaxTimeOffset &&
        other.AutoRegressive == AutoRegressive &&
        other.TargetVariable == TargetVariable &&
        other.trainingSamplesStart == trainingSamplesStart &&
        other.trainingSamplesEnd == trainingSamplesEnd &&
        other.validationSamplesStart == validationSamplesStart &&
        other.validationSamplesEnd == validationSamplesEnd &&
        other.testSamplesStart == testSamplesStart &&
        other.testSamplesEnd == testSamplesEnd &&
        other.InputVariables.Count() == InputVariables.Count() &&
        other.InputVariables.All(x => InputVariables.Contains(x)) &&
        // it would be safer to check if the dataset values are the same but 
        // it should be sufficient to check if the dimensions are equal for now (gkronber 09/21/2009) 
        other.Dataset.Rows == Dataset.Rows &&
        other.Dataset.Columns == Dataset.Columns;
    }

    public override int GetHashCode() {
      return
        LearningTask.GetHashCode() |
        TargetVariable.GetHashCode() |
        TrainingSamplesStart.GetHashCode() |
        TrainingSamplesEnd.GetHashCode() |
        ValidationSamplesStart.GetHashCode() |
        ValidationSamplesEnd.GetHashCode() |
        TestSamplesStart.GetHashCode() |
        TestSamplesEnd.GetHashCode() |
        InputVariables.Count().GetHashCode() |
        Dataset.Rows.GetHashCode() |
        Dataset.Columns.GetHashCode();
    }
  }
}
