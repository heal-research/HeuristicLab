package com.heuristiclab.problems.externalevaluation.repetitionsextension;

import java.io.IOException;
import java.util.*;
import com.heuristiclab.problems.externalevaluation.*;
import com.heuristiclab.problems.externalevaluation.ExternalEvaluationMessages.*;
import com.heuristiclab.problems.externalevaluation.repetitionsextension.RepetitionsQualityMessage.RepetitionsResponse;

public class RandomSocketPollEvaluator {

	public static void main(String[] args) {
		ServerSocketListenerFactory factory = new ServerSocketListenerFactory(8842);
		PollService service = new PollService(factory, 1);
		service.start();
		
		Random random = new Random();
		while (true) {
			SolutionMessage msg = service.getSolution();
			double quality = 0;
			
			// replace the line below to calculate the quality
			quality = random.nextDouble();
			// for the simple purpose here it's just a random number
			// now we want to populate the extension fields before we send the message back
			QualityMessage.Builder qualityMessage = 
					service.prepareQualityMessage(msg, quality)
					.setExtension(RepetitionsResponse.repetitions, 1);
			
			try {
				service.sendQualityMessage(msg, qualityMessage.build());
			} catch (IOException e) {
				e.printStackTrace();
			}
		}
	}
}
