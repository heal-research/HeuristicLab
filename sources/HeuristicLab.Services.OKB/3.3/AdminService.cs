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

using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using HeuristicLab.Services.OKB.DataAccess;

namespace HeuristicLab.Services.OKB {
  /// <summary>
  /// Implementation of the OKB administration service (interface <see cref="IAdminService"/>).
  /// </summary>
  [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
  public class AdminService : IAdminService {
    #region Platform Methods
    public DataTransfer.Platform GetPlatform(long id) {
      using (OKBDataContext okb = new OKBDataContext()) {
        return Convert.ToDto(okb.Platforms.FirstOrDefault(x => x.Id == id));
      }
    }
    public IEnumerable<DataTransfer.Platform> GetPlatforms() {
      using (OKBDataContext okb = new OKBDataContext()) {
        return okb.Platforms.Select(x => Convert.ToDto(x)).ToArray();
      }
    }
    public void StorePlatform(DataTransfer.Platform dto) {
      using (OKBDataContext okb = new OKBDataContext()) {
        DataAccess.Platform entity = okb.Platforms.FirstOrDefault(x => x.Id == dto.Id);
        if (entity == null) okb.Platforms.InsertOnSubmit(Convert.ToEntity(dto));
        else Convert.ToEntity(dto, entity);
        okb.SubmitChanges();
      }
    }
    public void DeletePlatform(long id) {
      using (OKBDataContext okb = new OKBDataContext()) {
        DataAccess.Platform entity = okb.Platforms.FirstOrDefault(x => x.Id == id);
        if (entity != null) okb.Platforms.DeleteOnSubmit(entity);
        okb.SubmitChanges();
      }
    }
    #endregion

    #region AlgorithmClass Methods
    public DataTransfer.AlgorithmClass GetAlgorithmClass(long id) {
      using (OKBDataContext okb = new OKBDataContext()) {
        return Convert.ToDto(okb.AlgorithmClasses.FirstOrDefault(x => x.Id == id));
      }
    }
    public IEnumerable<DataTransfer.AlgorithmClass> GetAlgorithmClasses() {
      using (OKBDataContext okb = new OKBDataContext()) {
        return okb.AlgorithmClasses.Select(x => Convert.ToDto(x)).ToArray();
      }
    }
    public void StoreAlgorithmClass(DataTransfer.AlgorithmClass dto) {
      using (OKBDataContext okb = new OKBDataContext()) {
        DataAccess.AlgorithmClass entity = okb.AlgorithmClasses.FirstOrDefault(x => x.Id == dto.Id);
        if (entity == null) okb.AlgorithmClasses.InsertOnSubmit(Convert.ToEntity(dto));
        else Convert.ToEntity(dto, entity);
        okb.SubmitChanges();
      }
    }
    public void DeleteAlgorithmClass(long id) {
      using (OKBDataContext okb = new OKBDataContext()) {
        DataAccess.AlgorithmClass entity = okb.AlgorithmClasses.FirstOrDefault(x => x.Id == id);
        if (entity != null) okb.AlgorithmClasses.DeleteOnSubmit(entity);
        okb.SubmitChanges();
      }
    }
    #endregion

    #region Algorithm Methods
    public DataTransfer.Algorithm GetAlgorithm(long id) {
      using (OKBDataContext okb = new OKBDataContext()) {
        return Convert.ToDto(okb.Algorithms.FirstOrDefault(x => x.Id == id));
      }
    }
    public IEnumerable<DataTransfer.Algorithm> GetAlgorithms() {
      using (OKBDataContext okb = new OKBDataContext()) {
        return okb.Algorithms.Select(x => Convert.ToDto(x)).ToArray();
      }
    }
    public void StoreAlgorithm(DataTransfer.Algorithm dto) {
      using (OKBDataContext okb = new OKBDataContext()) {
        DataAccess.Algorithm entity = okb.Algorithms.FirstOrDefault(x => x.Id == dto.Id);
        if (entity == null) okb.Algorithms.InsertOnSubmit(Convert.ToEntity(dto));
        else Convert.ToEntity(dto, entity);
        okb.SubmitChanges();
      }
    }
    public void DeleteAlgorithm(long id) {
      using (OKBDataContext okb = new OKBDataContext()) {
        DataAccess.Algorithm entity = okb.Algorithms.FirstOrDefault(x => x.Id == id);
        if (entity != null) okb.Algorithms.DeleteOnSubmit(entity);
        okb.SubmitChanges();
      }
    }
    #endregion

    /*

    /// <summary>
    /// Gets the complete algorithm object graph up to the following entities:
    /// <list type="bullet">
    ///   <item>Parameter</item>
    ///   <item>Algorithm_Paramters.Parameter.DataType</item>
    ///   <item>Algorithm_Results.Result.DataType</item>
    /// </list>
    /// </summary>
    /// <param name="id">The algorithm id.</param>
    /// <returns>An <see cref="Algorithm"/></returns>
    public Algorithm GetCompleteAlgorithm(int id) {
      using (OKBDataContext okb = new OKBDataContext()) {
        var dlo = new DataLoadOptions();
        dlo.LoadWith<Algorithm>(a => a.AlgorithmClass);
        dlo.LoadWith<Algorithm>(a => a.Platform);
        dlo.LoadWith<Algorithm>(a => a.AlgorithmUsers);
        dlo.LoadWith<AlgorithmUser>(u => u.User);
        okb.LoadOptions = dlo;
        return okb.Algorithms.Single(a => a.Id == id);
      }
    }

    /// <summary>
    /// Gets the complete problem object graph up to the following entities:
    /// <list type="bullet">
    ///   <item>Platform</item>
    ///   <item>SolutionRepresentation</item>
    ///   <item>Problem_Parameters.Parameter</item>
    ///   <item>IntProblemCharacteristicValues.ProblemCharacteristic.DataType</item>
    ///   <item>FloatProblemCharacteristicValues.ProblemCharacteristic.DataType</item>
    ///   <item>CharProblemCharacteristicValues.ProblemCharacteristic.DataType</item>
    /// </list>
    /// </summary>
    /// <param name="id">The problem id.</param>
    /// <returns>A <see cref="Problem"/></returns>
    public Problem GetCompleteProblem(int id) {
      using (OKBDataContext okb = new OKBDataContext()) {
        var dlo = new DataLoadOptions();
        dlo.LoadWith<Problem>(p => p.ProblemClass);
        dlo.LoadWith<Problem>(p => p.Platform);
        dlo.LoadWith<Problem>(p => p.ProblemUsers);
        dlo.LoadWith<ProblemUser>(u => u.User);
        okb.LoadOptions = dlo;
        return okb.Problems.Single(p => p.Id == id);
      }
    }

    /// <summary>
    /// Updates the algorithm object graph including the following properties and linked entitites:
    /// <list type="bullet">
    ///   <item>Name</item>
    ///   <item>Description</item>
    ///   <item>AlgorithmClassId</item>
    ///   <item>PlatformId</item>
    ///   <item>Algorithm_Parameters</item>
    ///   <item>Algorithm_Results</item>
    /// </list>
    /// <remarks>
    /// New <see cref="Parameter"/>s or <see cref="Result"/>s will not be
    /// created but have to be pre-existing.
    /// </remarks>
    /// </summary>
    /// <param name="algorithm">The algorithm.</param>
    public void UpdateCompleteAlgorithm(Algorithm algorithm) {
      using (OKBDataContext okb = new OKBDataContext()) {
        Algorithm original = okb.Algorithms.Single(a => a.Id == algorithm.Id);
        UpdateAlgorithmData(algorithm, original, okb);
        okb.SubmitChanges();
      }
    }

    /// <summary>
    /// Updates the problem object graph including the following properties and linked entities:
    /// <list type="bullet">
    ///   <item>Name</item>
    ///   <item>Description</item>
    ///   <item>ProblemClassId</item>
    ///   <item>PlatformId</item>
    ///   <item>SolutionRepresentationId</item>
    ///   <item>IntProblemCharacteristicValues.Value</item>
    ///   <item>FloatProblemCharacteristicValues.Value</item>
    ///   <item>CharProblemCharacteristicValues.Value</item>
    ///   <item>Problem_Parameters</item>
    /// </list>
    /// <remarks>
    /// New <see cref="ProblemCharacteristic"/>s or <see cref="Parameter"/>s will
    /// not be created but have to be pre-existing.
    /// </remarks>
    /// </summary>
    /// <param name="problem">The problem.</param>
    public void UpdateCompleteProblem(Problem problem) {
      using (OKBDataContext okb = new OKBDataContext()) {
        Problem originalProblem = okb.Problems.Single(p => p.Id == problem.Id);
        UpdateProblemData(problem, originalProblem, okb);
        okb.SubmitChanges();
      }
    }

    private static void UpdateAlgorithmData(Algorithm algorithm, Algorithm original, OKBDataContext okb) {
      original.Name = algorithm.Name;
      original.Description = algorithm.Description;
      original.AlgorithmClassId = algorithm.AlgorithmClassId;
      original.PlatformId = algorithm.PlatformId;
      okb.AlgorithmUsers.DeleteAllOnSubmit(original.AlgorithmUsers);
      original.AlgorithmUsers.Clear();
      foreach (var u in algorithm.AlgorithmUsers) {
        original.AlgorithmUsers.Add(new AlgorithmUser() {
          AlgorithmId = original.Id,
          UserId = u.UserId
        });
      }
    }

    private static void UpdateProblemData(Problem problem, Problem original, OKBDataContext okb) {
      original.Name = problem.Name;
      original.Description = problem.Description;
      original.ProblemClassId = problem.ProblemClassId;
      original.SolutionRepresentationId = problem.SolutionRepresentationId;
      original.PlatformId = problem.PlatformId;
      okb.ProblemUsers.DeleteAllOnSubmit(original.ProblemUsers);
      original.ProblemUsers.Clear();
      foreach (var u in problem.ProblemUsers) {
        original.ProblemUsers.Add(new ProblemUser() {
          ProblemId = original.Id,
          UserId = u.UserId
        });
      }
    }*/
  }
}
