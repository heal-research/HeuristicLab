#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Data.Linq;
using System.Linq;
using System.ServiceModel;
using HeuristicLab.Services.OKB.DataAccess;
using log4net;

namespace HeuristicLab.Services.OKB {

  /// <summary>
  /// Implementation of <see cref="IAdminService"/>
  /// </summary>
  [ServiceBehavior(
    InstanceContextMode = InstanceContextMode.PerSession,
    IncludeExceptionDetailInFaults = true)]
  public class AdminService : IAdminService {

    private static ILog logger = LogManager.GetLogger(typeof(AdminService));

    private OKBDataContext GetDataContext() {
      return new OKBDataContext();
    }

    /// <summary>
    /// Gets all available platforms.
    /// </summary>
    /// <returns>A list of <see cref="Platform"/>s.</returns>
    public Platform[] GetPlatformNames() {
      OKBDataContext okb = GetDataContext();
      return okb.Platforms.ToArray();
    }

    /// <summary>
    /// Gets the complete algorithm object graph up to the following entities:
    /// <list type="bullet">
    /// 		<item>Parameter</item>
    /// 		<item>Algorithm_Paramters.Parameter.DataType</item>
    /// 		<item>Algorithm_Results.Result.DataType</item>
    /// 	</list>
    /// </summary>
    /// <param name="id">The algorithm id.</param>
    /// <returns>An <see cref="Algorithm"/></returns>
    public Algorithm GetCompleteAlgorithm(int id) {
      OKBDataContext okb = GetDataContext();
      var dlo = new DataLoadOptions();
      dlo.LoadWith<Algorithm>(a => a.Platform);
      dlo.LoadWith<Algorithm>(a => a.Algorithm_Parameters);
      dlo.LoadWith<Algorithm>(a => a.Algorithm_Results);
      dlo.LoadWith<Algorithm_Parameter>(ap => ap.Parameter);
      dlo.LoadWith<Algorithm_Result>(ar => ar.Result);
      dlo.LoadWith<Parameter>(p => p.DataType);
      dlo.LoadWith<Result>(r => r.DataType);
      okb.LoadOptions = dlo;
      return okb.Algorithms.Single(a => a.Id == id);
    }

    /// <summary>
    /// Gets the complete problem object graph up to the following entities:
    /// <list type="bullet">
    /// 		<item>Platform</item>
    /// 		<item>SolutionRepresentation</item>
    /// 		<item>Problem_Parameters.Parameter</item>
    /// 		<item>IntProblemCharacteristicValues.ProblemCharacteristic.DataType</item>
    /// 		<item>FloatProblemCharacteristicValues.ProblemCharacteristic.DataType</item>
    /// 		<item>CharProblemCharacteristicValues.ProblemCharacteristic.DataType</item>
    /// 	</list>
    /// </summary>
    /// <param name="id">The problem id.</param>
    /// <returns>A <see cref="Problem"/></returns>
    public Problem GetCompleteProblem(int id) {
      OKBDataContext okb = GetDataContext();
      var dlo = new DataLoadOptions();
      dlo.LoadWith<Problem>(p => p.Platform);
      dlo.LoadWith<Problem>(p => p.SolutionRepresentation);
      dlo.LoadWith<Problem>(p => p.Problem_Parameters);
      dlo.LoadWith<Problem>(p => p.IntProblemCharacteristicValues);
      dlo.LoadWith<Problem>(p => p.FloatProblemCharacteristicValues);
      dlo.LoadWith<Problem>(p => p.CharProblemCharacteristicValues);
      dlo.LoadWith<Problem_Parameter>(ap => ap.Parameter);
      dlo.LoadWith<IntProblemCharacteristicValue>(ipcv => ipcv.ProblemCharacteristic);
      dlo.LoadWith<FloatProblemCharacteristicValue>(fpcv => fpcv.ProblemCharacteristic);
      dlo.LoadWith<CharProblemCharacteristicValue>(cpcv => cpcv.ProblemCharacteristic);
      dlo.LoadWith<Parameter>(p => p.DataType);
      dlo.LoadWith<ProblemCharacteristic>(pc => pc.DataType);
      okb.LoadOptions = dlo;
      return okb.Problems.Single(p => p.Id == id);
    }

