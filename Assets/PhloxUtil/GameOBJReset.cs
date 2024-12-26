using UnityEngine;

public class GameOBJReset : MonoBehaviour
{
    [SerializeField] private GameObject[] managedObjects;
    
    private void Start()
    {
        EnableAll();
        
        DisableAll();
    }
    
    public void EnableAll()
    {
        foreach (GameObject obj in managedObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }

    public void DisableAll()
    {
        foreach (GameObject obj in managedObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}
