using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class RouletteController : MonoBehaviour
{
    public float maxSpeed = 720f;       // 最高速度：度／秒
    public float acceleration = 360f;   // 加速度
    public float deceleration = 180f;   // 減速度

    private float speed = 0f;
    private bool spinning = false;
    private bool accelerating = false;
    public AudioClip tickSound;

    private AudioSource audioSource;
    private float accumulatedAngle = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        bool clicked = false;

#if ENABLE_INPUT_SYSTEM
        clicked =
            (Mouse.current != null &&
             Mouse.current.leftButton.wasPressedThisFrame) ||
            (Touchscreen.current != null &&
             Touchscreen.current.primaryTouch.press.wasPressedThisFrame);
#else
        clicked = Input.GetMouseButtonDown(0);
#endif

        // 停止時，點一下開始旋轉
        if (clicked && !spinning)
        {
            maxSpeed = Random.Range(600f, 1000f);
            speed = 0f;
            spinning = true;
            accelerating = true;
        }

        if (!spinning)
            return;

        if (accelerating)
        {
            speed = Mathf.MoveTowards(
                speed, maxSpeed, acceleration * Time.deltaTime);

            if (speed >= maxSpeed)
                accelerating = false;
        }
        else
        {
            speed = Mathf.MoveTowards(
                speed, 0f, deceleration * Time.deltaTime);

            if (speed <= 0f)
                spinning = false;
        }

        float angle = speed * Time.deltaTime;
        transform.Rotate(0f, 0f, -angle);

        // 每轉過 45 度，播放一次音效
        accumulatedAngle += angle;

        while (accumulatedAngle >= 45f)
        {
            accumulatedAngle -= 45f;

            if (tickSound != null)
                audioSource.PlayOneShot(tickSound);
        }
    }
}