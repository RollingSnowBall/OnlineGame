﻿using System;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System.IO;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

using ProtoBuf;

public class Client : MonoBehaviour {

	public static int iDataLength = 1024;
	private byte[] readData = new byte[iDataLength];
	public string receiveString;

	private Socket socket;
	// Use this for initialization
	void Start () {
		socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		socket.Connect("127.0.0.1",5001);//101.200.171.91
	}
	bool bIsSend = false;
	void Update()
	{
		if(socket.Connected){
			startReceive();
		}

		if(!bIsSend && socket.Connected){
			bIsSend = true;
			Send("123");
		}
		if(null != receiveString && !receiveString.Equals("")){
			Debug.Log(receiveString);
			receiveString = "";
		}
	}
	bool ReceiveFlag = true;
	void startReceive()
	{
		if (ReceiveFlag) {
			ReceiveFlag = false;
			socket.BeginReceive(readData, 0, readData.Length, SocketFlags.None, new AsyncCallback(endReceive), socket);
		}           
	}

	void endReceive(IAsyncResult iar) //接收数据
	{
		ReceiveFlag = true;
		Socket remote = (Socket)iar.AsyncState;
		int recv = remote.EndReceive(iar);
		if (recv > 0)
		{
			receiveString = Encoding.UTF8.GetString(readData, 0, recv);
		}

	}

	public void Send(string str)
	{
		byte[] msg = serial();//Encoding.UTF8.GetBytes(str);
		IAsyncResult asyncSend = socket.BeginSend(msg, 0, msg.Length, SocketFlags.None, new AsyncCallback(SendData), socket);    //开始发送
		bool success = asyncSend.AsyncWaitHandle.WaitOne(5000, true);
		if (!success)
		{
			Debug.Log("asyncSend error close socket");
		}else{
			Debug.Log("success");
		}
	}

	void SendData(IAsyncResult iar) //发送数据
	{
		Socket remote = (Socket)iar.AsyncState;
		int sent = remote.EndSend(iar);         //关闭发送
		Debug.Log("send the mag`s length is : " + sent);
	}

	byte[] serial(){
		NetMsg.LRequest re = new NetMsg.LRequest();
		re.nEmployeeID = 10;
		re.strSerialNumber = "123";
		re.strUrl = "321";

		using (MemoryStream ms = new MemoryStream()){
			Serializer.Serialize<NetMsg.LRequest>(ms, re);
			byte[] data = new byte[ms.Length];
			ms.Position = 0;
			ms.Read(data, 0, data.Length);
			return data;
		}
	}

}