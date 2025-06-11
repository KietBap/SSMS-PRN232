namespace SMMS.Application.DataObject.ResponseObject
{
    public class BlogResponse
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string? Image { get; set; }
        public string Content { get; set; }
        public string? Excerpt { get; set; }
        public int View { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset? UpdatedTime { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
