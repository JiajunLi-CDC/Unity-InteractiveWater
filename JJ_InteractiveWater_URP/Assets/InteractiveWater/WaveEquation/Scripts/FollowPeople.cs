using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPeople : MonoBehaviour
{
    public GameObject Human;

    void Update()
    {
        // 获取当前物体的位置
        Vector3 newPosition = this.transform.position;

        // 更新x和z的值为Human的x和z
        newPosition.x = Human.transform.position.x;
        newPosition.z = Human.transform.position.z;

        // 将更新后的位置赋值回去
        this.transform.position = newPosition;
    }
}