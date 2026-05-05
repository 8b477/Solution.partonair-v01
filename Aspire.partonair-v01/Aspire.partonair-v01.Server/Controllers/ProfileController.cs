using BLL.partonair_v01.MediatR.Commands.Profiles;
using BLL.partonair_v01.MediatR.Queries.Profiles;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedModels.partonair_v01.DTOS;


namespace API.partonair_v01.Controllers
{ 
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {


        // <--------------------------------> TODO <-------------------------------->
        // 
        // <--------------------------------> **** <-------------------------------->



        #region DI

        private readonly IMediator _mediator;
        public ProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #endregion



        #region <-------------> CREATE <------------->

        [HttpPost]
        public async Task<IActionResult> CreateAsync(Guid idUser, ProfileCreateDTO profileCreateDTO)
        {
            var profile = await _mediator.Send(new AddProfileCommand(idUser, profileCreateDTO));

            return
                profile is null
                ? BadRequest()
                : Ok(new { profile });
        }

        #endregion



        #region <-------------> GET <------------->

        /// <summary>
        /// Récupère un profil utilisateur par son identifiant unique.
        /// </summary>
        /// <remarks>
        /// Cet endpoint retourne les détails d'un profil spécifique en fonction de son GUID.
        /// </remarks>
        /// <param name="id">L'identifiant GUID unique du profil.</param>
        /// <response code="200">Succès - Profil trouvé et retourné.</response>
        /// <response code="204">Aucun contenu - Aucun profil avec cet identifiant.</response>
        /// <response code="500">Erreur interne serveur.</response>
        /// <returns>Les données du profil correspondant.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByGuidAsync(Guid id)
        {
            var profile = await _mediator.Send(new GetByIdProfileQuery(id));

            return
                profile is null
                ? NoContent()
                : Ok(new { profile });
        }


        /// <summary>
        /// Récupère les profils utilisateurs par rôle.
        /// </summary>
        /// <remarks>
        /// Cet endpoint retourne tous les profils assignés à un rôle spécifique.
        /// Exemples de rôles : "Visitor", "Professionnel", "Administrateur".
        /// </remarks>
        /// <param name="role">Le rôle par lequel filtrer les profils.</param>
        /// <response code="200">Succès - Profils avec ce rôle retournés.</response>
        /// <response code="204">Aucun contenu - Aucun profil avec ce rôle.</response>
        /// <response code="500">Erreur interne serveur.</response>
        /// <returns>Une collection de profils avec le rôle spécifié.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByRoleAsync(string role)
        {
            var profiles = await _mediator.Send(new GetByRoleProfileQuery(role));

            return
                profiles is null
                ? NoContent()
                : Ok(new { profiles });
        }

        #endregion



        #region <-------------> UPDATE <------------->

        /// <summary>
        /// Met à jour un profil utilisateur existant.
        /// </summary>
        /// <remarks>
        /// Cet endpoint met à jour les détails d'un profil existant.
        /// Seul le propriétaire du profil peut effectuer cette opération.
        /// </remarks>
        /// <param name="id">L'identifiant GUID du profil à mettre à jour.</param>
        /// <param name="profile">Les données de mise à jour de profil (ProfileUpdateDTO).</param>
        /// <response code="200">Succès - Profil mis à jour avec succès.</response>
        /// <response code="400">Requête invalide - Données manquantes ou invalides.</response>
        /// <response code="404">Non trouvé - Aucun profil avec cet identifiant.</response>
        /// <response code="500">Erreur interne serveur.</response>
        /// <returns>Les données du profil mis à jour.</returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync(Guid id, ProfileUpdateDTO profile)
        {
            var result = await _mediator.Send(new UpdateProfileCommand(id, profile));

            return
                result is null
                ? BadRequest()
                : Ok(result);
        }

        #endregion



        #region <-------------> DELETE <------------->

        /// <summary>
        /// Supprime un profil utilisateur.
        /// </summary>
        /// <remarks>
        /// Cet endpoint supprime un profil de la base de données.
        /// Seul le propriétaire du profil ou un administrateur peut effectuer cette opération.
        /// </remarks>
        /// <param name="id">L'identifiant GUID du profil à supprimer.</param>
        /// <response code="204">Succès - Profil supprimé sans contenu retourné.</response>
        /// <response code="400">Requête invalide - Identifiant manquant ou invalide.</response>
        /// <response code="404">Non trouvé - Aucun profil avec cet identifiant.</response>
        /// <response code="500">Erreur interne serveur.</response>
        /// <returns>Aucun contenu.</returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _mediator.Send(new DeleteProfileCommand(id));
            return NoContent();
        }

        #endregion

    }
}
