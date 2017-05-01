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
                MessageBox.Show("De gebruikersnaam bestaat al.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (password != passwordRepeat)
            {
                MessageBox.Show("Wachtwoorden komen niet overeen, geef a.u.b. 2 keer hetzelfde wachtwoord op", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                passwordBox.Password = "";
                passwordBoxRepeat.Password = "";
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
                MessageBox.Show("Gebruikersnaam en wachtwoord komen niet overeen.", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

        }
    }
}
