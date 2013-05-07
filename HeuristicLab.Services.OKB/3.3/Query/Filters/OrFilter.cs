#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq.Expressions;
using HeuristicLab.Services.OKB.DataAccess;

namespace HeuristicLab.Services.OKB.Query.Filters {
  public class OrFilter : IFilter {
    public List<IFilter> Filters { get; private set; }

    public Expression<Func<Run, bool>> Expression {
      get {
        ParameterExpression parameter = System.Linq.Expressions.Expression.Parameter(typeof(Run));
        Expression expr = System.Linq.Expressions.Expression.Constant(false);
        foreach (IFilter filter in Filters)
          expr = System.Linq.Expressions.Expression.OrElse(expr, System.Linq.Expressions.Expression.Invoke(filter.Expression, parameter));
        return System.Linq.Expressions.Expression.Lambda<Func<Run, bool>>(expr, parameter);
      }
    }

    public OrFilter(IEnumerable<IFilter> filters) {
      Filters = new List<IFilter>(filters);
    }
    public OrFilter(DataTransfer.CombinedFilter filter) {
      Filters = new List<IFilter>(filter.Filters.Select(x => (IFilter)Activator.CreateInstance(Type.GetType(x.FilterTypeName), x)));
    }
  }
}
