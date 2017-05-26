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
				System.out.println("已经与服务器成功建立连接...");
			}
			while (true) {
				if (!sc.socket().isConnected()) {
					System.out.println("已经与服务器失去了连接...");
					return;
				}
				System.out.println("接收服务器端消息...");

				Thread.sleep(100);

				// 将字节序列从此通道中读入给定的缓冲区r_bBuf
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
					// 监测算法是否及时计算出行动序列
					final String[] ThinkOK = { "0" };
					long ThinkStartTime = System.currentTimeMillis();
					Thread Think = new Thread(new Runnable() {
						public void run() {
							idea[0] = API.getActionAPI(begin_algorithm(layout), "nervermind");
							ThinkOK[0] = "1";
						}
					});
					Think.start();
					// 启动一个线程,执行begin_algorithm(),同时有条件阻塞主线程(未完成且未超时)
					while (ThinkOK[0].equals("0")
							&& (System.currentTimeMillis() - ThinkStartTime) / 1000 < limitTimeS) {
						Thread.sleep(50);
					}
					if (ThinkOK[0].equals("1")) {
						// 算出来了
						responseByteArray = idea[0];
					} else {
						// 没算出来,停止线程
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
					//游戏结束了或者还没有任何可向服务器传达的消息则跳过这次的数据发送
				} else {
					// 将缓冲区中数据写入通道
					w_bBuf = ByteBuffer.wrap(responseByteArray);
					sc.write(w_bBuf);
					System.out.println("数据发送成功...");
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
		System.out.println("比赛结束~~~");
		String symbol = (String) object;
		if (symbol.equals("win")) {
			System.out.println(" 我 ~ 赢 ~ 了 oh yeah!!!");
		} else if (symbol.equals("draw")) {
			System.out.println("皆大欢喜,平局!!!");
		} else {
			System.out.println(" 对不起, 我输 了 .");
		}
	}

	/**
	 * 服务器通知轮到你计算行动序列时执行的函数
	 * 
	 * @param object
	 *            当前的棋局的二维字符型数组表示
	 * @return 行动的序列,eg."5,3-4,2"...
	 */
	private List<String> begin_algorithm(Object object) {
		// TODO 玩家的算法代码就放这!
		printChessLayout((char[][])object);
		List<String> actionList = new ArrayList<String>();
		actionList.add("3,5-6,3");
		return actionList;
	}

	/**
	 * 服务器告知序列违法时执行的函数
	 */
	private void IllegalAction() {
		// TODO
		System.out.println("ellegal!!");
	}

	/**
	 * 计算行动序列时超时,服务器告知超时时执行的函数
	 */
	private void IamOut() {
		// TODO 计算行动序列时超时,服务器告知超时时执行的函数
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
