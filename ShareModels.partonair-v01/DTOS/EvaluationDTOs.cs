

using System.ComponentModel.DataAnnotations;

namespace SharedModels.partonair_v01.DTOS
{
    public record EvaluationCreateDTO
        (   
            Guid Id_Owner,

            [Required(ErrorMessage = "The field is required")]
            [MinLength(5,ErrorMessage = "The field must contain 5 characters")]
            [MaxLength(255,ErrorMessage = "The field should contain max 255 characters")]
            string Commentary,

            [Required(ErrorMessage = "The field is required")]
            [Range(1,5,ErrorMessage = "The attempt value is between 1 and 5 'integer'")]
            int Value
        );

    public record EvaluationUpdateDTO
        (
            Guid Id_Sender,

            [MaxLength(500, ErrorMessage = "The field should contain max 500 characters")]
            string? Commentary,

            [Range(0, 5, ErrorMessage = "The attempt value is between 0 and 5")]
            int? Value
        );

    public record EvaluationViewDTO(Guid Id, DateTime CreatedAt, DateTime? UpdateAt, int Value, string Commentary, Guid Id_Owner, string OwnerName, Guid Id_Sender, string SenderName);
}