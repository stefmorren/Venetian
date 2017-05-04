using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Microsoft.Win32;
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
        private Bitmap _image;

        public MessagePanel(User user, VenetianContext venetianContext)
        {
            InitializeComponent();
            _userRepository = new UserRepository(venetianContext);
            _messageRepository = new MessageRepository(venetianContext);
            _user = user;
            FillReceiverList();
            SetConversationLayout(false);
            resetSteganoForm(false);

        }

        private void FillReceiverList()
        {
            listBoxReceivers.DisplayMemberPath = "Username";
            listBoxReceivers.ItemsSource = _userRepository.GetReceiversExceptUser(_user);
        }

        private void listBoxReceivers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _receiver = (User)listBoxReceivers.SelectedValue;
            SetConversationLayout(true);
            LoadConversations();


        }

        private void LoadConversations()
        {
            List<Message> messages = _messageRepository.GetAConversation(_user, _receiver);
            textBlockMessages.Text = "";

            foreach (Message message in messages)
            {
                string decryptedText = AESUtility.DecryptStringFromBytes_Aes(message.EncryptedText,
                    RSAUtility.RSADecrypt(message.Receiver.PrivateKey, message.EncryptedAesKey), message.IV);

                if (RSAUtility.RSAVerifySignedHash(Encoding.ASCII.GetBytes(decryptedText),
                    message.RSAEncryptedHashedMessage, message.Sender.PublicKey))
                {
                    textBlockMessages.Text += message.Date.ToString("F") + "\n" + message.Sender.Username + ": " + decryptedText +
                                              "\n\n";
                }
                else
                {
                    MessageBox.Show("Error, someone tries to mess with the keys.", "Error", MessageBoxButton.OK,
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
            if (!(textBoxMessage.Text.Length == 0))
            {
                Message message = new Message();
                message.Date = DateTime.Now;
                //message.EncryptedText = textBoxMessage.Text;
                message.Receiver = _receiver;
                message.Sender = _user;

                using (Aes myAes = Aes.Create())
                {
                    byte[] encrypted = AESUtility.EncryptStringToBytes_Aes(textBoxMessage.Text, myAes.Key, myAes.IV);
                    message.EncryptedText = encrypted;
                    message.IV = myAes.IV;
                    message.EncryptedAesKey = RSAUtility.RSAEncrypt(_receiver.PublicKey, myAes.Key);
                }
                //Generate hash from original message & encrypted with RSA for signing
                message.RSAEncryptedHashedMessage = RSAUtility.RSASign(_user.PrivateKey,
                    Encoding.ASCII.GetBytes(textBoxMessage.Text));

                _messageRepository.InsertMessage(message);
                textBoxMessage.Text = "";
                LoadConversations();
            }
        }

        private void buttonOpenImage_Click(object sender, RoutedEventArgs e)
        {
            resetSteganoForm(false);
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string startdir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            openFileDialog.InitialDirectory = startdir;
            openFileDialog.Filter = "Image files (*.png, *.jpg, *.bmp) | *.png; *.jpg; *.bmp";
            if (openFileDialog.ShowDialog() == true)
            {
                imageBox.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                _image = new Bitmap(openFileDialog.FileName);
                textBoxImageLocation.Text = openFileDialog.FileName;
            }
            resetSteganoForm(true);
        }

        private void buttonEncode_Click(object sender, RoutedEventArgs e)
        {
            //Check if text fits in image and if not empty
            if (textBoxText.Text.Length == 0)
            {
                MessageBox.Show("The text you want to hide can't be empty", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else if (textBoxText.Text.Length > _image.Height * _image.Width * 0.375)
            {
                MessageBox.Show("This text is too long ("+ textBoxText.Text.Length + " characters) for the image.\n" +
                                "The maximum number of characters you can use: " + Math.Floor(_image.Height * _image.Width * 0.375), "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else
            {
                if (checkBoxEncrypted.IsChecked == true)
                {
                    if (passwordBox.Password.Length < 6)
                    {
                        MessageBox.Show("Password must have at least 6 characters.", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                    else
                    {
                        _image =
                            SteganographyUtility.embedText(
                                AESUtility.EncryptStringAES(textBoxText.Text, passwordBox.Password),
                                _image);
                        textBoxText.Text = "";
                    }
                }
                else
                {
                    _image =
                        SteganographyUtility.embedText(
                            textBoxText.Text,
                            _image);
                    textBoxText.Text = "";
                }
            }

        }

        private void checkBoxEncrypted_Checked(object sender, RoutedEventArgs e)
        {
            passwordBox.IsEnabled = true;
        }

        private void checkBoxEncrypted_Unchecked(object sender, RoutedEventArgs e)
        {
            passwordBox.IsEnabled = false;
            passwordBox.Password = "";
        }

        private void buttonDecode_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxEncrypted.IsChecked == true)
            {
                try
                {
                    textBoxText.Text = AESUtility.DecryptStringAES(SteganographyUtility.extractText(_image), passwordBox.Password);
                }
                catch (Exception)
                {

                    MessageBox.Show("Wrong password!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                textBoxText.Text = SteganographyUtility.extractText(_image);
            }
        }

        private void buttonSaveImage_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save_dialog = new SaveFileDialog();
            save_dialog.Filter = "Png Image|*.png|Bitmap Image|*.bmp";

            if (save_dialog.ShowDialog() == true)
            {
                switch (save_dialog.FilterIndex)
                {
                    case 1:
                        {
                            _image.Save(save_dialog.FileName, ImageFormat.Png);
                        }
                        break;
                    case 2:
                        {
                            _image.Save(save_dialog.FileName, ImageFormat.Bmp);
                        }
                        break;
                }
            }
            textBoxText.Text = "";
            MessageBox.Show("Images succesfully saved.", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void resetSteganoForm(bool value)
        {
            if (value == false)
            {
                _image = null;
                textBoxText.Text = "";
                passwordBox.Password = "";
                textBoxImageLocation.Text = "";
                passwordBox.IsEnabled = false;
                checkBoxEncrypted.IsChecked = false;
            }
            buttonDecode.IsEnabled = value;
            buttonEncode.IsEnabled = value;
            buttonSaveImage.IsEnabled = value;
            textBoxText.IsEnabled = value;
            checkBoxEncrypted.IsEnabled = value;

        }

        private void MenuMessageSignOut_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            MainWindow mainWindow = new MainWindow(); 
            mainWindow.Show();
        }

        private void MenuMessageClose_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
