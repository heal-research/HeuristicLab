#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Encodings.RealVectorEncoding;
using HEAL.Attic;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.MOCMAEvolutionStrategy {
  [StorableType("8D5AF328-84A0-4924-9909-A113CDCFC117")]
  public class Individual : IDeepCloneable {

    #region Properties
    [Storable]
    private MOCMAEvolutionStrategy strategy;

    //Chromosome
    [Storable]
    public RealVector Mean { get; private set; }
    [Storable]
    private double sigma;//stepsize
    [Storable]
    private RealVector evolutionPath; // pc
    [Storable]
    private RealVector lastStep;
    [Storable]
    private RealVector lastZ;
    [Storable]
    private double[,] lowerCholesky;

    //Phenotype
    [Storable]
    public double[] Fitness { get; set; }
    [Storable]
    public double[] PenalizedFitness { get; set; }
    [Storable]
    public bool Selected { get; set; }
    [Storable]
    public double SuccessProbability { get; set; }
    #endregion

    #region Constructors and Cloning
    [StorableConstructor]
    protected Individual(StorableConstructorFlag _) { }

    public Individual(RealVector mean, double pSucc, double sigma, RealVector pc, double[,] c, MOCMAEvolutionStrategy strategy) {
      Mean = mean;
      lastStep = new RealVector(mean.Length);
      SuccessProbability = pSucc;
      this.sigma = sigma;
      evolutionPath = pc;
      CholeskyDecomposition(c);
      Selected = true;
      this.strategy = strategy;
    }

    public Individual(Individual other) {
      SuccessProbability = other.SuccessProbability;
      sigma = other.sigma;
      evolutionPath = (RealVector)other.evolutionPath.Clone();
      Mean = (RealVector)other.Mean.Clone();
      lowerCholesky = (double[,])other.lowerCholesky.Clone();
      Selected = true;
      strategy = other.strategy;
    }

    public Individual(Individual other, Cloner cloner) {
      strategy = cloner.Clone(other.strategy);
      Mean = cloner.Clone(other.Mean);
      sigma = other.sigma;
      evolutionPath = cloner.Clone(other.evolutionPath);
      lastStep = cloner.Clone(other.evolutionPath);
      lastZ = cloner.Clone(other.lastZ);
      lowerCholesky = other.lowerCholesky != null ? other.lowerCholesky.Clone() as double[,] : null;
      Fitness = other.Fitness != null ? other.Fitness.Select(x => x).ToArray() : null;
      PenalizedFitness = other.PenalizedFitness != null ? other.PenalizedFitness.Select(x => x).ToArray() : null;
      Selected = other.Selected;
      SuccessProbability = other.SuccessProbability;
    }

    public object Clone() {
      return new Cloner().Clone(this);
    }

    public IDeepCloneable Clone(Cloner cloner) {
      return new Individual(this, cloner);
    }
    #endregion

    public void Mutate(NormalDistributedRandom gauss) {
      //sampling a random z from N(0,I) where I is the Identity matrix;
      lastZ = new RealVector(Mean.Length);
      var n = lastZ.Length;
      for (var i = 0; i < n; i++) lastZ[i] = gauss.NextDouble();
      //Matrixmultiplication: lastStep = lowerCholesky * lastZ;
      lastStep = new RealVector(Mean.Length);
      for (var i = 0; i < n; i++) {
        double sum = 0;
        for (var j = 0; j <= i; j++) sum += lowerCholesky[i, j] * lastZ[j];
        lastStep[i] = sum;
      }
      //add the step to x weighted by stepsize;
      for (var i = 0; i < Mean.Length; i++) Mean[i] += sigma * lastStep[i];
    }

    public void UpdateAsParent(bool offspringSuccessful) {
      SuccessProbability = (1 - strategy.StepSizeLearningRate) * SuccessProbability + strategy.StepSizeLearningRate * (offspringSuccessful ? 1 : 0);
      sigma *= Math.Exp(1 / strategy.StepSizeDampeningFactor * (SuccessProbability - strategy.TargetSuccessProbability) / (1 - strategy.TargetSuccessProbability));
      if (!offspringSuccessful) return;
      if (SuccessProbability < strategy.SuccessThreshold && lastZ != null) {
        var stepNormSqr = lastZ.Sum(d => d * d);
        var rate = strategy.CovarianceMatrixUnlearningRate;
        if (stepNormSqr > 1 && 1 < strategy.CovarianceMatrixUnlearningRate * (2 * stepNormSqr - 1)) rate = 1 / (2 * stepNormSqr - 1);
        CholeskyUpdate(lastStep, 1 + rate, -rate);
      } else RoundUpdate();
    }

    public void UpdateAsOffspring() {
      SuccessProbability = (1 - strategy.StepSizeLearningRate) * SuccessProbability + strategy.StepSizeLearningRate;
      sigma *= Math.Exp(1 / strategy.StepSizeDampeningFactor * (SuccessProbability - strategy.TargetSuccessProbability) / (1 - strategy.TargetSuccessProbability));
      var evolutionpathUpdateWeight = strategy.EvolutionPathLearningRate * (2.0 - strategy.EvolutionPathLearningRate);
      if (SuccessProbability < strategy.SuccessThreshold) {
        UpdateEvolutionPath(1 - strategy.EvolutionPathLearningRate, evolutionpathUpdateWeight);
        CholeskyUpdate(evolutionPath, 1 - strategy.CovarianceMatrixLearningRate, strategy.CovarianceMatrixLearningRate);
      } else RoundUpdate();
    }

    public void UpdateEvolutionPath(double learningRate, double updateWeight) {
      updateWeight = Math.Sqrt(updateWeight);
      for (var i = 0; i < evolutionPath.Length; i++) {
        evolutionPath[i] *= learningRate;
        evolutionPath[i] += updateWeight * lastStep[i];
      }
    }

    #region Helpers
    private void CholeskyDecomposition(double[,] c) {
      if (!alglib.spdmatrixcholesky(ref c, c.GetLength(0), false))
        throw new ArgumentException("Covariancematrix is not symmetric positiv definit");
      lowerCholesky = (double[,])c.Clone();
    }

    private void CholeskyUpdate(RealVector v, double alpha, double beta) {
      var n = v.Length;
      var temp = new double[n];
      for (var i = 0; i < n; i++) temp[i] = v[i];
      double betaPrime = 1;
      var a = Math.Sqrt(alpha);
      for (var j = 0; j < n; j++) {
        var ljj = a * lowerCholesky[j, j];
        var dj = ljj * ljj;
        var wj = temp[j];
        var swj2 = beta * wj * wj;
        var gamma = dj * betaPrime + swj2;
        var x1 = dj + swj2 / betaPrime;
        if (x1 < 0.0) return;
        var nLjj = Math.Sqrt(x1);
        lowerCholesky[j, j] = nLjj;
        betaPrime += swj2 / dj;
        if (j + 1 >= n) continue;
        for (var i = j + 1; i < n; i++) lowerCholesky[i, j] *= a;
        for (var i = j + 1; i < n; i++) temp[i] = wj / ljj * lowerCholesky[i, j];
        if (gamma.IsAlmost(0)) continue;
        for (var i = j + 1; i < n; i++) lowerCholesky[i, j] *= nLjj / ljj;
        for (var i = j + 1; i < n; i++) lowerCholesky[i, j] += nLjj * beta * wj / gamma * temp[i];
      }
    }

    private void RoundUpdate() {
      var evolutionPathUpdateWeight = strategy.EvolutionPathLearningRate * (2.0 - strategy.EvolutionPathLearningRate);
      UpdateEvolutionPath(1 - strategy.EvolutionPathLearningRate, 0);
      CholeskyUpdate(evolutionPath, 1 - strategy.CovarianceMatrixLearningRate + evolutionPathUpdateWeight, strategy.CovarianceMatrixLearningRate);
    }
    #endregion
  }
}
