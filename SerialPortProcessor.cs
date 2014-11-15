using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace SerialBT
{
    public class SerialPortProcessor
    {
        private SerialPort mSerialPort = null;
        private Thread mReceiveThread = null;

        /// <summary>ポート名</summary>
        public String PortName { get; set; }
        /// <summary>ボーレート</summary>
        public int BaudRate { get; set; }
        /// <summary>パリティ</summary>
        public Parity Parity { get; set; }
        /// <summary>データビット</summary>
        public int DataBits { get; set; }
        /// <summary>ストップビット</summary>
        public StopBits StopBits { get; set; }

        public delegate void DataReceivedHandler(byte[] data);
        /// <summary>データ受信時イベント</summary>
        public event DataReceivedHandler DataReceived;

        public SerialPortProcessor()
        {
        }

        /// <summary>
        /// シリアル通信開始処理
        /// </summary>
        public void Start()
        {
            mSerialPort = new SerialPort(
                 PortName, BaudRate, Parity, DataBits, StopBits);
            mSerialPort.Open();
            mReceiveThread = new Thread(SerialPortProcessor.ReceiveWork);
            mReceiveThread.Start(this);
        }

        /// <summary>
        /// データ受信開始処理
        /// </summary>
        /// <param name="target"></param>
        public static void ReceiveWork(object target)
        {
            SerialPortProcessor serialProc = target as SerialPortProcessor;
            serialProc.ReceiveData();
        }

        /// <summary>
        /// データ送信処理
        /// </summary>
        /// <param name="buffer"></param>
        public void WriteData(byte[] buffer)
        {
            mSerialPort.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// データ受信処理
        /// </summary>
        public void ReceiveData()
        {
            if (mSerialPort == null)
            {
                return;
            }
            do
            {
                try
                {
                    int rbyte = mSerialPort.BytesToRead;
                    byte[] buffer = new byte[rbyte];
                    int read = 0;
                    while (read < rbyte)
                    {
                        int length = mSerialPort.Read(buffer, read, rbyte - read);
                        read += length;
                    }
                    if (rbyte > 0)
                    {
                        DataReceived(buffer);
                    }
                }
                catch (IOException)
                {
                }
                catch (InvalidOperationException)
                {
                }
            } while (mSerialPort.IsOpen);
        }

        /// <summary>
        /// シリアル通信クローズ処理
        /// </summary>
        public void Close()
        {
            if (mReceiveThread != null && mSerialPort != null)
            {
                mSerialPort.Close();
                mReceiveThread.Join();
            }
        }
    }
}