using UnityEngine;
// [ExecuteAlways]
public class MoveInDirection : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.right; // 移动方向，默认是沿x轴正方向
    public float speed = 2f; // 移动速度
    public float moveDistance = 5f; // 移动的最大距离

    private Vector3 startPosition; // 记录物体的初始位置
    private bool movingForward = true; // 是否在向前移动

    void Start()
    {
        startPosition = transform.position; // 初始化物体的起始位置
    }

    void Update()
    {
        // 计算目标位置
        Vector3 targetPosition = startPosition + moveDirection.normalized * moveDistance;

        if (movingForward)
        {
            // 向目标方向移动
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // 如果到达目标距离，则反转方向
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                movingForward = false;
            }
        }
        else
        {
            // 向起点方向移动
            transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);

            // 如果到达起点，则再次反转方向
            if (Vector3.Distance(transform.position, startPosition) < 0.1f)
            {
                movingForward = true;
            }
        }
    }
}