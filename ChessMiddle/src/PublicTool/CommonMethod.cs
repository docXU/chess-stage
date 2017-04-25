using ChessMiddle.Basics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ChessMiddle.PublicTool
{
    /// <summary>
    /// 普通方法工具箱
    /// </summary>
    internal class CommonMethod
    {
        /// <summary>
        /// 外部调用是否需要用Invoket
        /// </summary>
        /// <param name="func">事件参数</param>
        internal static void eventInvoket(Action func)
        {
            Form form = Application.OpenForms.Cast<Form>().FirstOrDefault();
            if (form != null && form.InvokeRequired)
            {
                form.Invoke(func);
            }
            else
            {
                func();
            }
        }
        /// <summary>
        /// 具有返回值的 非bool 外部调用是否需要用Invoket
        /// </summary>
        /// <param name="func">方法</param>
        /// <returns>返回客户操作之后的数据</returns>
        internal static object eventInvoket(Func<object> func)
        {
            object haveStr;
            Form form = Application.OpenForms.Cast<Form>().FirstOrDefault();
            if (form != null && form.InvokeRequired)
            {
                haveStr = form.Invoke(func);
            }
            else
            {
                haveStr = func();
            }
            return haveStr;
        }

        /// <summary>
        /// 提供一个通过值查到键的独享函数
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        internal static List<object> getKeyByValue(Dictionary<char, StateBase> dic, object target)
        {
            List<object> keys = new List<object>();
            for (int i = 0; i < dic.Values.Count; i++)
            {
                if(dic.ElementAt(i).Value.Equals(target))
                {
                    keys.Add(dic.ElementAt(i).Key);
                }
            }
            return keys;
        }
    }
}
