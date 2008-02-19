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
using HeuristicLab.Data;

namespace HeuristicLab.BitVector
{
    public class RandomBitVectorGenerator : OperatorBase
    {
        public override string Description
        {
            get { return "Operator generating a new random bit vector."; }
        }

        public RandomBitVectorGenerator()
        {
            AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
            AddVariableInfo(new VariableInfo("Length", "Vector length", typeof(IntData), VariableKind.In));
            AddVariableInfo(new VariableInfo("BitVector", "Created random bit vector", typeof(BoolArrayData), VariableKind.New));
        }

        public static bool[] Apply(IRandom random, int length)
        {
            bool[] result = new bool[length];
            for (int i = 0; i < length; i++)
                result[i] = random.Next() < 0.5;
            return result;
        }

        public override IOperation Apply(IScope scope)
        {
            IRandom random = GetVariableValue<IRandom>("Random", scope, true);
            int length = GetVariableValue<IntData>("Length", scope, true).Data;

            bool[] vector = Apply(random, length);
            scope.AddVariable(new Variable(GetVariableInfo("BitVector").ActualName, new BoolArrayData(vector)));

            return null;
        }
    }
}
