using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthenticationMicroService.Models
{
    public class Login
    {
        [BsonElement("userName")]
        [Required]
        public string UserName { get; set; }

        [BsonElement("password")]
        [Required]
        public string Password { get; set; }

    }
}
