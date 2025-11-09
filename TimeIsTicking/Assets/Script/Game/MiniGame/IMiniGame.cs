using System;
using UnityEngine;

public interface IMiniGame
{
    void Init();
    void StartMiniGame();
 
    public GimmickType GimmickType { get; }
    public Action<bool> OnMiniGameEnd { get; set; }
}
