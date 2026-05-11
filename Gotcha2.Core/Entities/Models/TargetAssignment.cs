namespace Gotcha2.Core.Entities.Models
{
    public class TargetAssignment
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public required Guid HunterId { get; init; }
        public Player? Hunter { get; init; }
        public required Guid TargetId { get; init; }
        public Player? Target { get; init; }
        public DateTime TargetAssigned { get; init; } = DateTime.UtcNow;
        public DateTime? AssignmentFinished { get; set; }
        public Kill? Kill { get; set; }
        public string? Weapon { get; init; }
        public required Guid GameId { get; init; }
        public Game? Game { get; init; }
    }
}
