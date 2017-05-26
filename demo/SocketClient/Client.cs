using ChessMiddle;
using ChessMiddle.PublicClass;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;
namespace SocketClient
{
    public partial class Client : Form 
    {
        #region TCP客户端区
        private ITxClient TxClient = null;

        private void connectBtn_Click(object sender, EventArgs e)
        {
            try
            {
                TxClient = TxStart.startClient(textBox2.Text, int.Parse(textBox3.Text));
                TxClient.dateSuccess += new TxDelegate<IPEndPoint>(sendSuccess);//当对方已经成功收到我数据的时候
                TxClient.EngineClose += new TxDelegate(engineClose);//当客户端引擎完全关闭释放资源的时候
                TxClient.EngineLost += new TxDelegate<string>(engineLost);//当客户端非正常原因断开的时候
                TxClient.ReconnectionStart += new TxDelegate(reconnectionStart);//当自动重连开始的时候
                TxClient.StartResult += new TxDelegate<bool, string>(startResult);//当登录完成的时候
                TxClient.StartEngine();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        /// <summary>
        /// 当数据发送成功的时候
        /// </summary>
        private void sendSuccess(IPEndPoint end)
        {
            textBox1.Text = "数据发送成功";
        }
        /// <summary>
        /// 当客户端引擎完全关闭的时候
        /// </summary>
        private void engineClose()
        {
            textBox1.Text = "客户端已经关闭";
            this.button1.Enabled = false;
            this.connectBtn.Enabled = true;
        }
        /// <summary>
        /// 当客户端突然断开的时候
        /// </summary>
        /// <param name="str">断开原因</param>
        private void engineLost(string str)
        {
            MessageBox.Show(str);
        }
        /// <summary>
        /// 当自动重连开始的时候
        /// </summary>
        private void reconnectionStart()
        {
            textBox1.Text = "10秒后自动重连";
        }
        /// <summary>
        /// 当登录有结果的时候
        /// </summary>
        /// <param name="b">是否成功</param>
        /// <param name="str">失败或成功原因</param>
        private void startResult(bool b, string str)
        {
            textBox1.Text = str;
            if (b)
            {
                this.button1.Enabled = true;
                this.connectBtn.Enabled = false;
            }
            else
            {
                this.button1.Enabled = false;
                this.connectBtn.Enabled = true;
            }
        }
        /// <summary>
        /// 发送关闭api
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            byte[] closeapi = API.getCloseAPI("");
            TxClient.sendMessage(closeapi);
        }
        #endregion

        public Client()
        {
            InitializeComponent();
            this.FormClosing += Client_FormClosing;
        }

        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        { 
            if( TxClient == null)
                return;
            byte[] closeapi = API.getCloseAPI("");
            TxClient.sendMessage(closeapi);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> action = new List<string>();
            //action.Add("m/5,6-6,8/a");
            //action.Add("aasdasdasd");
            string actionString = textBox4.Text;
            action.Add(actionString);

            TxClient.sendMessage(API.getActionAPI(action, "a"));
        }
    }

}
