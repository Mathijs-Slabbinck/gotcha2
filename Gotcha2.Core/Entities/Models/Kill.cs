namespace Gotcha2.Core.Entities.Models
{
    public class Kill
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public required Guid GameId { get; init; }
        public Game? Game { get; init; }
        public required Guid KillerId { get; init; }
        public Player? Killer { get; init; }
        public required Guid VictimId { get; init; }
        public Player? Victim { get; init; }
        public DateTime Moment { get; init; } = DateTime.UtcNow;
    }
}
