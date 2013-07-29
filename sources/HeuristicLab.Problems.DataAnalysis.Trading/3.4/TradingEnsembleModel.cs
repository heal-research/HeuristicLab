#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// <summary>
  /// Represents trading solutions that contain an ensemble of multiple trading models
  /// </summary>
  [StorableClass]
  [Item("TradingEnsembleModel", "A trading model that contains an ensemble of multiple trading models")]
  public class TradingEnsembleModel : NamedItem, ITradingEnsembleModel {

    [Storable]
    private List<ITradingModel> models;
    public IEnumerable<ITradingModel> Models {
      get { return new List<ITradingModel>(models); }
    }
    [StorableConstructor]
    protected TradingEnsembleModel(bool deserializing) : base(deserializing) { }
    protected TradingEnsembleModel(TradingEnsembleModel original, Cloner cloner)
      : base(original, cloner) {
      this.models = original.Models.Select(m => cloner.Clone(m)).ToList();
    }
    public TradingEnsembleModel(IEnumerable<ITradingModel> models)
      : base() {
      this.name = ItemName;
      this.description = ItemDescription;
      this.models = new List<ITradingModel>(models);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TradingEnsembleModel(this, cloner);
    }

    #region ITradingEnsembleModel Members

    public IEnumerable<IEnumerable<double>> GetSignalVectors(Dataset dataset, IEnumerable<int> rows) {
      var signalEnumerators = (from model in models
                               select model.GetSignals(dataset, rows).GetEnumerator())
                                       .ToList();

      while (signalEnumerators.All(en => en.MoveNext())) {
        yield return from enumerator in signalEnumerators
                     select enumerator.Current;
      }
    }

    #endregion

    #region ITradingModel Members

    public IEnumerable<double> GetSignals(Dataset dataset, IEnumerable<int> rows) {
      throw new System.NotImplementedException();
    }

    #endregion
  }
}
