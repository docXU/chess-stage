using ChessMiddle;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
namespace SocketServer
{


    public partial class Server : Form
    {
        #region TCPServer服务器

        private char[,] chess;
        private ITxServer server = null;
        /// <summary>
        /// 当接收到来之客户端的文本信息的时候
        /// </summary>
        /// <param name="state"></param>
        /// <param name="str"></param>
        private void acceptString(IPEndPoint ipEndPoint, string str)
        {
            ListViewItem item = new ListViewItem(new string[] { DateTime.Now.ToString(), ipEndPoint.ToString(), str });
            this.listView1.Items.Insert(0, item);
        }
        /// <summary>
        /// 当接收到来之客户端的字节信息的时候
        /// </summary>
        /// <param name="ipEndPoint"></param>
        /// <param name="bytes"></param>
        private void acceptBytes(IPEndPoint ipEndPoint, byte[] bytes)
        {
            ListViewItem item = new ListViewItem(new string[] { DateTime.Now.ToString(),
                ipEndPoint.ToString(), System.Text.Encoding.Default.GetString(bytes) });
            this.listView1.Items.Insert(0, item);
            //MessageBox.Show(bytes.Length.ToString());
        }
        /// <summary>
        /// 当有客户端连接上来的时候
        /// </summary>
        /// <param name="state"></param>
        private void connect(IPEndPoint ipEndPoint)
        {
            show(ipEndPoint, "上线");
        }

        ///// <summary>
        ///// 当对方已收到我方发送数据的时候
        ///// </summary>
        ///// <param name="state"></param>
        //private void dateSuccess(IPEndPoint ipEndPoint)
        //{
        //    textBox_msg.Text = "已向" + ipEndPoint.ToString() + "发送成功";
        //}

        /// <summary>
        /// 当有客户端掉线的时候
        /// </summary>
        /// <param name="state"></param>
        /// <param name="str"></param>
        private void disconnection(IPEndPoint ipEndPoint, string str)
        {
            show(ipEndPoint, "下线");
            chessInit(new char[8, 8]
                {
                    {'0',  'a', '0',  'a',  '0',  'a',  '0',  'a' },
                    {'0',  '0', '0',  '0',  '0',  '0',  '0',  '0' },
                    {'0',  'a', '0',  '0',  'a',  '0',  'a',  '0' },
                    {'0',  '0', '0',  '0',  '0',  '0',  '0',  '0' },
                    {'0',  '0', 'a',  '0',  'a',  '0',  '0',  '0' },
                    {'0',  '0', '0',  'B',  '0',  '0',  '0',  '0' },
                    {'0',  '0', 'a',  'b',  'a',  '0',  '0',  '0' },
                    {'0',  '0', '0',  '0',  'b',  '0',  'b',  '0' }
                });
        }

        /// <summary>
        /// 当服务器完全关闭的时候
        /// </summary>
        private void engineClose()
        {
            MessageBox.Show("服务器已关闭");
        }

        /// <summary>
        /// 当服务器非正常原因断开的时候
        /// </summary>
        /// <param name="str"></param>
        private void engineLost(string str)
        {
            MessageBox.Show(str);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // server.clientClose(server.StateAll[0]);
            MessageBox.Show(server.ClientAll[0].ToString());
        }

        /// <summary>
        /// 启动按钮Tcp服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                server = TxStart.startServer(int.Parse(textBox_port.Text));
                server.AcceptByte += new TxDelegate<IPEndPoint, byte[]>(acceptBytes);
                server.Connect += new TxDelegate<IPEndPoint>(connect);
                //server.dateSuccess += new TxDelegate<IPEndPoint>(dateSuccess);
                server.Disconnection += new TxDelegate<IPEndPoint, string>(disconnection);
                server.EngineClose += new TxDelegate(engineClose);
                server.EngineLost += new TxDelegate<string>(engineLost);
                server.PlayChess += new TxDelegate<List<string>, char[,], char, char>(playChess);
                server.LimitThinkSeconds = 2;
                server.StartEngine();
                this.button1.Enabled = false;
                chessInit(server.GetChessLayout());
            }
            catch (Exception Ex) { MessageBox.Show(Ex.Message); }

        }

        private void playChess(List<string> actionMove, char[,] layout, char role, char result)
        {
            //画面的UI处理函数

            //bool eatSome = false;
            //foreach (string i in actionMove)
            //{
            //    Console.WriteLine(i);
            //    eatSome = false;
            //    string[] typeEat = Regex.Split(i, "--");

            //    if (typeEat.Length == 2)
            //    {
            //        eatSome = true;
            //    }

            //    string oldPos;
            //    string newPos;
            //    if (eatSome)
            //    {
            //        oldPos = typeEat[0];
            //        newPos = typeEat[1];
            //        Console.WriteLine("eat: " + i);
            //    }
            //    else
            //    {
            //        string[] typePush = Regex.Split(i, "-");
            //        for (int j = 0; j < typePush.Length; j++)
            //            Console.WriteLine("just push: " + typePush[j]);
            //        oldPos = typePush[0];
            //        newPos = typePush[1];

            //    }

            //    int x_old = int.Parse(oldPos.Split(',')[0]);
            //    int y_old = int.Parse(oldPos.Split(',')[1]);
            //    int x_new = int.Parse(newPos.Split(',')[0]);
            //    int y_new = int.Parse(newPos.Split(',')[1]);

            //    if (eatSome)
            //    {
            //        chess[x_old, y_old] = '0';
            //        chess[(x_old + x_new) / 2, (y_old + y_new) / 2] = '0';
            //        chess[x_new, y_new] = role;
            //    }
            //    else
            //    {
            //        chess[x_old, y_old] = '0';
            //        chess[x_new, y_new] = role;
            //    }

            //}
            convertToUI(layout, result);
        }

        /// <summary>
        /// 下面显示的
        /// </summary>
        /// <param name="ipEndPoint"></param>
        /// <param name="str"></param>
        private void show(IPEndPoint ipEndPoint, string str)
        {
            label_zt.Text = ipEndPoint.ToString() + ":" + str;
            label_all.Text = "当前在线人数:" + this.server.ClientNumber.ToString();
        }
        #endregion

        public Server()
        {
            InitializeComponent();

        }

        private void chessInit(char[,] chessLayout)
        {
            chess = chessLayout;
            convertToUI(chess, '~');
        }

        private void convertToUI(char[,] layout, char result)
        {
            string UI = server.GetChessLayoutStr();
            this.textBox1.Text = UI + "\r\n 局势:" + result;
        }
    }
}
