#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.TestFunctions {
  public class RealVectorToRealVectorEncoder : SingleSuccessorOperator, IRealVectorPSOEncoder, IRealVectorOperator {
    #region Parameters

    public IParameter OriginalRealVectorParameter {
      get { return (IParameter)Parameters["OriginalRealVector"]; }
    }

    public IParameter RealVectorParameter {
      get { return (IParameter)Parameters["RealVector"]; }
    }

    public ILookupParameter<IntValue> LengthParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Length"]; }
    }

    public IValueLookupParameter<DoubleMatrix> BoundsParameter {
      get { return (IValueLookupParameter<DoubleMatrix>)Parameters["Bounds"]; }
    }

    protected ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    #endregion

    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }

    public RealVectorToRealVectorEncoder()
      : base() {
      Parameters.Add(new LookupParameter<RealVector>("OriginalRealVector", "The original real vector."));
      Parameters.Add(new LookupParameter<RealVector>("RealVector", "The resulting reference to the original real vector."));
      Parameters.Add(new LookupParameter<IntValue>("Length", "Vector length."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope bounds matrix should be cloned."));
      Parameters.Add(new ValueLookupParameter<DoubleMatrix>("Bounds", "The lower and upper bounds in each dimension."));
    }

    public override IOperation Apply() {
      RealVectorParameter.ActualValue = OriginalRealVectorParameter.ActualValue;
      IItem value = (IItem)BoundsParameter.ActualValue.Clone();
      CurrentScope.Variables.Add(new Variable("ParticleBounds", BoundsParameter.Description, value == null ? null : (IItem)value.Clone()));
      return base.Apply();
    }

    public override bool CanChangeName {
      get { return false; }
    }
  }
}
