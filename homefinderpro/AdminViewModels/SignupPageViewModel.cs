using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homefinderpro.AdminModels;
using homefinderpro.Models;
using MongoDB.Driver;
using homefinderpro.LandlordViewModels;
using homefinderpro.CustomerModels;

namespace homefinderpro.AdminViewModels
{
    public class SignupViewModel : BindableObject
    {
        private string _fullname;
        private string _username;
        private string _password;
        private string _selectedRole;

        public string Fullname
        {
            get => _fullname;
            set
            {
                _fullname = value;
                OnPropertyChanged(nameof(Fullname));
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

        public string SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                OnPropertyChanged(nameof(SelectedRole));
            }
        }


        public async Task Signup(string password, string selectedRole)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Fullname) || string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    Console.WriteLine("Please fill in all required fields.");
                    return;
                }

                var dbConnection = new DBConnection();
                var database = dbConnection.GetDatabase();

                if (await IsUsernameExistsAsync(database, Username))
                {
                    await Shell.Current.DisplayAlert("Already exist", "Username already exists. Please choose a different username.", "Ok");
                    Console.WriteLine("Username already exists. Please choose a different username.");
                    return;
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);
                switch (_selectedRole.ToLowerInvariant())
                {
                    case "landlord":
                        var landlord = new Landlord
                        {
                            Fullname = Fullname,
                            Username = Username,
                            PasswordHash = hashedPassword,
                            Role = "Landlord"
                        };
                        await InsertAsync(database, landlord, "landlords");
                        break;

                    case "admin":
                        var admin = new users
                        {
                            Fullname = Fullname,
                            Username = Username,
                            PasswordHash = hashedPassword,
                            Role = "Admin"
                        };
                        await InsertAsync(database, admin, "users");
                        break;

                    case "customer":
                        var customer = new Customer
                        {
                            Fullname = Fullname,
                            Username = Username,
                            PasswordHash = hashedPassword,
                            Role = "Customer"
                        };
                        await InsertAsync(database, customer, "customers");
                        break;

                    default:
                        Console.WriteLine($"Unsupported role: {_selectedRole}");
                        break;
                }

                await App.Current.MainPage.Navigation.PushAsync(new MainPage());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during signup: {ex.Message}");
            }
        }

        private async Task InsertAsync<T>(IMongoDatabase database, T document, string collectionName)
        {
            try
            {
                var collection = database.GetCollection<T>(collectionName);
                await collection.InsertOneAsync(document);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during document insertion: {ex.Message}");
            }
        }

        private async Task<bool> IsUsernameExistsAsync(IMongoDatabase database, string username)
        {
            var adminsCollection = database.GetCollection<users>("users");
            var landlordsCollection = database.GetCollection<Landlord>("landlord");

            var adminUsernameExists = await adminsCollection.Find(a => a.Username == username).AnyAsync();
            var landlordUsernameExists = await landlordsCollection.Find(l => l.Username == username).AnyAsync();

            return adminUsernameExists || landlordUsernameExists;
        }



        private async Task InsertAdminAsync(users admin)
        {
            try
            {
                var dbConnection = new DBConnection();
                var database = dbConnection.GetDatabase();
                var adminsCollection = database.GetCollection<users>("users");

                await adminsCollection.InsertOneAsync(admin);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during admin insertion: {ex.Message}");
            }
        }

        private async Task InsertLandlordAsync(Landlord landlord)
        {
            try
            {
                var dbConnection = new DBConnection();
                var database = dbConnection.GetDatabase();
                var landlordsCollection = database.GetCollection<Landlord>("landlord");

                await landlordsCollection.InsertOneAsync(landlord);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during landlord insertion: {ex.Message}");
            }
        }


        public Microsoft.Maui.Controls.Command<string> SignupLandlordCommand => new Microsoft.Maui.Controls.Command<string>(async (password) => await Signup(password, "Landlord"));
        public Microsoft.Maui.Controls.Command<string> SignupCommand => new Microsoft.Maui.Controls.Command<string>(async (password) => await Signup(password, "Admin"));
        public Microsoft.Maui.Controls.Command<string> SignupCustomerCommand => new Microsoft.Maui.Controls.Command<string>(async (password) => await Signup(password, "Customer"));
    }
}
