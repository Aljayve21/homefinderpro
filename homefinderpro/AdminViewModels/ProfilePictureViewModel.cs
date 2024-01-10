using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homefinderpro.AdminModels;
using homefinderpro.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Extensions.Logging;

namespace homefinderpro.AdminViewModels
{

    public class UserDetailsModel
    {
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Role { get; set; }
    }

    public class ProfilePictureViewModel : INotifyPropertyChanged
    {
        
        private readonly IMongoCollection<users> _userCollection;
        
        public ProfilePictureViewModel()
        {
            _userCollection = DBConnection.Instance.GetDatabase().GetCollection<users>("users");
            LoadUserProfiles();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<users> _userProfiles;
        public ObservableCollection<users> UserProfiles
        {
            get { return _userProfiles; }
            set { _userProfiles = value; }
        }

        private users _selectedUserProfile;
        public users SelectedUserProfile
        {
            get { return _selectedUserProfile; }
            set { _selectedUserProfile = value; }
        }

        public async Task GetUserDetails(string username)
        {
            try
            {
                var user = await _userCollection.Find(u => u.Username == username).FirstOrDefaultAsync();

                if (user != null)
                {
                    string role = user.Role;
                    string fullName = user.Fullname;
                    string retrievedUsername = user.Username;

                    Console.WriteLine($"Role: {role}, Username: {retrievedUsername}, Fullname: {fullName}");

                    // Update UserDetailsCollection
                    UserDetailsCollection = new ObservableCollection<UserDetailsModel>
                {
                    new UserDetailsModel { Username = retrievedUsername, Fullname = fullName, Role = role }
                };

                    
                    UserSession.Instance.SetUser(retrievedUsername, fullName, role);
                }
                else
                {
                    Console.WriteLine($"No user found for username: {username}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving user details: {ex.Message}");
            }
        }



        private async void LoadUserProfiles()
        {
            try
            {
                var users = await _userCollection.Find(_ => true).ToListAsync();
                UserProfiles = new ObservableCollection<users>(users);

                if (UserProfiles.Count > 0)
                {
                    SelectedUserProfile = UserProfiles[0];

                    if (SelectedUserProfile.ProfilePicture != null)
                    {
                        SelectedUserProfilePicture = ImageSource.FromStream(() => new MemoryStream(SelectedUserProfile.ProfilePicture));
                    }
                    else
                    {
                        SelectedUserProfilePicture = ImageSource.FromFile("profilepicture.png");
                    }

                    UserSession.Instance.SetUser(SelectedUserProfile.Username, SelectedUserProfile.Fullname, SelectedUserProfile.Role);
                }
                else
                {
                    SelectedUserProfilePicture = ImageSource.FromFile("profilepicture.png");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user profiles: {ex.Message}");
                SelectedUserProfilePicture = ImageSource.FromFile("profilepicture.png");
            }
        }


        private ICommand _uploadProfilePictureCommand;
        public ICommand UploadProfilePictureCommand
        {
            get
            {
                return _uploadProfilePictureCommand ?? (_uploadProfilePictureCommand = new Command(async () => await ExecuteUploadProfilePictureCommand()));
            }
        }

        private ImageSource _profileImageSource;
        public ImageSource ProfileImageSource
        {
            get { return _profileImageSource; }
            set
            {
                _profileImageSource = value;
                OnPropertyChanged(nameof(ProfileImageSource));
            }
        }

        private ImageSource _selectedUserProfilePicture;

        public ImageSource SelectedUserProfilePicture
        {
            get { return _selectedUserProfilePicture; }
            set
            {
                if (_selectedUserProfilePicture != value)
                {
                    _selectedUserProfilePicture = value;
                    OnPropertyChanged(nameof(SelectedUserProfilePicture));
                }
            }
        }


        public async Task ExecuteUploadProfilePictureCommand()
        {
            try
            {
                Console.WriteLine("Executing UploadProfilePictureCommand");
                var pictureData = await GetPictureDataAsync();

                if (pictureData != null)
                {
                    await UploadProfilePicture(pictureData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during profile picture upload: {ex.Message}");
            }
        }



        public async Task<byte[]> GetPictureDataAsync()
        {
            try
            {
                Console.WriteLine("Attempting to pick a photo");
                var photo = await MediaPicker.PickPhotoAsync();

                if (photo != null)
                {
                    using (var stream = await photo.OpenReadAsync())
                    using (var memoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(memoryStream);
                        Console.WriteLine("Successfully obtained picture data");
                        return memoryStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting picture data: {ex.Message}");
            }

            return null;
        }



        public async Task SaveProfilePicture(string username, string role, byte[] pictureData)
        {
            try
            {
                Console.WriteLine($"Checking if admin username '{username}' exists in the users collection.");

                // Check if the admin username exists in the users collection
                var adminUsernameExists = await _userCollection.Find(u => u.Username == username).AnyAsync();

                if (adminUsernameExists)
                {
                    Console.WriteLine($"Admin username '{username}' exists in the users collection.");

                    // Proceed with the regular user update logic
                    Console.WriteLine($"Attempting to update profile picture for username: {username}, role: {role}");

                    var filter = Builders<users>.Filter.Eq("Username", username) & Builders<users>.Filter.Eq("Role", role);
                    var update = Builders<users>.Update.Set("ProfilePicture", BsonBinaryData.Create(pictureData));
                    var result = await _userCollection.UpdateOneAsync(filter, update);
                    Console.WriteLine($"Filter expression: {filter.ToString()}");

                    if (result.ModifiedCount == 0)
                    {
                        Console.WriteLine($"No matching document found for username: {username} and role: {role}");
                    }
                    else
                    {
                        Console.WriteLine($"Profile picture updated successfully for username: {username}, role: {role}");
                    }
                }
                else
                {
                    Console.WriteLine($"Admin username '{username}' does not exist in the users collection.");
                }

                LoadUserProfiles();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while saving profile picture: {ex.Message}");
            }
        }

        public async Task DeleteProfilePicture(string username, string role)
        {
            try
            {
                var adminUsernameExists = await _userCollection.Find(u => u.Username == username).AnyAsync();
                if (adminUsernameExists)
                {
                    Console.WriteLine($"Admin username '{username}' exists in the users collection.");

                    Console.WriteLine($"Attempting to delete profile picture for username: {username}, role: {role}");
                    string collectionName = role.ToLower();
                    //var userProfileCollection = DBConnection.Instance.GetDatabase().GetCollection<users>(collectionName);

                    var filter = Builders<users>.Filter.Eq("Username", username);
                    var update = Builders<users>.Update.Unset("ProfilePicture");

                    var result = await _userCollection.UpdateOneAsync(filter, update);

                    if (result.ModifiedCount == 0)
                    {
                        Console.WriteLine($"No matching document found for username: {username} and role: {role}");
                    }

                    LoadUserProfiles();
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while deleting profile picture: {ex.Message}");
            }
        }

        public async Task UploadProfilePicture(byte[] pictureData)
        {
            if (SelectedUserProfile != null)
            {
                await SaveProfilePicture(SelectedUserProfile.Username, SelectedUserProfile.Role, pictureData);
            }
            else
            {
                Console.WriteLine("No user profile selected for upload.");
            }
        }

        

        private ObservableCollection<UserDetailsModel> _userDetailsCollection;
        public ObservableCollection<UserDetailsModel> UserDetailsCollection
        {
            get { return _userDetailsCollection; }
            set
            {
                _userDetailsCollection = value;
                OnPropertyChanged(nameof(UserDetailsCollection));
            }
        }

        private async Task ExecuteGetUserDetailsCommand()
        {
            try
            {
                if (!string.IsNullOrEmpty(SelectedUserProfile?.Username))
                {
                    
                    await GetUserDetails(SelectedUserProfile.Username);
                }
                else
                {
                    Console.WriteLine("No user profile selected to retrieve details.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing GetUserDetailsCommand: {ex.Message}");
            }
        }

        




    }
}
