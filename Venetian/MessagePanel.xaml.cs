using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using Venetian.BusinessLayer;
using Venetian.DataLayer;
using Venetian.DomainClasses;

namespace Venetian
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MessagePanel : Window
    {
        private UserRepository _userRepository;
        private MessageRepository _messageRepository;
        private User _user;
        private User _receiver;
        public MessagePanel(User user, VenetianContext venetianContext)
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
                string decryptedText = AESUtility.DecryptStringFromBytes_Aes(message.EncryptedText, RSAUtility.RSADecrypt(message.Receiver.PrivateKey, message.EncryptedAesKey), message.IV);

                if (RSAUtility.RSAVerifySignedHash(Encoding.ASCII.GetBytes(decryptedText), message.RSAEncryptedHashedMessage, message.Sender.PublicKey))
                {
                    textBlockMessages.Text += message.Date + "\n" + message.Sender.Username + ": " + decryptedText + "\n\n";
                }
                else
                {
                    MessageBox.Show("Error, iemand probeert met de keys te knoeien.", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            scrollviewerConversation.ScrollToEnd();
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
                //message.EncryptedText = textBoxMessage.Text;
                message.Receiver = _receiver;
                message.Sender = _user;

                using(Aes myAes = Aes.Create())
                {
                    byte[] encrypted = AESUtility.EncryptStringToBytes_Aes(textBoxMessage.Text, myAes.Key, myAes.IV);
                    message.EncryptedText = encrypted;
                    message.IV = myAes.IV;
                    message.EncryptedAesKey = RSAUtility.RSAEncrypt(_receiver.PublicKey, myAes.Key); 
                }
                //Generate hash from original message & encrypted with RSA for signing
                message.RSAEncryptedHashedMessage = RSAUtility.RSASign(_user.PrivateKey, Encoding.ASCII.GetBytes(textBoxMessage.Text));

                _messageRepository.InsertMessage(message);
                textBoxMessage.Text = "";
                LoadConversations();
            }    
        }
    }
}
