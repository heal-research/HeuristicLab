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
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [StorableClass]
  public class RunCollectionContentConstraint : Constraint, IRunCollectionConstraint {
    public new RunCollection ConstrainedValue {
      get { return (RunCollection)base.ConstrainedValue; }
      set { base.ConstrainedValue = value; }
    }

    public new ItemSet<IRun> ConstraintData {
      get { return (ItemSet<IRun>)base.ConstraintData; }
      set { base.ConstraintData = value; }
    }

    public override IEnumerable<ConstraintOperation> AllowedConstraintOperations {
      get { return new List<ConstraintOperation>() { ConstraintOperation.Equal }; }
    }

    [StorableConstructor]
    protected RunCollectionContentConstraint(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterConstraintDataEvents();
    }
    protected RunCollectionContentConstraint(RunCollectionContentConstraint original, Cloner cloner)
      : base(original, cloner) {
      RegisterConstraintDataEvents();
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new RunCollectionContentConstraint(this, cloner); }

    public RunCollectionContentConstraint()
      : base() {
      ConstraintData = new ItemSet<IRun>();
    }
    public RunCollectionContentConstraint(RunCollection constrainedValue, IObservableSet<IRun> constraintData)
      : base(constrainedValue, ConstraintOperation.Equal, constraintData) {
    }
    public RunCollectionContentConstraint(RunCollection constrainedValue, IObservableSet<IRun> constraintData, bool active)
      : base(constrainedValue, ConstraintOperation.Equal, constraintData, active) {
    }

    protected override void OnConstraintDataChanged() {
      RegisterConstraintDataEvents();
      base.OnConstraintDataChanged();
    }
    private void RegisterConstraintDataEvents() {
      if (ConstraintData != null) {
        ConstraintData.ItemsAdded += new CollectionItemsChangedEventHandler<IRun>(ConstraintData_ItemsAdded);
        ConstraintData.ItemsRemoved += new CollectionItemsChangedEventHandler<IRun>(ConstraintData_ItemsRemoved);
        ConstraintData.CollectionReset += new CollectionItemsChangedEventHandler<IRun>(ConstraintData_CollectionReset);
      }
    }
    private void ConstraintData_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      base.OnConstraintDataChanged();
    }
    private void ConstraintData_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      base.OnConstraintDataChanged();
    }
    private void ConstraintData_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      base.OnConstraintDataChanged();
    }

    protected override bool Check(object constrainedMember) {
      if (!Active) return true;

      foreach (IRun run in ConstrainedValue.Where(r => r.Visible))
        run.Visible = !ConstraintData.Contains(run);
      return true;
    }

    protected override bool Check(object constrainedMember, out string errorMessage) {
      errorMessage = string.Empty;
      if (!Active) return true;

      foreach (IRun run in ConstrainedValue.Where(r => r.Visible))
        run.Visible = !ConstraintData.Contains(run);
      return true;
    }
  }
}
