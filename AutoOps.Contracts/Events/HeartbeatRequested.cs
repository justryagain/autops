namespace AutoOps.Contracts.Events;

public record HeartbeatRequested(Guid EventId, string Target, DateTime RequestedAtUtc);