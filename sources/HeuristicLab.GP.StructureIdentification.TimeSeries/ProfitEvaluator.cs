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
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.GP.StructureIdentification;

namespace HeuristicLab.GP.StructureIdentification.TimeSeries {
  public class ProfitEvaluator : GPEvaluatorBase {
    protected DoubleData profit;
    private int exchangeRateVarIndex;
    private double transactionCost;
    public override string Description {
      get {
        return @"TASK";
      }
    }

    public ProfitEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("ExchangeRate", "The index of the variable in the dataset for the exchange rate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Profit", "The estimated profit of the model", typeof(DoubleData), VariableKind.New));
      AddVariableInfo(new VariableInfo("TransactionCost", "Cost of a trade in percent of the transaction volume (0..1)", typeof(DoubleData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      exchangeRateVarIndex = GetVariableValue<IntData>("ExchangeRate", scope, true).Data;
      transactionCost = GetVariableValue<DoubleData>("TransactionCost", scope, true).Data;
      profit = GetVariableValue<DoubleData>("Profit", scope, false, false);
      if(profit == null) {
        profit = new DoubleData();
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("Profit"), profit));
      }

      return base.Apply(scope);
    }

    public override void Evaluate(int start, int end) {
      double cA = 1.0; // start with a capital of one entity of A
      double cB = 0;
      double exchangeRate = double.MaxValue;
      for(int sample = start; sample < end; sample++) {
        exchangeRate = dataset.GetValue(sample, exchangeRateVarIndex);
        double originalPercentageChange = GetOriginalValue(sample);
        double estimatedPercentageChange = GetEstimatedValue(sample);
        SetOriginalValue(sample, estimatedPercentageChange);
        if(!double.IsNaN(originalPercentageChange) && !double.IsInfinity(originalPercentageChange)) {
          if(estimatedPercentageChange > 0) {
            // prediction is the rate of B/A will increase (= get more B for one A) => exchange all B to A
            cA += (cB / exchangeRate) * (1-transactionCost);
            cB = 0;
          } else if(estimatedPercentageChange < 0) {
            // prediction is the rate of B/A will drop (= get more A for one B) => exchange all A to B
            cB += (cA * exchangeRate) * (1-transactionCost);
            cA = 0;
          }
        }
      }

      if(double.IsNaN(cA) || double.IsInfinity(cA) || double.IsInfinity(cB) || double.IsNaN(cB)) {
        cA = 0;
        cB = 0;
      }
      // at the end we must exchange all B back to A 
      // (because we want to buy the local beer not import one from B-country)
      profit.Data = cA + ((cB / exchangeRate) * (1-transactionCost));
      profit.Data -= 1.0; // substract the start capital to determine actual profit
    }
  }
}
