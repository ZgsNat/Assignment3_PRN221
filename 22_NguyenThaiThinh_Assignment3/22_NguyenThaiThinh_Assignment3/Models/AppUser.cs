using System;
using System.Collections.Generic;

namespace _22_NguyenThaiThinh_Assignment3.Models
{
    public partial class AppUser
    {
        public AppUser()
        {
            Posts = new HashSet<Post>();
        }

        public int UserId { get; set; }
        public string? Fullname { get; set; }
        public string? Address { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

        public virtual ICollection<Post> Posts { get; set; }
    }
}
