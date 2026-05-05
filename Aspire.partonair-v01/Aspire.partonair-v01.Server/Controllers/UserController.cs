using BLL.partonair_v01.MediatR.Commands.Users;
using BLL.partonair_v01.MediatR.Queries.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedModels.partonair_v01.DTOS;


namespace API.partonair_v01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IMediator mediator) : ControllerBase
    {

        // <--------------------------------> TODO <-------------------------------->
        // Need review later when Authentication is setup for retrieve some data by token
        // <--------------------------------> **** <-------------------------------->



        #region DI

        private readonly IMediator _mediator = mediator;

        #endregion


        #region <-------------> CREATE <------------->

        /// <summary>
        /// Crée un nouvel utilisateur.
        /// </summary>
        /// <remarks>
        /// Cet endpoint ajoute un nouvel utilisateur à la base de données.
        /// Les données utilisateur doivent être fournies dans le corps de la requête.
        /// </remarks>
        /// <param name="u">Les données de création d'utilisateur (UserCreateDTO).</param>
        /// <response code="200">Succès - Utilisateur créé avec succès.</response>
        /// <response code="400">Requête invalide - Données manquantes ou invalides.</response>
        /// <response code="500">Erreur interne serveur.</response>
        /// <returns>Les données de l'utilisateur créé.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddAsync(UserCreateDTO u)
        {
            var user = await _mediator.Send(new AddUserCommand(u));

            return
                user is not null
                ? Ok(user)
                : BadRequest();
        }

        #endregion



        #region <-------------> GET <------------->

        /// <summary>
        /// Récupère un utilisateur par son identifiant unique.
        /// </summary>
        /// <remarks>
        /// Cet endpoint retourne les détails d'un utilisateur spécifique en fonction de son GUID.
        /// </remarks>
        /// <param name="id">L'identifiant GUID unique de l'utilisateur.</param>
        /// <response code="200">Succès - Utilisateur trouvé et retourné.</response>
        /// <response code="404">Non trouvé - Aucun utilisateur avec cet identifiant.</response>
        /// <response code="500">Erreur interne serveur.</response>
        /// <returns>Les données de l'utilisateur correspondant.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var user = await _mediator.Send(new GetByIdUserQuery(id));

            return Ok(new { user });
        }

        /// <summary>
        /// Récupère la liste complète de tous les utilisateurs.
        /// </summary>
        /// <remarks>
        /// Cet endpoint retourne tous les utilisateurs présents dans la base de données.
        /// Cette opération pourrait être restrictive en fonction du rôle utilisateur (à implémenter).
        /// </remarks>
        /// <response code="200">Succès - Liste des utilisateurs retournée.</response>
        /// <response code="500">Erreur interne serveur.</response>
        /// <returns>Une collection de tous les utilisateurs.</returns>
        //[Authorize(Roles = "Visitor")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync()
        {
            var users = await _mediator.Send(new GetAllUserQuery());

            return Ok(new { users });
        }

        /// <summary>
        /// Récupère les utilisateurs par nom.
        /// </summary>
        /// <remarks>
        /// Cet endpoint retourne tous les utilisateurs correspondant au nom fourni en paramètre.
        /// La recherche peut être partielle ou exacte selon l'implémentation.
        /// </remarks>
        /// <param name="name">Le nom (ou partie du nom) de l'utilisateur à rechercher.</param>
        /// <response code="200">Succès - Utilisateurs trouvés et retournés.</response>
        /// <response code="204">Aucun contenu - Aucun utilisateur ne correspond au nom fourni.</response>
        /// <response code="500">Erreur interne serveur.</response>
        /// <returns>Une collection d'utilisateurs correspondant au nom.</returns>
        [HttpGet("Name")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByNameAsync(string name)
        {
            var users = await _mediator.Send(new GetByNameUserQuery(name));

            return
                users is null
                ? NoContent()
                : Ok(new { users });
        }

        /// <summary>
        /// Récupère les utilisateurs par rôle.
        /// </summary>
        /// <remarks>
        /// Cet endpoint retourne tous les utilisateurs assignés à un rôle spécifique.
        /// Exemples de rôles : "Visitor", "Professionnel", "Administrateur".
        /// </remarks>
        /// <param name="role">Le rôle par lequel filtrer les utilisateurs.</param>
        /// <response code="200">Succès - Utilisateurs avec ce rôle retournés.</response>
        /// <response code="204">Aucun contenu - Aucun utilisateur avec ce rôle.</response>
        /// <response code="500">Erreur interne serveur.</response>
        /// <returns>Une collection d'utilisateurs avec le rôle spécifié.</returns>
        [HttpGet("Role")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByRoleAsync(string role)
        {
            var users = await _mediator.Send(new GetByRoleUserQuery(role));

            return
                users is null
                ? NoContent()
                : Ok(new { users });
        }

        /// <summary>
        /// Récupère un utilisateur par adresse e-mail.
        /// </summary>
        /// <remarks>
        /// Cet endpoint retourne les détails d'un utilisateur basé sur son adresse e-mail.
        /// L'adresse e-mail doit être unique dans le système.
        /// </remarks>
        /// <param name="email">L'adresse e-mail de l'utilisateur à rechercher.</param>
        /// <response code="200">Succès - Utilisateur trouvé et retourné.</response>
        /// <response code="204">Aucun contenu - Aucun utilisateur avec cet e-mail.</response>
        /// <response code="500">Erreur interne serveur.</response>
        /// <returns>Les données de l'utilisateur correspondant.</returns>
        [HttpGet("Email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByEmailAsync(string email)
        {
            var user = await _mediator.Send(new GetByEmailUserQuery(email));

            return
                user is null
                ? NoContent()
                : Ok(new { user });
        }

        #endregion



        #region <-------------> UPDATE <------------->

        /// <summary>
        /// Change le rôle d'un utilisateur (Employee ou Company).
        /// </summary>
        /// <param name="id">L'identifiant GUID de l'utilisateur.</param>
        /// <param name="newRole">Le nouveau rôle : Employee ou Company.</param>
        [HttpPatch("{id:guid}/role")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeRole(Guid id, [FromQuery] string newRole)
        {
            bool result = await _mediator.Send(new ChangeUserRoleCommand(id, newRole));

            return result ? Ok() : BadRequest("Rôle invalide ou non autorisé.");
        }

        /// <summary>
        /// Met à jour un utilisateur existant.
        /// </summary>
        /// <remarks>
        /// Cet endpoint met à jour les détails d'un utilisateur (nom, email, mot de passe).
        /// Seul le propriétaire du compte ou un administrateur peut effectuer cette opération.
        /// </remarks>
        /// <param name="id">L'identifiant GUID de l'utilisateur à mettre à jour.</param>
        /// <param name="u">Les données de mise à jour d'utilisateur (UserUpdateNameOrMailOrPasswordDTO).</param>
        /// <response code="200">Succès - Utilisateur mis à jour avec succès.</response>
        /// <response code="400">Requête invalide - Données manquantes ou invalides.</response>
        /// <response code="404">Non trouvé - Aucun utilisateur avec cet identifiant.</response>
        /// <response code="500">Erreur interne serveur.</response>
        /// <returns>Les données de l'utilisateur mis à jour.</returns>
        [HttpPatch("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, UserUpdateNameOrMailOrPasswordDTO u)
        {
            var user = await _mediator.Send(new UpdateUserCommand(id, u));

            return
                user is null
                ? BadRequest()
                : Ok(new { user });
        }

        ///// <summary>
        ///// Change le rôle d'un utilisateur.
        ///// </summary>
        ///// <remarks>
        ///// Cet endpoint change le rôle assigné à un utilisateur.
        ///// Seul un administrateur peut effectuer cette opération.
        ///// </remarks>
        ///// <param name="user">Les données de changement de rôle (UserChangeRoleDTO).</param>
        ///// <response code="200">Succès - Rôle utilisateur modifié avec succès.</response>
        ///// <response code="400">Requête invalide - Données manquantes ou invalides.</response>
        ///// <response code="403">Accès refusé - Seul un administrateur peut changer les rôles.</response>
        ///// <response code="404">Non trouvé - Utilisateur introuvable.</response>
        ///// <response code="500">Erreur interne serveur.</response>
        ///// <returns>Message de confirmation de mise à jour du rôle.</returns>
        //[HttpPut("Role")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> Role(UserChangeRoleDTO user)
        //{
        //    // Retrieve id by header (token)
        //    Guid id = new("e7f614c8-6b60-4f91-8038-017bea60ccec"); // Temporary !!

        //    await _mediator.Send(new ChangeUserRoleCommand(id, user));

        //    return Ok(new { Message = "Successful role update !" });
        //}

        #endregion



        #region <-------------> DELETE <------------->

        /// <summary>
        /// Supprime un utilisateur.
        /// </summary>
        /// <remarks>
        /// Cet endpoint supprime un utilisateur de la base de données.
        /// Seul le propriétaire du compte ou un administrateur peut effectuer cette opération.
        /// </remarks>
        /// <param name="id">L'identifiant GUID de l'utilisateur à supprimer.</param>
        /// <response code="204">Succès - Utilisateur supprimé sans contenu retourné.</response>
        /// <response code="400">Requête invalide - Identifiant manquant ou invalide.</response>
        /// <response code="404">Non trouvé - Aucun utilisateur avec cet identifiant.</response>
        /// <response code="500">Erreur interne serveur.</response>
        /// <returns>Aucun contenu.</returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteUserCommand(id));

            return NoContent();
        }

        #endregion
    }

}
