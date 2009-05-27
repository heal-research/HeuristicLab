#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Modeling {
  public class Model : IModel {
    #region IModel Members

    private Dataset dataset;
    public Dataset Dataset {
      get { return dataset; }
      set { dataset = value; }
    }

    private string targetVariable;
    public string TargetVariable {
      get { return targetVariable; }
      set { targetVariable = value; }
    }

    private double trainingMSE;
    public double TrainingMeanSquaredError {
      get { return trainingMSE; }
      set { trainingMSE = value; }
    }

    private double validationMSE;
    public double ValidationMeanSquaredError {
      get { return validationMSE; }
      set { validationMSE = value; }
    }

    private double testMSE;
    public double TestMeanSquaredError {
      get { return testMSE; }
      set { testMSE = value; }
    }

    public double TrainingMeanAbsolutePercentageError {
      get;
      set;
    }

    public double ValidationMeanAbsolutePercentageError {
      get;
      set;
    }

    public double TestMeanAbsolutePercentageError {
      get;
      set;
    }

    public double TrainingMeanAbsolutePercentageOfRangeError {
      get;
      set;
    }

    public double ValidationMeanAbsolutePercentageOfRangeError {
      get;
      set;
    }

    public double TestMeanAbsolutePercentageOfRangeError {
      get;
      set;
    }

    public double TrainingCoefficientOfDetermination {
      get;
      set;
    }

    public double ValidationCoefficientOfDetermination {
      get;
      set;
    }

    public double TestCoefficientOfDetermination {
      get;
      set;
    }

    public double TrainingVarianceAccountedFor {
      get;
      set;
    }

    public double ValidationVarianceAccountedFor {
      get;
      set;
    }

    public double TestVarianceAccountedFor {
      get;
      set;
    }

    private IItem data;
    public IItem Data {
      get { return data; }
      set { data = value; }
    }

    #endregion
  }
}
