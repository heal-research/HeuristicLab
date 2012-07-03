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
using System.Linq;
using System.ServiceModel;
using HeuristicLab.Services.Access;
using HeuristicLab.Services.OKB.DataAccess;

namespace HeuristicLab.Services.OKB.Administration {
  /// <summary>
  /// Implementation of the OKB administration service (interface <see cref="IAdministrationService"/>).
  /// </summary>
  [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
  public class AdministrationService : IAdministrationService {
    IRoleVerifier roleVerifier = AccessServiceLocator.Instance.RoleVerifier;

    #region Platform Methods
    public DataTransfer.Platform GetPlatform(long id) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        return Convert.ToDto(okb.Platforms.FirstOrDefault(x => x.Id == id));
      }
    }
    public IEnumerable<DataTransfer.Platform> GetPlatforms() {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        return okb.Platforms.Select(x => Convert.ToDto(x)).ToArray();
      }
    }
    public long AddPlatform(DataTransfer.Platform dto) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        DataAccess.Platform entity = Convert.ToEntity(dto); entity.Id = 0;
        okb.Platforms.InsertOnSubmit(entity);
        okb.SubmitChanges();
        return entity.Id;
      }
    }
    public void UpdatePlatform(DataTransfer.Platform dto) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        DataAccess.Platform entity = okb.Platforms.FirstOrDefault(x => x.Id == dto.Id);
        Convert.ToEntity(dto, entity);
        okb.SubmitChanges();
      }
    }
    public void DeletePlatform(long id) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        DataAccess.Platform entity = okb.Platforms.FirstOrDefault(x => x.Id == id);
        if (entity != null) okb.Platforms.DeleteOnSubmit(entity);
        okb.SubmitChanges();
      }
    }
    #endregion

    #region AlgorithmClass Methods
    public DataTransfer.AlgorithmClass GetAlgorithmClass(long id) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        return Convert.ToDto(okb.AlgorithmClasses.FirstOrDefault(x => x.Id == id));
      }
    }
    public IEnumerable<DataTransfer.AlgorithmClass> GetAlgorithmClasses() {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        return okb.AlgorithmClasses.Select(x => Convert.ToDto(x)).ToArray();
      }
    }
    public long AddAlgorithmClass(DataTransfer.AlgorithmClass dto) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        DataAccess.AlgorithmClass entity = Convert.ToEntity(dto); entity.Id = 0;
        okb.AlgorithmClasses.InsertOnSubmit(entity);
        okb.SubmitChanges();
        return entity.Id;
      }
    }
    public void UpdateAlgorithmClass(DataTransfer.AlgorithmClass dto) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        DataAccess.AlgorithmClass entity = okb.AlgorithmClasses.FirstOrDefault(x => x.Id == dto.Id);
        Convert.ToEntity(dto, entity);
        okb.SubmitChanges();
      }
    }
    public void DeleteAlgorithmClass(long id) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        DataAccess.AlgorithmClass entity = okb.AlgorithmClasses.FirstOrDefault(x => x.Id == id);
        if (entity != null) okb.AlgorithmClasses.DeleteOnSubmit(entity);
        okb.SubmitChanges();
      }
    }
    #endregion

    #region Algorithm Methods
    public DataTransfer.Algorithm GetAlgorithm(long id) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        return Convert.ToDto(okb.Algorithms.FirstOrDefault(x => x.Id == id));
      }
    }
    public IEnumerable<DataTransfer.Algorithm> GetAlgorithms() {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        return okb.Algorithms.Select(x => Convert.ToDto(x)).ToArray();
      }
    }
    public long AddAlgorithm(DataTransfer.Algorithm dto) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        DataAccess.Algorithm entity = Convert.ToEntity(dto, okb); entity.Id = 0;
        okb.Algorithms.InsertOnSubmit(entity);
        okb.SubmitChanges();
        return entity.Id;
      }
    }
    public void UpdateAlgorithm(DataTransfer.Algorithm dto) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        DataAccess.Algorithm entity = okb.Algorithms.FirstOrDefault(x => x.Id == dto.Id);
        Convert.ToEntity(dto, entity, okb);
        okb.SubmitChanges();
      }
    }
    public void DeleteAlgorithm(long id) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        DataAccess.Algorithm entity = okb.Algorithms.FirstOrDefault(x => x.Id == id);
        if (entity != null) okb.Algorithms.DeleteOnSubmit(entity);
        okb.SubmitChanges();
      }
    }
    public IEnumerable<Guid> GetAlgorithmUsers(long algorithmId) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        return okb.AlgorithmUsers.Where(x => x.AlgorithmId == algorithmId).Select(x => x.UserGroupId).ToArray();
      }
    }
    public void UpdateAlgorithmUsers(long algorithmId, IEnumerable<Guid> users) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        okb.AlgorithmUsers.DeleteAllOnSubmit(okb.AlgorithmUsers.Where(x => x.AlgorithmId == algorithmId));
        okb.AlgorithmUsers.InsertAllOnSubmit(users.Select(x => new DataAccess.AlgorithmUser { AlgorithmId = algorithmId, UserGroupId = x }));
        okb.SubmitChanges();
      }
    }
    public byte[] GetAlgorithmData(long algorithmId) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        var data = okb.Algorithms.Where(x => x.Id == algorithmId).Select(x => x.BinaryData).FirstOrDefault();
        if (data != null) return data.Data.ToArray();
        else return null;
      }
    }
    public void UpdateAlgorithmData(long algorithmId, byte[] data) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        var entity = okb.Algorithms.Where(x => x.Id == algorithmId).FirstOrDefault();
        if (entity != null)
          entity.BinaryData = Convert.ToEntity(data, okb);
        okb.SubmitChanges();
      }
    }
    #endregion

    #region ProblemClass Methods
    public DataTransfer.ProblemClass GetProblemClass(long id) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        return Convert.ToDto(okb.ProblemClasses.FirstOrDefault(x => x.Id == id));
      }
    }
    public IEnumerable<DataTransfer.ProblemClass> GetProblemClasses() {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        return okb.ProblemClasses.Select(x => Convert.ToDto(x)).ToArray();
      }
    }
    public long AddProblemClass(DataTransfer.ProblemClass dto) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        DataAccess.ProblemClass entity = Convert.ToEntity(dto); entity.Id = 0;
        okb.ProblemClasses.InsertOnSubmit(entity);
        okb.SubmitChanges();
        return entity.Id;
      }
    }
    public void UpdateProblemClass(DataTransfer.ProblemClass dto) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        DataAccess.ProblemClass entity = okb.ProblemClasses.FirstOrDefault(x => x.Id == dto.Id);
        Convert.ToEntity(dto, entity);
        okb.SubmitChanges();
      }
    }
    public void DeleteProblemClass(long id) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        DataAccess.ProblemClass entity = okb.ProblemClasses.FirstOrDefault(x => x.Id == id);
        if (entity != null) okb.ProblemClasses.DeleteOnSubmit(entity);
        okb.SubmitChanges();
      }
    }
    #endregion

    #region Problem Methods
    public DataTransfer.Problem GetProblem(long id) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        return Convert.ToDto(okb.Problems.FirstOrDefault(x => x.Id == id));
      }
    }
    public IEnumerable<DataTransfer.Problem> GetProblems() {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        return okb.Problems.Select(x => Convert.ToDto(x)).ToArray();
      }
    }
    public long AddProblem(DataTransfer.Problem dto) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        DataAccess.Problem entity = Convert.ToEntity(dto, okb); entity.Id = 0;
        okb.Problems.InsertOnSubmit(entity);
        okb.SubmitChanges();
        return entity.Id;
      }
    }
    public void UpdateProblem(DataTransfer.Problem dto) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        DataAccess.Problem entity = okb.Problems.FirstOrDefault(x => x.Id == dto.Id);
        Convert.ToEntity(dto, entity, okb);
        okb.SubmitChanges();
      }
    }
    public void DeleteProblem(long id) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        DataAccess.Problem entity = okb.Problems.FirstOrDefault(x => x.Id == id);
        if (entity != null) okb.Problems.DeleteOnSubmit(entity);
        okb.SubmitChanges();
      }
    }
    public IEnumerable<Guid> GetProblemUsers(long problemId) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        return okb.ProblemUsers.Where(x => x.ProblemId == problemId).Select(x => x.UserGroupId).ToArray();
      }
    }
    public void UpdateProblemUsers(long problemId, IEnumerable<Guid> users) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        okb.ProblemUsers.DeleteAllOnSubmit(okb.ProblemUsers.Where(x => x.ProblemId == problemId));
        okb.ProblemUsers.InsertAllOnSubmit(users.Select(x => new DataAccess.ProblemUser { ProblemId = problemId, UserGroupId = x }));
        okb.SubmitChanges();
      }
    }
    public byte[] GetProblemData(long problemId) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        var data = okb.Problems.Where(x => x.Id == problemId).Select(x => x.BinaryData).FirstOrDefault();
        if (data != null) return data.Data.ToArray();
        else return null;
      }
    }
    public void UpdateProblemData(long problemId, byte[] data) {
      roleVerifier.AuthenticateForAllRoles(OKBRoles.OKBAdministrator);

      using (OKBDataContext okb = new OKBDataContext()) {
        var entity = okb.Problems.Where(x => x.Id == problemId).FirstOrDefault();
        if (entity != null)
          entity.BinaryData = Convert.ToEntity(data, okb);
        okb.SubmitChanges();
      }
    }
    #endregion
  }
}
