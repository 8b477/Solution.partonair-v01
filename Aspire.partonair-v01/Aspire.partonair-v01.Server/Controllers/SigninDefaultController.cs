using API.partonair_v01.Token;
using BLL.partonair_v01.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedModels.partonair_v01.DTOS;

namespace API.partonair_v01.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class SigninDefaultController(JWTService tokenService, IUserService userService) : ControllerBase
    {

        private readonly JWTService _tokenService = tokenService;
        private readonly IUserService _userService = userService;


        /// <summary>
        /// Authentifie un utilisateur avec ses identifiants par défaut.
        /// </summary>
        /// <remarks>
        /// Cet endpoint valide l'email et le mot de passe fournis et retourne un JWT token si l'authentification réussit.
        /// Le token peut être utilisé pour les requêtes authentifiées ultérieures.
        /// </remarks>
        /// <param name="log">Les identifiants de connexion (UserLoginDTO) contenant email et mot de passe.</param>
        /// <response code="200">Succès - Utilisateur authentifié, JWT token retourné.</response>
        /// <response code="400">Requête invalide - Identifiants incorrects ou données manquantes.</response>
        /// <response code="500">Erreur interne serveur.</response>
        /// <returns>Un objet LoginDefaultResponse contenant le JWT token.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginDefaultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login(UserLoginDTO log)
        {
            var result = await _userService.LogTest(log.Email, log.Password);

            if (result is not null)
            {
                string token = _tokenService.GenerateToken(result.Name, result.Role.ToString());
                return Ok(new LoginDefaultResponse(token));
            }
            return BadRequest("Les identifiants ne sont pas corrects");
        }
    }
}
