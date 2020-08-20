using System;
using UnityEngine;



public interface IState<T>
{
    void EnterState(T t);

    void ExitState();
}
