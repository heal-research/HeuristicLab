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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HEAL.Attic;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.OKB.RunCreation {
  [Item("EmptySolutionCreator", "A dummy solution creator which throws an exception when executed.")]
  [StorableType("A3793B3B-A85B-47D3-A3DB-D35565518598")]
  [NonDiscoverableType]
  public sealed class EmptySolutionCreator : Operator, ISolutionCreator {
    private string exceptionMessage;

    public override bool CanChangeName {
      get { return false; }
    }
    public override bool CanChangeDescription {
      get { return false; }
    }

    #region Persistence Properties
    [Storable(Name = "ExceptionMessage")]
    private string ExceptionMessage {
      get { return exceptionMessage; }
      set { exceptionMessage = value; }
    }
    #endregion

    [StorableConstructor]
    private EmptySolutionCreator(StorableConstructorFlag _) : base(_) { }
    private EmptySolutionCreator(EmptySolutionCreator original, Cloner cloner)
      : base(original, cloner) {
      exceptionMessage = original.exceptionMessage;
    }
    public EmptySolutionCreator() : base() { }
    public EmptySolutionCreator(string exceptionMessage)
      : this() {
      this.exceptionMessage = exceptionMessage;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new EmptySolutionCreator(this, cloner);
    }

    public override IOperation Apply() {
      throw new InvalidOperationException(string.IsNullOrEmpty(exceptionMessage) ? "Cannot execute an EmptySolutionCreator. Please choose an appropriate solution creator." : exceptionMessage);
    }
  }
}
