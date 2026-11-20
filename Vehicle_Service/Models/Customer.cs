using System.ComponentModel.DataAnnotations;

namespace Vehicle_Service.Models
{
    public class Customer
    {
        //property class
            public int Cid { set; get; }

            [Required(ErrorMessage = "Please Enter your Name")]
            public string? Cname { set; get; }

            [Required(ErrorMessage = "Address is Required")]
            public string? Caddr { set; get; }

            [Required(ErrorMessage = "Enter a valid Phone Number")]
            [RegularExpression(@"^(\d{10})$", ErrorMessage = "Enter valid no:")]
            public string? Cphone { set; get; }

            [EmailAddress(ErrorMessage = "Enter a valid Email Id")]
            public string? Email { set; get; }
        
            [Required(ErrorMessage = "Registration Date is required")]
            public DateTime Date { set; get; }
            public string? status { set; get; }
         
           [Required(ErrorMessage = "Username is required")]
            public string? una { get; set; }

            [Required(ErrorMessage = "Password is required")]
            public string? pw { get; set; }

            [Compare("pw", ErrorMessage = "Password do not match")]
            public string? cpw { get; set; }
            public string? customer_msg { get; set; }

    }
    public class LoginResult
    {
        public int RegId { get; set; }

        public string? Role { get; set; }

        public bool IsValid { get; set; }
    }
    public class Login
    {
        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? msg { get; set; }
    }
    //public class CustomerProfile
    //    {
    //        public int Cid { set; get; }
    //        public string? Cname { set; get; }
    //        public string? Caddr { set; get; }
    //        public string? Cphone { set; get; }

    //    }
}

