using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homefinderpro.LandlordModels;
using homefinderpro.Models;
using MongoDB.Bson;

namespace homefinderpro.LandlordViewModels
{
    public class LandlordPostViewModel : BindableObject
    {
        private ObservableCollection<byte[]> _photos = new ObservableCollection<byte[]>();
        public ObservableCollection<byte[]> Photos
        {
            get => _photos;
            set
            {
                _photos = value;
                OnPropertyChanged();
            }
        }

        private byte[] _validIdPicture;
        public byte[] ValidIdPicture
        {
            get => _validIdPicture;
            set
            {
                _validIdPicture = value;
                OnPropertyChanged();
            }
        }

        private byte[] _governmentDocument;
        public byte[] GovernmentDocument
        {
            get => _governmentDocument;
            set
            {
                _governmentDocument = value;
                OnPropertyChanged();
            }
        }

        private string _selectedCategory;
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private string _location;
        public string Location
        {
            get => _location;
            set
            {
                _location = value;
                OnPropertyChanged();
            }
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged();
            }
        }

        private ObjectId _id;
        public ObjectId Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public string LandlordUsername { get; set; }
        public string LandlordProfilePicture { get; set; }

        private ImageSource _selectedPhotoSource;
        public ImageSource SelectedPhotoSource
        {
            get { return _selectedPhotoSource; }
            set
            {
                _selectedPhotoSource = value;
                OnPropertyChanged(nameof(SelectedPhotoSource));
            }
        }

        public ObservableCollection<string> Categories { get; set; } = new ObservableCollection<string>
        {
            "2 people", "4 people", "Studio Type"
        };

        public void AddPhoto()
        {
            var selectedPhoto = OpenPhotoPicker();
            if (selectedPhoto != null && _photos.Count < 4)
            {
                _photos.Add(selectedPhoto);
                OnPropertyChanged(nameof(Photos));
            }
        }

        private byte[] OpenPhotoPicker()
        {
            byte[] placeholderPhotoData = new byte[] { 0x01, 0x02, 0x03 };

            return placeholderPhotoData;
        }

        public async void UploadValidId()
        {
            var selectedFileData = await OpenPhotoPickerAsync();

            if (selectedFileData != null)
            {
                _validIdPicture = await ProcessSelectedFileAsync(selectedFileData);
                OnPropertyChanged(nameof(ValidIdPicture));
            }
        }

        public async Task<byte[]> OpenFilePickerAsync()
        {
            var options = new PickOptions
            {
                FileTypes = FilePickerFileType.Pdf,
                PickerTitle = "Select a PDF file"
            };

            var pickedFile = await FilePicker.PickAsync(options);

            if (pickedFile != null)
            {
                return await ProcessSelectedFileAsync(pickedFile);
            }

            return null;
        }

        private async Task<byte[]> ProcessSelectedFileAsync(FileResult file)
        {
            byte[] fileData = null;

            if (file != null)
            {
                using (var stream = await file.OpenReadAsync())
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    fileData = memoryStream.ToArray();
                }
            }

            return fileData;
        }

        private byte[] ReadFileAsByteArray(string filePath)
        {
            return new byte[] { 0x01, 0x02, 0x03 };
        }

        public async void UploadGovernmentDocument()
        {
            var selectedFileData = await OpenFilePickerAsync();

            if (selectedFileData != null)
            {
                _governmentDocument = selectedFileData;
                OnPropertyChanged(nameof(GovernmentDocument));
            }
        }

        public async Task<string> PickLocationAsync()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
                var placemark = (await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude)).FirstOrDefault();

                if (placemark != null)
                {
                    return $"{placemark.Locality}, {placemark.AdminArea}, {placemark.CountryName}";
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Location: {ex.Message}");
            }
            return string.Empty;
        }

        public async Task SubmitPost()
        {
            var landlord = new Landlord(new DBConnection());

            string category = SelectedCategory;
            string description = Description;
            string location = Location;
            decimal price = Price;
            List<byte[]> photos = new List<byte[]>(Photos);
            byte[] validIdPicture = ValidIdPicture;
            byte[] governmentDocument = GovernmentDocument;


            var landlordPost = new LandlordPost
            {

                Category = category,
                Description = description,
                Location = location,
                Price = price,
                Photos = photos,
                ValidIdPicture = validIdPicture,
                GovernmentDocument = governmentDocument
            };


            bool submissionResult = landlord.SubmitPost(category, description, location, price, photos, validIdPicture, governmentDocument);

            if (submissionResult)
            {

                MessagingCenter.Send(this, "SubmissionSuccess");


                landlord.SendToAdminApproval(landlordPost);
            }
            else
            {

                MessagingCenter.Send(this, "SubmissionError");
            }
        }

        public async Task OnAddPhotoClicked()
        {
            var photo = await OpenPhotoPickerAsync();
            if(photo != null)
            {
                byte[] photoData = await ProcessSelectedPhotoAsync(photo);
            }
        }

        private async Task<FileResult> OpenPhotoPickerAsync()
        {
            var options = new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Select a photo"
            };

            return await FilePicker.PickAsync(options);
        }

        private async Task<byte[]> ProcessSelectedPhotoAsync(FileResult photo)
        {
            byte[] photoData = null;

            if (photo != null)
            {
                using (var stream = await photo.OpenReadAsync())
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    photoData = memoryStream.ToArray();
                }
            }

            return photoData;
        }

        public async Task OnUploadValidIdClicked()
        {
            var validId = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Select a valid ID picture"
            });

            if (validId != null)
            {
                byte[] validIdData = await ProcessSelectedDocumentAsync(validId);
                ValidIdPicture = validIdData;
                OnPropertyChanged(nameof(ValidIdPicture));
            }
        }

        public LandlordPostViewModel(LandlordPost landlordPost)
        {

            Id = landlordPost.Id;
            Description = landlordPost.Description;
            Location = landlordPost.Location;
            Price = landlordPost.Price;

        }

        public async Task<byte[]> ProcessSelectedValidIdAsync(FileResult validId)
        {
            byte[] validIdData = null;

            if (validId != null)
            {
                using (var stream = await validId.OpenReadAsync())
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    validIdData = memoryStream.ToArray();
                }
            }

            return validIdData;
        }

        public async Task OnUploadGovernmentDocumentClicked()
        {
            var document = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Select a government document"
            });

            if (document != null)
            {
                byte[] documentData = await ProcessSelectedDocumentAsync(document);
                GovernmentDocument = documentData;
                OnPropertyChanged(nameof(GovernmentDocument));
            }
        }

        private async Task<byte[]> ProcessSelectedDocumentAsync(FileResult document)
        {
            byte[] documentData = null;

            if (document != null)
            {
                using (var stream = await document.OpenReadAsync())
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    documentData = memoryStream.ToArray();
                }
            }

            return documentData;
        }






    }
}
