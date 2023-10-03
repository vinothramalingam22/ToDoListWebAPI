using MongoDB.Bson.Serialization.Attributes;

namespace AuthenticationMicroService.Models
{
    public class UserRole
    {
        [BsonElement("roleId")]
        public int RoleId { get; set; }

        [BsonElement("roleName")]
        public string RoleName { get; set; } = string.Empty;
    }
}
