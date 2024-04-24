using System;
using System.Collections.Generic;

namespace _22_NguyenThaiThinh_Assignment3.Models
{
    public partial class PostCategory
    {
        public PostCategory()
        {
            Posts = new HashSet<Post>();
        }

        public int CategoriesId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string Description { get; set; } = null!;

        public virtual ICollection<Post> Posts { get; set; }
    }
}
