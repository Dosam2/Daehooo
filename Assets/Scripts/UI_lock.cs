using UnityEngine;

public class Ui_lock : MonoBehaviour{
    private Transform shipTransform;
    private Canvas canvas;

    void Start()
    {
        // Ship 프리팹 Transform 찾기
        shipTransform = transform.parent;
        canvas = GetComponent<Canvas>();
    }

    void LateUpdate()
    {
        if (shipTransform != null)
        {
            // Ship의 Y축 회전값을 Canvas의 Z축 회전에 적용
            float yRotation = shipTransform.eulerAngles.y;
            transform.localRotation = Quaternion.Euler(90, 0, yRotation);
        }
    }
}