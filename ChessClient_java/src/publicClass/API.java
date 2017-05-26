package publicClass;

import java.util.Collection;
import java.util.Hashtable;
import java.util.Iterator;
import java.util.LinkedList;
import java.util.List;
import java.util.ListIterator;

import com.google.gson.Gson;

/// <summary>
/// �����ֽ���ʽͨ�ô������
/// </summary>
public class API
{

    //S:����˷����ͻ�����
    //C:�ͻ��˷����������
    //S/C��˫���


    /// <summary>
    /// �رն���(C/S)
    /// </summary>
    /// <param name="why">�ر�ԭ��@param
    /// <returns></returns>
    public static byte[] getCloseAPI(String why)
    {
        Hashtable<String, Object> obj = new Hashtable<String, Object>() ;
        obj.put("type","close");
        obj.put("why", why);
        String apiJson = new Gson().toJson(obj);
        System.out.println("call api:  " + apiJson);
        return apiJson.getBytes();
    }
    
    /// <summary>
    /// Υ��������ӿ� ��S��
    /// </summary>
    /// <returns>why</returns>
    public static byte[] getIllegalAPI(String why)
    {
        Hashtable<String, Object> obj = new Hashtable<String, Object>();
        obj.put("type", "illegal");
        obj.put("why", why);
        String apiJson = new Gson().toJson(obj);
        System.out.println("call api:  " + apiJson);
        return apiJson.getBytes();
    }

    /// <summary>
    /// �������峬ʱ�ӿ� (S)
    /// </summary>
    /// <returns>why</returns>
    public static byte[] getTimeoutAPI()
    {
        Hashtable<String, Object> obj = new Hashtable<String, Object>();
        obj.put("type", "timeout");
        String apiJson = new Gson().toJson(obj);
        System.out.println("call api:  " + apiJson);
        return apiJson.getBytes();
    }

    /// <summary>
    /// ����ӿ� (C)
    /// </summary>
    /// <param name="changes">�ı�����(�ַ�����ʾ),"5,3-6,4"...@param
    /// <param name="role">���֤(���Ӻ�����һ��ʡ��֤)@param
    /// <returns></returns>
    public static byte[] getActionAPI(List<String> changes, String role)
    {
        Hashtable<String, Object> obj = new Hashtable<String, Object>();
        obj.put("type", "action");
        obj.put("changes", changes);
        obj.put("role", role);
        String apiJson = new Gson().toJson(obj);
        System.out.println("call api:  " + apiJson);
        return apiJson.getBytes();
    }
    
  /// <summary>
    /// ����ӿ� (C)
    /// </summary>
    /// <param name="changes">������@param
    /// <param name="role">���֤(���Ӻ�����һ��ʡ��֤)@param
    /// <returns></returns>
    public static byte[] getActionAPI(char[][] now, String role)
    {
        Hashtable<String, Object> obj = new Hashtable<String, Object>();
        obj.put("type", "now");
        obj.put("now", now);
        obj.put("role", role);
        String apiJson = new Gson().toJson(obj);
        System.out.println("call api:  " + apiJson);
        return apiJson.getBytes();
    }

    /// <summary>
    /// ���ɷ�����һ��ѡ�ֵ�����ӿ� (S)
    /// </summary>
    /// <param name="now">��ǰ���@param
    /// <param name="size">���̹��(���ڱ�ʾ���̴�С,��ԭ���)@param
    /// <param name="role">��һ��ѡ�ֵ����@param
    /// <param name="limitTimeSecond">����ȴ�ʱ��@param
    /// <returns></returns>
    public static byte[] getNextEpisodeAPI(char[][] now, String size, String role, double limitTimeSecond)
    {
        Hashtable<String, Object> obj = new Hashtable<String, Object>();
        obj.put("type", "do_algorithm");
        obj.put("now", now);
        obj.put("size", size);
        obj.put("role", role);
        obj.put("limitTimeSecond", limitTimeSecond);
        String apiJson = new Gson().toJson(obj);
        System.out.println("call call api:  " + apiJson);
        return apiJson.getBytes();
    }

    /// <summary>
    /// ���ɽ���ӿ� (S)
    /// </summary>
    /// <param name="situation">��Ҫ�Ľ���(win|fail|draw)@param
    /// <returns></returns>
    public static byte[] getResultAPI(String situation)
    {
        Hashtable<String, Object> obj = new Hashtable<String, Object>();
        obj.put("type", "result");
        obj.put("result", situation);
        String apiJson = new Gson().toJson(obj);
        System.out.println("call api:  " + apiJson);
        return apiJson.getBytes();
    }


    public static void main(String[] args)
    {
        char[][] a ={
            {'g','s','d','s','a' },
            {'g','s','d','s','a' },
            {'g','s','d','s','a' }
        };
        getNextEpisodeAPI(a, "8,8", "a", 5f);
        
        List<String> actionList = new LinkedList<String>();
        actionList.add("5,6-7,8");
        actionList.add("7,8-8,9");
        getActionAPI(actionList, "2");
    }
}

