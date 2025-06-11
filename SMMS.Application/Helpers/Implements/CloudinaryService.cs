using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace SMMS.Application.Helpers.Implements
{
	public class CloudinaryService
	{
		private readonly Cloudinary _cloudinary;

		public CloudinaryService(IConfiguration configuration)
		{
			var cloudName = configuration["CloudinarySettings:CloudName"];
			var apiKey = configuration["CloudinarySettings:ApiKey"];
			var apiSecret = configuration["CloudinarySettings:ApiSecret"];

			// Kiểm tra giá trị null
			if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
			{
				throw new InvalidOperationException("Cloudinary configuration is missing. Please check CloudinarySettings in appsettings.json.");
			}
			var account = new Account(cloudName, apiKey, apiSecret);
			_cloudinary = new Cloudinary(account);
		}

		public async Task<string> UploadImageAsync(IFormFile file)
		{
			if (file == null || file.Length == 0) return null;

			using (var stream = file.OpenReadStream())
			{
				var uploadParams = new ImageUploadParams()
				{
					File = new FileDescription(file.FileName, stream),
					Transformation = new Transformation().Width(500).Height(500).Crop("fill")
				};
				var uploadResult = await _cloudinary.UploadAsync(uploadParams);
				return uploadResult.SecureUrl.ToString();
			}
		}
	}
}
