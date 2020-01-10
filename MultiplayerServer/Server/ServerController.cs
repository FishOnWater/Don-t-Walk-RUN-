using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class ServerController
    {
        private List<Player> _playerList;
        private List<Player> _deadPlayerList;
        private bool[] picked = new bool[4];

        public ServerController()
        {
            _playerList = new List<Player>();

            _deadPlayerList = new List<Player>();

            for (int i = 0; i < 4; i++)
            {
                picked[i] = false;
            }
        }

        public void StartServer()
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7777);
            tcpListener.Start();
            Console.WriteLine("Server started");
            Random rnd = new Random();

            while (true)
            {
                if (tcpListener.Pending())
                {
                    Console.WriteLine("New pending connection");
                    tcpListener.BeginAcceptTcpClient(AcceptTcpClient, tcpListener);
                }

                foreach (Player player in _playerList)
                {
                    switch(player.GameState)
                    {
                        case GameState.Connecting:
                            if (player.DataAvailable())
                            {
                                Console.WriteLine("New player registering");
                                string playerJson = player.BinaryReader.ReadString();
                                Player playerMsg = JsonConvert.DeserializeObject<Player>(playerJson);
                                player.Name = playerMsg.Name;

                                foreach (Player notifyPlayer in _playerList)
                                {
                                    Message msg = new Message();
                                    msg.MessageType = MessageType.NewPlayer;
                                    msg.Description = (notifyPlayer == player) ? 
                                        "Successfully joined" : 
                                        "Player " + player.Name + " has joined";
                                    PlayerInfo playerInfo = new PlayerInfo();
                                    playerInfo.Id = player.Id;
                                    playerInfo.Name = player.Name;
                                    playerInfo.X = 0;
                                    playerInfo.Y = 0;
                                    playerInfo.Z = 0;
                                    playerInfo.Rotation = 0;
                                    playerInfo.HP = 3;
                                    msg.PlayerInfo = playerInfo;

                                    int slot = Array.IndexOf(picked, false);
                                    playerInfo.SpawnLocation = slot;
                                    picked[slot] = true;
                                    Console.WriteLine(slot);

                                    string msgJson = JsonConvert.SerializeObject(msg);
                                    notifyPlayer.BinaryWriter.Write(msgJson);
                                    notifyPlayer.MessageList.Add(msg);
                                    Console.WriteLine(msgJson);
                                }
                                player.GameState = GameState.Sync;
                            }
                            break;
                        case GameState.Sync:
                            Console.WriteLine("New player sync");
                            // processar todos os NewPlayer
                            SyncNewPlayers(player);
                            Console.WriteLine("1231231212");
                            // processar todos os PlayerMovement
                            SyncPlayerMovement(player);

                            Message messagePlayer = new Message();
                            messagePlayer.MessageType = MessageType.FinishedSync;
                            string msgPlayerJson = JsonConvert.SerializeObject(messagePlayer);
                            player.BinaryWriter.Write(msgPlayerJson);
                            player.GameState = GameState.GameStarted;
                            Console.WriteLine(msgPlayerJson);
                            Console.WriteLine("dg4gaegae");
                            break;
                        case GameState.GameStarted:
                            if (player.DataAvailable())
                            {
                                Console.WriteLine("New player position");
                                string msgJson = player.BinaryReader.ReadString();
                                Console.WriteLine(msgJson);
                                Message message = JsonConvert.DeserializeObject<Message>(msgJson);
                                if (message.MessageType == MessageType.PlayerMovement)
                                {
                                    //if(message.PlayerInfo.HP <= 0)
                                    //{
                                    //   var remove = _playerList.First(item => item.Name == player.Name);
                                    //    _deadPlayerList.Add(remove);
                                    //    _playerList.Remove(remove);
                                    //}
                                    foreach (Player p in _playerList)
                                    {
                                        if (p.GameState == GameState.GameStarted)
                                        {
                                            p.BinaryWriter.Write(msgJson);
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        private void SyncPlayerMovement(Player player)
        {
            foreach (Player p in _playerList)
            {
                if (p.GameState == GameState.GameStarted)
                {
                    Console.WriteLine("23f34f2gf3qwg");
                    var last = p.MessageList.LastOrDefault(
                            m => m.MessageType == MessageType.PlayerMovement);
                    if (last != null)
                    {
                        Console.WriteLine("New c");
                        Message msg = new Message();
                        msg.PlayerInfo = last.PlayerInfo;
                        Console.WriteLine(msg.PlayerInfo.HitName);
                        string jsonMsg = JsonConvert.SerializeObject(msg);
                        player.BinaryWriter.Write(jsonMsg);
                    }
                }
            }
        }

        private void SyncNewPlayers(Player player)
        {
            foreach (Player p in _playerList)
            {
                if (p.GameState == GameState.GameStarted)
                {
                    Console.WriteLine("New adsfadsfadsfdsac");
                    Message msg = new Message();
                    msg.MessageType = MessageType.NewPlayer;
                    PlayerInfo info = p.MessageList.FirstOrDefault(m =>
                                                m.MessageType == MessageType.NewPlayer).PlayerInfo;
                    msg.PlayerInfo = info;

                    string jsonMsg = JsonConvert.SerializeObject(msg);
                    player.BinaryWriter.Write(jsonMsg);
                }
            }
        }

        private void AcceptTcpClient(IAsyncResult ar)
        {
            TcpListener tcpListener = (TcpListener)ar.AsyncState;
            TcpClient tcpClient = tcpListener.EndAcceptTcpClient(ar);
            if (tcpClient.Connected)
            {
                Console.WriteLine("Accepted new connection");

                Player player = new Player();
                Message message = new Message();
                message.Description = "Hello new player";
                message.MessageType = MessageType.Information;

                player.MessageList = new List<Message>();
                player.MessageList.Add(message);
                player.TcpClient = tcpClient;
                player.BinaryReader = new System.IO.BinaryReader(tcpClient.GetStream());
                player.BinaryWriter = new System.IO.BinaryWriter(tcpClient.GetStream());
                player.Id = Guid.NewGuid();
                player.GameState = GameState.Connecting;

                _playerList.Add(player);

                string playerJson = JsonConvert.SerializeObject(player);
                Console.WriteLine(playerJson);
                player.BinaryWriter.Write(playerJson);

            }
            else
            {
                Console.WriteLine("Connection refused");
            }
        }
    }
}
