using BehaviorDesigner.Runtime;

[System.Serializable]
public class SharedBehavior : SharedVariable<Behavior>
{
    public static implicit operator SharedBehavior(Behavior value) { return new SharedBehavior { Value = value }; }
}