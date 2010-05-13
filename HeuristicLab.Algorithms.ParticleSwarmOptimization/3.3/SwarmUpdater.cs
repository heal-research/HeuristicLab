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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Operators;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Parameters;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {
  public class SwarmUpdater : SingleSuccessorOperator {
    #region Parameter properties

    public ILookupParameter<DoubleValue> CurrentQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["CurrentQuality"]; }
    }

    public ILookupParameter<DoubleValue> LocalBestQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["LocalBestQuality"]; }
    }

    public ILookupParameter<DoubleValue> GlobalBestQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["GlobalBestQuality"]; }
    }

    public ILookupParameter<RealVector> CurrentPositionParameter {
      get { return (ILookupParameter<RealVector>)Parameters["CurrentPosition"]; }
    }

    public ILookupParameter<RealVector> BestLocalParameter {
      get { return (ILookupParameter<RealVector>)Parameters["BestLocal"]; }
    }

    public ILookupParameter<RealVector> BestGlobalParameter {
      get { return (ILookupParameter<RealVector>)Parameters["BestGlobal"]; }
    }

    public ValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    #endregion

    public SwarmUpdater()
      : base() {
      Parameters.Add(new LookupParameter<RealVector>("CurrentPosition", "Current position"));
      Parameters.Add(new LookupParameter<RealVector>("BestLocal", "Best local position"));
      Parameters.Add(new LookupParameter<RealVector>("BestGlobal", "Best global position"));
      Parameters.Add(new LookupParameter<DoubleValue>("LocalBestQuality", "Best local quality"));
      Parameters.Add(new LookupParameter<DoubleValue>("GlobalBestQuality", "Best global quality"));
      Parameters.Add(new LookupParameter<DoubleValue>("CurrentQuality", "Current quality"));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
    }

    public override IOperation Apply() {
      if (MaximizationParameter.ActualValue.Value) {
        if (CurrentQualityParameter.ActualValue.Value > LocalBestQualityParameter.ActualValue.Value) {
          LocalBestQualityParameter.ActualValue.Value = CurrentQualityParameter.ActualValue.Value;
          BestLocalParameter.ActualValue = (RealVector) CurrentPositionParameter.ActualValue.Clone();
        }
        if (CurrentQualityParameter.ActualValue.Value > GlobalBestQualityParameter.ActualValue.Value) {
          GlobalBestQualityParameter.ActualValue.Value = CurrentQualityParameter.ActualValue.Value;
          BestGlobalParameter.ActualValue = (RealVector)CurrentPositionParameter.ActualValue.Clone();
        }
      } else {
        if (CurrentQualityParameter.ActualValue.Value < LocalBestQualityParameter.ActualValue.Value) {
          LocalBestQualityParameter.ActualValue.Value = CurrentQualityParameter.ActualValue.Value;
          BestLocalParameter.ActualValue = (RealVector)CurrentPositionParameter.ActualValue.Clone();
        }
        if (CurrentQualityParameter.ActualValue.Value < GlobalBestQualityParameter.ActualValue.Value) {
          GlobalBestQualityParameter.ActualValue.Value = CurrentQualityParameter.ActualValue.Value;
          BestGlobalParameter.ActualValue = (RealVector)CurrentPositionParameter.ActualValue.Clone();
        }
      }
      return base.Apply();
    }

    public override bool CanChangeName {
      get { return false; }
    }
  }
}
