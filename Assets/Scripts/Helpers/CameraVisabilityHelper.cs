using System.Collections;
using UnityEngine;


public class CameraVisabilityHelper : MonoBehaviour
{
    private Vector3 _leftBottomStart;
    private Vector3 _leftBottomEnd;
    private Vector3 _leftTopStart;
    private Vector3 _leftTopEnd;
    private Vector3 _rightTopStart;
    private Vector3 _rightTopEnd;
    private Vector3 _rightBottomStart;
    private Vector3 _rightBottomEnd;

    private Coroutine _coroutine;


    private void Start()
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 endPointDirection = cameraForward * 9999f;

        _leftBottomStart = Camera.main.ViewportToWorldPoint(Vector3.zero);
        _leftBottomEnd = endPointDirection + _leftBottomStart;

        _leftTopStart = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, 0f));
        _leftTopEnd = endPointDirection + _leftTopStart;

        _rightTopStart = Camera.main.ViewportToWorldPoint(Vector3.one);
        _rightTopEnd = endPointDirection + _rightTopStart;

        _rightBottomStart = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0f, 0f));
        _rightBottomEnd = endPointDirection + _rightBottomStart;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (_coroutine == null)
                _coroutine = StartCoroutine(VisualiseBounds());
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }
    }

    private IEnumerator VisualiseBounds()
    {
        while (true)
        {
            yield return null;

            Debug.DrawLine(_leftTopStart,     _leftTopEnd,     Color.red);
            Debug.DrawLine(_leftBottomStart,  _leftBottomEnd,  Color.red);
            Debug.DrawLine(_rightTopStart,    _rightTopEnd,    Color.red);
            Debug.DrawLine(_rightBottomStart, _rightBottomEnd, Color.red);
        }
    }
}
