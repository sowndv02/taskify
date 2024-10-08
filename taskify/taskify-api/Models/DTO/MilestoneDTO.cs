﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace taskify_api.Models.DTO
{
    public class MilestoneDTO
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public DateTime StartAt { get; set; }
        [Required]
        public DateTime EndAt { get; set; }
        public int ProjectId { get; set; }
        public ProjectDTO? Project { get; set; }
        public string UserId { get; set; }
        public UserDTO? User { get; set; }

        [Range(0, 100)]
        public int Progress { get; set; }
        public bool Status { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
