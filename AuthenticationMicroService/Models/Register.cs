using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthenticationMicroService.Models
{   
    public class Register
    {
        [BsonElement("userName")]
        [Required]
        public string UserName { get; set; }

        [BsonElement("password")]
        [Required]
        public string Password { get; set; }

        [BsonElement("roleName")]
        public string RoleName { get; set; } = string.Empty;
    }
}
