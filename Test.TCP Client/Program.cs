using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.TCP_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            //サーバーのホスト名とポート番号
            string host = "localhost";
            int port = 2001;

            //TcpClientを作成し、サーバーと接続する
            System.Net.Sockets.TcpClient tcp =
                new System.Net.Sockets.TcpClient(host, port);
            Console.WriteLine("サーバー({0}:{1})と接続しました({2}:{3})。",
                ((System.Net.IPEndPoint)tcp.Client.RemoteEndPoint).Address,
                ((System.Net.IPEndPoint)tcp.Client.RemoteEndPoint).Port,
                ((System.Net.IPEndPoint)tcp.Client.LocalEndPoint).Address,
                ((System.Net.IPEndPoint)tcp.Client.LocalEndPoint).Port);

            //NetworkStreamを取得する
            System.Net.Sockets.NetworkStream ns = tcp.GetStream();

            //サーバーにデータを送信する
            //送信するデータを入力
            Console.WriteLine("文字列を入力し、Enterキーを押してください。");
            string sendMsg = Console.ReadLine();
            //何も入力されなかった時は切断する
            if (sendMsg == null || sendMsg.Length == 0)
            {
                tcp.Close();
                ns.Close();
                return;
            }
            //文字列をByte型配列に変換
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            byte[] sendBytes = enc.GetBytes(sendMsg);
            //データを送信する
            ns.Write(sendBytes, 0, sendBytes.Length);
            Console.WriteLine(sendMsg);

            //サーバーから送られたデータを受信する
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            byte[] resBytes = new byte[256];
            do
            {
                //データの一部を受信する
                int resSize = ns.Read(resBytes, 0, resBytes.Length);
                //Readが0を返した時はサーバーが切断したと判断
                if (resSize == 0)
                {
                    Console.WriteLine("サーバーが切断しました。");
                    break;
                }
                //受信したデータを蓄積する
                ms.Write(resBytes, 0, resSize);
            } while (ns.DataAvailable);
            //受信したデータを文字列に変換
            string resMsg = enc.GetString(ms.ToArray());
            ms.Close();
            Console.WriteLine(resMsg);

            //閉じる
            ns.Close();
            tcp.Close();
            Console.WriteLine("切断しました。");

            Console.ReadLine();
        }
    }
}
