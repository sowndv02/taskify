
namespace taskify_api.Models.DTO
{
    public class ProjectTagDTO
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int TagId { get; set; }
        public ProjectDTO? Project { get; set; }
        public TagDTO? Tag { get; set; }
    }
}
