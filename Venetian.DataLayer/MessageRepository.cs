using System.Collections.Generic;
using System.Linq;
using Venetian.DomainClasses;

namespace Venetian.DataLayer
{
    public class MessageRepository
    {
        private readonly VenetianContext _venetianContext;

        public MessageRepository(VenetianContext venetianContext)
        {
            _venetianContext = venetianContext;
        }

        public void InsertMessage(Message message)
        {
            _venetianContext.Messages.Add(message);
            _venetianContext.SaveChanges();
        }

        public List<Message> GetAConversation(User sender, User receiver)
        {
            return _venetianContext.Messages.Where(m => (m.Sender.Username == sender.Username || m.Sender.Username == receiver.Username) && (m.Receiver.Username == sender.Username || m.Receiver.Username == receiver.Username)).ToList();

        }
    }
}