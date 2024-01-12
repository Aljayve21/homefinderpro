using homefinderpro.AdminViewModels;
using homefinderpro.LandlordViewModels;
using homefinderpro.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homefinderpro.LandlordModels
{
    public class LandlordQuery
    {
        private readonly DBConnection _dbConnection;

        public LandlordQuery(DBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<LandlordPost>> GetLandlordPostsForCurrentUser()
        {
            try
            {
                // Check if DBConnection.Instance is not null before accessing its properties
                if (DBConnection.Instance != null)
                {
                    var username = UserSession.Instance.Username;

                    // Get the current landlord based on the username
                    var landlordCollection = _dbConnection.GetLandlordsCollection();
                    var landlordFilter = Builders<Landlord>.Filter.Eq(l => l.Username, username);
                    var currentLandlord = await landlordCollection.Find(landlordFilter).FirstOrDefaultAsync();

                    if (currentLandlord != null)
                    {
                        // Get the landlord posts based on the landlord's ID
                        var landlordPostsCollection = _dbConnection.GetLandlordPostsCollection();
                        var postsFilter = Builders<LandlordPost>.Filter.Eq(lp => lp.LandlordId, currentLandlord.Id);
                        var landlordPosts = await landlordPostsCollection.Find(postsFilter).ToListAsync();

                        return landlordPosts;
                    }
                }

                return null; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving landlord posts: {ex.Message}");
                return null;
            }
        }
    }
}
