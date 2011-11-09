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
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Clients.Hive {
  /// <summary>
  /// This class is used to communicate from the executor to the core, e.g. 
  /// if the appdomain should be killed
  /// </summary>
  [StorableClass]
  [Serializable]
  public class ExecutorMessageContainer<CallbackParameterType> : MessageContainer {

    [Storable]
    public Action<CallbackParameterType> Callback;
    public CallbackParameterType CallbackParameter;

    public ExecutorMessageContainer() : base() { }

    public ExecutorMessageContainer(MessageType message, Guid parentJobId, Action<CallbackParameterType> callback)
      : base(message, parentJobId) {
      this.Callback = callback;
    }

    protected ExecutorMessageContainer(ExecutorMessageContainer<CallbackParameterType> original, Cloner cloner)
      : base(original, cloner) {
      this.Callback = original.Callback;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExecutorMessageContainer<CallbackParameterType>(this, cloner);
    }

    public void execute() {
      Callback(CallbackParameter);
    }
  }
}
