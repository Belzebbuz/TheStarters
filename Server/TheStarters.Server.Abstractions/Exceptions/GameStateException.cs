namespace TheStarters.Server.Abstractions.Exceptions;

[GenerateSerializer, Immutable]
public class GameStateException(string message) : Exception(message);