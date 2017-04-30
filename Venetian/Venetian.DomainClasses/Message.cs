using System;
using System.ComponentModel.DataAnnotations;

namespace Venetian.DomainClasses
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public virtual User Sender { get; set; }
        public virtual User Receiver { get; set; }
    }
}