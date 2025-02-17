using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travl.Application.Interfaces
{   
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile imageFile);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
        Task<ImageUploadResult> UpdatePhotoAsync(IFormFile imageFile, string existingPublicId);
    }
}
