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

using System.Drawing;
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// The base class for all problems.
  /// </summary>
  [Item("Problem", "Base class for problems.")]
  public abstract class Problem : NamedItem, IProblem {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Type; }
    }

    private ParameterCollection parameters;
    [Storable]
    protected ParameterCollection Parameters {
      get { return parameters;}
      private set {
        if (parameters != null) parameters.Changed -= new ChangedEventHandler(Parameters_Changed);
        parameters = value;
        readOnlyParameters = null;
        if (parameters != null) parameters.Changed += new ChangedEventHandler(Parameters_Changed);
      }
    }
    private ReadOnlyObservableKeyedCollection<string, IParameter> readOnlyParameters;
    IObservableKeyedCollection<string, IParameter> IParameterizedItem.Parameters {
      get {
        if (readOnlyParameters == null) readOnlyParameters = parameters.AsReadOnly();
        return readOnlyParameters;
      }
    }

    protected Problem() {
      Name = ItemName;
      Parameters = new ParameterCollection();
      readOnlyParameters = null;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      Problem clone = (Problem)base.Clone(cloner);
      clone.Parameters = (ParameterCollection)cloner.Clone(parameters);
      return clone;
    }

    private void Parameters_Changed(object sender, ChangedEventArgs e) {
      OnChanged(e);
    }
  }
}
