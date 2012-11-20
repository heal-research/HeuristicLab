#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "MeanConst", Description = "Constant mean function for Gaussian processes.")]
  public sealed class MeanConst : ParameterizedNamedItem, IMeanFunction {
    [Storable]
    private double c;
    [Storable]
    private readonly HyperParameter<DoubleValue> valueParameter;
    public IValueParameter<DoubleValue> ValueParameter { get { return valueParameter; } }

    [StorableConstructor]
    private MeanConst(bool deserializing) : base(deserializing) { }
    private MeanConst(MeanConst original, Cloner cloner)
      : base(original, cloner) {
      this.c = original.c;
      this.valueParameter = cloner.Clone(original.valueParameter);
      RegisterEvents();
    }
    public MeanConst()
      : base() {
      this.name = ItemName;
      this.description = ItemDescription;

      this.valueParameter = new HyperParameter<DoubleValue>("Value", "The constant value for the constant mean function.");
      Parameters.Add(valueParameter);
      RegisterEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MeanConst(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }

    private void RegisterEvents() {
      Util.AttachValueChangeHandler<DoubleValue, double>(valueParameter, () => { c = valueParameter.Value.Value; });
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return valueParameter.Fixed ? 0 : 1;
    }

    public void SetParameter(double[] hyp) {
      if (!valueParameter.Fixed) {
        valueParameter.SetValue(new DoubleValue(hyp[0]));
      } else if (hyp.Length > 0)
        throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for the constant mean function.", "hyp");
    }

    public double[] GetMean(double[,] x) {
      return Enumerable.Repeat(c, x.GetLength(0)).ToArray();
    }

    public double[] GetGradients(int k, double[,] x) {
      if (k > 0) throw new ArgumentException();
      return Enumerable.Repeat(1.0, x.GetLength(0)).ToArray();
    }
  }
}
