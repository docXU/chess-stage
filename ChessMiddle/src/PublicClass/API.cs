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
        /// <param name="why">关闭原因</param>
        /// <returns></returns>
        public static byte[] getCloseAPI(string why)
        {
            Dictionary<string, object> obj = new Dictionary<string, object>();
            obj.Add("type", "close");
            obj.Add("why", why);
            Console.WriteLine("api:  "+jsonAndDictionary.DictionaryToJson(obj));
            return Encoding.Default.GetBytes(jsonAndDictionary.DictionaryToJson(obj));
        }

        /// <summary>
        /// 违法的走棋接口
        /// </summary>
        /// <returns>why</returns>
        public static byte[] getIllegalAPI(string why)
        {
            Dictionary<string, object> obj = new Dictionary<string, object>();
            obj.Add("type", "illegal");
            obj.Add("why", why);
            Console.WriteLine("api:  " + jsonAndDictionary.DictionaryToJson(obj));
            return Encoding.Default.GetBytes(jsonAndDictionary.DictionaryToJson(obj));
        }

        /// <summary>
        /// 生成走棋超时接口
        /// </summary>
        /// <returns>why</returns>
        public static byte[] getTimeoutAPI()
        {
            Dictionary<string, object> obj = new Dictionary<string, object>();
            obj.Add("type", "timeout");
            Console.WriteLine("api:  " + jsonAndDictionary.DictionaryToJson(obj));
            return Encoding.Default.GetBytes(jsonAndDictionary.DictionaryToJson(obj));
        }

        /// <summary>
        /// 走棋接口
        /// </summary>
        /// <param name="changes">改变序列(字符串表示),"5,3x6,4"...</param>
        /// <param name="role">身份证(连接后会分配一个省份证)</param>
        /// <returns></returns>
        public static byte[] getActionAPI(List<string> changes, string role)
        {
            Dictionary<string, object> obj = new Dictionary<string, object>();
            obj.Add("type", "action");
            obj.Add("changes", changes);
            obj.Add("role", role);
            Console.WriteLine("api:  " + jsonAndDictionary.DictionaryToJson(obj));
            return Encoding.Default.GetBytes(jsonAndDictionary.DictionaryToJson(obj));
        }

        /// <summary>
        /// 生成发给下一个选手的走棋接口
        /// </summary>
        /// <param name="now">当前棋局</param>
        /// <param name="size">棋盘规格(用于表示棋盘大小,还原棋局)</param>
        /// <param name="role">下一个选手的身份</param>
        /// <returns></returns>
        public static byte[] getNextEpisodeAPI(char[] now, string size, string role)
        {
            Dictionary<string, object> obj = new Dictionary<string, object>();
            obj.Add("type", "do_algorithm");
            obj.Add("now", now);
            obj.Add("size", size);
            obj.Add("role", role);
            Console.WriteLine("api:  " + jsonAndDictionary.DictionaryToJson(obj));
            return Encoding.Default.GetBytes(jsonAndDictionary.DictionaryToJson(obj));
        }

        /// <summary>
        /// 生成结果接口
        /// </summary>
        /// <param name="situation">需要的结论(win|fail|draw)</param>
        /// <returns></returns>
        public static byte[] getResultAPI(string situation)
        {
            Dictionary<string, object> obj = new Dictionary<string, object>();
            obj.Add("type", "result");
            obj.Add("result", situation);
            Console.WriteLine("api:  " + jsonAndDictionary.DictionaryToJson(obj));
            return Encoding.Default.GetBytes(jsonAndDictionary.DictionaryToJson(obj));
        }
    }
}
