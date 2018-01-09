#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.OKB.RunCreation {
  [Item("EmptyMultiObjectiveEvaluator", "A dummy multi-objective evaluator which throws an exception when executed.")]
  [StorableClass]
  [NonDiscoverableType]
  public sealed class EmptyMultiObjectiveEvaluator : Operator, IMultiObjectiveEvaluator {
    private string exceptionMessage;

    public ILookupParameter<DoubleArray> QualitiesParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["Qualities"]; }
    }

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
    private EmptyMultiObjectiveEvaluator(bool deserializing) : base(deserializing) { }
    private EmptyMultiObjectiveEvaluator(EmptyMultiObjectiveEvaluator original, Cloner cloner)
      : base(original, cloner) {
      exceptionMessage = original.exceptionMessage;
    }
    public EmptyMultiObjectiveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleArray>("Qualities", "The evaluated quality values of a solution."));
    }
    public EmptyMultiObjectiveEvaluator(string exceptionMessage)
      : this() {
      this.exceptionMessage = exceptionMessage;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new EmptyMultiObjectiveEvaluator(this, cloner);
    }

    public override IOperation Apply() {
      throw new InvalidOperationException(string.IsNullOrEmpty(exceptionMessage) ? "Cannot execute an EmptyMultiObjectiveEvaluator. Please choose an appropriate solution evaluator." : exceptionMessage);
    }
  }
}
