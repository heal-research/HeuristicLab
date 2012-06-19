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
using System.Linq.Expressions;
using HeuristicLab.Services.OKB.DataAccess;

namespace HeuristicLab.Services.OKB.Query.Filters {
  public class RunClientNameFilter : IFilter {
    public DataTransfer.StringComparison Comparison { get; set; }
    public string Value { get; set; }

    public Expression<Func<Run, bool>> Expression {
      get {
        switch (Comparison) {
          case DataTransfer.StringComparison.Equal:
            throw new NotSupportedException();
          case DataTransfer.StringComparison.NotEqual:
            throw new NotSupportedException();
          case DataTransfer.StringComparison.Contains:
            throw new NotSupportedException();
          case DataTransfer.StringComparison.NotContains:
            throw new NotSupportedException();
          case DataTransfer.StringComparison.Like:
            throw new NotSupportedException();
          case DataTransfer.StringComparison.NotLike:
            throw new NotSupportedException();
          default:
            return x => true;
        }
      }
    }

    public RunClientNameFilter(DataTransfer.StringComparison comparison, string value) {
      Comparison = comparison;
      Value = value;
    }
    public RunClientNameFilter(DataTransfer.StringComparisonFilter filter) {
      Comparison = filter.Comparison;
      Value = filter.Value;
    }
  }
}
