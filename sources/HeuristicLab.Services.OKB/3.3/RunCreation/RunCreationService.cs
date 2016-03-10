#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

    public IEnumerable<DataTransfer.Solution> GetSolutions(long problemId) {
      roleVerifier.AuthenticateForAnyRole(OKBRoles.OKBAdministrator, OKBRoles.OKBUser);

      using (OKBDataContext okb = new OKBDataContext()) {
        var problem = okb.Problems.SingleOrDefault(x => x.Id == problemId);
        if (problem == null) throw new FaultException<MissingProblem>(new MissingProblem(problemId));
        // TODO: In case of multi-objective problems one has to check whether it contains single- or multi-objective problems
        var result = problem.SingleObjectiveSolutions.Select(x => Convert.ToDto(x)).ToList();
        if (roleVerifier.IsInRole(OKBRoles.OKBAdministrator)) {
          return result;
        } else {
          var problemUsers = okb.ProblemUsers.Where(x => x.ProblemId == problemId).ToList();
          if (problemUsers.Count == 0 || userManager.VerifyUser(userManager.CurrentUserId, problemUsers.Select(y => y.UserGroupId).ToList())) {
            return result;
          } else {
            return null;
          }
        }
      }
    }

    public byte[] GetSolutionData(long solutionId) {
      roleVerifier.AuthenticateForAnyRole(OKBRoles.OKBAdministrator, OKBRoles.OKBUser);

      using (OKBDataContext okb = new OKBDataContext()) {
        var solution = okb.SingleObjectiveSolutions.SingleOrDefault(x => x.Id == solutionId);
        if (solution == null) throw new FaultException<MissingSolution>(new MissingSolution(solutionId));

        var result = solution.BinaryData.Data.ToArray();
        if (roleVerifier.IsInRole(OKBRoles.OKBAdministrator)) {
          return result;
        } else {
          var problemUsers = okb.ProblemUsers.Where(x => x.ProblemId == solution.ProblemId).ToList();
          if (problemUsers.Count == 0 || userManager.VerifyUser(userManager.CurrentUserId, problemUsers.Select(y => y.UserGroupId).ToList())) {
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

    public IEnumerable<DataTransfer.Value> GetCharacteristicValues(long problemId) {
      using (OKBDataContext okb = new OKBDataContext()) {
        var prob = okb.Problems.SingleOrDefault(x => x.Id == problemId);
        if (prob == null) return Enumerable.Empty<DataTransfer.Value>();
        return prob.CharacteristicValues.Select(Convert.ToDto).ToArray();
      }
    }

    public void SetCharacteristicValue(long problemId, DataTransfer.Value value) {
      roleVerifier.AuthenticateForAnyRole(OKBRoles.OKBAdministrator, OKBRoles.OKBUser);

      using (OKBDataContext okb = new OKBDataContext()) {
        var problem = okb.Problems.SingleOrDefault(x => x.Id == problemId);
        if (problem == null) throw new FaultException<MissingProblem>(new MissingProblem(problemId));

        DoSetCharacteristicValue(okb, problem, value);
        okb.SubmitChanges();
      }
    }

    public void SetCharacteristicValues(long problemId, DataTransfer.Value[] values) {
      roleVerifier.AuthenticateForAnyRole(OKBRoles.OKBAdministrator, OKBRoles.OKBUser);

      using (OKBDataContext okb = new OKBDataContext()) {
        var problem = okb.Problems.SingleOrDefault(x => x.Id == problemId);
        if (problem == null) throw new FaultException<MissingProblem>(new MissingProblem(problemId));

        foreach (var v in values) {
          DoSetCharacteristicValue(okb, problem, v);
        }
        okb.SubmitChanges();
      }
    }

    private void DoSetCharacteristicValue(OKBDataContext okb, Problem problem, DataTransfer.Value value) {
      CharacteristicType characteristicType;
      try {
        characteristicType = GetCharacteristicType(value);
      } catch (ArgumentException ex) {
        throw new FaultException<UnknownCharacteristicType>(new UnknownCharacteristicType(ex.Message));
      }

      var entity = problem.CharacteristicValues.SingleOrDefault(x => x.Characteristic.Name == value.Name && x.Characteristic.Type == characteristicType);
      if (entity != null) {
        // Update
        switch (characteristicType) {
          case CharacteristicType.Bool: entity.BoolValue = ((DataTransfer.BoolValue)value).Value; break;
          case CharacteristicType.Int: entity.IntValue = ((DataTransfer.IntValue)value).Value; break;
          case CharacteristicType.Long: entity.LongValue = ((DataTransfer.LongValue)value).Value; break;
          case CharacteristicType.Float: entity.FloatValue = ((DataTransfer.FloatValue)value).Value; break;
          case CharacteristicType.Double: entity.DoubleValue = ((DataTransfer.DoubleValue)value).Value; break;
          case CharacteristicType.Percent: entity.DoubleValue = ((DataTransfer.PercentValue)value).Value; break;
          case CharacteristicType.String: entity.StringValue = ((DataTransfer.StringValue)value).Value; break;
          case CharacteristicType.TimeSpan: entity.LongValue = ((DataTransfer.TimeSpanValue)value).Value; break;
        }
      } else {
        // Insert
        entity = Convert.ToEntity(value, okb, problem, characteristicType);
        okb.CharacteristicValues.InsertOnSubmit(entity);
      }

    }

    private CharacteristicType GetCharacteristicType(DataTransfer.Value source) {
      if (source is DataTransfer.BoolValue) {
        return CharacteristicType.Bool;
      } else if (source is DataTransfer.IntValue) {
        return CharacteristicType.Int;
      } else if (source is DataTransfer.TimeSpanValue) {
        return CharacteristicType.TimeSpan;
      } else if (source is DataTransfer.LongValue) {
        return CharacteristicType.Long;
      } else if (source is DataTransfer.FloatValue) {
        return CharacteristicType.Float;
      } else if (source is DataTransfer.DoubleValue) {
        return CharacteristicType.Double;
      } else if (source is DataTransfer.PercentValue) {
        return CharacteristicType.Percent;
      } else if (source is DataTransfer.StringValue) {
        return CharacteristicType.String;
      } else {
        throw new ArgumentException("Unknown characteristic type.", "source");
      }
    }
  }
}
