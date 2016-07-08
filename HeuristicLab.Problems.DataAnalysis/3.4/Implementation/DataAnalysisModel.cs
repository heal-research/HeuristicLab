#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  [Item("DataAnalysisModel", "Base class for data analysis models.")]
  public abstract class DataAnalysisModel : NamedItem, IDataAnalysisModel {
    [StorableConstructor]
    protected DataAnalysisModel(bool deserializing) : base(deserializing) { }
    protected DataAnalysisModel(DataAnalysisModel original, Cloner cloner)
      : base(original, cloner) { }
    protected DataAnalysisModel() { }
    protected DataAnalysisModel(string name) : base(name) { }
    protected DataAnalysisModel(string name, string description) : base(name, description) { }

    public abstract IEnumerable<string> VariablesUsedForPrediction { get; }
  }
}
