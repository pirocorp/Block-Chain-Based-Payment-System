namespace PaymentSystem.WalletApp.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;

    using Microsoft.AspNetCore.Http;

    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary cloudinary;

        public CloudinaryService(Cloudinary cloudinary)
        {
            this.cloudinary = cloudinary;
        }

        public async Task<string> Upload(IFormFile file)
        {
            byte[] destinationImage;

            await using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                destinationImage = memoryStream.ToArray();
            }

            await using (var destinationStream = new MemoryStream(destinationImage))
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, destinationStream),
                };

                var result = await this.cloudinary.UploadAsync(uploadParams);
                return result.PublicId;
            }
        }

        public async Task<IEnumerable<string>> Upload(ICollection<IFormFile> files)
        {
            var list = new List<string>();

            foreach (var file in files)
            {
                var address = await this.Upload(file);
                list.Add(address);
            }

            return list;
        }

        public string GetProfileImageAddress(string source)
        {
            return this.cloudinary.Api.UrlImgUp
                .Secure()
                .Transform(new Transformation()
                    .Height(100)
                    .Width(100)
                    .Crop("fit"))
                .BuildUrl(source);
        }
    }
}