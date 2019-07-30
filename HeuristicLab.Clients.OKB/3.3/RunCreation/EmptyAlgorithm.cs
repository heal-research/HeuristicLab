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
using HeuristicLab.Optimization;
using HEAL.Attic;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.OKB.RunCreation {
  [Item("Empty Algorithm", "A dummy algorithm which serves as a placeholder and cannot be executed.")]
  [StorableType("C98C8322-5A78-4518-A94D-FA20F12AB8D3")]
  [NonDiscoverableType]
  public sealed class EmptyAlgorithm : HeuristicLab.Optimization.Algorithm {
    private string exceptionMessage;

    public override bool CanChangeName {
      get { return false; }
    }
    public override bool CanChangeDescription {
      get { return false; }
    }

    private ResultCollection results;
    public override Optimization.ResultCollection Results {
      get { return results; }
    }

    #region Persistence Properties
    [Storable(Name = "ExceptionMessage")]
    private string ExceptionMessage {
      get { return exceptionMessage; }
      set { exceptionMessage = value; }
    }
    #endregion

    [StorableConstructor]
    private EmptyAlgorithm(StorableConstructorFlag _) : base(_) {
      this.results = new ResultCollection();
    }
    private EmptyAlgorithm(EmptyAlgorithm original, Cloner cloner)
      : base(original, cloner) {
      this.exceptionMessage = original.exceptionMessage;
      this.results = new ResultCollection();
    }
    public EmptyAlgorithm()
      : base() {
      results = new ResultCollection();
    }
    public EmptyAlgorithm(string exceptionMessage)
      : this() {
      this.exceptionMessage = exceptionMessage;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new EmptyAlgorithm(this, cloner);
    }

    public override void Prepare() {
      throw new InvalidOperationException(string.IsNullOrEmpty(exceptionMessage) ? "Cannot prepare an EmptyAlgorithm." : exceptionMessage);
    }
    public override void Start() {
      throw new InvalidOperationException(string.IsNullOrEmpty(exceptionMessage) ? "Cannot start an EmptyAlgorithm." : exceptionMessage);
    }
    public override void Pause() {
      throw new InvalidOperationException(string.IsNullOrEmpty(exceptionMessage) ? "Cannot pause an EmptyAlgorithm." : exceptionMessage);
    }
    public override void Stop() {
      throw new InvalidOperationException(string.IsNullOrEmpty(exceptionMessage) ? "Cannot stop an EmptyAlgorithm." : exceptionMessage);
    }
  }
}
