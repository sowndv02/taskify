using System.ComponentModel.DataAnnotations.Schema;

namespace taskify_font_end.Models.DTO
{
    public class ProjectTagDTO
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public ProjectDTO? Project { get; set; }
        public int TagId { get; set; }
        public TagDTO? Tag { get; set; }
    }
}
