using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public Card(MinionType _minionType, int _mana)
    {
        this.minionType = _minionType;
        this.mana = _mana;
    }

    public MinionType minionType;
    public int mana;
}
