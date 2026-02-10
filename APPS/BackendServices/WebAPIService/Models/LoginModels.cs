namespace WebApiService.Models
{
    /// <summary>
    /// Request model for user validation/login.
    /// </summary>
    public class LoginRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    /// <summary>
    /// Response model for validation result.
    /// </summary>
    public class LoginResponse
    {
        public bool IsValid { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public UserInfo? User { get; set; }
    }

    /// <summary>
    /// User information returned on successful validation.
    /// </summary>
    public class UserInfo
    {    //user id
        public string? sid { get; set; }

        //tenant id
        public string tid { get; set; }

        //login email id
        public string? unique_name { get; set; }

        //same as email id   
        public string? upn { get; set; }   

        //last name
        public string? family_name { get; set; }

        //first name
        public string? given_name { get; set; }
        
        //roles
        public string[]? scp { get; set; }
    }
}
