
public interface Ability 
{
    //icon sprite
    void EnterAbility();
    void ExitAbility();
    void Activate(PlayerController player);
    void GroundCheck(PlayerController player);
    void DeathReset();
}