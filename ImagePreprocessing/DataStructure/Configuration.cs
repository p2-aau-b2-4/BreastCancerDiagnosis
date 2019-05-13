using System;

namespace ImagePreprocessing
{
    public static class Configuration
    {
        public static string Get(string key)
        {
            string[] lines = System.IO.File.ReadAllLines("../config");
            foreach (string line in lines)
            {
                string[] substrings = line.Split('=');
                if (substrings[0].ToLower().Equals(key.ToLower()))
                {
                    if (substrings.Length != 2)
                        throw new ArgumentException("Key was configured improperly");
                    return substrings[1];
                }
            }
            throw new ArgumentException("Key was not found");
        }
    }
}