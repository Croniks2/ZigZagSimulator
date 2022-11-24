using System;

using UnityEngine;


public abstract class AbstractUIView : MonoBehaviour
{
    public abstract void ShowView(bool on, Action afterAction = null);
}