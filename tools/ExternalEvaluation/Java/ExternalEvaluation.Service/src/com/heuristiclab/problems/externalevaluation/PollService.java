package com.heuristiclab.problems.externalevaluation;

import java.io.*;
import java.util.*;
import java.util.concurrent.*;
import com.heuristiclab.problems.externalevaluation.ExternalEvaluationMessages.*;
import com.heuristiclab.problems.externalevaluation.ExternalEvaluationMessages.QualityMessage.Type;

public class PollService extends Service {
	private LinkedBlockingQueue<SolutionMessage> messageQueue;
	private ArrayList<SolutionEventListener> listenerList;
	
	public PollService(IListenerFactory listenerFactory, int nThreads) {
		super(listenerFactory, nThreads);
		messageQueue = new LinkedBlockingQueue<SolutionMessage>();
		listenerList = new ArrayList<SolutionEventListener>();
	}
	
	@Override
	public void stop() {
		messageQueue.clear();
		super.stop();
	}
	
	public synchronized SolutionMessage getSolution() {
		try {
			return messageQueue.take();
		} catch (InterruptedException e) {
			e.printStackTrace();
		}
		return null;
	}
	
	public void sendQualityMessage(SolutionMessage sMsg, QualityMessage qMsg) throws IOException {
		send(qMsg, sMsg);
	}
	
	public QualityMessage.Builder prepareQualityMessage(SolutionMessage msg, double quality) {
		return QualityMessage.newBuilder()
			.setSolutionId(msg.getSolutionId())
			.setType(Type.SingleObjectiveQualityMessage)
			.setExtension(
					SingleObjectiveQualityMessage.qualityMessage, 
					SingleObjectiveQualityMessage.newBuilder().setQuality(quality).build());
	}
		
	public void sendQuality(SolutionMessage msg, double quality) throws IOException {
		send(prepareQualityMessage(msg, quality).build(), msg);
	}
	
	public QualityMessage.Builder prepareQualitiesMessage(SolutionMessage msg, double[] qualities) {
		List<Double> qualitiesList = new ArrayList<Double>(qualities.length);
		for (double quality : qualities) qualitiesList.add(quality);
		return QualityMessage.newBuilder()
				.setSolutionId(msg.getSolutionId())
				.setType(Type.MultiObjectiveQualityMessage)
				.setExtension(
						MultiObjectiveQualityMessage.qualityMessage, 
						MultiObjectiveQualityMessage.newBuilder().addAllQualities(qualitiesList).build());
	}
		
	public void sendQualities(SolutionMessage msg, double[] qualities) throws IOException {
		send(prepareQualitiesMessage(msg, qualities).build(), msg);
	}
	
	public void addSolutionEventListener(SolutionEventListener l) {
		synchronized(listenerList) {
			listenerList.add(l);
		}
	}
	public void removeSolutionEventListener(SolutionEventListener l) {
		synchronized(listenerList) {
			listenerList.remove(l);
		}
	}
	
	@Override
	protected void onSolutionProduced(SolutionMessage msg, IChannel channel) {
		super.onSolutionProduced(msg, channel);
		try {
			messageQueue.put(msg);
		} catch (InterruptedException e) {
			e.printStackTrace();
			return;
		}
		synchronized(listenerList) {
			Iterator<SolutionEventListener> listeners = listenerList.iterator();
			while( listeners.hasNext() ) {
				listeners.next().solutionReceived(new EventObject(this));
			}
		}
	}
}
