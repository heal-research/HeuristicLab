package com.heuristiclab.problems.externalevaluation;

import java.io.*;
import java.net.*;
import java.util.*;

public class ServerSocketListener implements IListener {
	private ServerSocket serverSocket;
	private int port;
	private InetAddress bindAddress;
    private Thread listenerThread;
	private ListenerThread producer;
	private ArrayList<ChannelDiscoveredEventListener> listenerList;

    public ServerSocketListener(int port) {
		this.port = port;
		this.bindAddress = null;
		this.producer = new ListenerThread();
		this.listenerList = new ArrayList<ChannelDiscoveredEventListener>();
	}
	
	public ServerSocketListener(int port, String ipAddress) {
		this.port = port;
		try {
			this.bindAddress = InetAddress.getByName(ipAddress);
		} catch (UnknownHostException e) {
			e.printStackTrace();
		}
		this.producer = new ListenerThread();
		this.listenerList = new ArrayList<ChannelDiscoveredEventListener>();
	}

	@Override
    public void listen() {
		if (serverSocket == null) {
			try {
			if (bindAddress == null)
				serverSocket = new ServerSocket(port, 1);
			else
				serverSocket = new ServerSocket(port, 1, bindAddress);
			} catch (IOException e) {
				e.printStackTrace();
				return;
			}
		}
        listenerThread = new Thread(this.producer);
        listenerThread.start();
    }
	
	@Override
	public void stop() {
		listenerThread.interrupt();
	    if (serverSocket != null) {
	        try {
				serverSocket.close();
			} catch (IOException e) {
				e.printStackTrace();
			}
	        serverSocket = null;
	    }
	}

	@Override
	public void addChannelDiscoveredEventListener(ChannelDiscoveredEventListener l) {
		synchronized(listenerList) {
			listenerList.add(l);
		}
	}
	
	@Override
	public void removeChannelDiscoveredEventListener(ChannelDiscoveredEventListener l) {
		synchronized(listenerList) {
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
	
	private class ListenerThread implements Runnable {

		@Override
		public void run() {
			while (true) {
			    try {
			    	Socket client = serverSocket.accept();
			    	StreamChannel streamChannel = new StreamChannel(client.getInputStream(), client.getOutputStream());
			    	onDiscovered(streamChannel);
			    } catch (Exception e) {
			    	try {
			    		stop();
			    	} catch (Exception tae) {
			    		break;
			    	}
			    }
			}
		}
	}
	
}
