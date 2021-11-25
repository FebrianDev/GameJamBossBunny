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
    public bool character3;
    public bool character4;
    public uint win;
    public uint lose;
    public uint match;
    public double winRate;

    public DataUser(string id, string name, string password, uint coin, uint score, bool character1, bool character2, bool character3, bool character4, uint win, uint lose, uint match, double winRate)
    {
        this.id = id;
        this.name = name;
        this.password = password;
        this.coin = coin;
        this.score = score;
        this.character1 = character1;
        this.character2 = character2;
        this.character3 = character3;
        this.character4 = character4;
        this.win = win;
        this.lose = lose;
        this.match = match;
        this.winRate = winRate;
    }
}