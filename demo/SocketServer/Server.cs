using ChessMiddle;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
namespace SocketServer
{


    public partial class Server : Form
    {
        #region TCPServer服务器

        private char[,] chess;
        private ITxServer server = null;
        private PictureBox[] blackItems;
        private PictureBox[] redItems;

        public Server()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            chessUIInit();
        }

        private void chessUIInit()
        {
            this.layerImageBox.Load(@"C:\Users\FEyn\Desktop\ChessStage\demo\SocketServer\res\dribble.jpg");
            blackItems = new PictureBox[12];
            redItems = new PictureBox[12];
            Random x = new Random();
            for (int i = 0; i < 12; i++)
            {
                blackItems[i] = new PictureBox();
                layerImageBox.Controls.Add(blackItems[i]);
                blackItems[i].Name = "black_" + i;
                blackItems[i].BackColor = Color.Transparent;
                blackItems[i].Load(@"C:\Users\FEyn\Desktop\ChessStage\demo\SocketServer\res\black.png");
                blackItems[i].Size = new System.Drawing.Size(53, 53);
                blackItems[i].Location = GetAbsoluteLocation(i / 4, ((i / 4) % 2 == 0 ? 1 : 0) + 2 * (i % 4));
                blackItems[i].BringToFront();

                redItems[i] = new PictureBox();
                layerImageBox.Controls.Add(redItems[i]);
                redItems[i].Name = "black_" + i;
                redItems[i].BackColor = Color.Transparent;
                redItems[i].Load(@"C:\Users\FEyn\Desktop\ChessStage\demo\SocketServer\res\red.png");
                redItems[i].Size = new System.Drawing.Size(53, 53);
                redItems[i].Location = GetAbsoluteLocation(i / 4 + 5, ((i / 4) % 2 == 0 ? 0 : 1) + 2 * (i % 4));
                redItems[i].BringToFront();
            }
        }

        Point GetAbsoluteLocation(int x, int y)
        {
            Point p = new Point();
            p.Y = 125 + 70 * x;
            p.X = 28 + 70 * y;
            return p;
        }

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

        /// <summary>
        /// 当对方已收到我方发送数据的时候
        /// </summary>
        /// <param name="state"></param>
        private void datesuccess(IPEndPoint ipendpoint)
        {
            //textbox_msg.text = "已向" + ipendpoint.tostring() + "发送成功";
        }

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
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }

        }

        private void playChess(List<string> actionMove, char[,] layout, char role, char result)
        {
            //画面的UI处理函数
            convertToUI(layout, result);

            foreach(string s in actionMove)
            {
                string[] posFormat = Regex.Split(s, "-");
                int startPointX = Convert.ToInt32(posFormat[0].Split(',')[0]);
                int startPointY = Convert.ToInt32(posFormat[0].Split(',')[1]);
                int endPointX = Convert.ToInt32(posFormat[1].Split(',')[0]);
                int endPointY = Convert.ToInt32(posFormat[1].Split(',')[1]);
                Point startPos = GetAbsoluteLocation(startPointX, startPointY);
                Point endPos = GetAbsoluteLocation(endPointX, endPointY);
                Point victimPos= new Point(0,0);
                //这里的70表示两个点之间的距离超过一个格子的大小
                if (Math.Abs(startPointX - endPointX) > 1)
                {
                    victimPos = GetAbsoluteLocation((startPointX + endPointX) / 2, (endPointY + startPointY) / 2);
                }
                if (role == 'a')
                {
                    foreach (PictureBox pb in blackItems)
                    {
                        if (pb.Location.Equals(startPos))
                        {
                            pb.Location = endPos;
                            if (endPointX == 7)
                            {
                                pb.ImageLocation = @"C:\Users\FEyn\Desktop\ChessStage\demo\SocketServer\res\blackKing.png";
                            }
                        }
                    }
                    foreach (PictureBox pb in redItems)
                    {
                        if (pb.Location.Equals(victimPos))
                        {
                            pb.Location = new Point(50, 20);
                        }
                    }
                }
                else
                {
                    foreach (PictureBox pb in redItems)
                    {
                        if (pb.Location.Equals(startPos))
                        {
                            pb.Location = endPos;
                            if (endPointX == 0)
                            {
                                pb.ImageLocation = @"C:\Users\FEyn\Desktop\ChessStage\demo\SocketServer\res\redKing.png";
                            }
                        }
                    }
                    foreach (PictureBox pb in blackItems)
                    {
                        if (pb.Location.Equals(victimPos))
                        {
                            pb.Location = new Point(50, 740);
                        }
                    }
                }
                
            }
            
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

        private void button2_Click(object sender, EventArgs e)
        {
            Thread move = new Thread(new ParameterizedThreadStart(moveFun))
            {
                IsBackground = true

            };
            move.Start(1);
        }

        private void moveFun(object j)
        {
            int l = int.Parse(j.ToString());
            while (1 == 1)
            {
                for (int i = 0; i < 12; i++)
                {
                    blackItems[i].Location = new Point(blackItems[i].Location.X + 1, blackItems[i].Location.Y + 1);
                }
                Thread.Sleep(5);
            }

        }

        //private delegate void myDelegate(int index, Point b);
        //private void SetLocation(int index, Point b)
        //{
        //    if (this.blackItems[index].InvokeRequired)
        //    {
        //        myDelegate md = new myDelegate(this.SetLocation);
        //        this.Invoke(md, new object[] { index ,b });
        //    }
        //    else
        //        this.blackItems[index].Location = b;
        //}
    }
}
