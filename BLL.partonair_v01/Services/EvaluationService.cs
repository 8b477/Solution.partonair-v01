using SharedModels.partonair_v01.DTOS;
using BLL.partonair_v01.Interfaces;

using BLL.partonair_v01.Mappers;
using Domain.partonair_v01.Contracts;
using Domain.partonair_v01.Entities;


namespace BLL.partonair_v01.Services
{
    public class EvaluationService(IUnitOfWork UOW) : IEvaluationService
    {
        private readonly IUnitOfWork _UOW = UOW;


        #region COMMANDS
        public async Task<EvaluationViewDTO> CreateAsyncService(Guid idSender, EvaluationCreateDTO eval)
        {
            try
            {
                await _UOW.BeginTransactionAsync();

                var existingOwner = await _UOW.Users.GetByGuidAsync(eval.Id_Owner);
                var existingSender = await _UOW.Users.GetByGuidAsync(idSender);

                Evaluation entity = eval.ToEntity(existingOwner, existingSender);

                var created = await _UOW.Evaluations.CreateAsync(entity);

                await _UOW.SaveChangesAsync();
                await _UOW.CommitTransactionAsync();

                return created.ToView();
            }
            catch
            {
                await _UOW.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<EvaluationViewDTO> UpdateAsyncService(Guid idEval, EvaluationUpdateDTO eval)
        {
            try
            {
                await _UOW.BeginTransactionAsync();

                var existingEvaluation = await _UOW.Evaluations.GetByGuidAsync(idEval);

                Evaluation evalToUpdate = CompareAndUpdateValueNotNull(existingEvaluation, eval);

                var updated = await _UOW.Evaluations.Update(evalToUpdate);

                await _UOW.SaveChangesAsync();
                await _UOW.CommitTransactionAsync();

                return updated.ToView();
            }
            catch
            {
                await _UOW.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task DeleteAsyncService(Guid id)
        {
            try
            {
                await _UOW.BeginTransactionAsync();

                var existingEval = await _UOW.Evaluations.GetByGuidAsync(id);

                var ownerEvaluated = await _UOW.Users.GetByGuidAsync(existingEval.EvalReceiverId);
                var senderEvaluation = await _UOW.Users.GetByGuidAsync(existingEval.EvalSenderId);

                var evaluationOwnerToUp = ownerEvaluated.EvaluationsReceived.Select(e => e.Id == existingEval.Id);
                var evaluationSenderToUp = senderEvaluation.EvaluationsSent.Select(e => e.Id == existingEval.Id);

                ownerEvaluated.EvaluationsReceived.Remove(existingEval);
                senderEvaluation.EvaluationsSent.Remove(existingEval);

                await _UOW.Users.Update(ownerEvaluated);
                await _UOW.Users.Update(senderEvaluation);

                await _UOW.Evaluations.Delete(existingEval.Id);

                await _UOW.SaveChangesAsync();
                await _UOW.CommitTransactionAsync();

                return;
            }
            catch
            {
                await _UOW.RollbackTransactionAsync();
                throw;
            }
        }

        #endregion



        #region QUERIES

        public async Task<ICollection<EvaluationViewDTO>> GetAllGrouppByUserAsyncService()
        {
            var result = await _UOW.Evaluations.GetAllAsync();

            return result
                        .Select(e => e.ToView())
                        .ToList();
        }

        public async Task<EvaluationViewDTO> GetByGuidAsyncService(Guid id)
        {
            var result = await _UOW.Evaluations.GetByGuidAsync(id);

            return result.ToView();
        }

        #endregion



        #region PRIVATE METHODS
        private static Evaluation CompareAndUpdateValueNotNull(Evaluation existingEval, EvaluationUpdateDTO newEval)
        {
            existingEval.EvaluationCommentary = newEval.Commentary ?? existingEval.EvaluationCommentary;
            existingEval.EvaluationValue = (byte)newEval.Value; // FIX HER 
            existingEval.EvaluationUpdatedAt = DateTime.Now;

            return existingEval;
        }
        #endregion


    }
}
