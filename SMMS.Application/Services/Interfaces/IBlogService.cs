using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;

namespace SMMS.Application.Services.Interfaces
{
    public interface IBlogService
    {
        Task<List<BlogResponse>> GetAllBlogsAsync();
        Task<BlogResponse?> GetBlogByIdAsync(string id);
        Task<BlogResponse> CreateBlogAsync(BlogRequest request, string userId);
        Task<bool> UpdateBlogAsync(string id, BlogRequest request, string userId);
        Task<bool> DeleteBlogAsync(string id, string userId);
        Task<bool> IncrementViewAsync(string id);
        Task<List<BlogResponse>> GetBlogsByUserIdAsync(string userId);
    }
}
