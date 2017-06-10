import java.io.IOException;
import java.net.InetSocketAddress;
import java.nio.ByteBuffer;
import java.nio.channels.SocketChannel;
import java.nio.charset.Charset;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import com.google.gson.Gson;

import AI.GameTree;
import publicClass.API;
import publicClass.XYTQ;
import util.*;

class MiniClient {
	private SocketChannel sc;
	private ByteBuffer w_bBuf;
	private ByteBuffer r_bBuf = ByteBuffer.allocate(1024);
	private byte[] responseByteArray;
	private XYTQ chess;
	private char[][] lastLayout;

	// @SuppressWarnings("unchecked")
	public MiniClient(String host, int port) {
		chess= new XYTQ();
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
				// ���ֽ����дӴ�ͨ���ж�������Ļ�����r_bBuf
				r_bBuf.clear();
				sc.read(r_bBuf);
				r_bBuf.flip();
				String msg = Charset.forName("UTF-8").decode(r_bBuf).toString();
				System.out.println(msg);
				if(msg=="")
				{
					continue;
				}
				Gson gson = new Gson();
				@SuppressWarnings("unchecked")
				HashMap<String, Object> requestMap = (HashMap<String, Object>) gson.fromJson(msg, HashMap.class);
				responseByteArray = new byte[1024];
				switch ((String) requestMap.get("type")) {
					case "timeout":
						IamOut();
						responseByteArray = null;
						break;
					case "illegal":
						IllegalAction();
						printChessLayout(lastLayout);
						responseByteArray = null;
						break;
					case "do_algorithm": 
						double limitTimeS = (Double) requestMap.get("limitTimeSecond");
						@SuppressWarnings("unchecked")
						final char[][] layout = Convertor.arrayListTo2D((ArrayList<String>) requestMap.get("now"));
						final byte[][] idea = new byte[1][1024];
						final char role = ((String)requestMap.get("role")).toCharArray()[0];
						chess.setLayout(layout);
						// ����㷨�Ƿ�ʱ������ж�����
						final String[] ThinkOK = { "0" };
						long ThinkStartTime = System.currentTimeMillis();
						Thread Think = new Thread(new Runnable() {
							public void run() {
								List<String> actionMove = begin_algorithm(layout, role);
								if(actionMove!=null)
								{
									idea[0] = API.getActionAPI(actionMove, "nervermind");
									ThinkOK[0] = "1";
								}
								else{
									//��һ��Υ���壬���߾��������ţ��ᳬʱ
								}
							}
						});
						lastLayout=layout;
						Think.start();
						// ����һ���߳�,ִ��begin_algorithm(),ͬʱ�������������߳�(δ�����δ��ʱ)
						while (ThinkOK[0].equals("0")
								&& (System.currentTimeMillis() - ThinkStartTime) / 1000 < limitTimeS) {
							Thread.sleep(10);
						}
						if (ThinkOK[0].equals("1")) {
							// �������
							responseByteArray = idea[0];
						} else {
							// û�����,ֹͣ�߳�
							responseByteArray = null;
							Think.interrupt();
							System.out.println("û���ļ��������ʱ��");
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
				System.out.println("##################################");
			}

		} catch (IOException e) {
			// TODO Auto-generated catch block
			//e.printStackTrace();
			if(e.getMessage().indexOf("Connection refused: connect")!=-1)
			{
				System.out.println("�����桿���������ڹر�״̬");
			}
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
	private List<String> begin_algorithm(char[][] layout, char role) {
		// TODO ��ҵ��㷨����ͷ���!
		//printChessLayout(layout);
		GameTree test = new GameTree(layout, role);
		while(test.getThinking())
		{
			try {
				Thread.sleep(5);
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
		List<String> actionList = test.bestMove();
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
