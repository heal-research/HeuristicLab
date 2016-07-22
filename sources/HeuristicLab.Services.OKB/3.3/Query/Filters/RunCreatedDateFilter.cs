#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class RunCreatedDateFilter : IFilter {
    public DataTransfer.OrdinalComparison Comparison { get; set; }
    public DateTime Value { get; set; }

    public Expression<Func<Run, bool>> Expression {
      get {
        switch (Comparison) {
          case DataTransfer.OrdinalComparison.Less:
            return x => x.CreatedDate < Value;
          case DataTransfer.OrdinalComparison.LessOrEqual:
            return x => x.CreatedDate <= Value;
          case DataTransfer.OrdinalComparison.Equal:
            return x => x.CreatedDate == Value;
          case DataTransfer.OrdinalComparison.GreaterOrEqual:
            return x => x.CreatedDate >= Value;
          case DataTransfer.OrdinalComparison.Greater:
            return x => x.CreatedDate > Value;
          case DataTransfer.OrdinalComparison.NotEqual:
            return x => x.CreatedDate != Value;
          default:
            return x => true;
        }
      }
    }

    public RunCreatedDateFilter(DataTransfer.OrdinalComparison comparison, DateTime value) {
      Comparison = comparison;
      Value = value;
    }
    public RunCreatedDateFilter(DataTransfer.OrdinalComparisonDateTimeFilter filter) {
      Comparison = filter.Comparison;
      Value = filter.Value;
    }
  }
}
