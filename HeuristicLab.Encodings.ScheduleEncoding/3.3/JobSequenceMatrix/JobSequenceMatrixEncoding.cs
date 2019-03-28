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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [StorableType("9C090369-0214-42E6-8C3E-369751F5A9E1")]
  public sealed class JobSequenceMatrixEncoding : ScheduleEncoding<JSMEncoding> {
    [StorableConstructor]
    private JobSequenceMatrixEncoding(StorableConstructorFlag _) : base(_) { }
    private JobSequenceMatrixEncoding(JobSequenceMatrixEncoding original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new JobSequenceMatrixEncoding(this, cloner);
    }

    public JobSequenceMatrixEncoding()
      : base() {
      SolutionCreator = new JSMRandomCreator();
      Decoder = new JSMDecoder();
      DiscoverOperators();
    }

    #region Operator Discovery
    private static readonly IEnumerable<Type> encodingSpecificOperatorTypes;
    static JobSequenceMatrixEncoding() {
      encodingSpecificOperatorTypes = new List<Type>() {
          typeof (IJSMOperator)
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
  }
}
