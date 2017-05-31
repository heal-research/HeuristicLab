package com.heuristiclab.problems.externalevaluation;

import com.heuristiclab.problems.externalevaluation.ExternalEvaluationMessages.*;

public interface IEvaluationService {
	double evaluate(SolutionMessage request, QualityMessage.Builder responseBuilder);
}
