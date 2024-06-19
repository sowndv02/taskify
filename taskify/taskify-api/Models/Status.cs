﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace taskify_api.Models
{
    public class Status
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ColorId { get; set; }
        [ForeignKey(nameof(ColorId))]
        public Color Color { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }

    }
}
