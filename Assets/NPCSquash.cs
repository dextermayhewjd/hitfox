using System.Collections;
using UnityEngine;

public class NPCSquash : MonoBehaviour
{
    private Vector3 originalScale;
    private bool isSquashed;
    public NPC_Woodcutter woodcutter;
    public float originalSpeed;

    private void Start()
    {
        originalScale = transform.localScale;
        isSquashed = false;
        originalSpeed = woodcutter.speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Forklift") && !isSquashed)
        {
            Debug.Log("start squashing ");
            // The NPC has been hit by the forklift and is not already squashed
            StartCoroutine(SquashAndRestore());
        }
    }

    private IEnumerator SquashAndRestore()
    {
        // Squash the NPC and set isSquashed to true
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.2f, transform.localScale.z);
        isSquashed = true;
        transform.GetChild(0).gameObject.SetActive(false);
        woodcutter.agent.speed *= 0.33f;
        // Wait for 3 seconds
        yield return new WaitForSeconds(5f);

        // Restore the NPC to its original size and set isSquashed to false
        transform.GetChild(0).gameObject.SetActive(true);
        transform.localScale = originalScale;
        woodcutter.agent.speed = originalSpeed;
        isSquashed = false;
    }
}