    /// <summary>
    /// Updates the algorithm object graph including the following properties and linked entitites:
    /// <list type="bullet">
    /// 		<item>Name</item>
    /// 		<item>Description</item>
    /// 		<item>AlgorithmClassId</item>
    /// 		<item>PlatformId</item>
    /// 		<item>Algorithm_Parameters</item>
    /// 		<item>Algorithm_Results</item>
    /// 	</list>
    /// 	<remarks>
    /// New <see cref="Parameter"/>s or <see cref="Result"/>s will not be
    /// created but have to be pre-existing.
    /// </remarks>
    /// </summary>
    /// <param name="algorithm">The algorithm.</param>
    public void UpdateCompleteAlgorithm(Algorithm algorithm) {
      OKBDataContext okb = GetDataContext();
      Algorithm original = okb.Algorithms.Single(a => a.Id == algorithm.Id);
      original.Name = algorithm.Name;
      original.Description = algorithm.Description;
      original.AlgorithmClassId = algorithm.AlgorithmClassId;
      original.PlatformId = algorithm.PlatformId;
      okb.Algorithm_Parameters.DeleteAllOnSubmit(original.Algorithm_Parameters);
      foreach (var a_p in algorithm.Algorithm_Parameters)
        original.Algorithm_Parameters.Add(new Algorithm_Parameter() {
          AlgorithmId = original.Id,
          ParameterId = a_p.ParameterId,
        });
      okb.Algorithm_Results.DeleteAllOnSubmit(original.Algorithm_Results);
      foreach (var a_r in algorithm.Algorithm_Results)
        original.Algorithm_Results.Add(new Algorithm_Result() {
          AlgorithmId = original.Id,
          ResultId = a_r.ResultId,
        });
      logger.Info("Updating algorithm implementation for " + original.Name);
      okb.SubmitChanges();
    }

    /// <summary>
    /// Updates the problem object graph including the following properties and linked entities:
    /// <list type="bullet">
    /// 		<item>Name</item>
    /// 		<item>Description</item>
    /// 		<item>ProblemClassId</item>
    /// 		<item>PlatformId</item>
    /// 		<item>SolutionRepresentationId</item>
    /// 		<item>IntProblemCharacteristicValues.Value</item>
    /// 		<item>FloatProblemCharacteristicValues.Value</item>
    /// 		<item>CharProblemCharacteristicValues.Value</item>
    /// 		<item>Problem_Parameters</item>
    /// 	</list>
    /// 	<remarks>
    /// New <see cref="ProblemCharacteristic"/>s or <see cref="Parameter"/>s will
    /// not be created but have to be pre-existing.
    /// </remarks>
    /// </summary>
    /// <param name="problem">The problem.</param>
    public void UpdateCompleteProblem(Problem problem) {
      OKBDataContext okb = GetDataContext();
      Problem originalProblem = okb.Problems.Single(p => p.Id == problem.Id);
      UpdateProblemMasterInfo(problem, originalProblem);
      UpdateProblemCharacteristics(problem, okb, originalProblem);
      UpdateProblemParameters(problem, okb, originalProblem);
      logger.Info("Updating problem " + originalProblem.Name);
      okb.SubmitChanges();
    }

    private static void UpdateProblemParameters(Problem problem, OKBDataContext okb, Problem originalProblem) {
      okb.Problem_Parameters.DeleteAllOnSubmit(originalProblem.Problem_Parameters);
      originalProblem.Problem_Parameters.Clear();
      foreach (var p_p in problem.Problem_Parameters) {
        originalProblem.Problem_Parameters.Add(new Problem_Parameter() {
          ProblemId = originalProblem.Id,
          ParameterId = p_p.ParameterId,
        });
      }
    }

    private static void UpdateProblemCharacteristics(Problem problem, OKBDataContext okb, Problem originalProblem) {
      okb.IntProblemCharacteristicValues.DeleteAllOnSubmit(originalProblem.IntProblemCharacteristicValues);
      okb.FloatProblemCharacteristicValues.DeleteAllOnSubmit(originalProblem.FloatProblemCharacteristicValues);
      okb.CharProblemCharacteristicValues.DeleteAllOnSubmit(originalProblem.CharProblemCharacteristicValues);
      originalProblem.IntProblemCharacteristicValues.Clear();
      originalProblem.FloatProblemCharacteristicValues.Clear();
      originalProblem.CharProblemCharacteristicValues.Clear();
      foreach (var ipc in problem.IntProblemCharacteristicValues) {
        originalProblem.IntProblemCharacteristicValues.Add(new IntProblemCharacteristicValue() {
          ProblemId = originalProblem.Id,
          ProblemCharacteristicId = ipc.ProblemCharacteristicId,
          Value = ipc.Value,
        });
      }
      foreach (var fpc in problem.FloatProblemCharacteristicValues) {
        originalProblem.FloatProblemCharacteristicValues.Add(new FloatProblemCharacteristicValue() {
          ProblemId = originalProblem.Id,
          ProblemCharacteristicId = fpc.ProblemCharacteristicId,
          Value = fpc.Value,
        });
      }
      foreach (var cpc in problem.CharProblemCharacteristicValues) {
        originalProblem.CharProblemCharacteristicValues.Add(new CharProblemCharacteristicValue() {
          ProblemId = originalProblem.Id,
          ProblemCharacteristicId = cpc.ProblemCharacteristicId,
          Value = cpc.Value,
        });
      }
    }

    private static void UpdateProblemMasterInfo(Problem problem, Problem originalProblem) {
      originalProblem.Name = problem.Name;
      originalProblem.Description = problem.Description;
      originalProblem.ProblemClassId = problem.ProblemClassId;
      originalProblem.PlatformId = problem.PlatformId;
      originalProblem.SolutionRepresentationId = problem.SolutionRepresentationId;
    }
  }
}
