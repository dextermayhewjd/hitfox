using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStealable
{
    void StealFrom(SC_CharacterController characterController, Item item);
    List<Item> CheckItems();
}

