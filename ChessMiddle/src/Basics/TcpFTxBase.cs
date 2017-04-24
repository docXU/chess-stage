using ChessMiddle.PublicClass;
using ChessMiddle.PublicTool;
using System.Collections.Generic;
using System.Text;
namespace ChessMiddle.Basics
{
    /// <summary>
    /// 主要是继承FTxBase;还有一些TCP协议需要用到的一些方法；
    /// </summary>
    public class TcpFTxBase : FTxBase
    {
        /// <summary>
        /// 当Tcp收到数据全部在这里处理;也是数据的第一次处理
        /// </summary>
        /// <param name="stateOne">TcpState</param>
        /// <param name="reciverByte">数据</param>
        internal Dictionary<string,string> TcpDateOne(TcpState stateOne, byte[] reciverByte)
        {
            string json =Encoding.Default.GetString(reciverByte);
            Dictionary<string, string> obj = new Dictionary<string, string>();
            jsonAndObject.getObjectByJson(json, obj);
            return obj;
        }

        virtual internal void dataClassify(TcpState stateOne, object dic)
        {

        }
    }
}
