using System;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float jumpHeight = 3f;
    private Vector3 _velocity;
    private readonly float _gravity = Physics.gravity.y * 5;
    private bool _isGrounded;

    // Update is called once per frame
    void Update()
    {
        // Check if the player is grounded using a spherical overlap at the ground check position.
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Reset downward velocity if grounded and falling to ensure consistent grounding.
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
        
        // Get movement input from horizontal (A/D or Left/Right) and vertical (W/S or Up/Down) axes.
        // These needs to be put on Update method to constantly check the change of input value per frame
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        // Calculate movement direction based on local player's transform axes (right and forward).
        Vector3 move = transform.right * x + transform.forward * z;
        // Move the player using the calculated direction, speed, and frame time.
        controller.Move(move * (moveSpeed * Time.deltaTime));
        
        // Check for jump input when the player is grounded.
        // Calculate upward velocity using jump height and gravity (v = √(2 * g * h)).
        if (_isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * _gravity);
        }
        
        // Apply gravity to the player's vertical velocity over time (v = g * t).
        _velocity.y += _gravity * Time.deltaTime;
        
        // Apply vertical movement to the player using gravity-affected velocity.
        // Multiply velocity by Time.deltaTime to calculate displacement (s = v * t).
        controller.Move(_velocity * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        //Draw gizmo for Physics.CheckSphere
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        Gizmos.color = Color.green;
    }
}
