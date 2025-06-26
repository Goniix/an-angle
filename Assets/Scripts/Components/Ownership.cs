using UnityEngine;

public class Ownership : MonoBehaviour
{
    public int ownerId = -1;


    public bool AreInSameTeam(Ownership elem)
    {
        return ownerId != -1 && elem.ownerId == ownerId;
    }
}