using System;
using System.Collections.Generic;

namespace _22_NguyenThaiThinh_Assignment3.Models
{
    public partial class Post
    {
        public int PostId { get; set; }
        public int? AuthorId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public bool? PublishStatus { get; set; }
        public int? CategoryId { get; set; }

        public virtual AppUser? Author { get; set; }
        public virtual PostCategory? Category { get; set; }
    }
}
