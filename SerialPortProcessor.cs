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

        /// <summary>�|�[�g��</summary>
        public String PortName { get; set; }
        /// <summary>�{�[���[�g</summary>
        public int BaudRate { get; set; }
        /// <summary>�p���e�B</summary>
        public Parity Parity { get; set; }
        /// <summary>�f�[�^�r�b�g</summary>
        public int DataBits { get; set; }
        /// <summary>�X�g�b�v�r�b�g</summary>
        public StopBits StopBits { get; set; }

        public delegate void DataReceivedHandler(byte[] data);
        /// <summary>�f�[�^��M���C�x���g</summary>
        public event DataReceivedHandler DataReceived;

        public SerialPortProcessor()
        {
        }

        /// <summary>
        /// �V���A���ʐM�J�n����
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
        /// �f�[�^��M�J�n����
        /// </summary>
        /// <param name="target"></param>
        public static void ReceiveWork(object target)
        {
            SerialPortProcessor serialProc = target as SerialPortProcessor;
            serialProc.ReceiveData();
        }

        /// <summary>
        /// �f�[�^���M����
        /// </summary>
        /// <param name="buffer"></param>
        public void WriteData(byte[] buffer)
        {
            mSerialPort.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// �f�[�^��M����
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
        /// �V���A���ʐM�N���[�Y����
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