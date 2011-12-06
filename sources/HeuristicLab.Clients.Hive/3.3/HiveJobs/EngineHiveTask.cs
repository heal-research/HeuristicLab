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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.Hive {
  public class EngineHiveTask : HiveTask<EngineTask> {
    private IScope parentScopeClone;

    #region Constructors and cloning
    public EngineHiveTask() { }
    public EngineHiveTask(EngineTask engineJob, IScope parentScopeClone)
      : base(engineJob) {
      this.parentScopeClone = parentScopeClone;
    }

    protected EngineHiveTask(EngineHiveTask original, Cloner cloner)
      : base(original, cloner) {
      this.parentScopeClone = cloner.Clone(original.parentScopeClone);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new EngineHiveTask(this, cloner);
    }
    #endregion

    public override TaskData GetAsTaskData(bool withoutChildOptimizers, out List<IPluginDescription> plugins) {
      plugins = new List<IPluginDescription>();
      if (this.itemTask == null)
        return null;

      TaskData jobData = new TaskData();
      IEnumerable<Type> usedTypes;

      // clone operation and remove unnecessary scopes; don't do this earlier to avoid memory problems      
      ((IAtomicOperation)ItemTask.InitialOperation).Scope.Parent = parentScopeClone;
      ItemTask.InitialOperation = (IOperation)ItemTask.InitialOperation.Clone();
      ((IAtomicOperation)ItemTask.InitialOperation).Scope.ClearParentScopes();
      jobData.Data = PersistenceUtil.Serialize(ItemTask, out usedTypes);

      // add type objects from object graph to work around ticket #1527
      var typeObjects = ItemTask.GetObjectGraphObjects().OfType<Type>();
      usedTypes = new List<Type>(usedTypes).Union(typeObjects);

      PluginUtil.CollectDeclaringPlugins(plugins, usedTypes);

      return jobData;
    }

    public override void ClearData() {
      base.ClearData();
      this.ItemTask.InitialOperation = null;
    }
  }
}
