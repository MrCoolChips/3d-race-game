using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public Transform player; // Tu peux le laisser vide dans l'inspector
    public Vector3 offset = new Vector3(0, 50, 0);

    void LateUpdate()
    {
        // Si le champ est vide, chercher l'objet avec le tag "Player"
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
        }
        else if (player != null)
        {
            Vector3 newPos = player.position + offset;
            transform.position = new Vector3(newPos.x, newPos.y, newPos.z);
        }
    }
}