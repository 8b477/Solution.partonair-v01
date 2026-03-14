using SharedModels.partonair_v01.DTOS;

using Domain.partonair_v01.Entities;


namespace BLL.partonair_v01.Mappers
{
    internal static class EvaluationMapper
    {
        internal static Evaluation ToEntity(this EvaluationCreateDTO eval, User existingOwner, User sender)
        {
            return new Evaluation
            {
                EvaluationCommentary = eval.Commentary,
                EvaluationCreatedAt = DateTime.Now,
                EvaluationUpdatedAt = null,
                EvaluationValue = (byte)eval.Value, // Not safe may be overflow here
                Receiver = existingOwner,
                Sender = sender,
            };
        }

        internal static EvaluationViewDTO ToView(this Evaluation eval)
        {
            return new EvaluationViewDTO
                (
                    eval.Id,
                    eval.EvaluationCreatedAt,
                    eval.EvaluationUpdatedAt,
                    eval.EvaluationValue,
                    eval.EvaluationCommentary,
                    eval.EvalReceiverId,
                    eval.Receiver.Name,
                    eval.EvalSenderId,
                    eval.Sender.Name
                );
        }
    }
}
