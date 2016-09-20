using System;
using System.ComponentModel.DataAnnotations;

namespace Lilybot.Authentication.API.Models
{
    public class ExternalLoginViewModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string State { get; set; }
    }

    public class RegisterApiModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        public string ExternalAccessToken { get; set; }
    }

    public class ParsedExternalAccessToken
    {
        public string user_id { get; set; }
        public string app_id { get; set; }
        public DateTimeOffset expires_at { get; set; }
    }
}