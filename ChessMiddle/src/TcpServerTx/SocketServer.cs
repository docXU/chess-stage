﻿using ChessMiddle.Basics;
using ChessMiddle.PublicClass;
using ChessMiddle.PublicTool;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
namespace ChessMiddle
{
    /// <summary>
    /// 面向服务器的主线程类!
    /// </summary>
    public class SocketServer : TcpFTxBase, ITxServer
    {
        #region 基本属性区块
        private List<TcpState> state = null;//所有客户端
        private Socket listener = null;
        private int _clientMax = 20;//允许最多客户端数
        /// <summary>
        /// 当有客户连接成功的时候,触发此事件
        /// </summary>
        public event TxDelegate<IPEndPoint> Connect;
        /// <summary>
        /// 当有客户突然断开的时候,触发此事件,文本参数是代表断开的原因
        /// </summary>
        public event TxDelegate<IPEndPoint, string> Disconnection;
        /// <summary>
        /// 当有客户发送棋步的时候
        /// </summary>
        public event TxDelegate<List<string>> PlayChess;
        /// <summary>
        /// 当前客户端数量
        /// </summary>
        public int ClientNumber
        {
            get { return state.Count; }
        }
        /// <summary>
        /// 允许最多客户端数
        /// </summary>
        public int ClientMax
        {
            get { return _clientMax; }
            set
            {
                if (value > 100)
                    _clientMax = 100;
                else
                    _clientMax = value;
            }
        }
        /// <summary>
        /// 得到所有的客户端
        /// </summary>
        List<IPEndPoint> ITxServer.ClientAll
        {
            get
            {
                if (state == null || state.Count == 0)
                    return null;
                List<IPEndPoint> IpEndPoint = new List<IPEndPoint>();
                foreach (TcpState stateOne in state)
                {
                    IpEndPoint.Add(stateOne.IpEndPoint);
                }
                return IpEndPoint;
            }
        }
        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        /// <param name="port">端口号</param>
        internal SocketServer(int port)
        {
            Port = port;
            if (state == null)
                state = new List<TcpState>();
        }
        #endregion

        #region 启动以及接收客户端区块
        /// <summary>
        /// 启动服务器,如果没出现异常,说明启动成功
        /// </summary>
        override public void StartEngine()
        {
            if (EngineStart)
                return;
            try
            {
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(IpEndPoint);
                listener.Listen(20);
                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                _engineStart = true;//启动成功
            }
            catch (Exception Ex)
            {
                CloseEngine();
                throw new Exception(Ex.Message);
            }
        }
        /// <summary>
        /// 当连接一个客户端之后的回调函数
        /// </summary>
        /// <param name="ar">TcpClient</param>
        private void AcceptCallback(IAsyncResult ar)
        {
            TcpState stateOne = null;
            try
            {
                Socket Listener = (Socket)ar.AsyncState;
                Socket handler = Listener.EndAccept(ar);
                Console.WriteLine("get one...");
                stateOne = new TcpState(handler, BufferSize);
                loginSuccess(stateOne);
            }
            catch
            {
            }
            try
            {
                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
            }
            catch (Exception Ex)
            {
                OnEngineLost(Ex.Message);//当服务器突然断开触发此事件
                CloseEngine();
            }
        }
        #endregion

        #region 接收到数据区块

        /// <summary>
        /// 客户端完全登录成功之后要处理的事情
        /// </summary>
        /// <param name="stateOne">TcpState</param>
        private void loginSuccess(TcpState stateOne)
        {
            stateOne.ConnectOk = true;
            state.Add(stateOne);
            CommonMethod.eventInvoket(() => { Connect(stateOne.IpEndPoint); });
            stateOne.WorkSocket.BeginReceive(stateOne.Buffer, 0, stateOne.Buffer.Length, 0, new AsyncCallback(ReadCallback), stateOne);
        }

        /// <summary>
        /// 当接收到数据之后的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void ReadCallback(IAsyncResult ar)
        {
            TcpState stateOne = (TcpState)ar.AsyncState;
            Socket handler = stateOne.WorkSocket;
            try
            {
                int bytesRead = handler.EndReceive(ar);
                if (bytesRead > 0)
                {
                    byte[] haveDate = ReceiveDateOne.DateOneManage(stateOne, bytesRead);//接收完成之后对数组进行重置
                    //是否超时
                    //数据处理
                    dataClassify(stateOne, TcpDateOne(stateOne, haveDate));
                    //令牌刷新
                    handler.BeginReceive(stateOne.Buffer, 0, stateOne.Buffer.Length, 0, new AsyncCallback(ReadCallback), stateOne);
                    
                }
                else { handler.BeginReceive(stateOne.Buffer, 0, stateOne.Buffer.Length, 0, new AsyncCallback(ReadCallback), stateOne); }
            }
            catch (Exception Ex)
            {
                int i = Ex.Message.IndexOf("远程主机强迫关闭了一个现有的连接");
                if (stateOne != null && i != -1)
                { socketRemove(stateOne, Ex.Message); }
            }
        }

