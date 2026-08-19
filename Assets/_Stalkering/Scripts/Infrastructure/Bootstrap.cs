using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private CharacterLocomotion _playerLocomotion;
    [SerializeField] private PlayerMoveInput _playerMoveInput;
    [SerializeField] private CharacterControllerMotor _playerMotor;
    [SerializeField] private CharacterLocomotion _botLocomotion;
    [SerializeField] private PursuitMoveInput _botPursuit;
    [SerializeField] private RigidbodyMotor _botMotor;
    [SerializeField] private StepClimber _botStepClimber;

    private void Awake()
    {
        _playerLocomotion.Construct(_playerMoveInput, _playerMotor);
        _botLocomotion.Construct(_botPursuit, _botMotor);
        _botMotor.Construct(_botStepClimber);
        _botPursuit.Construct(_player.transform);
    }
}
