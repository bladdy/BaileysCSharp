using BaileysCSharp.Core.Events;
using BaileysCSharp.Core.Extensions;
using BaileysCSharp.Core.Helper;
using BaileysCSharp.Core.Logging;
using BaileysCSharp.Core.Models;
using BaileysCSharp.Core.NoSQL;
using BaileysCSharp.Core.Sockets;
using BaileysCSharp.Core.Types;
using BaileysCSharp.Exceptions;
using Proto;
using QRCoder;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace WhatsSocketWin
    {
    public partial class QRfrm : Form
    {
        public WASocket socket { get; private set; }
        static List<WebMessageInfo> messagesList = new List<WebMessageInfo>();
        public string sessionName { get; set; } = "27665458845745067";
        private SocketConfig config = new SocketConfig()
        {
        SessionName = "27665458845745067",
        };
        public QRfrm()
        {
            InitializeComponent();
            this.FormClosing += QRfrm_FormClosing;
        }

        private void QRfrm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        private void QRfrm_Load(object sender, EventArgs e)
        {
            var credsFile = Path.Join(config.CacheRoot, $"creds.json");
            AuthenticationCreds? authentication = null;
            if (File.Exists(credsFile))
            {
                authentication = AuthenticationCreds.Deserialize(File.ReadAllText(credsFile));
            }

            authentication = authentication ?? AuthenticationUtils.InitAuthCreds();

            BaseKeyStore keys = new FileKeyStore(config.CacheRoot);

            config.Logger.Level = LogLevel.Raw;
            config.Auth = new AuthenticationState()
            {
                Creds = authentication,
                Keys = keys
            };
            socket = new WASocket(config);

            
            socket.EV.Auth.Update += (senderObj, creds) =>
            {
                lock (this)
                {
                    var credsFile = Path.Join(socket.SocketConfig.CacheRoot, $"creds.json");
                    File.WriteAllText(credsFile, AuthenticationCreds.Serialize(creds));
                }
            };
            socket.EV.Connection.Update += async (senderObj, status) =>
            {
                var connection = status;
                if (connection.QR != null)
                {
                    var qr = connection.QR;
                    if (qr != null)
                    {
                        QRCodeGenerator qRCodeGenerator = new();
                        QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(qr, QRCodeGenerator.ECCLevel.Q);
                        QRCode qRCode = new(qRCodeData);
                        Bitmap qrImgB = qRCode.GetGraphic(20);
                        qrImgB.SetResolution(300, 300);
                        qrImg.Image = qrImgB;
                    }
                }
                if (connection.Connection == WAConnectionState.Close)
                    {
                    if (connection.LastDisconnect.Error is Boom boom && boom.Data?.StatusCode != (int)DisconnectReason.LoggedOut)
                        {
                        try
                            {
                            Thread.Sleep(1000);
                            socket.MakeSocket();
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                            }
                        catch (Exception e)
                            {

                            }
                        }
                    else
                        {
                        Console.WriteLine("You are logged out");
                        }
                    }
                if (connection.Connection == WAConnectionState.Open)
                {
                    var result = await socket.QueryRecommendedNewsletters();
                    }
        };
            socket.EV.MessageHistory.Set += (senderObj, messages) =>
            {
                var message = messages as MessageHistoryModel[];
                if (message != null)
                {
                    messagesList.AddRange(message[0].Messages);
                    var jsons = messagesList.Select(x => x.ToJson()).ToArray();
                    var array = $"[\n{string.Join(",", jsons)}\n]";


                    }
            };
            
                    socket.MakeSocket();
                    
              
            }
    }
}
