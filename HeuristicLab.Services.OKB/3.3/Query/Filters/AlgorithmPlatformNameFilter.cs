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
using System.Data.Linq.SqlClient;
using System.Linq.Expressions;
using HeuristicLab.Services.OKB.DataAccess;

namespace HeuristicLab.Services.OKB.Query.Filters {
  public class AlgorithmPlatformNameFilter : IFilter {
    public DataTransfer.StringComparison Comparison { get; set; }
    public string Value { get; set; }

    public Expression<Func<Run, bool>> Expression {
      get {
        switch (Comparison) {
          case DataTransfer.StringComparison.Equal:
            return x => x.Algorithm.Platform.Name == Value;
          case DataTransfer.StringComparison.NotEqual:
            return x => x.Algorithm.Platform.Name != Value;
          case DataTransfer.StringComparison.Contains:
            return x => x.Algorithm.Platform.Name.Contains(Value);
          case DataTransfer.StringComparison.NotContains:
            return x => !x.Algorithm.Platform.Name.Contains(Value);
          case DataTransfer.StringComparison.Like:
            return x => SqlMethods.Like(x.Algorithm.Platform.Name, Value);
          case DataTransfer.StringComparison.NotLike:
            return x => !SqlMethods.Like(x.Algorithm.Platform.Name, Value);
          default:
            return x => true;
        }
      }
    }

    public AlgorithmPlatformNameFilter(DataTransfer.StringComparison comparison, string value) {
      Comparison = comparison;
      Value = value;
    }
    public AlgorithmPlatformNameFilter(DataTransfer.StringComparisonFilter filter) {
      Comparison = filter.Comparison;
      Value = filter.Value;
    }
  }
}
