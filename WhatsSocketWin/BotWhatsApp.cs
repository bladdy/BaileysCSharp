
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
using System.Buffers;
//TODO: probar que funcione el QR y bloquear el boton de conectar para que no se vuelva a abrir el frm de Qr
//TODO: agregar el boton de desconectar y que cierre la sesion
//TODO: abregar un indicador de conexion
namespace WhatsSocketWin
{
    public partial class BotWhatsApp : Form
        {
        static List<WebMessageInfo> messages = new List<WebMessageInfo>();
        static WASocket socket;
        private SocketConfig config = new SocketConfig()
            {
            SessionName = "27665458845745067",
            };
        public BotWhatsApp()
            {
            InitializeComponent();
           
        }

        private void BotWhatsApp_Load(object sender, EventArgs e)
        {
            var credsFile = Path.Join(config.CacheRoot, $"creds.json");
            if (File.Exists(credsFile))
                {
                InitializeSocket();
                // Configuramos las columnas del DataGridView
                DgvMensage.Columns.Clear();  // Limpiar columnas existentes (si las hubiera)

                // Agregar las columnas necesarias
                DgvMensage.Columns.Add("From", "De");  // Columna para el remitente
                DgvMensage.Columns.Add("Message", "Mensaje");  // Columna para el texto del mensaje
                }
            }

        private void conetarToolStripMenuItem_Click(object sender, EventArgs e)
            {
            using (QRfrm qRfrm = new())
                {
                qRfrm.ShowDialog();
                if (qRfrm.DialogResult == DialogResult.OK)
                    {
                    socket = qRfrm.socket;
                    InitializeSocket();
                    }
                if (qRfrm.DialogResult == DialogResult.Cancel)
                    {
                    qRfrm.Dispose();
                    }
                }
            }

        private void InitializeSocket()
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
            socket.EV.Auth.Update += Auth_Update;
            socket.EV.Connection.Update += Connection_Update;
            socket.EV.Message.Upsert += Message_Upsert;
            socket.EV.MessageHistory.Set += GetMessageHistory_Set;
            //socket.EV.Pressence.Update += Pressence_Update;
            socket.MakeSocket();
            }

        private void Pressence_Update(object? sender, PresenceModel e)
            {
            throw new NotImplementedException();
            }

        private void GetMessageHistory_Set(object? sender, MessageHistoryModel[] e)
            {
            messages.AddRange(e[0].Messages);
            var jsons = messages.Select(x => x.ToJson()).ToArray();
            var array = $"[\n{string.Join(",", jsons)}\n]";
            }

        private static object GetMessageHistory_Set()
            {
            return new NotImplementedException();
            }

        private void Message_Upsert(object? sender, MessageEventModel e)
            {
            if (e.Type == MessageEventType.Append)
                {

                }
            if (e.Type == MessageEventType.Notify) 
                {
                foreach (var msg in e.Messages)
                    {
                    if (msg.Message == null)
                        continue;
                    string from = msg.Key.RemoteJid.ToString();
                    string incomingText = msg.Message.ExtendedTextMessage.Text;
                    // Usamos Invoke para asegurarnos de que la actualización del DataGridView se haga en el hilo de la UI
                    if (DgvMensage.InvokeRequired)
                        {
                        DgvMensage.Invoke(new Action(() =>
                        {
                            DgvMensage.Rows.Add(from, incomingText);  // Agregamos la fila al DataGridView
                        }));
                        }
                    else
                        {
                        DgvMensage.Rows.Add(from, incomingText);  // Si ya estamos en el hilo de la UI, agregamos directamente
                        }
                    }
                }
            }

        private async void Connection_Update(object? sender, ConnectionState e)
            {
            var connection = e as ConnectionState;
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
                    catch (Exception)
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
            
            
            }

        private void Auth_Update(object? sender, AuthenticationCreds e)
            {
            lock (this)
                {
                var credsFile = Path.Join(socket.SocketConfig.CacheRoot, $"creds.json");
                var json = AuthenticationCreds.Serialize(e);
                File.WriteAllText(credsFile, json);
                }
            }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }
    }
}
