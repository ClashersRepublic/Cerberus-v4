namespace ClashersRepublic.Magic.Client.Game.Network
{
    using System.IO;
    using ClashersRepublic.Magic.Logic;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Message;
    using ClashersRepublic.Magic.Logic.Message.Account;
    using ClashersRepublic.Magic.Titan;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Message.Security;
    using ClashersRepublic.Magic.Titan.Util;

    internal class ServerConnection
    {
        internal int State;

        private Messaging _messaging;
        private Account _account;
        
        private ServerType _serverType;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerConnection"/> class.
        /// </summary>
        internal ServerConnection()
        {
            this._messaging = new Messaging(this);
            this._account = new Account();
        }

        /// <summary>
        ///     Loads the account info from json.
        /// </summary>
        internal void LoadAccount()
        {
            if (File.Exists("Files/save.json"))
            {
                LogicJSONObject jsonRoot = (LogicJSONObject) LogicJSONParser.Parse(File.ReadAllText("Files/save.json"));

                this._account.AccountHighId = LogicJSONHelper.GetJSONNumber(jsonRoot, "High");
                this._account.AccountLowId = LogicJSONHelper.GetJSONNumber(jsonRoot, "Low");
                this._account.AccountPassToken = LogicJSONHelper.GetJSONString(jsonRoot, "Pass");
            }
        }

        /// <summary>
        ///     Saves the account info to json.
        /// </summary>
        internal void SaveAccount(ServerType serverType, string passToken, int accHighId, int accLowId)
        {
            LogicJSONObject jsonRoot = new LogicJSONObject();

            jsonRoot.Put("High", new LogicJSONNumber(accHighId));
            jsonRoot.Put("Low", new LogicJSONNumber(accLowId));
            jsonRoot.Put("Pass", new LogicJSONString(passToken));

            byte[] fileContent = LogicStringUtil.GetBytes(LogicJSONParser.CreateJSONString(jsonRoot));

            using (FileStream stream = new FileStream("Files/save.json", FileMode.OpenOrCreate))
            {
                stream.Write(fileContent, 0, fileContent.Length);
            }
        }

        /// <summary>
        ///     Resets the account info.
        /// </summary>
        internal void ResetAccount(ServerType serverType)
        {
            if (File.Exists("Files/save.json"))
            {
                File.Delete("Files/save.json");
            }

            this._account = new Account();
        }

        /// <summary>
        ///     Gets a value indicating whether the client should connect to chinese server.
        /// </summary>
        internal bool ShouldConnectToChineseServer()
        {
            return false;
        }

        /// <summary>
        ///     Initializes the encryption of client.
        /// </summary>
        internal void InitializeEncryption(string nonce)
        {
            RC4Encrypter sendEncrypter = new RC4Encrypter(LogicMagicMessageFactory.RC4_KEY, nonce);
            RC4Encrypter receiveEncrypter = new RC4Encrypter(LogicMagicMessageFactory.RC4_KEY, nonce);

            this._messaging.SetEncrypters(sendEncrypter, receiveEncrypter);
        }

        /// <summary>
        ///     Connects the game to server.
        /// </summary>
        internal void Connect()
        {
            if (this.ShouldConnectToChineseServer())
            {
                this.ConnectTo(ServerType.PROD_SERVER, "127.0.0.1", 9339);
            }
            else
            {
                this.ConnectTo(ServerType.PROD_SERVER, "127.0.0.1", 9339);
            }
        }

        /// <summary>
        ///     Connects the client to specified server.
        /// </summary>
        internal void ConnectTo(ServerType serverType, string serverUrl, int port)
        {
            this.State = 1;

            this.InitializeEncryption("nonce");
            this._messaging.Connect(serverUrl, port);
        }

        /// <summary>
        ///     Updates this instance.
        /// </summary>
        internal void Update(float time)
        {
            switch (this.State)
            {
                case 0:
                {
                    this.Connect();
                    break;
                }
            }

            if (!this._messaging.IsConnected())
            {
                if (this._messaging.HasConnectFailed())
                {
                    this._messaging.Disconnect();
                }
            }
            else
            {
                if (this.State == 1)
                {
                    LoginMessage loginMessage = new LoginMessage
                    {
                        AccountId = new LogicLong(this._account.AccountHighId, this._account.AccountLowId),
                        PassToken =  this._account.AccountPassToken,
                        ResourceSha = ResourceManager.FingerprintSha
                    };

                    if (LogicDataTables.GetClientGlobalsInstance().PepperEnabled())
                    {
                        ClientHelloMessage clientHelloMessage = new ClientHelloMessage
                        {
                            Protocol = 1,
                            DeviceType = 1,
                            AppStore = 1,
                            MajorVersion = LogicVersion.MajorVersion,
                            BuildVersion = LogicVersion.BuildVersion,
                            ContentHash = ResourceManager.FingerprintSha,
                            KeyVersion = PepperKey.VERSION 
                        };

                        this._messaging.SendPepperAuthentification(clientHelloMessage, loginMessage, PepperKey.SERVER_PUBLIC_KEY);
                    }
                    else
                    {
                        this._messaging.Send(loginMessage);
                    }
                }
            }
        }

        internal class Account
        {
            internal int AccountHighId;
            internal int AccountLowId;
            internal string AccountPassToken;
        }

        internal enum ServerType
        {
            STAGE_SERVER,
            PROD_SERVER,
            INTEGRATION_SERVER,
            CONTENT_STAGE_SERVER,
            OTHER_SERVER
        }
    }
}