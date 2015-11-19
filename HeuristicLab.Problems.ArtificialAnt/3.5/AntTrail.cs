#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.ArtificialAnt {
  /// <summary>
  /// Represents a trail of an artificial ant which can be visualized in the GUI.
  /// </summary>
  [Item("AntTrail", "Represents a trail of an artificial ant which can be visualized in the GUI.")]
  [StorableClass]
  public sealed class AntTrail : Item {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Image; }
    }
    [Storable]
    public ISymbolicExpressionTree SymbolicExpressionTree { get; private set; }
    [Storable]
    public BoolMatrix World { get; private set; }
    [Storable]
    public int MaxTimeSteps { get; private set; }
    [Storable]
    public double Quality { get; private set; }

    public AntTrail(BoolMatrix world, ISymbolicExpressionTree expression, int maxTimeSteps, double quality) {
      this.World = world;
      this.SymbolicExpressionTree = expression;
      this.MaxTimeSteps = maxTimeSteps;
      this.Quality = quality;
    }

    [StorableConstructor]
    private AntTrail(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    private AntTrail(AntTrail original, Cloner cloner)
      : base(original, cloner) {
      SymbolicExpressionTree = cloner.Clone(original.SymbolicExpressionTree);
      World = cloner.Clone(original.World);
      MaxTimeSteps = original.MaxTimeSteps;
      Quality = original.Quality;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AntTrail(this, cloner);
    }
  }
}
