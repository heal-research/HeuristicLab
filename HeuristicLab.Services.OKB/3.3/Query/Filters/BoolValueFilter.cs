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
using System.Linq;
using System.Linq.Expressions;
using HeuristicLab.Services.OKB.DataAccess;

namespace HeuristicLab.Services.OKB.Query.Filters {
  public class BoolValueFilter : IFilter {
    public string Name { get; protected set; }
    public DataTransfer.EqualityComparison Comparison { get; set; }
    public bool Value { get; set; }

    public Expression<Func<Run, bool>> Expression {
      get {
        switch (Comparison) {
          case DataTransfer.EqualityComparison.Equal:
            return x => x.Values.Any(y => (y.ValueName.Name == Name) && (y.BoolValue == Value));
          case DataTransfer.EqualityComparison.NotEqual:
            return x => x.Values.Any(y => (y.ValueName.Name == Name) && (y.BoolValue != Value));
          default:
            return x => true;
        }
      }
    }

    public BoolValueFilter(string name, DataTransfer.EqualityComparison comparison, bool value) {
      Name = name;
      Comparison = comparison;
      Value = value;
    }
    public BoolValueFilter(DataTransfer.NameEqualityComparisonBoolFilter filter) {
      Name = filter.Name;
      Comparison = filter.Comparison;
      Value = filter.Value;
    }
  }
}
