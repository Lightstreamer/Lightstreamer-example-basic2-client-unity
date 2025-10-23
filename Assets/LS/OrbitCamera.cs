using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public Transform target;        // oggetto attorno al quale orbitare
    public float distance = 10f;    // distanza iniziale dal target
    public float zoomSpeed = 5f;    
    public float minDistance = 2f;  
    public float maxDistance = 20f;

    public float xSpeed = 120f;     // sensibilità orizzontale
    public float ySpeed = 120f;     // sensibilità verticale
    public float yMinLimit = -20f;  // limite verticale in basso
    public float yMaxLimit = 80f;   // limite verticale in alto

    private float x = 0f;
    private float y = 0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }

    void LateUpdate()
    {
        if (target)
        {
            x += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
            y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;

            y = Mathf.Clamp(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0f);

            distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            Vector3 negDistance = new Vector3(0f, 0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }
}

