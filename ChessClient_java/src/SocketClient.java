import java.io.IOException;
import java.net.InetSocketAddress;
import java.nio.ByteBuffer;
import java.nio.channels.SocketChannel;
import java.nio.charset.Charset;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import com.google.gson.Gson;

import publicClass.API;
import util.*;

public class SocketClient {
	public static void main(String[] args) {
		new MiniClient("localhost", 10020);
	}
}

class MiniClient {
	private SocketChannel sc;
	private ByteBuffer w_bBuf;
	private ByteBuffer r_bBuf = ByteBuffer.allocate(1024);
	private byte[] responseByteArray;

	// @SuppressWarnings("unchecked")
	public MiniClient(String host, int port) {
		try {
			InetSocketAddress remote = new InetSocketAddress(host, port);
			sc = SocketChannel.open();
			sc.connect(remote);
			if (sc.finishConnect()) {
				System.out.println("�Ѿ���������ɹ���������...");
			}
			while (true) {
				if (!sc.socket().isConnected()) {
					System.out.println("�Ѿ��������ʧȥ������...");
					return;
				}
				System.out.println("���շ���������Ϣ...");

				Thread.sleep(100);

				// ���ֽ����дӴ�ͨ���ж�������Ļ�����r_bBuf
				r_bBuf.clear();
				sc.read(r_bBuf);
				r_bBuf.flip();
				String msg = Charset.forName("UTF-8").decode(r_bBuf).toString();
				System.out.println(msg);
				Gson gson = new Gson();
				@SuppressWarnings("unchecked")
				HashMap<String, Object> requestMap = (HashMap<String, Object>) gson.fromJson(msg, HashMap.class);
				responseByteArray = new byte[1024];
				switch ((String) requestMap.get("type")) {
				case "timeout":
					IamOut();
					break;
				case "illegal":
					IllegalAction();
					break;
				case "do_algorithm": 
					double limitTimeS = (Double) requestMap.get("limitTimeSecond");
					System.out.println(requestMap.get("now").getClass());
					@SuppressWarnings("unchecked")
					final char[][] layout = Convertor.arrayListTo2D((ArrayList<String>) requestMap.get("now"));
					
					final byte[][] idea = new byte[1][1024];
					// ����㷨�Ƿ�ʱ������ж�����
					final String[] ThinkOK = { "0" };
					long ThinkStartTime = System.currentTimeMillis();
					Thread Think = new Thread(new Runnable() {
						public void run() {
							idea[0] = API.getActionAPI(begin_algorithm(layout), "nervermind");
							ThinkOK[0] = "1";
						}
					});
					Think.start();
					// ����һ���߳�,ִ��begin_algorithm(),ͬʱ�������������߳�(δ�����δ��ʱ)
					while (ThinkOK[0].equals("0")
							&& (System.currentTimeMillis() - ThinkStartTime) / 1000 < limitTimeS) {
						Thread.sleep(50);
					}
					if (ThinkOK[0].equals("1")) {
						// �������
						responseByteArray = idea[0];
					} else {
						// û�����,ֹͣ�߳�
						Think.interrupt();
					}
					break;
				case "result":
					MatchFinish(requestMap.get("result"));
					sc.close();
					break;
				default:
					break;
				}

				if ( requestMap.get("result") != null || responseByteArray == null || (responseByteArray != null && responseByteArray.length == 0)) {
					//��Ϸ�����˻��߻�û���κο���������������Ϣ��������ε����ݷ���
				} else {
					// ��������������д��ͨ��
					w_bBuf = ByteBuffer.wrap(responseByteArray);
					sc.write(w_bBuf);
					System.out.println("���ݷ��ͳɹ�...");
					w_bBuf.clear();
				}
			}

		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	private void MatchFinish(Object object) {
		// TODO Auto-generated method stub
		System.out.println("��������~~~");
		String symbol = (String) object;
		if (symbol.equals("win")) {
			System.out.println(" �� ~ Ӯ ~ �� oh yeah!!!");
		} else if (symbol.equals("draw")) {
			System.out.println("�Դ�ϲ,ƽ��!!!");
		} else {
			System.out.println(" �Բ���, ���� �� .");
		}
	}

	/**
	 * ������֪ͨ�ֵ�������ж�����ʱִ�еĺ���
	 * 
	 * @param object
	 *            ��ǰ����ֵĶ�ά�ַ��������ʾ
	 * @return �ж�������,eg."5,3-4,2"...
	 */
	private List<String> begin_algorithm(Object object) {
		// TODO ��ҵ��㷨����ͷ���!
		printChessLayout((char[][])object);
		List<String> actionList = new ArrayList<String>();
		actionList.add("3,5-6,3");
		return actionList;
	}

	/**
	 * ��������֪����Υ��ʱִ�еĺ���
	 */
	private void IllegalAction() {
		// TODO
		System.out.println("ellegal!!");
	}

	/**
	 * �����ж�����ʱ��ʱ,��������֪��ʱʱִ�еĺ���
	 */
	private void IamOut() {
		// TODO �����ж�����ʱ��ʱ,��������֪��ʱʱִ�еĺ���
		System.out.println("timeout!!");
	}

	private void printChessLayout(char[][] layout) {
		System.out.println();
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				System.out.print(layout[i][j]);
			}
			System.out.println();
		}
	}
}
