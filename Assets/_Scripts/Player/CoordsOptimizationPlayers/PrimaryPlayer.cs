using UnityEngine;

public class PrimaryPlayer : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 20f;

    public RemotePlayer remotePlayer;
    private DOEncoder encoder = new DOEncoder();      //Data Optimization Encoder instance

    void Start()
    {
        //initially send the starting position
        byte[] initialData = encoder.Encode(transform.position);
        remotePlayer.ReceiveCoordinates(initialData);
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            Vector3 movement = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
            transform.position += movement;

            EncodeAndSendPosition();
        }
    }

    private void EncodeAndSendPosition()
    {
        byte[] encodedData = encoder.Encode(transform.position);
        Debug.Log($"[PrimaryPlayer] Sending: {transform.position} | Size: {DO.GetSizeInBits(encodedData)} bits");
        remotePlayer.ReceiveCoordinates(encodedData);
    }
}
