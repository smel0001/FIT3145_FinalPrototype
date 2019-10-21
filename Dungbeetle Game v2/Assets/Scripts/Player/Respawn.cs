using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Respawn : MonoBehaviour
{
    public Vector3 respawnPos = new Vector3(0, 0, 0);

    public void checkPointTrigger(Vector3 checkPointPos)
    {
        respawnPos = checkPointPos;
    }

    public void RespawnCharacter()
    {
        //will eventually move to player move or the respawn manager for now its fine here
        transform.position = respawnPos;

        PlayerMove player = GetComponent<PlayerMove>();
        player.CurAbility.DeathReset();
        player.CurAbility.ExitAbility();
        player.CurAbility.EnterAbility();

        PlayerController controller = GetComponent<PlayerController>();
        controller.SetVelocity(Vector2.zero);
    }

    public void ToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
