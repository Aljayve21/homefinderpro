using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using homefinderpro.Models;

namespace homefinderpro.AdminViewModels
{
    public class MainPageViewModel : BindableObject
    {
        private readonly ProfilePictureViewModel viewModel;
        public Command NavigateToSignupCommand { get; }

        public Command LoginCommand { get; }



        private string _username;
        private string _password;

        private ImageSource _imageSource;

        public ImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }



        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        



        private List<string> _roles;
        public List<string> Roles
        {
            get { return _roles; }
            set { _roles = value; }
        }

        private string _selectedRole;
        public string SelectedRole
        {
            get { return _selectedRole; }
            set { _selectedRole = value; }
        }



        private async Task HandleLogin(string username, string password, string role)
        {
            try
            {
                var dbConnection = new DBConnection();
                var isAuthenticated = await dbConnection.AuthenticateUser(username, password, role);

                if (isAuthenticated)
                {
                    Console.WriteLine("Login Successfully!");


                    switch (role.ToLower())
                    {
                        case "admin":
                            await Shell.Current.GoToAsync("//admin/adminDashboard");
                            break;
                        case "landlord":
                            await Shell.Current.GoToAsync("//landlord/home");
                            break;
                        case "customer":
                            await Shell.Current.GoToAsync("//customer/home");
                            break;
                        default:
                            Console.WriteLine("Invalid role");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Sorry! The username or password is invalid. Please try again");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login: {ex.Message}");
            }
        }

        private async Task ExecuteLoginCommand()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SelectedRole))
                {
                    Console.WriteLine("Please select a role");
                    return;
                }

                await HandleLogin(Username, Password, SelectedRole);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login: {ex.Message}");
            }
        }






        


        public MainPageViewModel()
        {
            NavigateToSignupCommand = new Command(NavigateToSignup);
            LoginCommand = new Command(async () => await ExecuteLoginCommand());
            Roles = new List<string> { "admin", "customer", "landlord" };

           

        }

        

        


        private async void NavigateToSignup()
        {
            var signupPage = new homefinderpro.SignupPage();
            await Shell.Current.Navigation.PushAsync(signupPage);
        }


    }
}
