using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Optimization;
using HeuristicLab.Operators;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Encodings.RealVectorEncoding;

namespace HeuristicLab.Problems.TestFunctions
{
    public class RealVectorToRealVectorEncoder : SingleSuccessorOperator, IRealVectorPSOEncoder, IRealVectorOperator
    {
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

      public RealVectorToRealVectorEncoder() : base() {
        Parameters.Add(new LookupParameter<RealVector>("OriginalRealVector", "The original real vector."));
        Parameters.Add(new LookupParameter<RealVector>("RealVector", "The resulting reference to the original real vector."));
        Parameters.Add(new LookupParameter<IntValue>("Length", "Vector length."));
        Parameters.Add(new ScopeParameter("CurrentScope", "The current scope bounds matrix should be cloned."));
        Parameters.Add(new ValueLookupParameter<DoubleMatrix>("Bounds", "The lower and upper bounds in each dimension."));
      }

      public override IOperation Apply() {
        RealVectorParameter.ActualValue = OriginalRealVectorParameter.ActualValue;
        IItem value = (IItem) BoundsParameter.ActualValue.Clone();
        CurrentScope.Variables.Add(new Variable("ParticleBounds", BoundsParameter.Description, value == null ? null : (IItem)value.Clone()));
        return base.Apply();
      }

      public override bool CanChangeName {
        get { return false; }
      }
    }
}
