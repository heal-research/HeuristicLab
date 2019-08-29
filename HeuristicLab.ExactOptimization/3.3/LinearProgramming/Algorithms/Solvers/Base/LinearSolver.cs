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
using System.Threading;
using Google.OrTools.LinearSolver;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.ExactOptimization.LinearProgramming {

  [StorableType("D0657902-BE8B-4826-B832-FDA84E9B24C3")]
  public class LinearSolver : ParameterizedNamedItem, ILinearSolver, IDisposable {

    [Storable]
    protected IValueParameter<EnumValue<ProblemType>> problemTypeParam;

    protected Solver solver;

    [Storable]
    protected IFixedValueParameter<TextValue> solverSpecificParametersParam;

    public static string LogDirectory { get; }

    static LinearSolver() {
      var solver = new Solver("", Solver.OptimizationProblemType.CbcMixedIntegerProgramming);
      LogDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
      Directory.CreateDirectory(LogDirectory);
      solver.InitLogging(Assembly.GetExecutingAssembly().Location, LogDirectory);
    }

    public LinearSolver() {
      Parameters.Add(problemTypeParam =
        new ValueParameter<EnumValue<ProblemType>>(nameof(ProblemType),
          new EnumValue<ProblemType>(ProblemType.MixedIntegerProgramming)));
      Parameters.Add(solverSpecificParametersParam =
        new FixedValueParameter<TextValue>(nameof(SolverSpecificParameters), new TextValue()));
    }

    [StorableConstructor]
    protected LinearSolver(StorableConstructorFlag _) : base(_) { }

    protected LinearSolver(LinearSolver original, Cloner cloner)
      : base(original, cloner) {
      problemTypeParam = cloner.Clone(original.problemTypeParam);
      solverSpecificParametersParam = cloner.Clone(original.solverSpecificParametersParam);
    }

    public double DualTolerance { get; set; } = SolverParameters.DefaultDualTolerance;

    public bool Incrementality { get; set; } =
      SolverParameters.DefaultIncrementality == SolverParameters.IncrementalityValues.IncrementalityOn;

    public SolverParameters.LpAlgorithmValues LpAlgorithm { get; set; }
    protected virtual Solver.OptimizationProblemType OptimizationProblemType { get; }
    public bool Presolve { get; set; } = SolverParameters.DefaultPresolve == SolverParameters.PresolveValues.PresolveOn;
    public double PrimalTolerance { get; set; } = SolverParameters.DefaultPrimalTolerance;

    public ProblemType ProblemType {
      get => problemTypeParam.Value.Value;
      set => problemTypeParam.Value.Value = value;
    }

    public IValueParameter<EnumValue<ProblemType>> ProblemTypeParameter => problemTypeParam;
    public double RelativeGapTolerance { get; set; } = SolverParameters.DefaultRelativeMipGap;
    public bool Scaling { get; set; }

    public string SolverSpecificParameters {
      get => solverSpecificParametersParam.Value.Value;
      set => solverSpecificParametersParam.Value.Value = value;
    }

    public IFixedValueParameter<TextValue> SolverSpecificParametersParameter => solverSpecificParametersParam;
    public virtual bool SupportsPause => true;
    public virtual bool SupportsStop => true;
    public virtual TimeSpan TimeLimit { get; set; } = TimeSpan.Zero;

    public override IDeepCloneable Clone(Cloner cloner) => new LinearSolver(this, cloner);

    public void Dispose() => solver?.Dispose();

    public bool ExportAsLp(string fileName, bool obfuscated = false) {
      var lpFormat = solver?.ExportModelAsLpFormat(obfuscated);
      if (string.IsNullOrEmpty(lpFormat))
        return false;
      File.WriteAllText(fileName, lpFormat);
      return true;
    }

    public bool ExportAsMps(string fileName, bool fixedFormat = false, bool obfuscated = false) {
      var mpsFormat = solver?.ExportModelAsMpsFormat(fixedFormat, obfuscated);
      if (string.IsNullOrEmpty(mpsFormat))
        return false;
      File.WriteAllText(fileName, mpsFormat);
      return true;
    }

    public bool ExportAsProto(string fileName, ProtoWriteFormat writeFormat = ProtoWriteFormat.ProtoBinary) =>
      solver != null && solver.ExportModelAsProtoFormat(fileName, (Google.OrTools.LinearSolver.ProtoWriteFormat)writeFormat);

    public MPSolverResponseStatus ImportFromMps(string fileName, bool? fixedFormat) =>
      solver?.ImportModelFromMpsFormat(fileName, fixedFormat.HasValue, fixedFormat ?? false) ??
      (MPSolverResponseStatus)SolverResponseStatus.Abnormal;

    public MPSolverResponseStatus ImportFromProto(string fileName) =>
      solver?.ImportModelFromProtoFormat(fileName) ?? (MPSolverResponseStatus)SolverResponseStatus.Abnormal;

    public bool InterruptSolve() => solver?.InterruptSolve() ?? false;

    public virtual void Reset() {
      solver?.Dispose();
      solver = null;
    }

    public virtual void Solve(ILinearProblemDefinition problemDefintion,
      ResultCollection results, CancellationToken cancellationToken) =>
      Solve(problemDefintion, results);

    public virtual void Solve(ILinearProblemDefinition problemDefinition,
      ResultCollection results) =>
      Solve(problemDefinition, results, TimeLimit);

    public virtual void Solve(ILinearProblemDefinition problemDefinition, ResultCollection results,
      TimeSpan timeLimit) {
      if (solver == null) {
        solver = CreateSolver(OptimizationProblemType);
        problemDefinition.BuildModel(solver);
      }

      if (timeLimit > TimeSpan.Zero) {
        solver.SetTimeLimit((long)timeLimit.TotalMilliseconds);
      } else {
        solver.SetTimeLimit(0);
      }

      ResultStatus resultStatus;

      using (var parameters = new SolverParameters()) {
        parameters.SetDoubleParam(SolverParameters.DoubleParam.RelativeMipGap, RelativeGapTolerance);
        parameters.SetDoubleParam(SolverParameters.DoubleParam.PrimalTolerance, PrimalTolerance);
        parameters.SetDoubleParam(SolverParameters.DoubleParam.DualTolerance, DualTolerance);
        parameters.SetIntegerParam(SolverParameters.IntegerParam.Presolve,
          (int)(Presolve ? SolverParameters.PresolveValues.PresolveOn : SolverParameters.PresolveValues.PresolveOff));
        parameters.SetIntegerParam(SolverParameters.IntegerParam.Incrementality,
          (int)(Incrementality ? SolverParameters.IncrementalityValues.IncrementalityOn : SolverParameters.IncrementalityValues.IncrementalityOff));
        parameters.SetIntegerParam(SolverParameters.IntegerParam.Scaling,
          (int)(Scaling ? SolverParameters.ScalingValues.ScalingOn : SolverParameters.ScalingValues.ScalingOff));

        if (!solver.SetSolverSpecificParametersAsString(SolverSpecificParameters))
          throw new ArgumentException("Solver specific parameters could not be set. " +
            $"For details, see the log files in '{LogDirectory}'.");

        resultStatus = (ResultStatus)solver.Solve(parameters);
      }

      var objectiveValue = solver.Objective()?.Value();

      problemDefinition.Analyze(solver, results);

      if (solver.IsMip()) {
        var objectiveBound = solver.Objective()?.BestBound();
        var absoluteGap = objectiveValue.HasValue && objectiveBound.HasValue
          ? Math.Abs(objectiveBound.Value - objectiveValue.Value)
          : (double?)null;
        // https://www.ibm.com/support/knowledgecenter/SSSA5P_12.7.1/ilog.odms.cplex.help/CPLEX/Parameters/topics/EpGap.html
        var relativeGap = absoluteGap.HasValue && objectiveValue.HasValue
          ? absoluteGap.Value / (1e-10 + Math.Abs(objectiveValue.Value))
          : (double?)null;

        if (resultStatus == ResultStatus.Optimal && absoluteGap.HasValue && !absoluteGap.Value.IsAlmost(0)) {
          resultStatus = ResultStatus.OptimalWithinTolerance;
        }

        results.AddOrUpdateResult("BestObjectiveBound", new DoubleValue(objectiveBound ?? double.NaN));
        results.AddOrUpdateResult("AbsoluteGap", new DoubleValue(absoluteGap ?? double.NaN));
        results.AddOrUpdateResult("RelativeGap", new PercentValue(relativeGap ?? double.NaN));
      }

      results.AddOrUpdateResult("ResultStatus", new EnumValue<ResultStatus>(resultStatus));
      results.AddOrUpdateResult("BestObjectiveValue", new DoubleValue(objectiveValue ?? double.NaN));

      results.AddOrUpdateResult("NumberOfConstraints", new IntValue(solver.NumConstraints()));
      results.AddOrUpdateResult("NumberOfVariables", new IntValue(solver.NumVariables()));

      if (solver.IsMip() && solver.Nodes() >= 0) {
        results.AddOrUpdateResult(nameof(solver.Nodes), new DoubleValue(solver.Nodes()));
      }

      if (solver.Iterations() >= 0) {
        results.AddOrUpdateResult(nameof(solver.Iterations), new DoubleValue(solver.Iterations()));
      }

      results.AddOrUpdateResult(nameof(solver.SolverVersion), new StringValue(solver.SolverVersion()));
    }

    protected virtual Solver CreateSolver(Solver.OptimizationProblemType optimizationProblemType, string libraryName = null) {
      if (!string.IsNullOrEmpty(libraryName) && !File.Exists(libraryName)) {
        var paths = new List<string> {
          Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath)
        };
        var path = Environment.GetEnvironmentVariable("PATH");
        if (path != null)
          paths.AddRange(path.Split(';'));
        if (!paths.Any(p => File.Exists(Path.Combine(p, libraryName))))
          throw new FileNotFoundException($"Could not find library {libraryName} in PATH.", libraryName);
      }

      try {
        solver = new Solver(Name, optimizationProblemType, libraryName ?? string.Empty);
      } catch {
        solver = null;
      }

      if (solver == null)
        throw new InvalidOperationException($"Could not create {optimizationProblemType}. " +
          $"For details, see the log files in '{LogDirectory}'.");

      solver.SuppressOutput();
      return solver;
    }
  }
}
