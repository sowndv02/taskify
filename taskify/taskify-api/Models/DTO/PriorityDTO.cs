using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace taskify_api.Models.DTO
{
    public class PriorityDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ColorId { get; set; }
        public bool IsDefault { get; set; }
        public ColorDTO Color { get; set; }
    }
}
