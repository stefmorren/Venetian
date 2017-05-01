using System;
using System.ComponentModel.DataAnnotations;

namespace Venetian.DomainClasses
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        public DateTime Date { get; set; }
        public byte[] EncryptedText { get; set; }
        public byte[] EncryptedAesKey { get; set; }
        public byte[] IV { get; set; }
        public byte[] RSAEncryptedHashedMessage { get; set; }
        public virtual User Sender { get; set; }
        public virtual User Receiver { get; set; }

    }
}