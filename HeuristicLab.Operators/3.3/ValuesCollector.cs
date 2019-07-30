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

using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which collects the actual values of parameters.
  /// </summary>
  [Item("ValuesCollector", "An operator which collects the actual values of parameters.")]
  [StorableType("83F958A5-AE91-44C9-B329-BC9A36DC4E40")]
  public abstract class ValuesCollector : SingleSuccessorOperator, IOperator {
    [Storable]
    private ParameterCollection collectedValues;
    public ParameterCollection CollectedValues {
      get { return collectedValues; }
    }

    [StorableConstructor]
    protected ValuesCollector(StorableConstructorFlag _) : base(_) { }
    protected ValuesCollector(ValuesCollector original, Cloner cloner)
      : base(original, cloner) {
      this.collectedValues = cloner.Clone<ParameterCollection>(original.collectedValues);
      Initialize();
    }
    public ValuesCollector()
      : base() {
      collectedValues = new ParameterCollection();
      Initialize();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }

    private void Initialize() {
      collectedValues.ItemsAdded += new CollectionItemsChangedEventHandler<IParameter>(collectedValues_ItemsAdded);
      collectedValues.ItemsRemoved += new CollectionItemsChangedEventHandler<IParameter>(collectedValues_ItemsRemoved);
      collectedValues.CollectionReset += new CollectionItemsChangedEventHandler<IParameter>(collectedValues_CollectionReset);
    }

    private void collectedValues_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      Parameters.AddRange(e.Items);
    }
    private void collectedValues_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      Parameters.RemoveRange(e.Items);
    }
    #region NOTE
    // NOTE: The ItemsReplaced event does not have to be handled here as it is only fired when the name (i.e. key) of a parameter
    // changes. As the same parameter is also contained in the Parameters collection of the operator, the Parameters collection
    // will react on this name change on its own.
    #endregion
    private void collectedValues_CollectionReset(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      Parameters.RemoveRange(e.OldItems);
      Parameters.AddRange(e.Items);
    }
  }
}
