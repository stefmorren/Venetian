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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Venetian.BusinessLayer;
using Venetian.DataLayer;
using Venetian.DomainClasses;

namespace Venetian
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserRepository _userRepository;
        private VenetianContext _venetianContext;
        public MainWindow()
        {
            InitializeComponent();
            _venetianContext = new VenetianContext();
            _userRepository = new UserRepository(_venetianContext);
        }

        private void buttonRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = textboxUsername.Text;
            string password = passwordBox.Password;
            string passwordRepeat = passwordBoxRepeat.Password;

            if (_userRepository.CheckIfUsernameAllreadyExists(username))
            {
                MessageBox.Show("The user allready exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (password != passwordRepeat)
            {
                MessageBox.Show("The repeated password isn't the same as the password", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                passwordBox.Password = "";
                passwordBoxRepeat.Password = "";
            }
            else if (password.Length<=6)
            {
                MessageBox.Show("The password must have at least 7 characters..", "Error",
    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (username.Length <= 3)
            {
                MessageBox.Show("The username must have at least 4 characters.", "Error",
    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                User user = new User();
                user.Username = username;
                user.Salt = HashUtility.GenerateSalt();
                user.Password = HashUtility.GenerateSHA256FromString(password + user.Salt);
                List<string> keys = RSAUtility.GenerateRSAPublicAndPrivateKeys();
                user.PublicKey = keys[0];
                user.PrivateKey = keys[1];
                _userRepository.AddUser(user);
                MessageBox.Show("Succesvol geregistreerd!");
                tabControl.SelectedIndex = 0;
            }

        }

        private void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = textboxLoginUsername.Text;
            string password = passwordBoxLogin.Password;
            string salt = _userRepository.ReturnUsersSalt(username);
            var user = _userRepository.LoginUser(username, HashUtility.GenerateSHA256FromString(password + salt));
            if (user != null)
            {
                Hide();
                MessagePanel messagePanel = new MessagePanel(user, _venetianContext);
                messagePanel.Show();

            }
            else
            {
                MessageBox.Show("The username and the password doesn't match.", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

        }

        private void MenuMainClose_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0); 
        }

        private void CleanTextboxes()
        {
            textboxLoginUsername.Text = "";
            textboxUsername.Text = "";
            passwordBox.Password = "";
            passwordBoxLogin.Password = "";
            passwordBoxRepeat.Password = "";
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CleanTextboxes();
        }
    }
}
