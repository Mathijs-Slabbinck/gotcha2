namespace Gotcha2.Core.Entities.Models
{
    public class Game
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Name { get; set; } = "New game";
        public DateTime CreationDate { get; init; } = DateTime.UtcNow;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<Player> Players { get; set; } = new List<Player>();
        public ICollection<Kill> Kills { get; set; } = new List<Kill>();
        public bool HasStarted { get; set; } // = false;
        public bool IsFinished { get; set; } // = false;
        public Guid? WinnerId { get; set; }
        public Guid CreatorId { get; set; }
    }
}
