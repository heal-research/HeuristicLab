#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming {

  [StorableClass]
  public class LinearSolver : ParameterizedNamedItem, ILinearSolver, IDisposable {

    [Storable]
    protected IValueParameter<EnumValue<ProblemType>> problemTypeParam;

    protected Solver solver;

    [Storable]
    protected IFixedValueParameter<TextValue> solverSpecificParametersParam;

    public LinearSolver() {
      Parameters.Add(problemTypeParam =
        new ValueParameter<EnumValue<ProblemType>>(nameof(ProblemType), new EnumValue<ProblemType>()));
      Parameters.Add(solverSpecificParametersParam =
        new FixedValueParameter<TextValue>(nameof(SolverSpecificParameters), new TextValue()));
    }

    [StorableConstructor]
    protected LinearSolver(bool deserializing)
      : base(deserializing) {
    }

    protected LinearSolver(LinearSolver original, Cloner cloner)
      : base(original, cloner) {
      problemTypeParam = cloner.Clone(original.problemTypeParam);
      solverSpecificParametersParam = cloner.Clone(original.solverSpecificParametersParam);
    }

    public double DualTolerance { get; set; } = MPSolverParameters.kDefaultDualTolerance;

    public string ExportModel { get; set; }

    public bool Incrementality { get; set; } =
          MPSolverParameters.kDefaultIncrementality == MPSolverParameters.INCREMENTALITY_ON;

    public LpAlgorithmValues LpAlgorithm { get; set; }

    public bool Presolve { get; set; } = MPSolverParameters.kDefaultPresolve == MPSolverParameters.PRESOLVE_ON;

    public double PrimalTolerance { get; set; } = MPSolverParameters.kDefaultPrimalTolerance;

    public ProblemType ProblemType {
      get => problemTypeParam.Value.Value;
      set => problemTypeParam.Value.Value = value;
    }

    public double RelativeGapTolerance { get; set; } = MPSolverParameters.kDefaultRelativeMipGap;

    public bool Scaling { get; set; }

    public string SolverSpecificParameters {
      get => solverSpecificParametersParam.Value.Value;
      set => solverSpecificParametersParam.Value.Value = value;
    }

    public virtual bool SupportsPause => true;
    public virtual bool SupportsStop => true;
    public virtual TimeSpan TimeLimit { get; set; } = TimeSpan.Zero;
    protected virtual OptimizationProblemType OptimizationProblemType { get; }

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
      solver != null && solver.ExportModelAsProtoFormat(fileName, (int)writeFormat);

    public SolverResponseStatus ImportFromMps(string fileName, bool? fixedFormat) => solver == null
      ? SolverResponseStatus.Abnormal
      : (SolverResponseStatus)solver.ImportModelFromMpsFormat(fileName, fixedFormat.HasValue, fixedFormat ?? false);

    public SolverResponseStatus ImportFromProto(string fileName) => solver == null
      ? SolverResponseStatus.Abnormal
      : (SolverResponseStatus)solver.ImportModelFromProtoFormat(fileName);

    public bool InterruptSolve() => solver?.InterruptSolve() ?? false;

    public virtual void Reset() {
      solver?.Dispose();
      solver = null;
    }

    public virtual void Solve(ILinearProgrammingProblemDefinition problemDefintion, ref TimeSpan executionTime,
      ResultCollection results, CancellationToken cancellationToken) =>
      Solve(problemDefintion, ref executionTime, results);

    public virtual void Solve(ILinearProgrammingProblemDefinition problemDefinition, ref TimeSpan executionTime,
      ResultCollection results) =>
      Solve(problemDefinition, results, TimeLimit);

    public virtual void Solve(ILinearProgrammingProblemDefinition problemDefinition, ResultCollection results,
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

      using (var parameters = new MPSolverParameters()) {
        parameters.SetDoubleParam(MPSolverParameters.RELATIVE_MIP_GAP, RelativeGapTolerance);
        parameters.SetDoubleParam(MPSolverParameters.PRIMAL_TOLERANCE, PrimalTolerance);
        parameters.SetDoubleParam(MPSolverParameters.DUAL_TOLERANCE, DualTolerance);
        parameters.SetIntegerParam(MPSolverParameters.PRESOLVE,
          Presolve ? MPSolverParameters.PRESOLVE_ON : MPSolverParameters.PRESOLVE_OFF);
        parameters.SetIntegerParam(MPSolverParameters.LP_ALGORITHM, (int)LpAlgorithm);
        parameters.SetIntegerParam(MPSolverParameters.INCREMENTALITY,
          Incrementality ? MPSolverParameters.INCREMENTALITY_ON : MPSolverParameters.INCREMENTALITY_OFF);
        parameters.SetIntegerParam(MPSolverParameters.SCALING,
          Scaling ? MPSolverParameters.SCALING_ON : MPSolverParameters.SCALING_OFF);

        if (!solver.SetSolverSpecificParametersAsString(SolverSpecificParameters))
          throw new ArgumentException("Solver specific parameters could not be set.");

        if (!string.IsNullOrWhiteSpace(ExportModel)) {
          var fileInfo = new FileInfo(ExportModel);

          if (!fileInfo.Directory?.Exists ?? false) {
            Directory.CreateDirectory(fileInfo.Directory.FullName);
          }

          bool exportSuccessful;
          switch (fileInfo.Extension) {
            case ".lp":
              exportSuccessful = ExportAsLp(ExportModel);
              break;

            case ".mps":
              exportSuccessful = ExportAsMps(ExportModel);
              break;

            case ".prototxt":
              exportSuccessful = ExportAsProto(ExportModel, ProtoWriteFormat.ProtoText);
              break;

            case ".bin": // remove file extension as it is added by OR-Tools
              exportSuccessful = ExportAsProto(Path.ChangeExtension(ExportModel, null));
              break;

            default:
              throw new NotSupportedException("File format selected to export model is not supported.");
          }
        }

        // TODO: show warning if file export didn't work (if exportSuccessful is false)

        resultStatus = (ResultStatus)solver.Solve(parameters);
      }

      var objectiveValue = solver.Objective()?.Value();

      problemDefinition.Analyze(solver, results);
      results.AddOrUpdateResult("ResultStatus", new EnumValue<ResultStatus>(resultStatus));
      results.AddOrUpdateResult("BestObjectiveValue", new DoubleValue(objectiveValue ?? double.NaN));

      if (solver.IsMIP()) {
        var objectiveBound = solver.Objective()?.BestBound();
        var absoluteGap = objectiveValue.HasValue && objectiveBound.HasValue
          ? Math.Abs(objectiveBound.Value - objectiveValue.Value)
          : (double?)null;
        // https://www.ibm.com/support/knowledgecenter/SSSA5P_12.7.1/ilog.odms.cplex.help/CPLEX/Parameters/topics/EpGap.html
        var relativeGap = absoluteGap.HasValue && objectiveValue.HasValue
          ? absoluteGap.Value / (1e-10 + Math.Abs(objectiveValue.Value))
          : (double?)null;

        results.AddOrUpdateResult("BestObjectiveBound", new DoubleValue(objectiveBound ?? double.NaN));
        results.AddOrUpdateResult("AbsoluteGap", new DoubleValue(absoluteGap ?? double.NaN));
        results.AddOrUpdateResult("RelativeGap", new PercentValue(relativeGap ?? double.NaN));
      }

      results.AddOrUpdateResult("NumberOfConstraints", new IntValue(solver.NumConstraints()));
      results.AddOrUpdateResult("NumberOfVariables", new IntValue(solver.NumVariables()));

      if (solver.IsMIP() && solver.Nodes() >= 0) {
        results.AddOrUpdateResult(nameof(solver.Nodes), new DoubleValue(solver.Nodes()));
      }

      if (solver.Iterations() >= 0) {
        results.AddOrUpdateResult(nameof(solver.Iterations), new DoubleValue(solver.Iterations()));
      }

      results.AddOrUpdateResult(nameof(solver.SolverVersion), new StringValue(solver.SolverVersion()));
    }

    protected virtual Solver CreateSolver(OptimizationProblemType optimizationProblemType, string libraryName = null) {
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
        solver = new Solver(Name, (int)optimizationProblemType, libraryName ?? string.Empty);
      } catch {
        throw new InvalidOperationException($"Could not create {optimizationProblemType}.");
      }

      if (solver == null)
        throw new InvalidOperationException($"Could not create {optimizationProblemType}.");

      solver.SuppressOutput();
      return solver;
    }
  }
}
