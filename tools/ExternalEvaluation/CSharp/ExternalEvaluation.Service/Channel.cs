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
using Google.ProtocolBuffers;

namespace HeuristicLab.Problems.ExternalEvaluation.Service {
  public abstract class Channel : IChannel {
    protected bool initialized;

    protected Channel() {
    }

    public bool IsInitialized {
      get { return initialized; }
    }

    public virtual void Open() {
      initialized = true;
    }

    public abstract void Send(IMessage msg);

    public abstract IMessage Receive(IBuilder builder);

    public virtual void Close() {
      initialized = false;
    }
  }
}
