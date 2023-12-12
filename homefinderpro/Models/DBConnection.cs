using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using homefinderpro.AdminModels;
using MongoDB.Bson;

namespace homefinderpro.Models
{
    public class DBConnection
    {
        private static readonly IMongoClient _mongoClient;
        private static readonly IMongoDatabase _database;

        private static DBConnection _instance;
        static DBConnection()
        {
            try
            {
                var connectionString = "mongodb://admin:test123@ac-nnq7dcj-shard-00-00.4jhaesp.mongodb.net:27017,ac-nnq7dcj-shard-00-01.4jhaesp.mongodb.net:27017,ac-nnq7dcj-shard-00-02.4jhaesp.mongodb.net:27017/?ssl=true&replicaSet=atlas-e18t97-shard-0&authSource=admin&retryWrites=true&w=majority";

                _mongoClient = new MongoClient(connectionString);

                _database = _mongoClient.GetDatabase("homefinder");
                Console.WriteLine("Connection to the database is successful.");
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"Error during database connection: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error during database connection: {ex}");
                throw;
            }
        }

        public static DBConnection Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DBConnection();
                }
                return _instance;
            }
        }

        public IMongoDatabase GetDatabase()
        {


            return _database ?? throw new InvalidOperationException("Database is not initialized. Make sure the MongoDB Database is connected to your application");
        }

        public IMongoCollection<LandlordPost> GetLandlordPostsCollection()
        {
            return _database.GetCollection<LandlordPost>("landlordposts");
        }

        public IMongoCollection<LandlordPost> GetAdminApprovalCollection()
        {
            return _database.GetCollection<LandlordPost>("adminapproval");
        }

        public void InsertLandlordPost(LandlordPost post)
        {
            var collection = GetLandlordPostsCollection();
            collection.InsertOne(post);
        }

        public void InsertAdminApproval(LandlordPost post)
        {
            var collection = GetAdminApprovalCollection();
            collection.InsertOne(post);
        }



        public async Task<bool> AuthenticateUser(string username, string password, string role)
        {
            try
            {
                var database = GetDatabase();


                var collectionName = role.ToLowerInvariant() switch
                {
                    "admin" => "users",
                    "landlord" => "landlords",
                    "customer" => "customers",
                    _ => throw new ArgumentException("Invalid role", nameof(role))
                };

                var usersCollection = database.GetCollection<users>(collectionName);

                var filter = Builders<users>.Filter.Eq(u => u.Username, username);
                var user = await usersCollection.Find(filter).FirstOrDefaultAsync();

                if (user != null)
                {
                    Console.WriteLine($"Entered Password: {password}");
                    Console.WriteLine($"Retrieved Hashed Password: {user.PasswordHash}");


                    return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Incorrect!", "Incorrect Username or Password please try again!", "OK");
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during user authentication: {ex.Message}");
                return false;
            }
        }
    }
}
