using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.Services.Interfaces;
using SMMS.Application.Helpers.Implements;
using System.Security.Claims;

namespace SMMS.API.Controllers
{
    [ApiController]
    [Route("api/blogs")]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly CloudinaryService _cloudinaryService;

        public BlogController(IBlogService blogService, CloudinaryService cloudinaryService)
        {
            _blogService = blogService;
            _cloudinaryService = cloudinaryService;
        }

        /// <summary>
        /// Get all blogs
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllBlogs()
        {
            try
            {
                var blogs = await _blogService.GetAllBlogsAsync();
                return Ok(blogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get blog by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogById(string id)
        {
            try
            {
                var blog = await _blogService.GetBlogByIdAsync(id);
                if (blog == null)
                {
                    return NotFound("Blog not found.");
                }
                return Ok(blog);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Create a new blog
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBlog([FromForm] BlogRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized("User ID not found in claims.");
                }

                if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Content))
                {
                    return BadRequest("Title and Content are required.");
                }

                var blog = await _blogService.CreateBlogAsync(request, userId);
                return CreatedAtAction(nameof(GetBlogById), new { id = blog.Id }, blog);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update an existing blog
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBlog(string id, [FromForm] BlogRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized("User ID not found in claims.");
                }

                if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Content))
                {
                    return BadRequest("Title and Content are required.");
                }

                var result = await _blogService.UpdateBlogAsync(id, request, userId);
                if (!result)
                {
                    return NotFound("Blog not found or you don't have permission to update this blog.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete a blog
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBlog(string id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized("User ID not found in claims.");
                }

                var result = await _blogService.DeleteBlogAsync(id, userId);
                if (!result)
                {
                    return NotFound("Blog not found or you don't have permission to delete this blog.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Increment view count for a blog
        /// </summary>
        [HttpPost("{id}/view")]
        public async Task<IActionResult> IncrementView(string id)
        {
            try
            {
                var result = await _blogService.IncrementViewAsync(id);
                if (!result)
                {
                    return NotFound("Blog not found.");
                }

                return Ok("View count incremented successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get blogs by user ID
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetBlogsByUserId(string userId)
        {
            try
            {
                var blogs = await _blogService.GetBlogsByUserIdAsync(userId);
                return Ok(blogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get current user's blogs
        /// </summary>
        [HttpGet("my-blogs")]
        [Authorize]
        public async Task<IActionResult> GetMyBlogs()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized("User ID not found in claims.");
                }

                var blogs = await _blogService.GetBlogsByUserIdAsync(userId);
                return Ok(blogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Upload image for blog
        /// </summary>
        [HttpPost("upload-image")]
        [Authorize]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            try
            {
                if (image == null || image.Length == 0)
                {
                    return BadRequest("No image file provided.");
                }

                var imageUrl = await _cloudinaryService.UploadImageAsync(image);
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return BadRequest("Failed to upload image.");
                }

                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
