using API.partonair_v01.Token;
using BLL.partonair_v01.MediatR.Commands.Profiles;
using BLL.partonair_v01.MediatR.Queries.Profiles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IHttpContextAccesor _httpAccesor;
        public ProfileController(IMediator mediator, IHttpContextAccesor httpAccesor)
        {
            _mediator = mediator;
            _httpAccesor = httpAccesor;
        }

        #endregion



        #region <-------------> CREATE <------------->
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(ProfileCreateDTO profileCreateDTO)
        {
            string? idUser = _httpAccesor.UserId();

            if (string.IsNullOrEmpty(idUser))
            {
                return Unauthorized("Vous n'êtes pas autorisée à faire cette action");
            }
            else
            {
                Guid GuidUser = idUser.FromStringToGuid();
                var profile = await _mediator.Send(new AddProfileCommand(GuidUser, profileCreateDTO));

                return
                    profile is null
                    ? BadRequest()
                    : Ok(new { profile });
            }
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

        [HttpGet("public/{id:guid}")]
        [ProducesResponseType(typeof(PublicProfileViewDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPublicByGuidAsync(Guid id)
        {
            Guid? viewerId = Guid.TryParse(_httpAccesor.UserId(), out var guidViewer) ? guidViewer : null;
            var profile = await _mediator.Send(new GetPublicProfileByIdQuery(id, viewerId));

            return profile is null
                ? NotFound()
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
            // L'utilisateur connecté (optionnel) permet d'appliquer sa préférence « m'afficher dans mes recherches ».
            Guid? currentUserId = Guid.TryParse(_httpAccesor.UserId(), out var guidUser) ? guidUser : null;
            var profiles = await _mediator.Send(new GetByRoleProfileQuery(role, currentUserId));

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
        [Authorize]
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync(Guid id, ProfileUpdateDTO profile)
        {
            if (!Guid.TryParse(_httpAccesor.UserId(), out Guid guidUser))
                return Unauthorized("Vous n'êtes pas autorisée à faire cette action");

            var result = await _mediator.Send(new UpdateProfileCommand(guidUser, id, profile));

            return
                result is null
                ? BadRequest()
                : Ok(result);
        }

        #endregion



        #region <-------------> ME <------------->

        /// <summary>
        /// Récupère le profil complet de l'utilisateur connecté (nom, rôle, visibilité, avatar, quota d'avatars).
        /// </summary>
        /// <response code="200">Succès - Profil retourné.</response>
        /// <response code="404">Non trouvé - L'utilisateur n'a pas encore de profil.</response>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(MyProfileViewDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMeAsync()
        {
            if (!Guid.TryParse(_httpAccesor.UserId(), out Guid guidUser))
                return Unauthorized("Vous n'êtes pas autorisée à faire cette action");

            var profile = await _mediator.Send(new GetMyProfileQuery(guidUser));
            return Ok(new { profile });
        }

        /// <summary>
        /// Met à jour le profil de l'utilisateur connecté, nom d'affichage compris.
        /// </summary>
        /// <response code="200">Succès - Profil mis à jour.</response>
        /// <response code="400">Requête invalide - Données manquantes ou invalides.</response>
        /// <response code="404">Non trouvé - L'utilisateur n'a pas encore de profil.</response>
        [Authorize]
        [HttpPut("me")]
        [ProducesResponseType(typeof(MyProfileViewDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMeAsync(MyProfileUpdateDTO profileUpdate)
        {
            if (!Guid.TryParse(_httpAccesor.UserId(), out Guid guidUser))
                return Unauthorized("Vous n'êtes pas autorisée à faire cette action");

            var profile = await _mediator.Send(new UpdateMyProfileCommand(guidUser, profileUpdate));
            return Ok(new { profile });
        }

        /// <summary>
        /// Génère un nouvel avatar DiceBear pour l'utilisateur connecté.
        /// </summary>
        /// <remarks>Limité à 10 régénérations par jour (UTC) pour ne pas surcharger l'API DiceBear.</remarks>
        /// <response code="200">Succès - Nouveau seed et nombre de régénérations restantes.</response>
        /// <response code="429">Quota journalier atteint.</response>
        [Authorize]
        [HttpPost("me/avatar/regenerate")]
        [ProducesResponseType(typeof(AvatarRegenerateResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> RegenerateAvatarAsync()
        {
            if (!Guid.TryParse(_httpAccesor.UserId(), out Guid guidUser))
                return Unauthorized("Vous n'êtes pas autorisée à faire cette action");

            var result = await _mediator.Send(new RegenerateAvatarCommand(guidUser));
            return Ok(result);
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
        /// <param name="idProfile">L'identifiant GUID du profil à supprimer.</param>
        /// <response code="204">Succès - Profil supprimé sans contenu retourné.</response>
        /// <response code="400">Requête invalide - Identifiant manquant ou invalide.</response>
        /// <response code="404">Non trouvé - Aucun profil avec cet identifiant.</response>
        /// <response code="500">Erreur interne serveur.</response>
        /// <returns>Aucun contenu.</returns>
        [HttpDelete("{idProfile:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid idProfile)
        {
            string? idUser = _httpAccesor.UserId();

            if (string.IsNullOrEmpty(idUser))
            {
                return Unauthorized("Vous n'êtes pas autorisée à faire cette action");
            }

            if (!Guid.TryParse(idUser, out Guid guidUser))
            {
                return BadRequest("L'identifiant utilisateur est invalide.");
            }

            await _mediator.Send(new DeleteProfileCommand(guidUser, idProfile));
            return NoContent();
        }

        #endregion

    }
}
