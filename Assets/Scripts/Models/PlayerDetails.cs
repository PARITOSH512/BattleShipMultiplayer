using System;
namespace Application.Modal
{
    [Serializable]
    public class PlayerDetails
    {
        public PlayerData[] allUsers;
    }

    [Serializable]
    public class PlayerData
    {
        public string team;
        public string name;
        public string type;
        public string color;
    }
}
