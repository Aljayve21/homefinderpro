using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homefinderpro.AdminModels;
using homefinderpro.Models;

namespace homefinderpro.AdminViewModels
{
    public class ProfilePictureViewModel : INotifyPropertyChanged
    {

        private readonly IMongoDatabase _database;

        private ImageSource profilePicture;
        public Command UploadPictureCommand => new Command(async () => await ExecuteUploadPictureCommand());
        private UserProfile _loggedInUser;
        public UserProfile LoggedInUser
        {
            get => _loggedInUser;
            set
            {
                _loggedInUser = value;
                OnPropertyChanged(nameof(LoggedInUser));
            }
        }
        public ImageSource ProfilePicture
        {
            get => profilePicture;
            set
            {
                profilePicture = value;
                OnPropertyChanged(nameof(ProfilePicture));
            }
        }

        private List<UserProfile> _userProfiles;

        public List<UserProfile> UserProfiles
        {
            get => _userProfiles;
            set
            {
                _userProfiles = value;
                OnPropertyChanged(nameof(UserProfiles));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ImageSource ByteArrayToImageSource(byte[] imageData)
        {
            return ImageSource.FromStream(() => new MemoryStream(imageData));
        }




        public ProfilePictureViewModel()
        {

            var dbConnection = new DBConnection();
            _database = dbConnection.GetDatabase();

            //Task.Run(async () => await LoadLoggedInUserProfile());

        }

        //public ObservableCollection<UserProfile> UserProfiles { get; set; }



        public async Task SaveProfilePicture(string username, string role, byte[] pictureData)
        {
            try
            {

                string collectionName = "UserProfile";


                var userProfileCollection = _database.GetCollection<BsonDocument>(collectionName);

                if (userProfileCollection != null)
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("Username", username) & Builders<BsonDocument>.Filter.Eq("Role", role);
                    var update = Builders<BsonDocument>.Update.Set("ProfilePicture", BsonBinaryData.Create(pictureData));

                    var result = await userProfileCollection.UpdateOneAsync(filter, update);

                    if (result.ModifiedCount == 0)
                    {
                        Console.WriteLine($"No matching document found for username: {username} and role: {role}");

                    }


                    ProfilePicture = ByteArrayToImageSource(pictureData);
                }
                else
                {
                    Console.WriteLine($"UserProfile collection is null");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while saving profile picture: {ex.Message}");

            }
        }

        /*
        public async Task LoadLoggedInUserProfile()
        {
            try
            {
                var profiles = await GetLoggedInUserProfile();
                UserProfiles = new ObservableCollection<UserProfile>(profiles);

                OnPropertyChanged(nameof(UserProfiles));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while loading user profiles: {ex.Message}");
            }
        }
        */

        private async Task ExecuteUploadPictureCommand()
        {
            try
            {
                var stream = GetPictureStream();

                if (stream != null)
                {
                    var byteArray = ReadFully(stream);

                    await SaveProfilePictureToDatabase(byteArray);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during upload: {ex.Message}");
            }
        }
        /*
        private async void UploadPicture()
        {
            var stream = GetPictureStream();

            if (stream != null)
            {
                var byteArray = ReadFully(stream);
                await SaveProfilePicture("username", "user_role", byteArray);

            }
        }
        */

        public Stream GetPictureStream()
        {
            try
            {
                var mediaFile = MediaPicker.PickVideoAsync().Result;

                if (mediaFile != null)
                {
                    return mediaFile.OpenReadAsync().Result;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error picking a photo: {ex.Message}");
                return null;
            }
        }



        public byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }






        public async Task<List<UserProfile>> GetLoggedInUserProfile()
        {
            try
            {
                if (!UserSession.Instance.IsUserLoggedIn())
                {
                    Console.WriteLine("User not logged in.");
                    return new List<UserProfile>();
                }

                var username = UserSession.Instance.Username;
                var role = UserSession.Instance.Role;

                IMongoCollection<BsonDocument> userCollection;
                switch (role.ToLowerInvariant())
                {
                    case "admin":
                        userCollection = _database.GetCollection<BsonDocument>("users");
                        break;
                    case "customer":
                        userCollection = _database.GetCollection<BsonDocument>("customers");
                        break;
                    case "landlord":
                        userCollection = _database.GetCollection<BsonDocument>("landlords");
                        break;
                    default:
                        Console.WriteLine($"Unsupported role: {role}");
                        return new List<UserProfile>();
                }

                var filter = Builders<BsonDocument>.Filter.Eq("Username", username);
                var projection = Builders<BsonDocument>.Projection.Include("Username")
                                                            .Include("Fullname")
                                                            .Include("ProfilePicture");

                var result = await userCollection.Find(filter).Project(projection).ToListAsync();

                var userProfileList = result.Select(doc =>
                    new UserProfile
                    {
                        Username = doc.GetValue("Username").AsString ?? string.Empty,
                        Fullname = doc.GetValue("Fullname").AsString ?? string.Empty,
                        ProfilePicture = doc.GetValue("ProfilePicture").AsByteArray ?? Array.Empty<byte>()
                    }
                ).ToList();

                return userProfileList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while retrieving data: {ex.Message}");
                throw;
            }
        }

        public async Task SaveProfilePictureToDatabase(byte[] pictureData)
        {
            try
            {
                if (LoggedInUser != null)
                {
                    var userProfileCollection = _database.GetCollection<UserProfile>("UserProfile");

                    var filter = Builders<UserProfile>.Filter.Eq("Id", LoggedInUser.Id);
                    var update = Builders<UserProfile>.Update.Set("ProfilePicture", pictureData);

                    await userProfileCollection.UpdateOneAsync(filter, update);
                }
                else
                {
                    Console.WriteLine("No logged-in user found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while saving profile picture: {ex.Message}");

            }
        }



        public async Task<byte[]> ImageSourceToByteArray(ImageSource imageSource)
        {
            try
            {
                var streamImageSource = (StreamImageSource)imageSource;
                var stream = await streamImageSource.Stream(CancellationToken.None);
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while converting ImageSource to byte: {ex.Message}");
                throw;
            }
        }

        /*
        public Command UploadPictureCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var stream = GetPictureStream();
                    if (stream != null)
                    {
                        var byteArray = ReadFully(stream);
                        await SaveProfilePictureToDatabase("username", "user_role", ImageSource.FromStream(() => new MemoryStream(byteArray)));
                    }
                });
            }
        }
        */

        public void LoadUserProfile()
        {
            if (UserSession.Instance.IsUserLoggedIn())
            {
                var username = UserSession.Instance.Username;
                var filter = Builders<users>.Filter.Eq("Username", username);
                var projection = Builders<users>.Projection.Include(a => a.Username).Include(a => a.Fullname).Include(a => a.ProfilePicture);

                var user = _database.GetCollection<users>("users").Find(filter).Project(projection).FirstOrDefault();

                if (user != null)
                {
                    var profilePicture = user.GetValue("ProfilePicture").AsByteArray;
                    ProfilePicture = ByteArrayToImageSource(profilePicture);
                }
            }
        }


    }
}
