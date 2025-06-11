using Microsoft.AspNetCore.Http;

namespace SMMS.Application.DataObject.RequestObject
{
    public class BlogRequest
    {
        public string Title { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? ImageUrl { get; set; } // For cases where image URL is provided directly
        public string Content { get; set; }
        public string? Excerpt { get; set; }
    }
}
