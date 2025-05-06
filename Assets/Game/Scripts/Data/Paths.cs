using UnityEngine;

namespace Game.Scripts.Data
{
    public static class Paths
    {
        public static readonly string Inputs = Application.persistentDataPath + "/controls.txt";
        public static readonly string Options = Application.persistentDataPath + "/config.txt";
    }
}