using UnityEngine;

public class Wiggle : MonoBehaviour
{
    public enum WiggleType
    {
        TRIANGLE,
        SIN,
        SQUARE,
        SAWTOOTH,
        RANDOM
    }

    [Header("Configuration")]
    [SerializeField] private WiggleType _wiggleType = WiggleType.SIN;
    [SerializeField] private bool _startActive = true;
    [SerializeField] private bool _isContinuous = true;
    
    [Header("Wave Parameters")]
    [SerializeField] [Range(0.1f, 10f)] private float _amplitude = 1f;
    [SerializeField] [Range(0.1f, 10f)] private float _frequency = 1f;
    [SerializeField] private float _duration = 2f;

    [Header("Axes")]
    [SerializeField] private bool _wiggleX = true;
    [SerializeField] private bool _wiggleY = true;
    [SerializeField] private bool _wiggleZ = true;

    private Vector3 _originalRotation;
    private float _timer;
    private bool _isActive;

    void Start()
    {
        _originalRotation = transform.localEulerAngles;
        _isActive = _startActive;
    }

    void Update()
    {
        if (!_isActive) return;

        if (!_isContinuous)
        {
            _timer += Time.deltaTime;
            if (_timer >= _duration)
            {
                StopWiggle();
                return;
            }
        }

        ApplyWiggle();
    }

    private void ApplyWiggle()
    {
        float time = Time.time * _frequency;
        Vector3 offset = Vector3.zero;

        switch (_wiggleType)
        {
            case WiggleType.TRIANGLE:
                offset = TriangleWave(time);
                break;
            case WiggleType.SIN:
                offset = SineWave(time);
                break;
            case WiggleType.SQUARE:
                offset = SquareWave(time);
                break;
            case WiggleType.SAWTOOTH:
                offset = SawtoothWave(time);
                break;
            case WiggleType.RANDOM:
                offset = RandomWave();
                break;
        }

        ApplyRotation(offset);
    }

    private Vector3 TriangleWave(float time)
    {
        float value = Mathf.PingPong(time, 1f) * 4f - 1f;
        return new Vector3(
            _wiggleX ? value : 0f,
            _wiggleY ? value : 0f,
            _wiggleZ ? value : 0f
        ) * _amplitude;
    }

    private Vector3 SineWave(float time)
    {
        float value = Mathf.Sin(time * Mathf.PI * 2f);
        return new Vector3(
            _wiggleX ? value : 0f,
            _wiggleY ? value : 0f,
            _wiggleZ ? value : 0f
        ) * _amplitude;
    }

    private Vector3 SquareWave(float time)
    {
        float value = Mathf.Sign(Mathf.Sin(time * Mathf.PI));
        return new Vector3(
            _wiggleX ? value : 0f,
            _wiggleY ? value : 0f,
            _wiggleZ ? value : 0f
        ) * _amplitude;
    }

    private Vector3 SawtoothWave(float time)
    {
        float value = time % 1f;
        return new Vector3(
            _wiggleX ? value : 0f,
            _wiggleY ? value : 0f,
            _wiggleZ ? value : 0f
        ) * _amplitude;
    }

    private Vector3 RandomWave()
    {
        return new Vector3(
            _wiggleX ? Random.Range(-1f, 1f) : 0f,
            _wiggleY ? Random.Range(-1f, 1f) : 0f,
            _wiggleZ ? Random.Range(-1f, 1f) : 0f
        ) * _amplitude;
    }

    private void ApplyRotation(Vector3 offset)
    {
        transform.localEulerAngles = _originalRotation + offset;
    }

    public void StartWiggle()
    {
        _isActive = true;
        _timer = 0f;
    }

    public void StopWiggle()
    {
        _isActive = false;
        transform.localEulerAngles = _originalRotation;
    }

    // Editor-friendly toggle
    public void ToggleWiggle()
    {
        _isActive = !_isActive;
        if (!_isActive) transform.localEulerAngles = _originalRotation;
    }
}