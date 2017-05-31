package com.heuristiclab.problems.externalevaluation;

import java.io.IOException;

import com.heuristiclab.problems.externalevaluation.ExternalEvaluationMessages.QualityMessage;
import com.heuristiclab.problems.externalevaluation.ExternalEvaluationMessages.QualityMessage.Type;
import com.heuristiclab.problems.externalevaluation.ExternalEvaluationMessages.SingleObjectiveQualityMessage;
import com.heuristiclab.problems.externalevaluation.ExternalEvaluationMessages.SolutionMessage;

public class PushService extends Service {
	private IEvaluationService customService;
	
	public PushService(IListenerFactory listenerFactory, int nThreads, IEvaluationService customService) {
		super(listenerFactory, nThreads);
		this.customService = customService;
	}

	@Override
	protected void onSolutionProduced(SolutionMessage msg, IChannel channel) {
		super.onSolutionProduced(msg, channel);
		try {
			QualityMessage.Builder qualityMessage = QualityMessage.newBuilder();
			double quality = customService.evaluate(msg, qualityMessage);
			qualityMessage
				.setSolutionId(msg.getSolutionId())
				.setType(Type.SingleObjectiveQualityMessage)
				.setExtension(
					SingleObjectiveQualityMessage.qualityMessage, 
					SingleObjectiveQualityMessage.newBuilder().setQuality(quality).build());
			send(qualityMessage.build(), msg);
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

}
