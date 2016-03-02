#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 * and the BEACON Center for the Study of Evolution in Action.
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


using System.Diagnostics.Contracts;

namespace HeuristicLab.Algorithms.DataAnalysis.MctsSymbolicRegression {

  // more states for individual variables are created dynamically
  internal class ConstraintHandler {
    private int nVars;
    private readonly int maxVariables;

    public int prevTermFirstVariableState;
    public int curTermFirstVariableState;
    public int prevTermFirstFactorType;
    public int curTermFirstFactorType;
    public int prevFactorType;
    public int curFactorType;
    public int prevFactorFirstVariableState;
    public int curFactorFirstVariableState;
    public int prevVariableRef;


    public ConstraintHandler(int maxVars) {
      this.maxVariables = maxVars;
    }

    // 1) an expression is a sum of terms t_1 ... t_n
    //    FirstFactorType(t_i) <= FirstFactorType(t_j) for each pair t_i, t_j where i < j 
    //    FirstVarReference(t_i) <= FirstVarReference(t_j) for each pair t_i, t_j where i < j and FirstFactorType(t_i) = FirstFactorType(t_j)
    // 2) a term is a product of factors, each factor is either a variable factor, an exp factor, a log factor or an inverse factor
    //    FactorType(f_i) <= FactorType(f_j) for each pair of factors f_i, f_j and i < j
    //    FirstVarReference(f_i) <= FirstVarReference(f_j) for each pair of factors f_i, f_j and i < j and FactorType(f_i) = FactorType(f_j)
    // 3) a variable factor is a product of variable references v1...vn
    //    VarIdx(v_i) <= VarIdx(v_j) for each pair of variable references v_i, v_j and i < j
    //    (IMPLICIT) FirstVarReference(t) <= VarIdx(v_i) for each variable reference v_i in term t
    // 4) an exponential factor is the exponential of a product of variables v1...vn
    //    VarIdx(v_i) <= VarIdx(v_j) for each pair of variable references v_i, v_j and i < j
    //    (IMPLICIT) FirstVarReference(t) <= VarIdx(v_i) for each variable reference v_i in term t
    // 5) a log factor is a sum of terms t_i where each term is a product of variables
    //    FirstVarReference(t_i) <= FirstVarReference(t_j) for each pair of terms t_i, t_j and i < j 
    //    for each term t: VarIdx(v_i) <= VarIdx(v_j) for each pair of variable references v_i, v_j and i < j in t
    public bool IsAllowedFollowState(int currentState, int followState) {
      // the following states are always allowed
      if (
        followState == Automaton.StateVariableFactorEnd ||
        followState == Automaton.StateExpFEnd ||
        followState == Automaton.StateExpFactorEnd ||
        followState == Automaton.StateLogTFEnd ||
        followState == Automaton.StateLogTEnd ||
        followState == Automaton.StateLogFactorEnd ||
        followState == Automaton.StateInvTFEnd ||
        followState == Automaton.StateInvTEnd ||
        followState == Automaton.StateInvFactorEnd ||
        followState == Automaton.StateFactorEnd ||
        followState == Automaton.StateTermEnd ||
        followState == Automaton.StateExprEnd
      ) return true;


      // all other states are only allowed if we can add more variables
      if (nVars >= maxVariables) return false;

      // the following states are always allowed when we can add more variables
      if (
        followState == Automaton.StateTermStart ||
        followState == Automaton.StateFactorStart ||
        followState == Automaton.StateExpFStart ||
        followState == Automaton.StateLogTStart ||
        followState == Automaton.StateLogTFStart ||
        followState == Automaton.StateInvTStart ||
        followState == Automaton.StateInvTFStart
        ) return true;

      // enforce non-decreasing factor types 
      if (currentState == Automaton.StateFactorStart) {
        if (curFactorType < 0) {
          //    FirstFactorType(t_i) <= FirstFactorType(t_j) for each pair t_i, t_j where i < j
          return prevTermFirstFactorType <= followState;
        } else {
          // FactorType(f_i) <= FactorType(f_j) for each pair of factors f_i, f_j and i < j
          return curFactorType <= followState;
        }
      }
      // enforce non-decreasing variables references in variable and exp factors
      if (currentState == Automaton.StateVariableFactorStart || currentState == Automaton.StateExpFStart || currentState == Automaton.StateLogTFStart || currentState == Automaton.StateInvTFStart) {
        if (prevVariableRef > followState) return false; // never allow decreasing variables
        if (prevFactorType < 0) {
          // FirstVarReference(t_i) <= FirstVarReference(t_j) for each pair t_i, t_j where i < j
          return prevTermFirstVariableState <= followState;
        } else if (prevFactorType == curFactorType) {
          // (FirstVarReference(f_i) <= FirstVarReference(f_j) for each pair of factors f_i, f_j and i < j and FactorType(f_i) = FactorType(f_j)
          return prevFactorFirstVariableState <= followState;
        }
      }


      return true;
    }


    public void Reset() {
      nVars = 0;


      prevTermFirstVariableState = -1;
      curTermFirstVariableState = -1;
      prevTermFirstFactorType = -1;
      curTermFirstFactorType = -1;
      prevVariableRef = -1;
      prevFactorType = -1;
      curFactorType = -1;
      curFactorFirstVariableState = -1;
      prevFactorFirstVariableState = -1;
    }

    public void StartTerm() {
      // reset factor type. in each term we can start with each type of factor
      prevTermFirstVariableState = curTermFirstVariableState;
      curTermFirstVariableState = -1;

      prevTermFirstFactorType = curTermFirstFactorType;
      curTermFirstFactorType = -1;


      prevFactorType = -1;
      curFactorType = -1;

      curFactorFirstVariableState = -1;
      prevFactorFirstVariableState = -1;
    }

    public void StartFactor(int state) {
      prevFactorType = curFactorType;
      curFactorType = -1;

      prevFactorFirstVariableState = curFactorFirstVariableState;
      curFactorFirstVariableState = -1;


      // store the first factor type
      if (curTermFirstFactorType < 0) {
        curTermFirstFactorType = state;
      }
      curFactorType = state;

      // reset variable references. in each factor we can start with each variable reference
      prevVariableRef = -1;
    }


    public void AddVarToCurrentFactor(int state) {

      Contract.Assert(prevVariableRef <= state);

      // store the first variable reference for each factor 
      if (curFactorFirstVariableState < 0) {
        curFactorFirstVariableState = state;

        // store the first variable reference for each term
        if (curTermFirstVariableState < 0) {
          curTermFirstVariableState = state;
        }
      }
      prevVariableRef = state;

      nVars++;
    }

    public void EndFactor() {
      Contract.Assert(prevFactorFirstVariableState <= curFactorFirstVariableState);
      Contract.Assert(prevFactorType <= curFactorType);
    }

    public void EndTerm() {

      Contract.Assert(prevFactorType <= curFactorType);
      Contract.Assert(prevTermFirstVariableState <= curTermFirstVariableState);
    }
  }
}
