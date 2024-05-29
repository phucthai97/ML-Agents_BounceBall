using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class ReferPongPlayer : Agent
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _power = 10f;
    [SerializeField] private Rigidbody _rbBall;
    [SerializeField] private Material _mWin;
    [SerializeField] private Material _mLose;
    [SerializeField] private MeshRenderer _meshRenderer;

    public override void OnEpisodeBegin()
    {
        _rbBall.transform.localPosition = new Vector3(0, 3, 0);
        _rbBall.velocity = Vector3.zero;
        Vector3 dest = new Vector3(Random.Range(-5, 5), 20);
        Vector3 direction = (dest - _rbBall.transform.localPosition).normalized;

        _rbBall.AddForce(direction * _power, ForceMode.VelocityChange);
        //transform.localPosition = new Vector3(0, 1, 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(_rbBall.transform.localPosition.x);
        sensor.AddObservation(_rbBall.transform.localPosition.y);
        sensor.AddObservation(_rbBall.velocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var discreteActions = actions.DiscreteActions;
        int action = discreteActions[0];
        int h = 0;
        Debug.Log($"actions {action}");
        switch (action)
        {
            case 0: h = 0; break;
            case 1: h = -1; break;
            case 2: h = 1; break;
        }

        float currentX = transform.localPosition.x;
        float nextX = currentX + h * Time.deltaTime * _moveSpeed;

        nextX = Mathf.Clamp(nextX, -6f, 6f);
        transform.localPosition = new Vector3(nextX, 1, 0);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        float h = Input.GetAxisRaw("Horizontal");
        h = h == 0 ? 0 : h < 0 ? 1 : 2;
        actionsOut.DiscreteActions.Array[0] = (int)h;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"AddReward 0.01!!");
            AddReward(0.01f);
            _meshRenderer.material = _mWin;

            Vector3 dest = new Vector3(Random.Range(-5, 5), 20);
            Vector3 direction = (dest - _rbBall.transform.localPosition).normalized;

            _rbBall.AddForce(direction * _power, ForceMode.VelocityChange);

            //EndEpisode();
        }
    }

    public void Penalty()
    {
        Debug.Log($"SetReward = -1");
        SetReward(-1);
        _meshRenderer.material = _mLose;
        EndEpisode();
    }
}
