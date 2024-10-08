﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using taskify_font_end.Validation;

namespace taskify_font_end.Models.DTO
{
    public class TaskDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int StatusId { get; set; }
        public StatusDTO? Status { get; set; }
        public string OwnerId { get; set; }
        public UserDTO? Owner { get; set; }
        public int ProjectId { get; set; }
        public ProjectDTO? Project { get; set; }
        [DateRange("EndAt", ErrorMessage = "StartAt must be earlier than EndAt")]
        public DateTime StartAt { get; set; } = DateTime.Now;
        public DateTime EndAt { get; set; } = DateTime.Now;
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<TaskUserDTO>? TaskUsers { get; set; } = new List<TaskUserDTO>();
        public List<string>? TaskUserIds { get; set; } = new List<string>();
    }
}
