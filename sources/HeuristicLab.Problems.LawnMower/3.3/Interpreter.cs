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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.LawnMower {
  public class Interpreter {
    private enum Heading {
      South,
      East,
      North,
      West
    };

    private class MowerState {
      public Heading Heading { get; set; }
      public Tuple<uint, uint> Position { get; set; }
      public int Energy { get; set; }
      public MowerState() {
        Heading = Heading.South;
        Position = new Tuple<uint, uint>(0, 0);
        Energy = 0;
      }
    }


    public static bool[,] EvaluateLawnMowerProgram(int length, int width, ISymbolicExpressionTree tree) {

      bool[,] lawn = new bool[length, width];
      var mowerState = new MowerState();
      mowerState.Heading = Heading.South;
      mowerState.Energy = length * width * 2;
      lawn[mowerState.Position.Item1, mowerState.Position.Item2] = true;
      EvaluateLawnMowerProgram(tree.Root, ref mowerState, lawn);

      return lawn;
    }


    private static Tuple<int, int> EvaluateLawnMowerProgram(ISymbolicExpressionTreeNode node, ref MowerState mowerState, bool[,] lawn) {
      if (mowerState.Energy <= 0) return new Tuple<int, int>(0, 0);

      if (node.Symbol is ProgramRootSymbol) {
        return EvaluateLawnMowerProgram(node.GetSubtree(0), ref mowerState, lawn);
      } else if (node.Symbol is StartSymbol) {
        return EvaluateLawnMowerProgram(node.GetSubtree(0), ref mowerState, lawn);
      } else if (node.Symbol is Left) {
        switch (mowerState.Heading) {
          case Heading.East: mowerState.Heading = Heading.North;
            break;
          case Heading.North: mowerState.Heading = Heading.West;
            break;
          case Heading.West: mowerState.Heading = Heading.South;
            break;
          case Heading.South:
            mowerState.Heading = Heading.East;
            break;
        }
        return new Tuple<int, int>(0, 0);
      } else if (node.Symbol is Forward) {
        int dRow = 0;
        int dCol = 0;
        switch (mowerState.Heading) {
          case Heading.East:
            dCol++;
            break;
          case Heading.North:
            dRow--;
            break;
          case Heading.West:
            dCol--;
            break;
          case Heading.South:
            dRow++;
            break;
        }
        uint newRow = (uint)((mowerState.Position.Item1 + lawn.GetLength(0) + dRow) % lawn.GetLength(0));
        uint newColumn = (uint)((mowerState.Position.Item2 + lawn.GetLength(1) + dCol) % lawn.GetLength(1));
        mowerState.Position = new Tuple<uint, uint>(newRow, newColumn);
        mowerState.Energy = mowerState.Energy - 1;
        lawn[newRow, newColumn] = true;
        return new Tuple<int, int>(0, 0);
      } else if (node.Symbol is Constant) {
        var constNode = node as ConstantTreeNode;
        return constNode.Value;
      } else if (node.Symbol is Sum) {
        var p = EvaluateLawnMowerProgram(node.GetSubtree(0), ref mowerState, lawn);
        var q = EvaluateLawnMowerProgram(node.GetSubtree(1), ref mowerState, lawn);
        return new Tuple<int, int>(p.Item1 + q.Item1,
          p.Item2 + q.Item2);
      } else if (node.Symbol is Prog) {
        EvaluateLawnMowerProgram(node.GetSubtree(0), ref mowerState, lawn);
        return EvaluateLawnMowerProgram(node.GetSubtree(1), ref mowerState, lawn);
      } else if (node.Symbol is Frog) {
        var p = EvaluateLawnMowerProgram(node.GetSubtree(0), ref mowerState, lawn);

        uint newRow = (uint)((mowerState.Position.Item1 + lawn.GetLength(0) + p.Item1 % lawn.GetLength(0)) % lawn.GetLength(0));
        uint newCol = (uint)((mowerState.Position.Item2 + lawn.GetLength(1) + p.Item2 % lawn.GetLength(1)) % lawn.GetLength(1));
        mowerState.Position = new Tuple<uint, uint>(newRow, newCol);
        mowerState.Energy = mowerState.Energy - 1;
        lawn[newRow, newCol] = true;
        return new Tuple<int, int>(0, 0);
      } else {
        throw new ArgumentException("Invalid symbol in the lawn mower program.");
      }
    }

  }
}
