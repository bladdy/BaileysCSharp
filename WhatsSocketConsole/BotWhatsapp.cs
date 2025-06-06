﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsSocketConsole
{
    public class BotWhatsapp
    {
        public string? Menssage { get; set; }
        public string? From { get; set; } 
        public string? PushName { get; set; }
        public bool HasOrder { get; set; } = false;
        public Status StatusOrder { get; set; }
        public List<Item> Items { get; set; } = [];
        public int? SelectedItem { get; set; }
        public int? Quantity { get; set; }
        public string PaymentMethod { get; set; }
        public string CustomerName { get; set; }

        }
    public enum Status
    {
        NoStatus,
        WithoutOrdering, 
        Ordering,
        Selecting, 
        RequestingName,
        ReponsingName,
        ReponsingPayment,
        RequestingPayment,
        Closing,
        Closed
    }
    public class Item 
    {
        public int Id { get; set; }
        public string? Product { get; set; }
        public int Cant { get; set; }
        public bool HasOrder { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal => Price * Cant;


        }
    public class MenuItem
    {
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
        }

    }
