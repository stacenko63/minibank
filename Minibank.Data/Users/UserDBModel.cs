using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Minibank.Data.Users
{
    [Table("user")]
    public class UserDBModel
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("login")]
        public string Login { get; set; }
        [Column("email")]
        public string Email { get; set; }
        
    }
}