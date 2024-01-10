using homefinderpro.AdminModels;
using homefinderpro.LandlordModels;
using homefinderpro.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homefinderpro.AdminViewModels
{
    public class AdminApprovalViewModel :INotifyPropertyChanged
    {
        private string _username;
        private byte[] _ProfilePicture;

        public ObjectId Id { get; set; }
        public ObjectId LandlordId { get; set; }
        public ObjectId LandlordPostId { get; set; }
        public string ApprovalStatus { get; set; }

        public string LandlordUsername
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged(nameof(LandlordUsername));
                }
            }
        }

        public byte[] LandlordProfilePicture
        {
            get => _ProfilePicture;
            set
            {
                if (_ProfilePicture != value)
                {
                    _ProfilePicture = value;
                    OnPropertyChanged(nameof(LandlordProfilePicture));
                }
            }
        }

        

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Dictionary<ObjectId, string> _landlordPostApprovalStatus = new Dictionary<ObjectId, string>();

        public void UpdateLandlordPostApprovalStatus(ObjectId landlordPostId, string approvalStatus)
        {
            
            _landlordPostApprovalStatus[landlordPostId] = approvalStatus;
        }

        public string GetLandlordPostApprovalStatus(ObjectId landlordPostId)
        {
            
            if (_landlordPostApprovalStatus.TryGetValue(landlordPostId, out string approvalStatus))
            {
                return approvalStatus;
            }

            
            return "NotProcessed";
        }

        public ObservableCollection<AdminApprovalViewModel> AdminApprovalList { get; set; }


        private readonly IMongoCollection<AdminApproval> _adminApprovalCollection;
        private readonly IMongoCollection<LandlordPost> _landlordPostCollection;

        public AdminApprovalViewModel()
        {
            // Check if DBConnection.Instance is not null before accessing its properties
            if (DBConnection.Instance != null)
            {
                _adminApprovalCollection = DBConnection.Instance.GetAdminApprovalCollection();
                _landlordPostCollection = DBConnection.Instance.GetLandlordPostsCollection();
            }
            else
            {
                // Handle the case where DBConnection.Instance is null (throw an exception or log an error)
                throw new InvalidOperationException("DBConnection.Instance is null");
            }
        }

        public async Task<List<LandlordPost>> GetLandlordPostsForApproval()
        {
            // Check if collections are not null before using them
            if (_adminApprovalCollection == null || _landlordPostCollection == null)
            {
                // Handle the case where collections are null
                throw new InvalidOperationException("Collections are null");
            }

            var adminApprovals = await _adminApprovalCollection.Find(Builders<AdminApproval>.Filter.Eq(a => a.ApprovalStatus, "Pending")).ToListAsync();

            var landlordPostIds = adminApprovals.Select(a => a.LandlordPostId).ToList();

            var landlordPosts = await _landlordPostCollection.Find(Builders<LandlordPost>.Filter.In(lp => lp.Id, landlordPostIds)).ToListAsync();

            return landlordPosts;
        }

        public async Task ApproveLandlordPost(ObjectId landlordPostId)
        {
            await UpdateApprovalStatus(landlordPostId, "Approved");
        }

        public async Task RejectLandlordPost(ObjectId landlordPostId)
        {
            await UpdateApprovalStatus(landlordPostId, "Rejected");
        }

        private async Task UpdateApprovalStatus(ObjectId landlordPostId, string approvalStatus)
        {
           
            if (_adminApprovalCollection == null)
            {
                
                throw new InvalidOperationException("_adminApprovalCollection is null");
            }

            var filter = Builders<AdminApproval>.Filter.Eq(a => a.LandlordPostId, landlordPostId);
            var update = Builders<AdminApproval>.Update.Set(a => a.ApprovalStatus, approvalStatus);
            await _adminApprovalCollection.UpdateOneAsync(filter, update);
        }


    }


}

