using UnoBackend.Models.Enum;

namespace UnoBackend.Interfaces;

public interface ICard
{
    Color Color{get; }
    CardValue CardValue{get; }
}