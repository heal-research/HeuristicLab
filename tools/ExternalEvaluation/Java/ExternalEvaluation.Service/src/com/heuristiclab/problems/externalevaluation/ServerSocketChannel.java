package com.heuristiclab.problems.externalevaluation;

import java.io.*;
import java.net.*;

import com.google.protobuf.Message;
import com.google.protobuf.Message.Builder;

public class ServerSocketChannel extends Channel {
	private ServerSocket serverSocket;
	private Socket clientSocket;
	private int port;
	private InetAddress bindAddress;
	private StreamChannel streamChannel;
	
	public ServerSocketChannel(int port) {
		this.port = port;
		this.bindAddress = null;
	}
	
	public ServerSocketChannel(int port, String ipAddress) {
		this.port = port;
		try {
			this.bindAddress = InetAddress.getByName(ipAddress);
		} catch (UnknownHostException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	@Override
	public void open() throws IOException {
		super.open();
		if (serverSocket == null) {
			if (bindAddress == null)
				serverSocket = new ServerSocket(port, 1);
			else
				serverSocket = new ServerSocket(port, 1, bindAddress);
		}
		listenForClient();
	}
	
	private void listenForClient() throws IOException {
		clientSocket = serverSocket.accept();
		streamChannel = new StreamChannel(clientSocket.getInputStream(), clientSocket.getOutputStream());
		streamChannel.open();	
	}
	
	@Override
	public Message receive(Builder builder) throws IOException {
		return streamChannel.receive(builder);
	}

	@Override
	public void send(Message msg) throws IOException {
		streamChannel.send(msg);
	}
	
	@Override
	public void close() throws IOException {
		streamChannel.close();
		streamChannel = null;
		clientSocket.close();
		clientSocket = null;
		serverSocket.close();
		serverSocket = null;
	}

}
