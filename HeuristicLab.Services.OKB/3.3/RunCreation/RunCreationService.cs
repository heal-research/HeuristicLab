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

using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.ServiceModel;
using HeuristicLab.Services.Access;
using HeuristicLab.Services.OKB.DataAccess;

namespace HeuristicLab.Services.OKB.RunCreation {
  /// <summary>
  /// Implementation of the OKB run creation service (interface <see cref="IRunCreationService"/>).
  /// </summary>
  [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
  public class RunCreationService : IRunCreationService {
    IRoleVerifier roleVerifier = AccessServiceLocator.Instance.RoleVerifier;
    IUserManager userManager = AccessServiceLocator.Instance.UserManager;

    public IEnumerable<DataTransfer.Algorithm> GetAlgorithms(string platformName) {
      roleVerifier.AuthenticateForAnyRole(OKBRoles.OKBAdministrator, OKBRoles.OKBUser);

      using (OKBDataContext okb = new OKBDataContext()) {
        DataLoadOptions dlo = new DataLoadOptions();
        dlo.LoadWith<Algorithm>(x => x.AlgorithmClass);
        dlo.LoadWith<Algorithm>(x => x.DataType);
        dlo.LoadWith<Algorithm>(x => x.AlgorithmUsers);
        okb.LoadOptions = dlo;

        var query = okb.Algorithms.Where(x => x.Platform.Name == platformName);
        List<Algorithm> results = new List<Algorithm>();

        if (roleVerifier.IsInRole(OKBRoles.OKBAdministrator)) {
          results.AddRange(query);
        } else {
          foreach (var alg in query) {
            if (alg.AlgorithmUsers.Count() == 0 || userManager.VerifyUser(userManager.CurrentUserId, alg.AlgorithmUsers.Select(y => y.UserGroupId).ToList())) {
              results.Add(alg);
            }
          }
        }
        return results.Select(x => Convert.ToDto(x)).ToArray();
      }
    }

    public byte[] GetAlgorithmData(long algorithmId) {
      roleVerifier.AuthenticateForAnyRole(OKBRoles.OKBAdministrator, OKBRoles.OKBUser);

      using (OKBDataContext okb = new OKBDataContext()) {
        var result = okb.Algorithms.Where(x => x.Id == algorithmId).Select(x => x.BinaryData.Data.ToArray()).FirstOrDefault();

        if (roleVerifier.IsInRole(OKBRoles.OKBAdministrator)) {
          return result;
        } else {
          var algUsers = okb.AlgorithmUsers.Where(x => x.AlgorithmId == algorithmId);
          if (algUsers.Count() == 0 || userManager.VerifyUser(userManager.CurrentUserId, algUsers.Select(y => y.UserGroupId).ToList())) {
            return result;
          } else {
            return null;
          }
        }
      }
    }

    public IEnumerable<DataTransfer.Problem> GetProblems(string platformName) {
      roleVerifier.AuthenticateForAnyRole(OKBRoles.OKBAdministrator, OKBRoles.OKBUser);

      using (OKBDataContext okb = new OKBDataContext()) {
        DataLoadOptions dlo = new DataLoadOptions();
        dlo.LoadWith<Problem>(x => x.ProblemClass);
        dlo.LoadWith<Problem>(x => x.DataType);
        dlo.LoadWith<Problem>(x => x.ProblemUsers);
        okb.LoadOptions = dlo;

        var query = okb.Problems.Where(x => x.Platform.Name == platformName);
        List<Problem> results = new List<Problem>();

        if (roleVerifier.IsInRole(OKBRoles.OKBAdministrator)) {
          results.AddRange(query);
        } else {
          foreach (var problem in query) {
            if (problem.ProblemUsers.Count() == 0 || userManager.VerifyUser(userManager.CurrentUserId, problem.ProblemUsers.Select(y => y.UserGroupId).ToList())) {
              results.Add(problem);
            }
          }
        }
        return results.Select(x => Convert.ToDto(x)).ToArray();
      }
    }

    public byte[] GetProblemData(long problemId) {
      roleVerifier.AuthenticateForAnyRole(OKBRoles.OKBAdministrator, OKBRoles.OKBUser);

      using (OKBDataContext okb = new OKBDataContext()) {
        var result = okb.Problems.Where(x => x.Id == problemId).Select(x => x.BinaryData.Data.ToArray()).FirstOrDefault();

        if (roleVerifier.IsInRole(OKBRoles.OKBAdministrator)) {
          return result;
        } else {
          var problemUsers = okb.ProblemUsers.Where(x => x.ProblemId == problemId);
          if (problemUsers.Count() == 0 || userManager.VerifyUser(userManager.CurrentUserId, problemUsers.Select(y => y.UserGroupId).ToList())) {
            return result;
          } else {
            return null;
          }
        }
      }
    }

    public void AddRun(DataTransfer.Run run) {
      roleVerifier.AuthenticateForAnyRole(OKBRoles.OKBAdministrator, OKBRoles.OKBUser);

      using (OKBDataContext okb = new OKBDataContext()) {
        DataAccess.Run entity = Convert.ToEntity(run, okb);
        okb.Runs.InsertOnSubmit(entity);
        okb.SubmitChanges();
      }
    }
  }
}
