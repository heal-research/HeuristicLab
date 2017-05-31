package com.heuristiclab.problems.externalevaluation;

public class ServerSocketListenerFactory implements IListenerFactory {

	private String ipAddress;
	private int port;
	
	public ServerSocketListenerFactory(int port) {
		this.ipAddress = null;
		this.port = port;
	}
	
	public ServerSocketListenerFactory(int port, String ipAddress) {
		this.ipAddress = ipAddress;
		this.port = port;
	}
	
	@Override
	public IListener createListener() {
		if (ipAddress == null)
			return new ServerSocketListener(port);
		else
			return new ServerSocketListener(port, ipAddress);
	}

}
