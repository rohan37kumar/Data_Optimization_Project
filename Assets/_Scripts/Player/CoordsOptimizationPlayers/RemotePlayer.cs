using UnityEngine;

public class RemotePlayer : MonoBehaviour
{
    [SerializeField] private float offsetXAxis = 52f;
    //may change acc to local scene design - current scene setup has two playgrounds with this distance

    private DODecoder decoder = new DODecoder();         //Data Optimization Decoder instance

    public void ReceiveCoordinates(byte[] data)          //simple decode data and update position
    {
        Vector3 actualPosition = decoder.Decode(data);
        transform.position = actualPosition + new Vector3(offsetXAxis, 0, 0);
        Debug.Log($"[RemotePlayer] Received: {actualPosition}");
    }
}