        internal override void dataClassify(TcpState stateOne, object dic)
        {
            Dictionary<string, string> data = (Dictionary<string, string>)dic;
            string type = data["type"];
            switch (type)
            {
                case "closed":
                    {
                        socketRemove(stateOne, "客户端自己退出！");
                        break;
                    }
                case "changes":
                    {
                        //判断令牌对不对
                        //
                        OnChessPlay(data);
                        break;
                    }
                default:
                    break;

            }
        }

        /// <summary>
        /// 下棋
        /// </summary>
        /// <param name="data">关键数据</param>
        public void OnChessPlay( Dictionary<string, string> data)
        {
            string[] changes = data["changes"].Split('x');
            List<string> changeList = new List<string>();
            foreach (string s in changes)
                changeList.Add(s);
            CommonMethod.eventInvoket(() => { PlayChess(changeList); });
        }

        #endregion

        #region 向客户端发送数据的区块
        /// <summary>
        /// 向客户端发送数据,最基础的发送
        /// </summary>
        /// <param name="stateBase">TcpState</param>
        /// <param name="data">发送的数据</param>
        override internal void Send(StateBase stateBase, byte[] data)
        {
            if (stateBase == null)
                return;
            try
            {
                stateBase.WorkSocket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), stateBase);
            }
            catch (Exception Ex)
            {
                int i = Ex.Message.IndexOf("远程主机强迫关闭了一个现有的连接");
                if (i != -1)
                {
                    TcpState stateOne = IPEndPointToState(stateBase.IpEndPoint);
                    socketRemove(stateOne, Ex.Message);
                }
            }
        }
        /// <summary>
        /// 发送完数据之后的回调函数
        /// </summary>
        /// <param name="ar">Clicent</param>
        private void SendCallback(IAsyncResult ar)
        {
            StateBase stateBase = (StateBase)ar.AsyncState;
            if (stateBase == null)
                return;
            Socket handler = stateBase.WorkSocket;
            try
            {
                int bytesSent = handler.EndSend(ar);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 服务器向客户端发送字节数据
        /// </summary>
        /// <param name="ipEndPoint">IPEndPoint</param>
        /// <param name="data">字节的数据</param>
        public void sendMessage(IPEndPoint ipEndPoint, byte[] data)
        {
            TcpState stateOne = IPEndPointToState(ipEndPoint);
            Send(stateOne, data);
        }
        #endregion

        #region 客户需要操作的一些方法
        /// <summary>
        /// 关闭服务器,释放所有资源
        /// </summary>
        override public void CloseEngine()
        {
            try
            {
                clientAllClose();
                state = null;
                if (listener != null)
                    listener.Close();
                listener = null;
                OnEngineClose();
            }
            catch { }
        }

        /// <summary>
        /// 关闭所有客户端连接
        /// </summary>
        public void clientAllClose()
        {
            foreach (TcpState stateo in state)
            {
                socketRemove(stateo, "服务器关闭所有的客户端");
            }
        }

        /// <summary>
        /// 发送代码的形式服务器强制关闭一个客户端
        /// </summary>
        /// <param name="stateOne">TcpState</param>
        private void clientClose(TcpState stateOne)
        {
            if (stateOne == null || ClientNumber == 0)
                return;
            state.Remove(stateOne);
            Send(stateOne, API.getCloseAPI());//发送一个强制关闭的代码过去
        }
        /// <summary>
        /// 服务器强制关闭一个客户端
        /// </summary>
        /// <param name="ipEndPoint">IPEndPoint</param>
        public void clientClose(IPEndPoint ipEndPoint)
        {
            TcpState stateOne = IPEndPointToState(ipEndPoint);
            clientClose(stateOne);
        }

        /// <summary>
        /// 关闭相连的scoket以及关联的TcpState,释放所有的资源
        /// </summary>
        /// <param name="stateOne">TcpState</param>
        /// <param name="str">原因</param>
        private void socketRemove(TcpState stateOne, string str)
        {
            if (stateOne == null)
                return;
            stateOne.WorkSocket.Close();
            if (state.Remove(stateOne))//当没有登录的时候断掉，不触发下面的事件
            {
                CommonMethod.eventInvoket(() => { Disconnection(stateOne.IpEndPoint, str); }); //当客户端断掉的时候触发此事件
            }
            stateOne = null;
        }

        /// <summary>
        /// 检查某个客户端是否在线
        /// </summary>
        /// <param name="ipEndPoint">IPEndPoint</param>
        /// <returns>bool</returns>
        public bool clientCheck(IPEndPoint ipEndPoint)
        {
            TcpState stateOne = IPEndPointToState(ipEndPoint);
            if (stateOne == null)
                return false;
            return true;
        }

        /// <summary>
        /// 用ip地址从state里找到TcpState
        /// </summary>
        /// <param name="ipEndPoint">IPEndPoint</param>
        /// <returns>TcpState</returns>
        private TcpState IPEndPointToState(IPEndPoint ipEndPoint)
        {
            try
            {
                return state.Find(delegate (TcpState state1) { return state1.IpEndPoint == ipEndPoint; });
            }
            catch { return null; }
        }

        #endregion
    }
}
