/* An example for communication between Unity and Arduino through serial port
 *
 * ref: https://www.alanzucconi.com/2016/12/01/asynchronous-serial-communication/
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class SerialPortExample : MonoBehaviour {

    public int turns;
	private CommunicateWithArduino Uno = new CommunicateWithArduino();

	bool reachLaps;

	void Start () {

		new Thread(Uno.connectToArdunio).Start();		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.W)) {
			new Thread(Uno.SendData).Start("2 50");
		}
		if (Input.GetKey(KeyCode.E)) {
			new Thread(Uno.SendData).Start("2 75");
		}
		if (Input.GetKey(KeyCode.R)) {
			new Thread(Uno.SendData).Start("2 100");
		}
		if (Input.GetKey(KeyCode.Q)) {
			new Thread(Uno.SendData).Start("1 0");
		}
        turns = Int32.Parse(Uno.ReadData());
		Debug.Log(turns);
	}

	class CommunicateWithArduino
	{
		public bool connected = true;
		public bool mac = true;
		public string choice = "cu.usbmodem1411";
		private SerialPort arduinoController;

		public void connectToArdunio()
		{

			if (connected)
			{
				string portChoice = "COM5";
				if (mac)
				{
					int p = (int)Environment.OSVersion.Platform;
					// Are we on Unix?
					if (p == 4 || p == 128 || p == 6)
					{
						List<string> serial_ports = new List<string>();
						string[] ttys = Directory.GetFiles("/dev/", "cu.*");
						foreach (string dev in ttys)
						{
							if (dev.StartsWith("/dev/tty."))
							{
								serial_ports.Add(dev);
								Debug.Log(String.Format(dev));
							}
						}
					}
					portChoice = "/dev/" + choice;
				}
				arduinoController = new SerialPort(portChoice, 9600, Parity.None, 8, StopBits.One);
				arduinoController.Handshake = Handshake.None;
				arduinoController.RtsEnable = true;
				// read waiting time
				arduinoController.ReadTimeout = 5;
				arduinoController.Open();
				Debug.LogWarning(arduinoController);
			}

		}
		public void SendData(object obj)
		{
			string data = obj as string;
			Debug.Log(data);
			if (connected)
			{
				if (arduinoController != null)
				{
					arduinoController.Write(data);
					arduinoController.Write("\n");
				}
				else
				{
					Debug.Log(arduinoController);
					Debug.Log("nullport");
				}
			}
			else
			{
				Debug.Log("not connected");
			}
			Thread.Sleep(500);
		}
	
		public string ReadData()
		{
			string read = "";
			if (connected)
			{
				if (arduinoController != null)
				{
					read = arduinoController.ReadLine();
				}
				else
				{
					Debug.Log(arduinoController);
					Debug.Log("nullport");
				}
			}
			else
			{
				Debug.Log("not connected");
			}

			return read;
		}
	}


}
