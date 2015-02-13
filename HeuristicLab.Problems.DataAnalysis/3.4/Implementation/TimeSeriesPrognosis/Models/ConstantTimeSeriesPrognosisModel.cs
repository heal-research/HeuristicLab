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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  [Item("Constant TimeSeries Model", "A time series model that returns for all prediciton the same constant value.")]
  public class ConstantTimeSeriesPrognosisModel : ConstantRegressionModel, ITimeSeriesPrognosisModel {
    [StorableConstructor]
    protected ConstantTimeSeriesPrognosisModel(bool deserializing) : base(deserializing) { }
    protected ConstantTimeSeriesPrognosisModel(ConstantTimeSeriesPrognosisModel original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ConstantTimeSeriesPrognosisModel(this, cloner);
    }

    public ConstantTimeSeriesPrognosisModel(double constant) : base(constant) { }

    public IEnumerable<IEnumerable<double>> GetPrognosedValues(Dataset dataset, IEnumerable<int> rows, IEnumerable<int> horizons) {
      return horizons.Select(horizon => Enumerable.Repeat(Constant, horizon));
    }

    public ITimeSeriesPrognosisSolution CreateTimeSeriesPrognosisSolution(ITimeSeriesPrognosisProblemData problemData) {
      return new TimeSeriesPrognosisSolution(this, new TimeSeriesPrognosisProblemData(problemData));
    }
  }
}
