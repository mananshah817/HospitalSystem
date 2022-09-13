using System.Collections.Generic;
using System.Web.Mvc;

namespace HMS.Models
{
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class Status
    {
        public string Message { get; set; }
        public IEnumerable<SelectListItem> UserList { get; set; }
    }
}