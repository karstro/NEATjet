using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Visualiser : MonoBehaviour
{
    protected float time;
    protected Creature c;
    public abstract void SayHi(Creature c);
}
