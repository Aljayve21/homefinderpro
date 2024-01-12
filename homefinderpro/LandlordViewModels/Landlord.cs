using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.GridFS;
using homefinderpro.Models;
using homefinderpro.LandlordModels;
using homefinderpro.AdminModels;
using homefinderpro.AdminViewModels;

namespace homefinderpro.LandlordViewModels
{
    public class Landlord : IUser
    {

        private readonly DBConnection _dbConnection;

        public ObjectId Id { get; set; }
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public byte[] ProfilePicture { get; set; }


        public Landlord(DBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        private string _selectedLocation;

        public void SetSelectedLocation(string location)
        {
            _selectedLocation = location;
        }

        public string GetSelectedLocation()
        {
            return _selectedLocation;
        }





        public bool SubmitPost(string category, string description, string location, decimal price, List<byte[]> photos, byte[] validIdPicture, byte[] governmentDocument)
        {
            try
            {
                var post = new LandlordPost
                {
                    LandlordId = this.Id,
                    Category = category,
                    Description = description,
                    Location = location,
                    Price = price,
                    Photos = photos,
                    ValidIdPicture = validIdPicture,
                    GovernmentDocument = governmentDocument,
                    Status = "Pending",
                    SubmissionDate = DateTime.Now
                };


                var photoIds = new List<ObjectId>();
                foreach (var photo in photos)
                {
                    var photoId = UploadPhoto(photo);
                    photoIds.Add(photoId);
                }

                post.PhotoIds = photoIds;


                var database = _dbConnection.GetDatabase();
                var collection = database.GetCollection<LandlordPost>("landlordposts");
                collection.InsertOne(post);


                NotifyAdmin(post);



                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during post submission: {ex.Message} ");
                return false;
            }
        }

        private ObjectId UploadPhoto(byte[] photo)
        {
            var database = _dbConnection.GetDatabase();


            var gridFsBucket = new GridFSBucket(database);
            var photoId = gridFsBucket.UploadFromBytes("photos", photo);

            return photoId;
        }

        private void NotifyAdmin(LandlordPost post, Landlord currentLandlord)
        {
            try
            {
                if(currentLandlord == null)
                {
                    Console.WriteLine("Current landlord not available");
                    return;
                }
                var adminCollection = _dbConnection.GetDatabase().GetCollection<AdminApproval>("adminapproval");

                var adminApproval = new AdminApproval
                {
                    LandlordPostId = post.Id,
                    ApprovalStatus = "Pending",
                    
                };

                adminCollection.InsertOne(adminApproval);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error notifying admin: {ex.Message} ");
            }
        }

        private void NotifyAdmin(LandlordPost post)
        {
            NotifyAdmin(post, this);
        }



        public bool SendToApproval(LandlordPost post)
        {
            try
            {

                var approvalCollection = _dbConnection.GetDatabase().GetCollection<LandlordPost>("approval");


                approvalCollection.InsertOne(post);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending data to approval collection: {ex.Message}");
                return false;
            }
        }

        public bool SendToAdminApproval(LandlordPost post)
        {
            try
            {

                var adminCollection = _dbConnection.GetDatabase().GetCollection<LandlordPost>("adminapproval");


                adminCollection.InsertOne(post);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending data to adminapproval collection: {ex.Message}");
                return false;
            }
        }




    }




}
