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
using System.IO;
using System.Linq;
using System.Reflection;
using Google.OrTools.LinearSolver;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.ExactOptimization.LinearProgramming {

  [Item("Mixed-Integer Linear Programming Problem (LP, MIP)", "Represents a linear/mixed integer problem.")]
  [StorableType("0F6BD4A4-8C70-4922-9BA1-1F372820DD76")]
  public sealed class LinearProblem : Problem, IStorableContent {

    [Storable]
    private readonly IValueParameter<ILinearProblemDefinition> problemDefinitionParam;

    public LinearProblem() {
      Parameters.Remove(Parameters["Operators"]);
      Parameters.Add(problemDefinitionParam = new ValueParameter<ILinearProblemDefinition>("Model", "The linear programming problem",
        new ProgrammableLinearProblemDefinition()) { GetsCollected = false });
    }

    private LinearProblem(LinearProblem original, Cloner cloner)
      : base(original, cloner) {
      problemDefinitionParam = cloner.Clone(original.problemDefinitionParam);
    }

    [StorableConstructor]
    private LinearProblem(StorableConstructorFlag _) : base(_) { }

    public event EventHandler ProblemDefinitionChanged;

    public string Filename { get; set; }

    public ILinearProblemDefinition ProblemDefinition {
      get => problemDefinitionParam.Value;
      set {
        if (problemDefinitionParam.Value == value)
          return;
        problemDefinitionParam.Value = value;
        ProblemDefinitionChanged?.Invoke(this, EventArgs.Empty);
      }
    }

    public IValueParameter<ILinearProblemDefinition> ProblemDefinitionParameter => problemDefinitionParam;

    public override IDeepCloneable Clone(Cloner cloner) => new LinearProblem(this, cloner);

    public override void CollectParameterValues(IDictionary<string, IItem> values) {
      base.CollectParameterValues(values);

      if (ProblemDefinition == null) return;

      values.Add("Model Type", new StringValue(
        (ProblemDefinition.GetType().GetCustomAttributes().Single(a => a is ItemAttribute) as ItemAttribute).Name));

      if (ProblemDefinition is ProgrammableLinearProblemDefinition model) {
        values.Add("Model Name", new StringValue(model.Name));
      }
    }

    public void ExportModel(string fileName) {
      if (string.IsNullOrWhiteSpace(fileName))
        throw new ArgumentNullException(nameof(fileName));
      if (ProblemDefinition == null)
        throw new ArgumentNullException(nameof(ProblemDefinition));

      var fileInfo = new FileInfo(fileName);

      if (!fileInfo.Directory?.Exists ?? false) {
        Directory.CreateDirectory(fileInfo.Directory.FullName);
      }

      var solver = new Solver(ProblemDefinition.ItemName, Solver.OptimizationProblemType.CbcMixedIntegerProgramming);

      ProblemDefinition.BuildModel(solver);

      var exportSuccessful = false;
      switch (fileInfo.Extension) {
        case ".lp":
          var lpFormat = solver.ExportModelAsLpFormat(false);
          if (!string.IsNullOrEmpty(lpFormat)) {
            File.WriteAllText(fileName, lpFormat);
            exportSuccessful = true;
          }
          break;

        case ".mps":
          var mpsFormat = solver.ExportModelAsMpsFormat(false, false);
          if (!string.IsNullOrEmpty(mpsFormat)) {
            File.WriteAllText(fileName, mpsFormat);
            exportSuccessful = true;
          }
          break;

        case ".prototxt":
          exportSuccessful = solver.ExportModelAsProtoFormat(fileName,
            (Google.OrTools.LinearSolver.ProtoWriteFormat)ProtoWriteFormat.ProtoText);
          break;

        case ".bin": // remove file extension as it is added by OR-Tools
          fileName = Path.ChangeExtension(fileName, null);
          exportSuccessful = solver.ExportModelAsProtoFormat(fileName,
            (Google.OrTools.LinearSolver.ProtoWriteFormat)ProtoWriteFormat.ProtoBinary);
          break;

        default:
          throw new NotSupportedException($"File format {fileInfo.Extension} to export model is not supported.");
      }

      if (!exportSuccessful)
        throw new InvalidOperationException($"Model could not be exported. " +
          $"For details, see the log files in '{LinearSolver.LogDirectory}'.");
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }
  }
}
