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
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [StorableType("9C419EE5-F3A8-4F06-8263-7D37D3AE1C72")]
  public sealed class PriorityRulesVectorEncoding : ScheduleEncoding<PRVEncoding> {

    private IFixedValueParameter<IntValue> numberOfRulesParameter;
    public IFixedValueParameter<IntValue> NumberOfRulesParameter {
      get { return numberOfRulesParameter; }
      set {
        if (value == null) throw new ArgumentNullException("Number of Rules parameter must not be null.");
        if (value.Value == null) throw new ArgumentNullException("Number of Rules parameter value must not be null.");
        if (numberOfRulesParameter == value) return;

        if (numberOfRulesParameter != null) Parameters.Remove(numberOfRulesParameter);
        numberOfRulesParameter = value;
        Parameters.Add(numberOfRulesParameter);
        OnNumberOfRulesParameterChanged();
      }
    }


    [StorableConstructor]
    private PriorityRulesVectorEncoding(StorableConstructorFlag _) : base(_) { }
    private PriorityRulesVectorEncoding(PriorityRulesVectorEncoding original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PriorityRulesVectorEncoding(this, cloner);
    }

    public PriorityRulesVectorEncoding()
      : base() {
      //TODO change to meaningful value
      numberOfRulesParameter = new FixedValueParameter<IntValue>(Name + ".NumberOfRules", new IntValue(10));
      Parameters.Add(numberOfRulesParameter);

      SolutionCreator = new PRVRandomCreator();
      Decoder = new PRVDecoder();
      DiscoverOperators();
    }

    private void OnNumberOfRulesParameterChanged() {
      ConfigureOperators(Operators);
    }

    #region Operator Discovery
    private static readonly IEnumerable<Type> encodingSpecificOperatorTypes;
    static PriorityRulesVectorEncoding() {
      encodingSpecificOperatorTypes = new List<Type>() {
          typeof (IPRVOperator)
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

    public override void ConfigureOperators(IEnumerable<IItem> operators) {
      base.ConfigureOperators(operators);
      ConfigureRulesParameter(operators.OfType<IPRVRulesOperator>());
    }

    private void ConfigureRulesParameter(IEnumerable<IPRVRulesOperator> rulesOperators) {
      foreach (var rulesOperator in rulesOperators)
        rulesOperator.NumberOfRulesParameter.ActualName = numberOfRulesParameter.Name;
    }

    #endregion
  }
}
