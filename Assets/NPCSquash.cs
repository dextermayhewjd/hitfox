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
        if (collision.gameObject.CompareTag("Forklift") && collision.gameObject.GetComponent<NewCarUserControl>().driving && !isSquashed)
        {
            Debug.Log("start squashing ");
            GameObject objectives = GameObject.Find("ObjectivesTracker");
            Debug.Log("5 points for squashing");
            objectives.GetComponent<ObjectivesScript>().IncreaseScore(5);
            // The NPC has been hit by the forklift and is not already squashed
            StartCoroutine(SquashAndRestore());
        }
    }

    private IEnumerator SquashAndRestore()
    {
        // Squash the NPC and set isSquashed to true
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.2f, transform.localScale.z);
        isSquashed = true;
        transform.Find("Collision").gameObject.SetActive(false);
        woodcutter.agent.speed *= 0.33f;
        // Wait for n seconds
        yield return new WaitForSeconds(5f);

        // Restore the NPC to its original size and set isSquashed to false
        transform.Find("Collision").gameObject.SetActive(true);
        transform.localScale = originalScale;
        woodcutter.agent.speed = originalSpeed;
        isSquashed = false;
    }
}


