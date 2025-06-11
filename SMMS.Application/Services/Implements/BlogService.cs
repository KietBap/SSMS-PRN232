using Microsoft.EntityFrameworkCore;
using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;
using SMMS.Application.Services.Interfaces;
using SMMS.Application.Helpers.Implements;
using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;

namespace SMMS.Application.Services.Implements
{
    public class BlogService : IBlogService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly CloudinaryService _cloudinaryService;

        public BlogService(IRepositoryManager repositoryManager, CloudinaryService cloudinaryService)
        {
            _repositoryManager = repositoryManager;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<List<BlogResponse>> GetAllBlogsAsync()
        {
            var blogs = await _repositoryManager.BlogRepository.FindAll(false)
                .Include(b => b.User)
                .OrderByDescending(b => b.CreatedTime)
                .Select(b => new BlogResponse
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    UserName = b.User.FullName,
                    Title = b.Title,
                    Image = b.Image,
                    Content = b.Content,
                    Excerpt = b.Excerpt,
                    View = b.View,
                    CreatedTime = b.CreatedTime,
                    CreatedBy = b.CreatedBy,
                    UpdatedTime = b.LastUpdatedTime,
                    UpdatedBy = b.LastUpdatedBy
                })
                .ToListAsync();

            return blogs;
        }

        public async Task<BlogResponse?> GetBlogByIdAsync(string id)
        {
            var blog = await _repositoryManager.BlogRepository.FindByCondition(b => b.Id == id, false)
                .Include(b => b.User)
                .Select(b => new BlogResponse
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    UserName = b.User.FullName,
                    Title = b.Title,
                    Image = b.Image,
                    Content = b.Content,
                    Excerpt = b.Excerpt,
                    View = b.View,
                    CreatedTime = b.CreatedTime,
                    CreatedBy = b.CreatedBy,
                    UpdatedTime = b.LastUpdatedTime,
                    UpdatedBy = b.LastUpdatedBy
                })
                .FirstOrDefaultAsync();

            return blog;
        }

        public async Task<BlogResponse> CreateBlogAsync(BlogRequest request, string userId)
        {
            var user = await _repositoryManager.UserRepository.FindByCondition(u => u.Id == userId, false)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("User not found.", nameof(userId));
            }

            string? imageUrl = null;

            // Handle image upload
            if (request.ImageFile != null)
            {
                imageUrl = await _cloudinaryService.UploadImageAsync(request.ImageFile);
            }
            else if (!string.IsNullOrEmpty(request.ImageUrl))
            {
                imageUrl = request.ImageUrl;
            }

            var blog = new Blog
            {
                UserId = userId,
                Title = request.Title,
                Image = imageUrl,
                Content = request.Content,
                Excerpt = request.Excerpt,
                View = 0,
                CreatedBy = user.FullName,
                CreatedTime = DateTimeOffset.UtcNow
            };

            _repositoryManager.BlogRepository.Create(blog);
            await _repositoryManager.SaveAsync();

            return new BlogResponse
            {
                Id = blog.Id,
                UserId = blog.UserId,
                UserName = user.FullName,
                Title = blog.Title,
                Image = blog.Image,
                Content = blog.Content,
                Excerpt = blog.Excerpt,
                View = blog.View,
                CreatedTime = blog.CreatedTime,
                CreatedBy = blog.CreatedBy,
                UpdatedTime = blog.LastUpdatedTime,
                UpdatedBy = blog.LastUpdatedBy
            };
        }

        public async Task<bool> UpdateBlogAsync(string id, BlogRequest request, string userId)
        {
            var blog = await _repositoryManager.BlogRepository.FindByCondition(b => b.Id == id, true)
                .FirstOrDefaultAsync();

            if (blog == null || blog.UserId != userId)
            {
                return false;
            }

            var user = await _repositoryManager.UserRepository.FindByCondition(u => u.Id == userId, false)
                .FirstOrDefaultAsync();

            blog.Title = request.Title;
            blog.Content = request.Content;
            blog.Excerpt = request.Excerpt;

            // Handle image update
            if (request.ImageFile != null)
            {
                var imageUrl = await _cloudinaryService.UploadImageAsync(request.ImageFile);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    blog.Image = imageUrl;
                }
            }
            else if (!string.IsNullOrEmpty(request.ImageUrl))
            {
                blog.Image = request.ImageUrl;
            }

            blog.LastUpdatedBy = user?.FullName;
            blog.LastUpdatedTime = DateTimeOffset.UtcNow;

            _repositoryManager.BlogRepository.Update(blog);
            await _repositoryManager.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteBlogAsync(string id, string userId)
        {
            var blog = await _repositoryManager.BlogRepository.FindByCondition(b => b.Id == id, true)
                .FirstOrDefaultAsync();

            if (blog == null || blog.UserId != userId)
            {
                return false;
            }

            var user = await _repositoryManager.UserRepository.FindByCondition(u => u.Id == userId, false)
                .FirstOrDefaultAsync();

            blog.DeletedBy = user?.FullName;
            blog.DeletedTime = DateTimeOffset.UtcNow;

            _repositoryManager.BlogRepository.Update(blog);
            await _repositoryManager.SaveAsync();

            return true;
        }

        public async Task<bool> IncrementViewAsync(string id)
        {
            var blog = await _repositoryManager.BlogRepository.FindByCondition(b => b.Id == id, true)
                .FirstOrDefaultAsync();

            if (blog == null)
            {
                return false;
            }

            blog.View++;
            _repositoryManager.BlogRepository.Update(blog);
            await _repositoryManager.SaveAsync();

            return true;
        }

        public async Task<List<BlogResponse>> GetBlogsByUserIdAsync(string userId)
        {
            var blogs = await _repositoryManager.BlogRepository.FindByCondition(b => b.UserId == userId, false)
                .Include(b => b.User)
                .OrderByDescending(b => b.CreatedTime)
                .Select(b => new BlogResponse
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    UserName = b.User.FullName,
                    Title = b.Title,
                    Image = b.Image,
                    Content = b.Content,
                    Excerpt = b.Excerpt,
                    View = b.View,
                    CreatedTime = b.CreatedTime,
                    CreatedBy = b.CreatedBy,
                    UpdatedTime = b.LastUpdatedTime,
                    UpdatedBy = b.LastUpdatedBy
                })
                .ToListAsync();

            return blogs;
        }
    }
}
