using BCrypt.Net;

namespace GestaoChamadosAI_API.Services
{
    /// <summary>
    /// Serviço para hash e verificação de senhas usando BCrypt
    /// </summary>
    public class PasswordHashService
    {
        /// <summary>
        /// Gera hash de uma senha
        /// </summary>
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        /// <summary>
        /// Verifica se a senha corresponde ao hash
        /// </summary>
        public bool VerifyPassword(string password, string hash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }
    }
}
