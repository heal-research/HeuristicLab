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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Represents classification solutions that contain an ensemble of multiple classification models
  /// </summary>
  [StorableClass]
  [Item("ClassificationEnsembleModel", "A classification model that contains an ensemble of multiple classification models")]
  public class ClassificationEnsembleModel : NamedItem, IClassificationEnsembleModel {

    [Storable]
    private List<IClassificationModel> models;
    public IEnumerable<IClassificationModel> Models {
      get { return new List<IClassificationModel>(models); }
    }

    [StorableConstructor]
    protected ClassificationEnsembleModel(bool deserializing) : base(deserializing) { }
    protected ClassificationEnsembleModel(ClassificationEnsembleModel original, Cloner cloner)
      : base(original, cloner) {
      this.models = original.Models.Select(m => cloner.Clone(m)).ToList();
    }

    public ClassificationEnsembleModel() : this(Enumerable.Empty<IClassificationModel>()) { }
    public ClassificationEnsembleModel(IEnumerable<IClassificationModel> models)
      : base() {
      this.name = ItemName;
      this.description = ItemDescription;
      this.models = new List<IClassificationModel>(models);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ClassificationEnsembleModel(this, cloner);
    }

    #region IClassificationEnsembleModel Members
    public void Add(IClassificationModel model) {
      models.Add(model);
    }
    public void Remove(IClassificationModel model) {
      models.Remove(model);
    }

    public IEnumerable<IEnumerable<double>> GetEstimatedClassValueVectors(Dataset dataset, IEnumerable<int> rows) {
      var estimatedValuesEnumerators = (from model in models
                                        select model.GetEstimatedClassValues(dataset, rows).GetEnumerator())
                                       .ToList();

      while (estimatedValuesEnumerators.All(en => en.MoveNext())) {
        yield return from enumerator in estimatedValuesEnumerators
                     select enumerator.Current;
      }
    }

    #endregion

    #region IClassificationModel Members

    public IEnumerable<double> GetEstimatedClassValues(Dataset dataset, IEnumerable<int> rows) {
      foreach (var estimatedValuesVector in GetEstimatedClassValueVectors(dataset, rows)) {
        // return the class which is most often occuring
        yield return
          estimatedValuesVector
          .GroupBy(x => x)
          .OrderBy(g => -g.Count())
          .Select(g => g.Key)
          .First();
      }
    }

    IClassificationSolution IClassificationModel.CreateClassificationSolution(IClassificationProblemData problemData) {
      return new ClassificationEnsembleSolution(models, new ClassificationEnsembleProblemData(problemData));
    }
    #endregion
  }
}
