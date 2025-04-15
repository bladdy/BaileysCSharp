using BaileysCSharp.Core.Events;
using BaileysCSharp.Core.Extensions;
using BaileysCSharp.Core.Helper;
using BaileysCSharp.Core.Logging;
using BaileysCSharp.Core.Models;
using BaileysCSharp.Core.Models.Sending.NonMedia;
using BaileysCSharp.Core.NoSQL;
using BaileysCSharp.Core.Sockets;
using BaileysCSharp.Core.Types;
using BaileysCSharp.Exceptions;
using Proto;
using QRCoder;
using System.Buffers;
using System.Diagnostics;
using System.Text.Json;

namespace WhatsSocketConsole
    {
    internal class Program
        {

        static List<WebMessageInfo> messages = new List<WebMessageInfo>();
        static WASocket? socket;
        public static object locker = new object();

        // Puedes agregar esto como static en Program.cs o como servicio
        static Dictionary<int, string> payMethod = new Dictionary<int, string>()
        {
            {1, "Efectivo" },
            {2, "Tarjeta" },
            {3, "Transferencia" }
        };
        static Dictionary<int, MenuItem> menuItems = new Dictionary<int, MenuItem>()
        {
            {1, new MenuItem { Name = "Tacos al Pastor", Price = 25.00m }},
            {2, new MenuItem { Name = "Hamburguesa con Queso", Price = 35.50m }},
            {3, new MenuItem { Name = "Pizza Margarita", Price = 45.00m }},
            {4, new MenuItem { Name = "Agua", Price = 10.00m }},
            {5, new MenuItem { Name = "Refresco", Price = 12.00m }}
        };

        // Guardar estados de conversación por número de usuario
        static Dictionary<string, BotWhatsapp> botUsers = new Dictionary<string, BotWhatsapp>();



        static void Main(string[] args)
            {
            var config = new SocketConfig()
                {
                SessionName = "27665458845745067",
                };

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
            socket.EV.Message.Upsert += Message_Upsert1;
            //socket.EV.Message.Upsert += Message_Upsert;
            socket.EV.MessageHistory.Set += MessageHistory_Set;
            socket.EV.Pressence.Update += Pressence_Update;

            socket.MakeSocket();

            Console.ReadLine();
            }


        private static void Message_Upsert1(object? sender, MessageEventModel e)
            {


            //offline messages synced
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

                    BotWhatsapp botWhatsapp;
                    if (botUsers.ContainsKey(from))
                        {
                        botWhatsapp = botUsers[from];
                        botWhatsapp.Menssage = incomingText; // Actualizamos solo el texto
                        }
                    else
                        {
                        botWhatsapp = new BotWhatsapp
                            {
                            Menssage = incomingText,
                            From = from,
                            PushName = msg.PushName.ToString(),
                            StatusOrder = Status.NoStatus,
                            Items = new List<Item>() // importante inicializar si es nuevo
                            };
                        }

                    //Validar si es un mensaje
                    bool isMsg = IsMenssage(msg);
                    if (!isMsg) return;

                    botWhatsapp = AnalyzeMessage(botWhatsapp);

                    switch (botWhatsapp.StatusOrder)
                        {
                        case Status.WithoutOrdering:
                            WelcomeAsync(botWhatsapp);
                            break;
                        case Status.Ordering:
                            MakeOrder(botWhatsapp);
                            break;
                        case Status.Selecting:
                            MakeOrderCant(botWhatsapp);
                            break;
                        case Status.RequestingName:
                            AskCustomerName(botWhatsapp);
                            botWhatsapp.StatusOrder = Status.ReponsingName;
                            break;

                        case Status.RequestingPayment:
                            AskPaymentMethod(botWhatsapp);
                            botWhatsapp.StatusOrder = Status.ReponsingPayment;
                            break;

                        case Status.Closing:
                            MakeOrderConfirmation(botWhatsapp);
                            break;
                        case Status.Closed:
                            FinalizeOrder(botWhatsapp);
                            botWhatsapp.StatusOrder = Status.WithoutOrdering;
                            botWhatsapp.Items.Clear();
                            botWhatsapp.SelectedItem = null;
                            botWhatsapp.Quantity = null;
                            botWhatsapp.HasOrder = false;
                            break;
                        default:
                            WelcomeAsync(botWhatsapp);
                            break;
                        }
                    botUsers[from] = botWhatsapp;
                    messages.Add(msg);
                    }
                }
            }

        private static BotWhatsapp AnalyzeMessage(BotWhatsapp botWhatsapp)
            {
            string msg = botWhatsapp.Menssage.ToLower().Trim();

            List<string> menuKeywords = new List<string> { "menu", "ordenar", "ver", "platos", "menú", "terminar", "finalizar" };

            switch (botWhatsapp.StatusOrder)
                {
                case Status.NoStatus:
                case Status.WithoutOrdering:
                    foreach (var keyword in menuKeywords)
                        {
                        if (msg.Contains(keyword))
                            {
                            botWhatsapp.StatusOrder = Status.Ordering;
                            return botWhatsapp;
                            }
                        }
                    botWhatsapp.StatusOrder = Status.WithoutOrdering; // Explícito si no detecta keyword
                    break;

                case Status.Ordering:
                    if (int.TryParse(msg, out int selectedItem) && menuItems.ContainsKey(selectedItem))
                        {
                        botWhatsapp.SelectedItem = selectedItem;
                        botWhatsapp.StatusOrder = Status.Selecting;
                        }
                    else if (msg.Contains("terminar") || msg.Contains("finalizar"))
                        {
                        botWhatsapp.StatusOrder = Status.Closing; // Cambia al estado de cierre si quieren terminar
                        }
                    break;

                case Status.Selecting:
                    if (int.TryParse(msg, out int quantity) && quantity > 0)
                        {
                        botWhatsapp.Quantity = quantity;
                        botWhatsapp.Items.Add(new Item
                            {
                            Id = botWhatsapp.SelectedItem.Value,
                            Product = menuItems[botWhatsapp.SelectedItem.Value].Name,
                            Cant = quantity,
                            HasOrder = true,
                            Price = menuItems[botWhatsapp.SelectedItem.Value].Price,
                            });

                        botWhatsapp.HasOrder = true;
                        botWhatsapp.StatusOrder = Status.Closing;
                        }
                    break;

                case Status.Closing:
                    if (msg.Contains("2") || msg.Contains("si") || msg.Contains("confirmar"))
                        {
                        botWhatsapp.StatusOrder = Status.RequestingName; // PRIMERO pago
                        }
                    else if (msg.Contains("3") || msg.Contains("cancelar"))
                        {
                        botWhatsapp.StatusOrder = Status.WithoutOrdering;
                        botWhatsapp.Items.Clear();
                        botWhatsapp.SelectedItem = null;
                        botWhatsapp.Quantity = null;
                        botWhatsapp.HasOrder = false;
                        }
                    else if (msg.Contains("1") || msg.Contains("ordenar"))
                        {
                        botWhatsapp.StatusOrder = Status.Ordering;
                        }
                    break;
                case Status.ReponsingPayment:
                    botWhatsapp.PaymentMethod = payMethod[Convert.ToInt32(botWhatsapp.Menssage)];
                    botWhatsapp.StatusOrder = Status.Closed;
                    break;

                case Status.ReponsingName:
                    botWhatsapp.CustomerName = botWhatsapp.Menssage.Trim();
                    botWhatsapp.StatusOrder = Status.RequestingPayment;
                    break;


                case Status.Closed:
                    botWhatsapp.StatusOrder = Status.WithoutOrdering;
                    break;
                }

            return botWhatsapp;
            }


        private static async Task FinalizeOrder(BotWhatsapp botWhatsapp)
            {
            decimal total = 0;
            string resumen = "*🧾 Tu factura *\n";
            Random random = new Random();
            resumen += $"👤 Nombre: {botWhatsapp.CustomerName}\n";
            resumen += $"💳 Pago: {botWhatsapp.PaymentMethod}\n";

            resumen += "----------------------------------\n";
            resumen += $"N° Orden: {random.Next(1000, 10000)}\n";
            resumen += "----------------------------------\n";

            foreach (var item in botWhatsapp.Items)
                {
                resumen += $"{item.Cant}x {item.Product} - ${item.Price:F2} c/u = ${item.Subtotal:F2}\n";
                total += item.Subtotal;
                }

            resumen += "----------------------------------\n";
            resumen += $"*Total a pagar:* ${total:F2}\n\n";
            resumen += "🥳 ¡Tu orden ha sido confirmada y está en proceso! Gracias por usar *Botanas John*.";

            await SendMenssageAsync(botWhatsapp, resumen);
            }

        private static async Task WelcomeAsync(BotWhatsapp botWhatsapp)
            {
            await SendMenssageAsync(botWhatsapp, "Bienvenid@s a *Botanas John* \nEstas son las opciones \nMenú, ordenar y paltos");
            }
        private static async Task AskCustomerName(BotWhatsapp botWhatsapp)
            {
            await SendMenssageAsync(botWhatsapp, "¿A nombre de quién estará la orden?");
            }

        private static async Task AskPaymentMethod(BotWhatsapp botWhatsapp)
            {
            string msn = "¿Cuál será el método de pago?\n";
            msn += "1 -  Efectivo\n";
            msn += "2 -  Tarjeta\n";
            msn += "3 -  Transferencia\n\n";
            msn += "*Escribe el número del método de pago.*";

            await SendMenssageAsync(botWhatsapp, msn);
            }

        private static async Task MakeOrderCant(BotWhatsapp botWhatsapp)
            {
            if (botWhatsapp.SelectedItem != null && menuItems.ContainsKey(botWhatsapp.SelectedItem.Value))
                {
                string itemName = menuItems[botWhatsapp.SelectedItem.Value].Name;
                await SendMenssageAsync(botWhatsapp, $"Has elegido *{itemName}*.\n¿Cuántas unidades deseas?");
                }
            else
                {
                botWhatsapp.StatusOrder = Status.Ordering;
                await MakeOrder(botWhatsapp);
                }
            }

        private static async Task MakeOrderConfirmation(BotWhatsapp botWhatsapp)
            {
            if (botWhatsapp.Items.Any())
                {
                decimal total = 0;
                string resumen = "*🧾 Tu factura provisional:*\n";
                resumen += "----------------------------------\n";

                foreach (var item in botWhatsapp.Items)
                    {
                    resumen += $"{item.Cant}x {item.Product} - ${item.Price:F2} c/u = ${item.Subtotal:F2}\n";
                    total += item.Subtotal;
                    }

                resumen += "----------------------------------\n";
                resumen += $"*Total a pagar:* ${total:F2}\n\n";
                resumen += "¿Deseas *Seguir Ordenando (1)*\n *Confirmar la Orden (2)* o *Cancelarla (3)*?\n\n";
                resumen += "*Escribe el número dela opcion.*";

                await SendMenssageAsync(botWhatsapp, resumen);
                }
            else
                {
                botWhatsapp.StatusOrder = Status.Ordering;
                await MakeOrder(botWhatsapp);
                }
            }


        private static async Task MakeOrder(BotWhatsapp botWhatsapp)
            {
            string menu = "🍽️ *Menú de Botanas John*:\n";
            foreach (var item in menuItems)
                {
                menu += $"{item.Key}. {item.Value.Name} - {item.Value.Price}\n";
                }
            menu += "\n*Escribe el número del platillo que deseas ordenar.*";
            await SendMenssageAsync(botWhatsapp, menu);
            }

        private static async Task SendMenssageAsync(BotWhatsapp botWhatsapp, string msg)
            {
            await socket.SendMessage(botWhatsapp.From, new TextMessageContent()
                {
                Text = msg,
                });
            }
        private static bool IsMenssage(WebMessageInfo msg)
            {
            if (msg.Key.RemoteJid.ToString().Contains("status"))
                {
                return false;
                }
            return true;
            }
        private static void Pressence_Update(object? sender, PresenceModel e)
        {
            Console.WriteLine(JsonSerializer.Serialize(e));
        }

        private static void MessageHistory_Set(object? sender, MessageHistoryModel[] e)
            {
            messages.AddRange(e[0].Messages);
            var jsons = messages.Select(x => x.ToJson()).ToArray();
            var array = $"[\n{string.Join(",", jsons)}\n]";
            Debug.WriteLine(array);
            }

        private static async void Connection_Update(object? sender, ConnectionState e)
            {
            var connection = e;
            Debug.WriteLine(JsonSerializer.Serialize(connection));
            if (connection.QR != null)
                {
                QRCodeGenerator QrGenerator = new QRCodeGenerator();
                QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(connection.QR, QRCodeGenerator.ECCLevel.L);
                AsciiQRCode qrCode = new AsciiQRCode(QrCodeInfo);
                var data = qrCode.GetGraphic(1);
                Console.WriteLine(data);
                }
            if (connection.Connection == WAConnectionState.Close)
                {
                if (connection.LastDisconnect.Error is Boom boom && boom.Data?.StatusCode != (int)DisconnectReason.LoggedOut)
                    {
                    try
                        {
                        Thread.Sleep(1000);
                        socket.MakeSocket();
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

                //var mentioned = await socket.SendMessage("27797798179@s.whatsapp.net ", new TextMessageContent()
                //{
                //    Text = $"Hi this is a button",
                //    //Buttons = [
                //    //
                //    //    new Message.Types.ButtonsMessage.Types.Button()
                //    //    {
                //    //        ButtonId = "btn1",
                //    //        ButtonText = new Message.Types.ButtonsMessage.Types.Button.Types.ButtonText()
                //    //        {
                //    //            DisplayText = "Test 1"
                //    //        },
                //    //        Type = Message.Types.ButtonsMessage.Types.Button.Types.Type.Response
                //    //    }
                //    //]
                //});
                var result = await socket.QueryRecommendedNewsletters();

                //var onWhatsApp = await socket.OnWhatsApp("+27797798179", "+15558889234");

                //var count = onWhatsApp.Length;
                //var letter = result.Result[0];
                //await socket.NewsletterFollow(letter.Id);
                //await socket.NewsletterMute(letter.Id);
                //await socket.NewsletterUnMute(letter.Id);
                //await socket.NewsletterUnFollow(letter.Id);




                //await socket.AcceptTOSNotice();
                //var nl = await socket.NewsletterCreate("Test Newsletter");
                //await socket.NewsletterUpdateName(nl.Id, "Newsletter Name");
                //await socket.NewsletterUpdateDescription(nl.Id, "Newsletter Description");
                //var admin = await socket.NewsletterAdminCount(nl.Id);

                //var info = await socket.NewsletterMetadata("120363184364170818@newsletter", BaileysCSharp.Core.Models.Newsletters.NewsletterMetaDataType.JID);



                //var snd = await socket.SendNewsletterMessage("120363285541953068@newsletter", new NewsletterTextMessage()
                //{
                //    Text = "Hello Channel"
                //});

                //await socket.NewsletterDelete(nl.Id);
                //var imageMessage = await socket.SendMessage(nl.Id, new ImageMessageContent()
                //{
                //    Image = File.Open($"{Directory.GetCurrentDirectory()}\\Media\\icon.png", FileMode.Open),
                //    Caption = "Cat.jpeg"
                //});

                //Thread.Sleep(10000);
                //await socket.NewsletterDelete(nl.Id);

                //var standard = await socket.SendMessage("27797798179@s.whatsapp.net", new TextMessageContent()
                //{
                //    Text = "Hi there from C#",
                //});

                }
            }

        private static void Auth_Update(object? sender, AuthenticationCreds e)
            {
            lock (locker)
                {
                var credsFile = Path.Join(socket.SocketConfig.CacheRoot, $"creds.json");
                var json = AuthenticationCreds.Serialize(e);
                File.WriteAllText(credsFile, json);
                }
            }
        }
    }
