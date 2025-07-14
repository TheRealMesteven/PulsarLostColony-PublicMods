using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace OpenTrack_Tracker
{
    public class UDPReceiver : MonoBehaviour
    {   // source:https://github.com/opentrack/opentrack/discussions/1850
        private UdpClient udpClient;
        private IPEndPoint endPoint;
        private Thread receiveThread;

        private double[] values;
        private Vector3 Position;
        private Vector3 Rotation;

        // GameObject to apply position and rotation data
        //public GameObject targetObject;

        void Start()
        {
            endPoint = new IPEndPoint(IPAddress.Any, 4242);
            udpClient = new UdpClient(endPoint);
            udpClient.Client.ReceiveBufferSize = 512;

            // Start the receive thread
            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        private void ReceiveData()
        {
            while (true)
            {
                try
                {
                    byte[] data = udpClient.Receive(ref endPoint);
                    if (data.Length != 48) throw new Exception("Data length is not 48 bytes");
                    int sectionSize = 8;
                    values = new double[data.Length / sectionSize];
                    int numSections = data.Length / sectionSize;
                    byte[][] sections = new byte[numSections][];

                    for (int i = 0; i < numSections; i++)
                    {
                        sections[i] = new byte[sectionSize];
                        Buffer.BlockCopy(data, i * sectionSize, sections[i], 0, sectionSize);
                    }

                    // Convert sections to doubles and assign to Position and Rotation
                    for (int i = 0; i < 6; i++)
                    {
                        double value = BitConverter.ToDouble(sections[i], 0);
                        values[i] = value;

                        if (i < 3)
                        {
                            // Assign the first three values to Position
                            Position[i] = (float)value / -100;
                        }
                        else
                        {
                            // Switch the y and x rotation
                            if (i == 3)
                            {
                                Rotation.y = (float)value;
                            }
                            else if (i == 4)
                            {
                                Rotation.x = (float)value * -1;
                            }
                            else
                            {
                                Rotation.z = (float)value;
                            }
                        }
                    }


                }
                catch (Exception e)
                {
                    Debug.Log("Error: " + e.ToString());
                }

                // Sleep for 10 milliseconds to reduce the frequency of UDP calls
                //Thread.Sleep(10);
            }
        }

        private void OnApplicationQuit()
        {
            if (receiveThread != null)
            {
                receiveThread.Abort();
            }

            udpClient.Close();
        }

        void Update()
        {
            //Debug.Log($"[OpenTrack] Position:({Position.x}, {Position.y}, {Position.z}) Rotation:({Rotation.x}, {Rotation.y}, {Rotation.z})");
            var camera = PLCameraSystem.Instance.CurrentSubSystem.LocalPawnCameras[0];
            if (camera == null) return;
            camera.transform.localPosition = Position;
            camera.transform.localEulerAngles = Rotation;
        }
    }

}
