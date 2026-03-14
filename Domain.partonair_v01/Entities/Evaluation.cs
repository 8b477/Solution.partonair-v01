
using Domain.partonair_v01.Contracts;

namespace Domain.partonair_v01.Entities
{
    public class Evaluation : IEntity
    {
        public Guid Id { get; set; }

        public string EvaluationCommentary { get; set; } = null!;

        public DateTime EvaluationCreatedAt { get; set; }

        public DateTime? EvaluationUpdatedAt { get; set; }

        public byte EvaluationValue { get; set; }


        // FK
        public Guid EvalReceiverId { get; set; }

        public Guid EvalSenderId { get; set; }


        // Navigation properties
        public User Receiver { get; set; } = null!;

        public User Sender { get; set; } = null!;
    }
}
