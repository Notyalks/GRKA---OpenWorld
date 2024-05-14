using UnityEngine;

public class KeypadKey : MonoBehaviour
{
    public string key;

    public void SendKey()
    {
        this.transform.GetComponentInParent<KeypadController>().PassawordEntry(key);
    }
}
