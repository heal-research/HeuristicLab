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

using System.Linq;
using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System;

namespace HeuristicLab.Selection {
  /// <summary>
  /// A quality proportional selection operator which considers a single double quality value for selection.
  /// </summary>
  [Item("ProportionalSelector", "A quality proportional selection operator which considers a single double quality value for selection.")]
  [EmptyStorableClass]
  [Creatable("Test")]
  public sealed class ProportionalSelector : StochasticSingleObjectiveSelector {
    private ValueParameter<BoolData> WindowingParameter {
      get { return (ValueParameter<BoolData>)Parameters["Windowing"]; }
    }

    public BoolData Windowing {
      get { return WindowingParameter.Value; }
      set { WindowingParameter.Value = value; }
    }

    public ProportionalSelector()
      : base() {
      Parameters.Add(new ValueParameter<BoolData>("Windowing", "Apply windowing strategy (selection probability is proportional to the quality differences and not to the total quality).", new BoolData(true)));
      CopySelected.Value = true;
    }

    protected override ScopeList Select(ScopeList scopes) {
      int count = NumberOfSelectedSubScopesParameter.ActualValue.Value;
      bool copy = CopySelectedParameter.Value.Value;
      IRandom random = RandomParameter.ActualValue;
      bool maximization = MaximizationParameter.ActualValue.Value;
      List<DoubleData> qualities = new List<DoubleData>(QualityParameter.ActualValue);
      bool windowing = WindowingParameter.Value.Value;
      ScopeList selected = new ScopeList();

      // prepare qualities for proportional selection
      double minQuality = qualities.Min(x => x.Value);
      double maxQuality = qualities.Max(x => x.Value);
      if (minQuality == maxQuality) {  // all quality values are equal
        qualities.ForEach(x => x.Value = 1);
      } else {
        if (windowing) {
          if (maximization)
            qualities.ForEach(x => x.Value = x.Value - minQuality);
          else
            qualities.ForEach(x => x.Value = maxQuality - x.Value);
        } else {
          if (minQuality < 0.0) throw new InvalidOperationException("Proportional selection without windowing does not work with quality values < 0.");
          if (!maximization) {
            double limit = Math.Min(maxQuality * 2, double.MaxValue);
            qualities.ForEach(x => x.Value = limit - x.Value);
          }
        }
      }

      double qualitySum = qualities.Sum(x => x.Value);
      for (int i = 0; i < count; i++) {
        double selectedQuality = random.NextDouble() * qualitySum;
        int index = 0;
        double currentQuality = qualities[index].Value;
        while (currentQuality < selectedQuality) {
          index++;
          currentQuality += qualities[index].Value;
        }
        if (copy)
          selected.Add((IScope)scopes[index].Clone());
        else {
          selected.Add(scopes[index]);
          scopes.RemoveAt(index);
          qualitySum -= qualities[index].Value;
          qualities.RemoveAt(index);
        }
      }
      return selected;
    }
  }
}
