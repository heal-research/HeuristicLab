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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings;
using HeuristicLab.Problems.VehicleRouting.Encodings.Alba;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// An operator that improves vehicle routing solutions.
  /// </summary>
  [Item("VRPImprovementOperator", "An operator that improves vehicle routing solutions.")]
  [StorableClass]
  public sealed class VRPImprovementOperator : VRPOperator, IGeneralVRPOperator, ISingleObjectiveImprovementOperator {
    #region Parameter properties
    public ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IValueParameter<IntValue> ImprovementAttemptsParameter {
      get { return (IValueParameter<IntValue>)Parameters["ImprovementAttempts"]; }
    }
    public IValueParameter<IntValue> LambdaParameter {
      get { return (IValueParameter<IntValue>)Parameters["Lambda"]; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public IValueParameter<IntValue> SampleSizeParameter {
      get { return (IValueParameter<IntValue>)Parameters["SampleSize"]; }
    }
    public IValueLookupParameter<IItem> SolutionParameter {
      get { return (IValueLookupParameter<IItem>)Parameters["Solution"]; }
    }
    #endregion

    [StorableConstructor]
    private VRPImprovementOperator(bool deserializing) : base(deserializing) { }
    private VRPImprovementOperator(VRPImprovementOperator original, Cloner cloner) : base(original, cloner) { }
    public VRPImprovementOperator()
      : base() {
      #region Create parameters
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope that contains the solution to be improved."));
      Parameters.Add(new ValueParameter<IntValue>("ImprovementAttempts", "The number of improvement attempts the operator should perform.", new IntValue(100)));
      Parameters.Add(new ValueParameter<IntValue>("Lambda", "The number of neighbors that should be exchanged.", new IntValue(1)));
      Parameters.Add(new LookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueParameter<IntValue>("SampleSize", "The number of moves that should be executed.", new IntValue(100)));
      Parameters.Add(new ValueLookupParameter<IItem>("Solution", "The solution to be improved. This parameter is used for name translation only."));
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new VRPImprovementOperator(this, cloner);
    }

    public override IOperation Apply() {
      AlbaEncoding solution = SolutionParameter.ActualValue is AlbaEncoding
                                ? SolutionParameter.ActualValue as AlbaEncoding
                                : AlbaEncoding.ConvertFrom(SolutionParameter.ActualValue as IVRPEncoding, ProblemInstance);

      if (solution == null)
        throw new ArgumentException("Cannot improve solution because it has the wrong type.");

      double quality = -1;
      int evaluatedSolutions;

      AlbaLambdaInterchangeLocalImprovementOperator.Apply(solution, ImprovementAttemptsParameter.Value.Value,
                                                          LambdaParameter.Value.Value, SampleSizeParameter.Value.Value,
                                                          RandomParameter.ActualValue, ProblemInstance, ref quality,
                                                          out evaluatedSolutions);

      SolutionParameter.ActualValue = PotvinEncoding.ConvertFrom(solution, ProblemInstance);
      CurrentScopeParameter.ActualValue.Variables.Add(new Variable("LocalEvaluatedSolutions", new IntValue(evaluatedSolutions)));

      return base.Apply();
    }
  }
}
