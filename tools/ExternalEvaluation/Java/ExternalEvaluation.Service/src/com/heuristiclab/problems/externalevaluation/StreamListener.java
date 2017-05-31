package com.heuristiclab.problems.externalevaluation;

import java.io.*;
import java.util.*;

public class StreamListener implements IListener {
	private InputStream input;
	private OutputStream output;
	private ArrayList<ChannelDiscoveredEventListener> listenerList;
	
	public StreamListener(InputStream input, OutputStream output) {
		this.input = input;
		this.output = output;
		this.listenerList = new ArrayList<ChannelDiscoveredEventListener>();
	}
	
	@Override
	public void listen() {
		onDiscovered(new StreamChannel(this.input, this.output));
	}

	@Override
	public void stop() {
	}

	@Override
	public void addChannelDiscoveredEventListener(
			ChannelDiscoveredEventListener l) {
		synchronized (listenerList) {
			listenerList.add(l);
		}
	}

	@Override
	public void removeChannelDiscoveredEventListener(
			ChannelDiscoveredEventListener l) {
		synchronized (listenerList) {
			listenerList.remove(l);
		}
	}
	
	private void onDiscovered(IChannel channel) {
		synchronized(listenerList) {
			Iterator<ChannelDiscoveredEventListener> listeners = listenerList.iterator();
			while( listeners.hasNext() ) {
				listeners.next().discovered(new EventObject(channel));
			}
		}
	}

}
