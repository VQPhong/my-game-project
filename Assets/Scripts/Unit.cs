using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private Animator unitAnimator;

    private Vector3 targetPosition;

    private void Awake()
    {
        // Tạm thời để object giữ nguyên vị trí được đặt
        // Set vị trí start bằng với vị trí hiện tại
        targetPosition = transform.position;
    }

    private void Update()
    {

        float stoppingDistance = .1f;
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            float moveSpeed = 4f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            float rotateSpeed = 10f;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

            unitAnimator.SetBool("IsWalking", true);
        }
        else
        {
            unitAnimator.SetBool("IsWalking", false);
        }
    }

    public void Move(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

}
