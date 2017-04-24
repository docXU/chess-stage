using System;
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
    }
}
