using UnoBackend.Interfaces;

namespace UnoBackend.Models;
public class Player : IPlayer
{
    public string Name{get; set;}

    public Player(string name)
    {
        Name = name;
    }
}



