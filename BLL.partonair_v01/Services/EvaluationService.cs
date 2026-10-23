using SharedModels.partonair_v01.DTOS;
using BLL.partonair_v01.Interfaces;
using BLL.partonair_v01.Mappers;
using Domain.partonair_v01.Contracts;
using Domain.partonair_v01.Entities;
using Domain.partonair_v01.Exceptions;
using Domain.partonair_v01.Exceptions.Enums;


namespace BLL.partonair_v01.Services
{
    /// <summary>
    /// Avis entre membres : un seul avis (note 0-5 + commentaire) par auteur et par membre évalué,
    /// réservé aux amis non bloqués. La moyenne est dénormalisée sur le profil évalué.
    /// </summary>
    public class EvaluationService(IUnitOfWork UOW, IContactService contactService) : IEvaluationService
    {
        private readonly IUnitOfWork _UOW = UOW;
        private readonly IContactService _contactService = contactService;


        #region COMMANDS

        public async Task<EvaluationUpsertResultDTO> UpsertAsyncService(Guid senderId, Guid receiverId, EvaluationUpsertDTO evaluation)
        {
            await EnsureCanEvaluateAsync(senderId, receiverId);

            return await _UOW.ExecuteInTransactionAsync(async () =>
            {
                var receiver = await _UOW.Users.GetByForeignKeyProfilAsync(receiverId);
                var sender = await _UOW.Users.GetByGuidAsync(senderId);
                // Profil de l'auteur suivi par EF : son avatar apparaît dans la réponse et la notification.
                await _UOW.Profiles.GetByUserIdAsync(senderId);

                var evaluations = await _UOW.Evaluations.GetByReceiverAsync(receiverId);
                var mine = evaluations.FirstOrDefault(e => e.EvalSenderId == senderId);
                var isNew = mine is null;

                if (mine is null)
                {
                    mine = new Evaluation
                    {
                        Id = Guid.NewGuid(),
                        EvalSenderId = sender.Id,
                        EvalReceiverId = receiver.Id,
                        Sender = sender,
                        Receiver = receiver,
                        EvaluationCreatedAt = DateTime.Now
                    };
                    await _UOW.Evaluations.CreateAsync(mine);
                    evaluations.Add(mine);
                }
                else
                {
                    mine.EvaluationUpdatedAt = DateTime.Now;
                }

                mine.EvaluationValue = (byte)evaluation.Value;
                mine.EvaluationCommentary = evaluation.Commentary.Trim();

                UpdateRatingStats(receiver.ProfileUser!, evaluations);

                return new EvaluationUpsertResultDTO(mine.ToView(), isNew);
            });
        }

        public async Task DeleteMineAsyncService(Guid senderId, Guid receiverId)
        {
            await _UOW.ExecuteInTransactionAsync(async () =>
            {
                var receiver = await _UOW.Users.GetByForeignKeyProfilAsync(receiverId);
                var evaluations = await _UOW.Evaluations.GetByReceiverAsync(receiverId);
                var mine = evaluations.FirstOrDefault(e => e.EvalSenderId == senderId)
                    ?? throw new ApplicationLayerException(ApplicationLayerErrorType.EntityIsNotExistingException, "Tu n’as pas encore évalué ce membre.");

                await _UOW.Evaluations.Delete(mine.Id);
                evaluations.Remove(mine);

                UpdateRatingStats(receiver.ProfileUser!, evaluations);
            });
        }

        #endregion



        #region QUERIES

        public async Task<UserEvaluationsDTO> GetUserEvaluationsAsyncService(Guid viewerId, Guid userId)
        {
            var evaluations = await _UOW.Evaluations.GetByReceiverAsync(userId);
            var canEvaluate = viewerId != userId && await _contactService.AreMutualFriendsAsyncService(viewerId, userId);
            var mine = evaluations.FirstOrDefault(e => e.EvalSenderId == viewerId);

            return new UserEvaluationsDTO(
                userId,
                Average(evaluations),
                evaluations.Count,
                canEvaluate,
                mine?.ToView(),
                evaluations.Select(e => e.ToView()).ToList());
        }

        #endregion



        #region PRIVATE METHODS

        private async Task EnsureCanEvaluateAsync(Guid senderId, Guid receiverId)
        {
            if (senderId == receiverId)
                throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, "Tu ne peux pas t’évaluer toi-même.");

            if (!await _contactService.AreMutualFriendsAsyncService(senderId, receiverId))
                throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, "Tu dois être ami (et non bloqué) avec ce membre pour l’évaluer.");
        }

        private static decimal Average(ICollection<Evaluation> evaluations) =>
            evaluations.Count == 0 ? 0m : Math.Round((decimal)evaluations.Average(e => e.EvaluationValue), 2);

        private static void UpdateRatingStats(ProfileUser profile, ICollection<Evaluation> evaluations)
        {
            profile.RatingAverage = Average(evaluations);
            profile.RatingCount = evaluations.Count;
            profile.Stars = (int)Math.Round(profile.RatingAverage, MidpointRounding.AwayFromZero);
        }

        #endregion
    }
}
