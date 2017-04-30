using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Venetian.DataLayer;
using Venetian.DomainClasses;

namespace Venetian
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Panel : Window
    {
        private UserRepository _userRepository;
        private MessageRepository _messageRepository;
        private User _user;
        private User _receiver;
        public Panel(User user, VenetianContext venetianContext)
        {
            InitializeComponent();
            _userRepository = new UserRepository(venetianContext);
            _messageRepository = new MessageRepository(venetianContext);
            _user = user;
            FillReceiverList();
            SetConversationLayout(false);
        }

        private void FillReceiverList()
        {
            listBoxReceivers.DisplayMemberPath = "Username";
            listBoxReceivers.ItemsSource = _userRepository.GetReceiversExceptUser(_user);
        }

        private void listBoxReceivers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _receiver = (User) listBoxReceivers.SelectedValue;
            SetConversationLayout(true);
            LoadConversations();


        }

        private void LoadConversations()
        {
            List<Message> messages = _messageRepository.GetAConversation(_user, _receiver);
            textBlockMessages.Text = "";

            foreach (Message message in messages)
            {
                textBlockMessages.Text += message.Date + "\n" + message.Sender.Username + ": " + message.Text + "\n";
            }
        }

        private void SetConversationLayout(bool value)
        {
            textBlockMessages.IsEnabled = value;
            textBoxMessage.IsEnabled = value;
            buttonSend.IsEnabled = value;
        }

        private void buttonSend_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxMessage.Text.Length == 0)
            {
                MessageBox.Show("Een bericht mag niet leeg zijn!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Message message = new Message();
                message.Date = DateTime.Now;
                message.Text = textBoxMessage.Text;
                message.Receiver = _receiver;
                message.Sender = _user;

                _messageRepository.InsertMessage(message);
                textBoxMessage.Text = "";
                LoadConversations();
            }    
        }
    }
}
