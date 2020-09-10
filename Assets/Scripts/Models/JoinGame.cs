using System;
namespace Application.Modal
{
    [Serializable]
    public class JoinGame
    {
        public bool status;
        public string message;
        public TeamUser[] team;
    }
}
