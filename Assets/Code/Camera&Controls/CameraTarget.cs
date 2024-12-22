using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    //This is bad, but I can live with that
    [SerializeField] private HighlightArea _highlightLeft;
    [SerializeField] private HighlightArea _highlightRight;
    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(x, y, 0);
        Move(direction);
    }

    public void Move(Vector3 direction)
    {
        transform.position += direction * speed * Time.deltaTime;

        _highlightLeft.OnCameraMove();
        _highlightRight.OnCameraMove();
    }
}
