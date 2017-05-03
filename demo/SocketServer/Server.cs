using ChessMiddle;
using System;
using System.Collections.Generic;
using System.Net;
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
            chessInit();
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
        { MessageBox.Show(str); }
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
                server.PlayChess += new TxDelegate<List<string>, char, char>(playChess);
                server.StartEngine();
                this.button1.Enabled = false;
            }
            catch (Exception Ex) { MessageBox.Show(Ex.Message); }

        }

        private void playChess(List<string> actionMove,char role, char result)
        {
            foreach (string i in actionMove)
            {
                int x = int.Parse(i.Split(',')[0]);
                int y = int.Parse(i.Split(',')[1]);
                chess[x - 1, y - 1] = role;
            }
            convertToUI(chess, result);
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
            chessInit();

        }

        private void chessInit()
        {
            chess = new char[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    chess[i, j] = ' ';
                }
            }
            convertToUI(chess, '~');
        }

        private void convertToUI(char[,] chess, char result)
        {
            string UI = "";
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (j < 2)
                        UI += chess[i, j].ToString() + "|";
                    else
                        UI += chess[i, j];
                }
                UI += "\r\n";
            }
            this.textBox1.Text = UI + "\r\n 局势:"+result;
        }
    }
}
