using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITouchable
{
    void TouchBegan(Touch t);

    void TouchMove(Touch t);

    void TouchEnded(Touch t);
}
