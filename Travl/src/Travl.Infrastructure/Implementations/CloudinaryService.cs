using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Application.Interfaces;
using Travl.Domain.Commons;

namespace Travl.Application.Implementation
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile imageFile)
        {
            try
            {
                var uploadResult = new ImageUploadResult();

                using var stream = imageFile.OpenReadStream();

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(imageFile.FileName, stream),
                    Transformation = new Transformation().Width(500).Crop("fill").Gravity("face")
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);

                return uploadResult;
            }
            catch (Exception ex) 
            {
                throw new Exception("Image upload failed " + ex.Message);
            }
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            try
            {
                if (string.IsNullOrEmpty(publicId))
                {
                    throw new ArgumentException("Public ID cannot be null or empty", nameof(publicId));
                }

                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);

                return result;
            }

            catch (Exception ex) 
            {
                throw new Exception("Image deletion failed " + ex.Message);
            }
        }

        public async Task<ImageUploadResult> UpdatePhotoAsync(IFormFile imageFile, string existingPublicId)
        {
            try
            {
                // Step 1: Delete the existing image
                if (!string.IsNullOrEmpty(existingPublicId))
                {
                    var deleteResult = await DeletePhotoAsync(existingPublicId);
                    if (deleteResult.Result != "ok")
                    {
                        throw new Exception("Failed to delete existing image before updating.");
                    }
                }

                // Step 2: Upload the new image
                return await AddPhotoAsync(imageFile);
            }
            catch (Exception ex)
            {
                throw new Exception("Image update failed: " + ex.Message);
            }
        }

    }
}
