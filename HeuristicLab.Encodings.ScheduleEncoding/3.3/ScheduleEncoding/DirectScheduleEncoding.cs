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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.ScheduleEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.Schedule {
  [StorableClass]
  public sealed class DirectScheduleEncoding : ScheduleEncoding {
    [StorableConstructor]
    private DirectScheduleEncoding(bool deserializing) : base(deserializing) { }
    private DirectScheduleEncoding(DirectScheduleEncoding original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new DirectScheduleEncoding(this, cloner);
    }

    public DirectScheduleEncoding()
      : base() {
      SolutionCreator = new DirectScheduleRandomCreator();
      DiscoverOperators();
    }

    #region Operator Discovery
    private static readonly IEnumerable<Type> encodingSpecificOperatorTypes;
    static DirectScheduleEncoding() {
      encodingSpecificOperatorTypes = new List<Type>() {
          typeof (IDirectScheduleOperator)
      };
    }
    private void DiscoverOperators() {
      var assembly = typeof(IDirectScheduleOperator).Assembly;
      var discoveredTypes = ApplicationManager.Manager.GetTypes(encodingSpecificOperatorTypes, assembly, true, false, false);
      var operators = discoveredTypes.Select(t => (IOperator)Activator.CreateInstance(t));
      var newOperators = operators.Except(Operators, new TypeEqualityComparer<IOperator>()).ToList();

      ConfigureOperators(newOperators);
      foreach (var @operator in newOperators)
        AddOperator(@operator);
    }
    #endregion

    public override void ConfigureOperators(IEnumerable<IItem> operators) {
      base.ConfigureOperators(operators);
      ConfigureDirectScheduleOperators(operators.OfType<IDirectScheduleOperator>());
    }

    private void ConfigureDirectScheduleOperators(IEnumerable<IDirectScheduleOperator> operators) {
      foreach (var @operator in operators)
        @operator.JobDataParameter.ActualName = JobDataParameter.Name;
    }
  }
}
