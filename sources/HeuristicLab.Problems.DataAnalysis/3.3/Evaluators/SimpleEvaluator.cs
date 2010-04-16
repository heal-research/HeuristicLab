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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Evaluators {
  public abstract class SimpleEvaluator : SingleSuccessorOperator {
    public const int ORIGINAL_INDEX = 0;
    public const int ESTIMATION_INDEX = 1;

    public ILookupParameter<DoubleMatrix> ValuesParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Values"]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public SimpleEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleMatrix>("Values", "Table of original and predicted values for which the quality value should be evaluated."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality value calculated from the values table."));
    }

    public override IOperation Apply() {
      DoubleMatrix values = ValuesParameter.ActualValue;
      QualityParameter.ActualValue = new DoubleValue(Apply(values));
      return null;
    }

    protected abstract double Apply(DoubleMatrix values);
  }
}
