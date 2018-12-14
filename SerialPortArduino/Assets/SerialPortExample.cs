/* An example for communication between Unity and Arduino through serial port
 *
 * ref: https://www.alanzucconi.com/2016/12/01/asynchronous-serial-communication/
 * Change the API Compatibility Level to 2.0
 * Edit > Project Setting > Player > Inspector
 * Find API Compatibility Level and change it to 2.0, so that you can use System.IO.Ports 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class SerialPortExample : MonoBehaviour {

    private CommunicateWithArduino Uno = new CommunicateWithArduino("COM5", baudRate:115200);


	void Start () {

		new Thread(Uno.connectToArdunio).Start();		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.T)) {
            Debug.Log("Send thermal stimulus to peltiers.");
			new Thread(Uno.SendData).Start("50 50");
		}
        if (Input.GetKeyDown(KeyCode.R)) {
            Debug.Log("Reset peltiers with '0 0'."); 
            new Thread(Uno.SendData).Start("0 0"); 
        }
	}

	class CommunicateWithArduino
	{
		public bool connected = true;
		public bool mac = false;
		public string choice = "cu.usbmodem1411";
		private SerialPort arduinoController;

        private string portName;
	    private int baudRate;
	    private Parity parity;
	    private int dataBits;
	    private StopBits stopBits;
	    private Handshake handshake;
	    private bool RtsEnable;
	    private int ReadTimeout;
	    private bool isMac;
	    private bool isConnected;

        public CommunicateWithArduino(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One, Handshake handshake = Handshake.None,
		        bool RtsEnable = true, int ReadTimeout = 1, bool isMac = false, bool isConnected = true)
        {
		        this.portName = portName;
		        this.baudRate = baudRate;
		        this.parity = parity;
		        this.dataBits = dataBits;
		        this.stopBits = stopBits;
		        this.handshake = handshake;
		        this.RtsEnable = RtsEnable;
		        this.ReadTimeout = ReadTimeout;
		        this.isMac = isMac;
		        this.isConnected = isConnected;
		        //connectToArdunio();
		}

		public void connectToArdunio()
		{

			if (connected)
			{
				string portChoice = portName;
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
				arduinoController = new SerialPort(portChoice, 115200, Parity.None, 8, StopBits.One);
				arduinoController.Handshake = Handshake.None;
				arduinoController.RtsEnable = true;
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
    }


}
