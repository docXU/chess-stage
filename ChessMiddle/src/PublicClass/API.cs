using ChessMiddle.PublicTool;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessMiddle.PublicClass
{
    /// <summary>
    /// 通用传输对象
    /// </summary>
    public class API
    {
        /// <summary>
        /// 关闭对象
        /// </summary>
        /// <returns></returns>
        public static byte[] getCloseAPI()
        {
            Dictionary<string, string> obj = new Dictionary<string, string>();
            obj.Add("type", "close");
            Console.WriteLine(jsonAndObject.getJsonByObject(obj));
            return Encoding.Default.GetBytes( jsonAndObject.getJsonByObject(obj));
        }
    }
}
