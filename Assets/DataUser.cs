using System;
using UnityEngine;

[Serializable]
public struct DataUser
{
    public string id;
    public string name;
    public string password;
    public uint coin;
    public uint score;
    public bool character1;
    public bool character2;

    public DataUser(string id, string name, string pwd, uint coin, uint score, bool character1,
        bool character2)
    {
        this.id = id;
        this.name = name;
        this.password = pwd;
        this.coin = coin;
        this.score = score;
        this.character1 = character1;
        this.character2 = character2;
    }
}