using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PongPlayer : Agent
{
    private float _moveSpeed = 100f;
    private float _power = 400f;
    [SerializeField] private Rigidbody _rbBall;
    [SerializeField] private Material _mWin;
    [SerializeField] private Material _mLose;
    [SerializeField] private MeshRenderer _meshRenderer;

    public override void OnEpisodeBegin()
    {
        _rbBall.transform.localPosition = new Vector3(0, 30, 0);
        Vector3 destination = new Vector3(Random.Range(-47f, 47f), 120, 0);
        Vector3 direction = (destination - _rbBall.transform.localPosition).normalized;
        _rbBall.angularVelocity = Vector3.zero;
        _rbBall.velocity = Vector3.zero;
        _rbBall.AddForce(direction * _power, ForceMode.Impulse);
        //Debug.Log($"direction {direction} -> {direction * _power}");
        //transform.localPosition = new Vector3(0, 20, 0);
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
        // Move the agent using the action.
        MoveAgent(actions.DiscreteActions);
    }

    private void MoveAgent(ActionSegment<int> act)
    {
        int action = act[0];
        int h = 0;
        //Debug.Log($"actions {action}");
        switch (action)
        {
            case 0: h = 0; break;
            case 1: h = -1; break;
            case 2: h = 1; break;
        }

        float currentX = transform.localPosition.x;
        float nextX = currentX + h * Time.deltaTime * _moveSpeed;

        nextX = Mathf.Clamp(nextX, -47f, 47f);
        transform.localPosition = new Vector3(nextX, 20, 0);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        float h = Input.GetAxisRaw("Horizontal");
        Debug.Log($"Heuristic {h}");
        h = h == 0 ? 0 : h < 0 ? 1 : 2;
        discreteActionsOut[0] = (int)h;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (GetCumulativeReward() < 0.55f)
            {
                AddReward(0.3f);
                Debug.Log($"AddReward += 0.03!!");
                _rbBall.velocity = Vector3.zero;
                Vector3 dest = new Vector3(Random.Range(-47, 47), 120);
                Vector3 direction = (dest - _rbBall.transform.localPosition).normalized;
                _rbBall.AddForce(direction * _power, ForceMode.Impulse);
            }
            else
            {
                Debug.Log($"SetReward = 1f");
                SetReward(1);
                EndEpisode();
            }
            _meshRenderer.material = _mWin;
        }
    }

    public void Penalty()
    {
        Debug.Log($"SetReward = -1!");
        _meshRenderer.material = _mLose;
        SetReward(-1f);
        EndEpisode();
    }

    public void Reset()
    {
        Debug.Log($"Reset Ball!!!");
        SetReward(-1f);
        EndEpisode();
    }
}
