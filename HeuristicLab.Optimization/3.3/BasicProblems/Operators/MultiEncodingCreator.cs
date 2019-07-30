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
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;

namespace HeuristicLab.Optimization {
  [Item("MultiEncodingCreator", "Contains solution creators that together create a multi-encoding.")]
  [StorableType("E261B506-6F74-4BC4-8164-5ACE20FBC319")]
  public sealed class MultiEncodingCreator : MultiEncodingOperator<ISolutionCreator>, ISolutionCreator, IStochasticOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public override string OperatorPrefix => "Creator";

    [StorableConstructor]
    private MultiEncodingCreator(StorableConstructorFlag _) : base(_) { }

    private MultiEncodingCreator(MultiEncodingCreator original, Cloner cloner) : base(original, cloner) { }
    public MultiEncodingCreator() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator used by the individual operators."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiEncodingCreator(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey("Random")) {
        Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator used by the individual operators."));
      }
    }

    public override void AddEncoding(IEncoding encoding) {
      base.AddEncoding(encoding);
      var parameter = GetParameter(encoding);
      parameter.Value = encoding.SolutionCreator;
      encoding.SolutionCreatorChanged += Encoding_SolutionCreatorChanged;
    }

    public override bool RemoveEncoding(IEncoding encoding) {
      var success = base.RemoveEncoding(encoding);
      encoding.SolutionCreatorChanged -= Encoding_SolutionCreatorChanged;
      return success;
    }

    private void Encoding_SolutionCreatorChanged(object sender, EventArgs e) {
      var encoding = (IEncoding)sender;
      var parameter = GetParameter(encoding);

      var oldCreator = parameter.ValidValues.Single(creator => creator.GetType() == encoding.SolutionCreator.GetType());
      parameter.ValidValues.Remove(oldCreator);
      parameter.ValidValues.Add(encoding.SolutionCreator);
      parameter.Value = encoding.SolutionCreator;
    }
  }
}
