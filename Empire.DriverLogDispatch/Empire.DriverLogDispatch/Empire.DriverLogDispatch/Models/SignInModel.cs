using System.ComponentModel.DataAnnotations;

namespace Empire.DriverLog.Models
{
    public class SignInModel
    {
        #region Properies


        [Required]
        public string Username { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool WillRememberUser { get; set; }
        #endregion
    }
}