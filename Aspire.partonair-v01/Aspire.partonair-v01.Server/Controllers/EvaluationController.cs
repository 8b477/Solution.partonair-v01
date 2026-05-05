using BLL.partonair_v01.MediatR.Commands.Evaluations;
using BLL.partonair_v01.MediatR.Queries.Evaluations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedModels.partonair_v01.DTOS;


namespace API.partonair_v01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluationController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;


        /// <summary>
        /// Récupère une évaluation par son identifiant unique.
        /// </summary>
        /// <remarks>
        /// Cet endpoint retourne les détails d'une évaluation spécifique en fonction de son GUID.
        /// </remarks>
        /// <param name="id">L'identifiant GUID unique de l'évaluation.</param>
        /// <response code="200">Succès - Évaluation trouvée et retournée.</response>
        /// <response code="204">Aucun contenu - Aucune évaluation avec cet identifiant.</response>
        /// <response code="500">Erreur interne serveur.</response>
        /// <returns>Les données de l'évaluation correspondante.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByGuidAsync(Guid id)
        {
            var evaluation = await _mediator.Send(new GetByGuidEvaluationQuery(id));

            return
                  evaluation is null
                ? NoContent()
                : Ok(new { evaluation });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var evaluations = await _mediator.Send(new GetAllEvaluationFilteredbyUserQuery());

            return
                  evaluations is null
                ? NoContent()
                : Ok(new { evaluations });
        }

        /// <summary>
        /// Crée une nouvelle évaluation.
        /// </summary>
        /// <remarks>
        /// Cet endpoint crée une nouvelle évaluation associée à l'utilisateur propriétaire spécifié.
        /// Les données d'évaluation doivent être fournies dans le corps de la requête.
        /// </remarks>
        /// <param name="idOwner">L'identifiant GUID de l'utilisateur propriétaire de l'évaluation.</param>
        /// <param name="eval">Les données de création d'évaluation (EvaluationCreateDTO).</param>
        /// <response code="200">Succès - Évaluation créée avec succès.</response>
        /// <response code="400">Requête invalide - Données manquantes ou invalides.</response>
        /// <response code="500">Erreur interne serveur.</response>
        /// <returns>Les données de l'évaluation créée.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync([FromQuery] Guid idOwner, EvaluationCreateDTO eval)
        {
            var evaluationCreated = await _mediator.Send(new AddEvaluationCommand(idOwner, eval));

            return Ok(evaluationCreated);
        }

        /// <summary>
        /// Met à jour une évaluation existante.
        /// </summary>
        /// <remarks>
        /// Cet endpoint met à jour les détails d'une évaluation existante.
        /// Seul le propriétaire de l'évaluation peut effectuer cette opération.
        /// </remarks>
        /// <param name="idEval">L'identifiant GUID de l'évaluation à mettre à jour.</param>
        /// <param name="eval">Les données de mise à jour d'évaluation (EvaluationUpdateDTO).</param>
        /// <response code="200">Succès - Évaluation mise à jour avec succès.</response>
        /// <response code="400">Requête invalide - Données manquantes ou invalides.</response>
        /// <response code="404">Non trouvé - Aucune évaluation avec cet identifiant.</response>
        /// <response code="500">Erreur interne serveur.</response>
        /// <returns>Les données de l'évaluation mise à jour.</returns>
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync(Guid idEval, EvaluationUpdateDTO eval)
        {
            var evaluationCreated = await _mediator.Send(new UpdateEvaluationCommand(idEval, eval));

            return Ok(evaluationCreated);
        }

        /// <summary>
        /// Supprime une évaluation existante.
        /// </summary>
        /// <remarks>
        /// Cet endpoint supprime une évaluation de la base de données.
        /// Seul le propriétaire de l'évaluation peut effectuer cette opération.
        /// </remarks>
        /// <param name="id">L'identifiant GUID de l'évaluation à supprimer.</param>
        /// <response code="204">Succès - Évaluation supprimée sans contenu retourné.</response>
        /// <response code="400">Requête invalide - Identifiant manquant ou invalide.</response>
        /// <response code="404">Non trouvé - Aucune évaluation avec cet identifiant.</response>
        /// <response code="500">Erreur interne serveur.</response>
        /// <returns>Aucun contenu.</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _mediator.Send(new DeleteEvaluationCommand(id));

            return NoContent();
        }
    }
}
