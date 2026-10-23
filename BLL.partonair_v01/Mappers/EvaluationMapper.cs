using SharedModels.partonair_v01.DTOS;

using Domain.partonair_v01.Entities;


namespace BLL.partonair_v01.Mappers
{
    internal static class EvaluationMapper
    {
        internal static EvaluationViewDTO ToView(this Evaluation eval)
        {
            return new EvaluationViewDTO(
                eval.Id,
                eval.EvaluationValue,
                eval.EvaluationCommentary,
                eval.EvaluationCreatedAt,
                eval.EvaluationUpdatedAt,
                eval.EvalReceiverId,
                eval.EvalSenderId,
                eval.Sender.Name,
                eval.Sender.Role.ToString(),
                eval.Sender.ProfileUser?.AvatarSeed);
        }
    }
}
