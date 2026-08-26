using UnoBackend.Models.Enum;

namespace UnoBackend.Interfaces;

public interface ICard
{
    Guid Id { get; }
    Color Color{get; }
    CardValue CardValue{get; }
}