using System.ComponentModel.DataAnnotations;

namespace SharedModels.partonair_v01.DTOS
{
    /// <summary>Création ou modification de mon avis sur un membre.</summary>
    public record EvaluationUpsertDTO
        (
            [Range(0, 5, ErrorMessage = "The value must be between 0 and 5")]
            int Value,

            [Required(ErrorMessage = "The field is required")]
            [MinLength(10, ErrorMessage = "The field must contain at least 10 characters")]
            [MaxLength(500, ErrorMessage = "The field must not exceed 500 characters")]
            string Commentary
        );

    public record EvaluationViewDTO(
        Guid Id,
        int Value,
        string Commentary,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        Guid ReceiverId,
        Guid SenderId,
        string SenderName,
        string SenderRole,
        string? SenderAvatarSeed);

    /// <summary>Avis reçus par un membre, vus par l'utilisateur courant.</summary>
    public record UserEvaluationsDTO(
        Guid UserId,
        decimal Average,
        int Count,
        bool CanEvaluate,
        EvaluationViewDTO? MyEvaluation,
        ICollection<EvaluationViewDTO> Evaluations);

    public record EvaluationUpsertResultDTO(EvaluationViewDTO Evaluation, bool IsNew);
}
