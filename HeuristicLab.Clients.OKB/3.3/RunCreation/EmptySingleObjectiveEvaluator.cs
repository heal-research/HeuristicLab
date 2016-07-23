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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.OKB.RunCreation {
  [Item("EmptySingleObjectiveEvaluator", "A dummy single-objective evaluator which throws an exception when executed.")]
  [StorableClass]
  [NonDiscoverableType]
  public sealed class EmptySingleObjectiveEvaluator : Operator, ISingleObjectiveEvaluator {
    private string exceptionMessage;

    public override bool CanChangeName {
      get { return false; }
    }
    public override bool CanChangeDescription {
      get { return false; }
    }

    public ILookupParameter<Data.DoubleValue> QualityParameter {
      get { return (ILookupParameter<Data.DoubleValue>)Parameters["Quality"]; }
    }

    #region Persistence Properties
    [Storable(Name = "ExceptionMessage")]
    private string ExceptionMessage {
      get { return exceptionMessage; }
      set { exceptionMessage = value; }
    }
    #endregion

    [StorableConstructor]
    private EmptySingleObjectiveEvaluator(bool deserializing) : base(deserializing) { }
    private EmptySingleObjectiveEvaluator(EmptySingleObjectiveEvaluator original, Cloner cloner)
      : base(original, cloner) {
      exceptionMessage = original.exceptionMessage;
    }
    public EmptySingleObjectiveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<Data.DoubleValue>("Quality", "The evaluated quality of a solution."));
    }
    public EmptySingleObjectiveEvaluator(string exceptionMessage)
      : this() {
      this.exceptionMessage = exceptionMessage;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new EmptySingleObjectiveEvaluator(this, cloner);
    }

    public override IOperation Apply() {
      throw new InvalidOperationException(string.IsNullOrEmpty(exceptionMessage) ? "Cannot execute an EmptySingleObjectiveEvaluator. Please choose an appropriate solution evaluator." : exceptionMessage);
    }
  }
}
