using UnityEngine;

// 날짜 : 2021-08-10 PM 10:54:47
// 작성자 : Rito

namespace Rito
{
    /// <summary> 
    /// 쌓인 눈 지우기
    /// </summary>
    public class SnowEraser : MonoBehaviour
    {
        public GroundSnowPainter groundSnow;
        public float sizeMultiplier = 1f;
        public bool eraseOn = true;

        [Space, Range(1f, 10f)]
        public float moveSpeed = 5f;

        [SerializeField]
        private float currentSpeed;

        private float acceleration = 1f;
        private const float AccelMin = 1f;
        private const float AccelMax = 5f;

        private void Update()
        {
            Accelerate();
            Move();
            Erase();
        }

        /// <summary> 눈 지우기 </summary>
        private void Erase()
        {
            if (!eraseOn || groundSnow == null || groundSnow.isActiveAndEnabled == false) return;
            groundSnow.ClearSnow(transform.position, sizeMultiplier * transform.lossyScale.x);
        }

        /// <summary> LShift 가속 </summary>
        private void Accelerate()
        {
            if (Input.GetKey(KeyCode.LeftShift)) acceleration += Time.deltaTime;
            else acceleration -= Time.deltaTime;

            acceleration = Mathf.Clamp(acceleration, AccelMin, AccelMax);
        }

        /// <summary> WASD 이동 </summary>
        private void Move()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector3 moveVec = new Vector3(h, 0f, v).normalized * moveSpeed * acceleration;
            transform.Translate(moveVec * Time.deltaTime, Space.Self);

            currentSpeed = moveVec.sqrMagnitude;
            currentSpeed = (int)(currentSpeed * 100f) * 0.01f;
        }
    }
}