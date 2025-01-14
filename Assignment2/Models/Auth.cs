using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Assignment2.Models
{
    public class Auth
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }

        public string UserEmail { get; set; } = string.Empty;
        public string PassWord { get; set; }
        public string ConfirmPassWord { get; set; }
    }
}