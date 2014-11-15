using System;
using System.IO.Ports;
using System.Threading;

namespace SerialBT
{
	public class SerialBT
	{
		/// <summary>�V���A���ʐM����</summary>
		private static SerialPortProcessor serialPortP;
		/// <summary>�V���A���ʐM���~�������ǂ���</summary>
		private static bool isStop = false;

		/// <summary>�}�E�X����</summary>
		private static MouseProc mMouseProc;

		/// <summary>����:�|�[�g��</summary>
		private const string PARAM_PORT_NAME = "-p";
		/// <summary>����:�҂�����(sec)</summary>
		private const string PARAM_WAIT_TIME = "-t";
		/// <summary>����:�w���v</summary>
		private const string PARAM_HELP = "-h";

		/// <summary>�����l�F�|�[�g��</summary>
		private const string DEF_PORT_NAME = "COM3";
		/// <summary>�����l�F�҂�����</summary>
		private const int DEF_WAIT_TIME = 30;

		/// <summary>�҂����Ԃɑ΂���|����</summary>
		/// <remarks>
		/// main������sec�P�ʂœn����邪�A�҂����[�v���ɂ�
		/// 100msec�̑҂������{���邽�߁A10�{���Ă����K�v������B
		/// </remarks>
		private const int WAIT_TIME_WEIGHT = 10;

		/// <summary>
		/// ����^�C�v���
		/// </summary>
		private class TypInfo
		{
			/// <summary>�^�C�v���</summary>
			public const int LEN = 2;

			/// <summary>�ړ�</summary>
			public const string MOVE = "mv";
			/// <summary>�_�u���^�b�v</summary>
			public const string DTAP = "dt";
			/// <summary>�����O�v���X</summary>
			public const string LPRS = "lp";
		}

		public static void Main(string[] args)
		{
			try
			{
				string portName;
				int waitCnt;
				bool isShowHelp;
				GetParam(args, out isShowHelp, out portName, out waitCnt);

				if (isShowHelp)
				{
					Console.WriteLine("-p : Port Name. Default " + DEF_PORT_NAME);
					Console.WriteLine("-t : Wait Time (sec). Default " + DEF_WAIT_TIME);
					Console.WriteLine("-h : Help");
					return;
				}

				mMouseProc = new MouseProc();

				serialPortP = new SerialPortProcessor()
				{
					PortName = portName,
					BaudRate = 9600,
					Parity = Parity.None,
					DataBits = 8,
					StopBits = StopBits.One,
				};

				serialPortP.DataReceived += DataReceived;
				serialPortP.Start();

				Console.WriteLine("Start");

				var cnt = 0;
				while (!isStop)
				{
					Thread.Sleep(100);
					cnt++;

					if (waitCnt < cnt)
					{
						Close();
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Close();
			}
		}

		~SerialBT()
		{
			Close();
		}

		/// <summary>
		/// �f�[�^��M����
		/// </summary>
		/// <param name="data"></param>
		public static void DataReceived(byte[] data)
		{
			string mes = System.Text.Encoding.UTF8.GetString(data);
			Console.WriteLine(mes);

			if (String.IsNullOrWhiteSpace(mes) || mes.Length < TypInfo.LEN)
			{
				return;
			}

			if (mMouseProc == null)
			{
				mMouseProc = new MouseProc();
			}

			var mesType = mes.Substring(0, TypInfo.LEN);
			switch (mesType)
			{
				case TypInfo.MOVE:
					string posStr = mes.Substring(TypInfo.LEN);
					if (String.IsNullOrWhiteSpace(posStr))
					{
						break;
					}

					string[] posAry = posStr.Split(',');
					if (posAry == null || posAry.Length < 2)
					{
						break;
					}

					int mvX, mvY;
					if (!Int32.TryParse(posAry[0], out mvX) || !Int32.TryParse(posAry[1], out mvY))
					{
						break;
					}

					mMouseProc.SetCursor(mvX, mvY);
					break;
				case TypInfo.DTAP:
					mMouseProc.LeftClick();
					break;
				case TypInfo.LPRS:
					mMouseProc.RightClick();
					break;
			}

		}

		/// <summary>
		/// �V���A���ʐM�N���[�Y����
		/// </summary>
		public static void Close()
		{
			if (serialPortP != null)
			{
				serialPortP.Close();
				Console.WriteLine("Close");
			}

			isStop = true;
		}

		/// <summary>
		/// �����̒l���擾����
		/// </summary>
		/// <param name="args">����</param>
		/// <param name="isShowHelp">�w���v��\�����邩�ǂ���</param>
		/// <param name="portName">�|�[�g��</param>
		/// <param name="waitCnt">�҂��J�E���^</param>
		public static void GetParam(string[] args, out bool isShowHelp, out string portName, out int waitCnt)
		{
			portName = DEF_PORT_NAME;
			waitCnt = DEF_WAIT_TIME * WAIT_TIME_WEIGHT;
			isShowHelp = false;

			if (args == null || args.Length <= 0)
			{
				return;
			}

			if (args[0] == PARAM_HELP)
			{
				isShowHelp = true;
				return;
			}

			for (var idx = 0; (idx + 1) < args.Length; idx++)
			{
				var key = args[idx];
				var val = args[idx + 1];

				switch (key)
				{
					case PARAM_PORT_NAME:
						portName = val;
						break;
					case PARAM_WAIT_TIME:
						int wait;
						if (Int32.TryParse(val, out wait))
						{
							waitCnt = wait * WAIT_TIME_WEIGHT;
						}
						break;
					default:
						continue;
				}

				idx++;
			}
		}
	}
}