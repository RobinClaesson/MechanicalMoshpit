using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class RobotFlags : NetworkBehaviour
{
    MultiplayerWorldParse worldScript;
    RobotCollision collisionScript;
    RobotRoundsHandler roundsScript;

    public Slider flagSlider;

    NetworkVariable<int> flagCount = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        worldScript = GameObject.Find("Load World Multiplayer").GetComponent<MultiplayerWorldParse>();
        collisionScript = GetComponent<RobotCollision>();
        roundsScript = GetComponent<RobotRoundsHandler>();

        flagCount.OnValueChanged += FlagCountChange;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FlagCountChange(int oldInt, int newInt)
    {
        flagSlider.value = newInt;
    }

    public void CaptureFlag()
    {
        //num of flags ++
        //Check victory
        if (collisionScript.onFlagTile)
        {
            IncreaseFlagCountServerRpc();
            MoveFlagServerRPC();

            if (flagCount.Value == flagSlider.maxValue)
            {
                roundsScript.SetGameStateForAllServerRpc(GameState.GameOver);
            }
        }
    }

    public void LoseFlag()
    {
        DecreaseFlagCountServerRpc();
    }

    [ServerRpc]
    private void IncreaseFlagCountServerRpc()
    {
        if (flagCount.Value < flagSlider.maxValue)
            flagCount.Value++;

    }

    [ServerRpc]
    private void DecreaseFlagCountServerRpc()
    {
        if (flagCount.Value > 0)
            flagCount.Value--;
    }

    [ServerRpc]
    private void MoveFlagServerRPC()
    {
        worldScript.RandomFlagPosition();
        Vector3 newFlagPos = worldScript.GetFlagPosition();
        SetFlagPositionClientRpc(newFlagPos);
    }

    [ClientRpc]
    private void SetFlagPositionClientRpc(Vector3 newFlagPos)
    {
        worldScript.SetFlagPosition(newFlagPos);
    }

}