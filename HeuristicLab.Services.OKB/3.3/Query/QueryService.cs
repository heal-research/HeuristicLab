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
using System.Data.Linq;
using System.Linq;
using System.ServiceModel;
using HeuristicLab.Services.Access;
using HeuristicLab.Services.OKB.DataAccess;
using HeuristicLab.Services.OKB.Query.DataTransfer;
using HeuristicLab.Services.OKB.Query.Filters;

namespace HeuristicLab.Services.OKB.Query {
  /// <summary>
  /// Implementation of the OKB query service (interface <see cref="IQueryService"/>).
  /// </summary>
  [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
  public class QueryService : IQueryService {
    IRoleVerifier roleVerifier = AccessServiceLocator.Instance.RoleVerifier;
    IUserManager userManager = AccessServiceLocator.Instance.UserManager;

    public IEnumerable<Filter> GetFilters() {
      List<Filter> filters = new List<Filter>();
      using (OKBDataContext okb = new OKBDataContext()) {
        #region Run Filters
        filters.Add(new OrdinalComparisonDateTimeFilter(typeof(RunCreatedDateFilter).AssemblyQualifiedName, "Run Created Date"));
        filters.Add(new StringComparisonFilter(typeof(RunUserNameFilter).AssemblyQualifiedName, "Run User Name"));
        filters.Add(new StringComparisonFilter(typeof(RunClientNameFilter).AssemblyQualifiedName, "Run Client Name"));
        #endregion

        #region Parameter Filters
        filters.Add(new StringComparisonAvailableValuesFilter(
          typeof(ValueNameFilter).AssemblyQualifiedName,
          "Parameter Name",
          okb.ValueNames.Where(x => x.Category == ValueNameCategory.Parameter).Select(x => x.Name).Distinct().ToArray()
        ));
        foreach (string name in okb.ValueNames.Where(x => (x.Category == ValueNameCategory.Parameter) && (x.Type == ValueNameType.Binary)).Select(x => x.Name).Distinct())
          filters.Add(new NameStringComparisonAvailableValuesFilter(
            typeof(ValueDataTypeNameFilter).AssemblyQualifiedName,
            "Parameter " + name + " Value Data Type Name",
            name,
            okb.Values.Where(x => (x.ValueName.Name == name) && (x.ValueName.Category == ValueNameCategory.Parameter) && (x.ValueName.Type == ValueNameType.Binary)).Select(x => x.DataType.Name).Distinct().ToArray()
          ));
        foreach (string name in okb.ValueNames.Where(x => (x.Category == ValueNameCategory.Parameter) && (x.Type == ValueNameType.Bool)).Select(x => x.Name).Distinct())
          filters.Add(new NameEqualityComparisonBoolFilter(
            typeof(BoolValueFilter).AssemblyQualifiedName,
            "Parameter " + name + " Value",
            name
          ));
        foreach (string name in okb.ValueNames.Where(x => (x.Category == ValueNameCategory.Parameter) && (x.Type == ValueNameType.Int)).Select(x => x.Name).Distinct())
          filters.Add(new NameOrdinalComparisonIntFilter(
            typeof(IntValueFilter).AssemblyQualifiedName,
            "Parameter " + name + " Value",
            name
          ));
        foreach (string name in okb.ValueNames.Where(x => (x.Category == ValueNameCategory.Parameter) && (x.Type == ValueNameType.Long)).Select(x => x.Name).Distinct())
          filters.Add(new NameOrdinalComparisonLongFilter(
            typeof(LongValueFilter).AssemblyQualifiedName,
            "Parameter " + name + " Value",
            name
          ));
        foreach (string name in okb.ValueNames.Where(x => (x.Category == ValueNameCategory.Parameter) && (x.Type == ValueNameType.TimeSpan)).Select(x => x.Name).Distinct())
          filters.Add(new NameOrdinalComparisonTimeSpanFilter(
            typeof(TimeSpanValueFilter).AssemblyQualifiedName,
            "Parameter " + name + " Value",
            name
          ));
        foreach (string name in okb.ValueNames.Where(x => (x.Category == ValueNameCategory.Parameter) && (x.Type == ValueNameType.Float)).Select(x => x.Name).Distinct())
          filters.Add(new NameOrdinalComparisonFloatFilter(
            typeof(FloatValueFilter).AssemblyQualifiedName,
            "Parameter " + name + " Value",
            name
          ));
        foreach (string name in okb.ValueNames.Where(x => (x.Category == ValueNameCategory.Parameter) && (x.Type == ValueNameType.Double)).Select(x => x.Name).Distinct())
          filters.Add(new NameOrdinalComparisonDoubleFilter(
            typeof(DoubleValueFilter).AssemblyQualifiedName,
            "Parameter " + name + " Value",
            name
          ));
        foreach (string name in okb.ValueNames.Where(x => (x.Category == ValueNameCategory.Parameter) && (x.Type == ValueNameType.Percent)).Select(x => x.Name).Distinct())
          filters.Add(new NameOrdinalComparisonPercentFilter(
            typeof(PercentValueFilter).AssemblyQualifiedName,
            "Parameter " + name + " Value",
            name
          ));
        foreach (string name in okb.ValueNames.Where(x => (x.Category == ValueNameCategory.Parameter) && (x.Type == ValueNameType.String)).Select(x => x.Name).Distinct())
          filters.Add(new NameStringComparisonFilter(
            typeof(StringValueFilter).AssemblyQualifiedName,
            "Parameter " + name + " Value",
            name
          ));
        foreach (string name in okb.ValueNames.Where(x => (x.Category == ValueNameCategory.Parameter) && (x.Type == ValueNameType.Binary)).Select(x => x.Name).Distinct())
          filters.Add(new NameEqualityComparisonByteArrayFilter(
            typeof(BinaryValueFilter).AssemblyQualifiedName,
            "Parameter " + name + " Value",
            name
          ));
        #endregion

        #region Result Filters
        filters.Add(new StringComparisonAvailableValuesFilter(
          typeof(ValueNameFilter).AssemblyQualifiedName,
          "Result Name",
          okb.ValueNames.Where(x => x.Category == ValueNameCategory.Result).Select(x => x.Name).Distinct().ToArray()
        ));
        foreach (string name in okb.ValueNames.Where(x => (x.Category == ValueNameCategory.Result) && (x.Type == ValueNameType.Binary)).Select(x => x.Name).Distinct())
          filters.Add(new NameStringComparisonAvailableValuesFilter(
            typeof(ValueDataTypeNameFilter).AssemblyQualifiedName,
            "Result " + name + " Value Data Type Name",
            name,
            okb.Values.Where(x => (x.ValueName.Name == name) && (x.ValueName.Category == ValueNameCategory.Result) && (x.ValueName.Type == ValueNameType.Binary)).Select(x => x.DataType.Name).Distinct().ToArray()
          ));
        foreach (string name in okb.ValueNames.Where(x => (x.Category == ValueNameCategory.Result) && (x.Type == ValueNameType.Bool)).Select(x => x.Name).Distinct())
          filters.Add(new NameEqualityComparisonBoolFilter(
            typeof(BoolValueFilter).AssemblyQualifiedName,
            "Result " + name + " Value",
            name
          ));
        foreach (string name in okb.ValueNames.Where(x => (x.Category == ValueNameCategory.Result) && (x.Type == ValueNameType.Int)).Select(x => x.Name).Distinct())
          filters.Add(new NameOrdinalComparisonIntFilter(
            typeof(IntValueFilter).AssemblyQualifiedName,
            "Result " + name + " Value",
            name
          ));
        foreach (string name in okb.ValueNames.Where(x => (x.Category == ValueNameCategory.Result) && (x.Type == ValueNameType.Long)).Select(x => x.Name).Distinct())
          filters.Add(new NameOrdinalComparisonLongFilter(
            typeof(LongValueFilter).AssemblyQualifiedName,
            "Result " + name + " Value",
            name
          ));
        foreach (string name in okb.ValueNames.Where(x => (x.Category == ValueNameCategory.Result) && (x.Type == ValueNameType.TimeSpan)).Select(x => x.Name).Distinct())
          filters.Add(new NameOrdinalComparisonTimeSpanFilter(
            typeof(TimeSpanValueFilter).AssemblyQualifiedName,
            "Result " + name + " Value",
            name
          ));
        foreach (string name in okb.ValueNames.Where(x => (x.Category == ValueNameCategory.Result) && (x.Type == ValueNameType.Float)).Select(x => x.Name).Distinct())
          filters.Add(new NameOrdinalComparisonFloatFilter(
            typeof(FloatValueFilter).AssemblyQualifiedName,
            "Result " + name + " Value",
            name
          ));
        foreach (string name in okb.ValueNames.Where(x => (x.Category == ValueNameCategory.Result) && (x.Type == ValueNameType.Double)).Select(x => x.Name).Distinct())
          filters.Add(new NameOrdinalComparisonDoubleFilter(
            typeof(DoubleValueFilter).AssemblyQualifiedName,
            "Result " + name + " Value",
            name
          ));
        foreach (string name in okb.ValueNames.Where(x => (x.Category == ValueNameCategory.Result) && (x.Type == ValueNameType.Percent)).Select(x => x.Name).Distinct())
          filters.Add(new NameOrdinalComparisonPercentFilter(
            typeof(PercentValueFilter).AssemblyQualifiedName,
            "Result " + name + " Value",
            name
          ));
        foreach (string name in okb.ValueNames.Where(x => (x.Category == ValueNameCategory.Result) && (x.Type == ValueNameType.String)).Select(x => x.Name).Distinct())
          filters.Add(new NameStringComparisonFilter(
            typeof(StringValueFilter).AssemblyQualifiedName,
            "Result " + name + " Value",
            name
          ));
        foreach (string name in okb.ValueNames.Where(x => (x.Category == ValueNameCategory.Result) && (x.Type == ValueNameType.Binary)).Select(x => x.Name).Distinct())
          filters.Add(new NameEqualityComparisonByteArrayFilter(
            typeof(BinaryValueFilter).AssemblyQualifiedName,
            "Result " + name + " Value",
            name
          ));
        #endregion

        #region Algorithm Filters
        filters.Add(new StringComparisonAvailableValuesFilter(
          typeof(AlgorithmNameFilter).AssemblyQualifiedName,
          "Algorithm Name",
          okb.Algorithms.Select(x => x.Name).Distinct().ToArray()
        ));
        filters.Add(new StringComparisonAvailableValuesFilter(
          typeof(AlgorithmClassNameFilter).AssemblyQualifiedName,
          "Algorithm Class Name",
          okb.AlgorithmClasses.Select(x => x.Name).Distinct().ToArray()
        ));
        filters.Add(new StringComparisonAvailableValuesFilter(
          typeof(AlgorithmPlatformNameFilter).AssemblyQualifiedName,
          "Algorithm Platform Name",
          okb.Platforms.Select(x => x.Name).Distinct().ToArray()
        ));
        filters.Add(new StringComparisonAvailableValuesFilter(
          typeof(AlgorithmDataTypeNameFilter).AssemblyQualifiedName,
          "Algorithm Data Type Name",
          okb.Algorithms.Select(x => x.DataType.Name).Distinct().ToArray()
        ));
        #endregion

        #region Problem Filters
        filters.Add(new StringComparisonAvailableValuesFilter(
          typeof(ProblemNameFilter).AssemblyQualifiedName,
          "Problem Name",
          okb.Problems.Select(x => x.Name).Distinct().ToArray()
        ));
        filters.Add(new StringComparisonAvailableValuesFilter(
          typeof(ProblemClassNameFilter).AssemblyQualifiedName,
          "Problem Class Name",
          okb.ProblemClasses.Select(x => x.Name).Distinct().ToArray()
        ));
        filters.Add(new StringComparisonAvailableValuesFilter(
          typeof(ProblemPlatformNameFilter).AssemblyQualifiedName,
          "Problem Platform Name",
          okb.Platforms.Select(x => x.Name).Distinct().ToArray()
        ));
        filters.Add(new StringComparisonAvailableValuesFilter(
          typeof(ProblemDataTypeNameFilter).AssemblyQualifiedName,
          "Problem Data Type Name",
          okb.Problems.Select(x => x.DataType.Name).Distinct().ToArray()
        ));
        #endregion

        #region Combined Filters
        filters.Add(new CombinedFilter(typeof(AndFilter).AssemblyQualifiedName, "AND", BooleanOperation.And));
        filters.Add(new CombinedFilter(typeof(OrFilter).AssemblyQualifiedName, "OR", BooleanOperation.Or));
        #endregion
      }
      return filters.OrderBy(x => x.Label);
    }

    public long GetNumberOfRuns(Filter filter) {
      using (OKBDataContext okb = new OKBDataContext()) {
        return FilterRuns(okb.Runs, filter, okb).LongCount();
      }
    }

    public IEnumerable<long> GetRunIds(Filter filter) {
      using (OKBDataContext okb = new OKBDataContext()) {
        return FilterRuns(okb.Runs, filter, okb).Select(x => x.Id).ToArray();
      }
    }

    public IEnumerable<DataTransfer.Run> GetRuns(IEnumerable<long> ids, bool includeBinaryValues) {
      using (OKBDataContext okb = new OKBDataContext()) {
        DataLoadOptions dlo = new DataLoadOptions();
        // TODO: specify LoadWiths
        okb.LoadOptions = dlo;

        return FilterUnauthorizedRuns(okb.Runs.Where(x => ids.Contains(x.Id)).ToList(), okb).Select(x => Convert.ToDto(x, includeBinaryValues)).ToArray();
      }
    }

    public IEnumerable<DataTransfer.Run> GetRunsWithValues(IEnumerable<long> ids, bool includeBinaryValues, IEnumerable<DataTransfer.ValueName> valueNames) {
      using (OKBDataContext okb = new OKBDataContext()) {
        DataLoadOptions dlo = new DataLoadOptions();
        // TODO: specify LoadWiths
        okb.LoadOptions = dlo;

        return FilterUnauthorizedRuns(okb.Runs.Where(x => ids.Contains(x.Id)).ToList(), okb).Select(x => Convert.ToDto(x, includeBinaryValues, valueNames)).ToArray();
      }
    }

    public IEnumerable<DataTransfer.ValueName> GetValueNames() {
      using (OKBDataContext okb = new OKBDataContext()) {
        return okb.ValueNames.Select(x => Convert.ToDto(x)).ToArray();
      }
    }

    private List<DataAccess.Run> FilterRuns(IQueryable<DataAccess.Run> runs, Filter filter, OKBDataContext okb) {
      IFilter f = (IFilter)Activator.CreateInstance(Type.GetType(filter.FilterTypeName), filter);

      var query = runs.Where(f.Expression);
      List<DataAccess.Run> results = new List<DataAccess.Run>();

      if (roleVerifier.IsInRole(OKBRoles.OKBAdministrator)) {
        results.AddRange(query);
      } else {
        foreach (DataAccess.Run r in query) {
          if (IsAuthorizedForAlgorithm(r.AlgorithmId, okb) && IsAuthorizedForProblem(r.ProblemId, okb)) {
            results.Add(r);
          }
        }
      }
      return results;
    }

    private List<DataAccess.Run> FilterUnauthorizedRuns(List<DataAccess.Run> runs, OKBDataContext okb) {
      List<DataAccess.Run> results = new List<DataAccess.Run>();

      if (roleVerifier.IsInRole(OKBRoles.OKBAdministrator)) {
        results.AddRange(runs);
      } else {
        foreach (DataAccess.Run r in runs) {
          if (IsAuthorizedForAlgorithm(r.AlgorithmId, okb) && IsAuthorizedForProblem(r.ProblemId, okb)) {
            results.Add(r);
          }
        }
      }
      return results;
    }

    private bool IsAuthorizedForAlgorithm(long algorithmId, OKBDataContext okb) {
      var algUsers = okb.AlgorithmUsers.Where(x => x.AlgorithmId == algorithmId).Select(x => x.UserGroupId).ToList();

      if (algUsers.Count == 0 || userManager.VerifyUser(userManager.CurrentUserId, algUsers)) {
        return true;
      }
      return false;
    }

    private bool IsAuthorizedForProblem(long problemId, OKBDataContext okb) {
      var problemUsers = okb.ProblemUsers.Where(x => x.ProblemId == problemId).Select(x => x.UserGroupId).ToList();

      if (problemUsers.Count == 0 || userManager.VerifyUser(userManager.CurrentUserId, problemUsers)) {
        return true;
      }
      return false;
    }
  }
}
