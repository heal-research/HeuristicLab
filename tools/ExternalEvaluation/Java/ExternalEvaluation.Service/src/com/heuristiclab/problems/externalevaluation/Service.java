package com.heuristiclab.problems.externalevaluation;

import java.io.*;
import java.util.*;
import java.util.concurrent.*;
import com.google.protobuf.*;
import com.heuristiclab.problems.externalevaluation.ExternalEvaluationMessages.*;

public abstract class Service implements ChannelDiscoveredEventListener {
	public static final int MAXCONNECTIONS = 10;
	private IListener listener;
	private int nThreads;
	private ArrayList<IChannel> channels;
	private HashMap<SolutionMessage, IChannel> recipientMemory;
	
	public Service(IListenerFactory listenerFactory, int nThreads) {
		listener = listenerFactory.createListener();
		this.nThreads = Math.max(nThreads, 1);
		channels = new ArrayList<IChannel>();
		recipientMemory = new HashMap<SolutionMessage, IChannel>();
		listener.addChannelDiscoveredEventListener(this);
	}
	
	@Override
	public void discovered(EventObject channel) {
		synchronized (channels) {
			if (channels.size() < MAXCONNECTIONS) {
				IChannel c = (IChannel)channel.getSource();
				channels.add(c);
				SolutionProducer producer = new SolutionProducer(c);
				Thread tmp = new Thread(producer);
				tmp.start();
			} else {
				try {
					((IChannel)channel.getSource()).close();
				} catch (IOException e) { e.printStackTrace(); }
			}
		}
	}
	
	public void start() {
		listener.listen();
	}
	
	public void stop() {
		listener.stop();
		synchronized (channels) {
			for (IChannel channel : channels) {
				try {
					channel.close();
				} catch (IOException e) { e.printStackTrace(); }
			}
			channels.clear();
		}
		synchronized (recipientMemory) {
			recipientMemory.clear();
		}
	}
	
	private void producerFinished(SolutionProducer producer) {
		synchronized (channels) {
			channels.remove(producer.channel);
		}
		synchronized (recipientMemory) {
			for (Iterator<Map.Entry<SolutionMessage, IChannel>> iter = recipientMemory.entrySet().iterator(); iter.hasNext();) {
				if (iter.next().getValue() == producer.channel)
					iter.remove();
			}
		}
	}
	
	protected void send(Message qualityMessage, Message solutionMessage) throws IOException {
		synchronized (recipientMemory) {
			if (recipientMemory.containsKey(solutionMessage)) {
				recipientMemory.get(solutionMessage).send(qualityMessage);
				recipientMemory.remove(solutionMessage);
			}
		}
	}
	
	protected void onSolutionProduced(SolutionMessage msg, IChannel channel) {
		synchronized (recipientMemory) {
			recipientMemory.put(msg, channel);
		}
	}
	
	private class SolutionProducer implements Runnable {
		private Executor threadPool;
		private IChannel channel;
		
		public SolutionProducer(IChannel channel) {
			this.channel = channel;
			if (nThreads == 1)
				threadPool = Executors.newSingleThreadExecutor();
			else
				threadPool = Executors.newFixedThreadPool(nThreads);
		}
		@Override
		public void run() {
			while (true) {
				SolutionMessage.Builder builder = SolutionMessage.newBuilder();
				try {
					SolutionMessage msg = (SolutionMessage)channel.receive(builder);
					threadPool.execute(new MessageNotifier(msg, channel));
				} catch (Exception e) {
					break;
				}
			}
			producerFinished(this);
		}
		
		private class MessageNotifier implements Runnable {
			SolutionMessage msg;
			IChannel channel;
			public MessageNotifier(SolutionMessage msg, IChannel channel) {
				this.msg = msg;
				this.channel = channel;
			}
			@Override
			public void run() {
				onSolutionProduced(msg, channel);
			}
			
		}
	}
}
