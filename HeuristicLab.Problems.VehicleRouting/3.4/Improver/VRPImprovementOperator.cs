#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Problems.VehicleRouting.Encodings;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// A base class for operators which improve VRP solutions.
  /// </summary>
  [Item("VRPImprovementOperator", "A base class for operators which improve VRP solutions.")]
  [StorableType("478FC98D-CB2E-4071-80E4-970BDC09EE32")]
  public abstract class VRPImprovementOperator : VRPOperator, IGeneralVRPOperator, ISingleObjectiveImprovementOperator {
    #region Parameter properties
    public ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IValueParameter<IntValue> ImprovementAttemptsParameter {
      get { return (IValueParameter<IntValue>)Parameters["ImprovementAttempts"]; }
    }
    public IValueLookupParameter<IntValue> LocalEvaluatedSolutions {
      get { return (IValueLookupParameter<IntValue>)Parameters["LocalEvaluatedSolutions"]; }
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
    protected VRPImprovementOperator(StorableConstructorFlag _) : base(_) { }
    protected VRPImprovementOperator(VRPImprovementOperator original, Cloner cloner) : base(original, cloner) { }
    protected VRPImprovementOperator()
      : base() {
      #region Create parameters
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope that contains the solution to be improved."));
      Parameters.Add(new ValueParameter<IntValue>("ImprovementAttempts", "The number of improvement attempts the operator should perform.", new IntValue(100)));
      Parameters.Add(new ValueLookupParameter<IntValue>("LocalEvaluatedSolutions", "The number of evaluated solutions."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueParameter<IntValue>("SampleSize", "The number of moves that should be executed.", new IntValue(25)));
      Parameters.Add(new ValueLookupParameter<IItem>("Solution", "The solution to be improved. This parameter is used for name translation only."));
      #endregion
    }

    public override IOperation InstrumentedApply() {
      var solution = SolutionParameter.ActualValue as IVRPEncoding;
      var potvinSolution = solution is PotvinEncoding ? solution as PotvinEncoding : PotvinEncoding.ConvertFrom(solution, ProblemInstance);

      if (solution == null)
        throw new ArgumentException("Cannot improve solution because it has the wrong type.");

      int evaluatedSolutions = Improve(potvinSolution);

      SolutionParameter.ActualValue = solution;
      LocalEvaluatedSolutions.ActualValue = new IntValue(evaluatedSolutions);

      return base.InstrumentedApply();
    }

    protected abstract int Improve(PotvinEncoding solution);
  }
}
