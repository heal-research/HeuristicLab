﻿#region License Information
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

using Google.Protobuf;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("EvaluationChannel", "Abstract base class for channels to be used in an external evaluation problem.")]
  [StorableType("2BE4FE9D-D5FE-45C3-9F85-3BF7200F0913")]
  public abstract class EvaluationChannel : NamedItem, IEvaluationChannel {
    public override bool CanChangeName { get { return false; } }
    public override bool CanChangeDescription { get { return false; } }

    [StorableConstructor]
    protected EvaluationChannel(StorableConstructorFlag _) : base(_) { }
    protected EvaluationChannel(EvaluationChannel original, Cloner cloner) : base(original, cloner) { }
    protected EvaluationChannel()
      : base() {
      name = ItemName;
      description = ItemDescription;
    }

    #region IExternalEvaluationChannel Members
    // will not be serialized, since it will always be false after deserialization
    public bool IsInitialized { get; protected set; }

    public virtual void Open() {
      IsInitialized = true;
    }

    public abstract void Send(IMessage message);

    public abstract IMessage Receive(IMessage builder, ExtensionRegistry extensions);

    public virtual void Close() {
      IsInitialized = false;
    }

    #endregion
  }
}
