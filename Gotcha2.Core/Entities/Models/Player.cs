namespace Gotcha2.Core.Entities.Models
{
    public class Player
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public Guid UserId { get; init; }
        public GotchaUser User { get; init; }
        public Guid GameId { get; init; }
        public Game Game { get; init; }
        public bool IsAlive { get; set; } = true;
        public string Notes { get; set; } = string.Empty;
        public ICollection<TargetAssignment> TargetAssignments { get; set; } = new List<TargetAssignment>();
    }
}
