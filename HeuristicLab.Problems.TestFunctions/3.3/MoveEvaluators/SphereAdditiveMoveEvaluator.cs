#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions {
  [Item("SphereAdditiveMoveEvaluator", "Class for evaluating an additive move on the Sphere function.")]
  [StorableClass]
  public class SphereAdditiveMoveEvaluator : AdditiveMoveEvaluator, ISphereMoveEvaluator {
    /// <summary>
    /// The parameter C modifies the steepness of the objective function y = C * ||X||^Alpha. Default is C = 1.
    /// </summary>
    public ValueParameter<DoubleValue> CParameter {
      get { return (ValueParameter<DoubleValue>)Parameters["C"]; }
    }
    /// <summary>
    /// The parameter Alpha modifies the steepness of the objective function y = C * ||X||^Alpha. Default is Alpha = 2.
    /// </summary>
    public ValueParameter<DoubleValue> AlphaParameter {
      get { return (ValueParameter<DoubleValue>)Parameters["Alpha"]; }
    }
    /// <summary>
    /// The parameter C modifies the steepness of the objective function y = C * ||X||^Alpha. Default is C = 1.
    /// </summary>
    public DoubleValue C {
      get { return CParameter.Value; }
      set { if (value != null) CParameter.Value = value; }
    }
    /// <summary>
    /// The parameter Alpha modifies the steepness of the objective function y = C * ||X||^Alpha. Default is Alpha = 2.
    /// </summary>
    public DoubleValue Alpha {
      get { return AlphaParameter.Value; }
      set { if (value != null) AlphaParameter.Value = value; }
    }

    public override System.Type EvaluatorType {
      get { return typeof(SphereEvaluator); }
    }

    [StorableConstructor]
    protected SphereAdditiveMoveEvaluator(bool deserializing) : base(deserializing) { }
    protected SphereAdditiveMoveEvaluator(SphereAdditiveMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    public SphereAdditiveMoveEvaluator() {
      Parameters.Add(new ValueParameter<DoubleValue>("C", "The parameter C modifies the steepness of the objective function y = C * ||X||^Alpha. Default is C = 1.", new DoubleValue(1)));
      Parameters.Add(new ValueParameter<DoubleValue>("Alpha", "The parameter Alpha modifies the steepness of the objective function y = C * ||X||^Alpha. Default is Alpha = 2.", new DoubleValue(2)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SphereAdditiveMoveEvaluator(this, cloner);
    }

    protected override double Evaluate(double quality, RealVector point, AdditiveMove move) {
      RealVectorAdditiveMoveWrapper wrapper = new RealVectorAdditiveMoveWrapper(move, point);
      return SphereEvaluator.Apply(wrapper, C.Value, Alpha.Value);
    }
  }
}
