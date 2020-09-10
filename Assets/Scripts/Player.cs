using System;
using UnityEngine;

namespace Application
{
    public class Player
    {
        private const string UserNameKey = "PlayerName";

        public void SaveUserName(string userName)
        {
            PlayerPrefs.SetString(UserNameKey, userName);
        }

        public string GetUserName()
        {
            return PlayerPrefs.GetString(UserNameKey, "");
        }
    }
}
