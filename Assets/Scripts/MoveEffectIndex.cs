using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEffectIndex : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private delegate void IndexMethod(ChoiceInfo choiceInfo, int occurance);

    private readonly List<IndexMethod> indexMethods = new();

    private void Awake()
    {
        PopulateIndex();
    }

    public void MoveEffect(ChoiceInfo choiceInfo, int occurance)
    {
        int indexNumber = choiceInfo.casterSlot.data.moves[choiceInfo.choice].indexNumber;

        if (indexMethods.Count < indexNumber + 1)
        {
            Debug.LogError("The following moveEffect index method was not found: " + indexNumber);
            return;
        }

        int moveType = choiceInfo.casterSlot.data.moves[choiceInfo.choice].pokeType;
        indexMethods[indexNumber](choiceInfo, occurance);
    }



    private void Protect(ChoiceInfo choiceInfo, int occurance) // 0
    {

    }
    private void FakeOut(ChoiceInfo choiceInfo, int occurance) // 1
    {

    }
    private void Thunderbolt(ChoiceInfo choiceInfo, int occurance) // 2
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack, 4);
    }
    private void VoltSwitch(ChoiceInfo choiceInfo, int occurance) // 3
    {
        int amount = choiceInfo.casterSlot.data.currentAttack - 2;
        if (amount < 1)
            amount = 1;
        choiceInfo.casterSlot.enemySlots[0].DealDamage(amount, 4);
        choiceInfo.casterSlot.enemySlots[1].DealDamage(amount, 4);

        gameManager.Switch(choiceInfo.casterSlot, choiceInfo.targetSlot);
    }
    private void ThunderWave(ChoiceInfo choiceInfo, int occurance) // 4
    {
        //choiceInfo.targetSlot.NewStatus(2);
    }
    private void WeatherBall(ChoiceInfo choiceInfo, int occurance) // 5
    {

    }
    private void Hurricane(ChoiceInfo choiceInfo, int occurance) // 6
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack, 9);
    }
    private void Tailwind(ChoiceInfo choiceInfo, int occurance) // 7
    {

    }
    private void MatchaGotcha(ChoiceInfo choiceInfo, int occurance) // 8
    {
        choiceInfo.casterSlot.enemySlots[0].DealDamage(choiceInfo.casterSlot.data.currentAttack, 3);
        choiceInfo.casterSlot.enemySlots[1].DealDamage(choiceInfo.casterSlot.data.currentAttack, 3);

        choiceInfo.casterSlot.GainHealth(1);
    }
    private void Hex(ChoiceInfo choiceInfo, int occurance) // 9
    {
        int amount = choiceInfo.casterSlot.data.currentAttack;
        if (choiceInfo.targetSlot.data.status.name != null)
            amount += 1;
        choiceInfo.targetSlot.DealDamage(amount, 13);
    }
    private void RagePowder(ChoiceInfo choiceInfo, int occurance) // 10
    {

    }
    private void LifeDew(ChoiceInfo choiceInfo, int occurance) // 11
    {
        choiceInfo.casterSlot.GainHealth(1);
        choiceInfo.casterSlot.ally.GainHealth(2);
    }
    private void CloseCombat(ChoiceInfo choiceInfo, int occurance) // 12
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack + 1, 6);
        choiceInfo.casterSlot.BaseHealthChange(-1);
    }
    private void FirePunch(ChoiceInfo choiceInfo, int occurance) // 13
    {
        int amount = choiceInfo.casterSlot.data.currentAttack - 1;
        if (amount < 1)
            amount = 1;
        choiceInfo.targetSlot.DealDamage(amount, 1);
    }
    private void DrainPunch(ChoiceInfo choiceInfo, int occurance) // 14
    {
        int amount = choiceInfo.casterSlot.data.currentAttack - 1;
        if (amount < 1)
            amount = 1;
        choiceInfo.targetSlot.DealDamage(amount, 6);

        choiceInfo.casterSlot.GainHealth(1);
    }
    private void Coil(ChoiceInfo choiceInfo, int occurance) // 15
    {
        choiceInfo.casterSlot.AttackChange(1);
        choiceInfo.casterSlot.BaseHealthChange(1);
        choiceInfo.casterSlot.GainHealth(1);
    }
    private void PoisonFang(ChoiceInfo choiceInfo, int occurance) // 16
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack, 7);
    }
    private void Toxic(ChoiceInfo choiceInfo, int occurance) // 17
    {
        //choiceInfo.targetSlot.NewStatus(3);
    }
    private void SuckerPunch(ChoiceInfo choiceInfo, int occurance) // 18
    {

    }
    private void DoubleEdge(ChoiceInfo choiceInfo, int occurance) // 19
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack + 1, 0);
        choiceInfo.casterSlot.DealDamage(1, -1);
    }
    private void ThunderPunch(ChoiceInfo choiceInfo, int occurance) // 20
    {
        int amount = choiceInfo.casterSlot.data.currentAttack - 1;
        if (amount < 1)
            amount = 1;
        choiceInfo.targetSlot.DealDamage(amount, 4);
    }
    private void Overheat(ChoiceInfo choiceInfo, int occurance) // 21
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack + 2, 1);
        choiceInfo.casterSlot.AttackChange(-2);
    }
    private void WilloWisp(ChoiceInfo choiceInfo, int occurance) // 22
    {
        //choiceInfo.targetSlot.NewStatus(1);
    }
    private void AuroraVeil(ChoiceInfo choiceInfo, int occurance) // 23
    {

    }
    private void Blizzard(ChoiceInfo choiceInfo, int occurance) // 24
    {
        int amount = choiceInfo.casterSlot.data.currentAttack - 1;
        if (amount < 1)
            amount = 1;
        choiceInfo.casterSlot.enemySlots[0].DealDamage(amount, 5);
        choiceInfo.casterSlot.enemySlots[1].DealDamage(amount, 5);
    }
    private void Moonblast(ChoiceInfo choiceInfo, int occurance) // 25
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack, 17);
    }
    private void Psychic(ChoiceInfo choiceInfo, int occurance) // 26
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack, 10);
    }
    private void TrickRoom(ChoiceInfo choiceInfo, int occurance) // 27
    {

    }
    private void FollowMe(ChoiceInfo choiceInfo, int occurance) // 28
    {

    }
    private void HelpingHand(ChoiceInfo choiceInfo, int occurance) // 29
    {

    }
    private void RockSlide(ChoiceInfo choiceInfo, int occurance) // 30
    {
        int amount = choiceInfo.casterSlot.data.currentAttack - 1;
        if (amount < 1)
            amount = 1;
        choiceInfo.casterSlot.enemySlots[0].DealDamage(amount, 12);
        choiceInfo.casterSlot.enemySlots[1].DealDamage(amount, 12);
    }
    private void KnockOff(ChoiceInfo choiceInfo, int occurance) // 31
    {
        int amount = choiceInfo.casterSlot.data.currentAttack;
        if (!choiceInfo.targetSlot.data.knockedOff)
        {
            amount += 1;
            choiceInfo.targetSlot.data.knockedOff = true;
        }
        choiceInfo.targetSlot.DealDamage(amount, 15);
    }
    private void Earthquake(ChoiceInfo choiceInfo, int occurance) // 32
    {
        int amount = choiceInfo.casterSlot.data.currentAttack;

        choiceInfo.casterSlot.ally.DealDamage(amount, 8);
        choiceInfo.casterSlot.enemySlots[0].DealDamage(amount, 8);
        choiceInfo.casterSlot.enemySlots[1].DealDamage(amount, 8);
    }

    private void PopulateIndex()
    {
        indexMethods.Add(Protect);
        indexMethods.Add(FakeOut);
        indexMethods.Add(Thunderbolt);
        indexMethods.Add(VoltSwitch);
        indexMethods.Add(ThunderWave);
        indexMethods.Add(WeatherBall);
        indexMethods.Add(Hurricane);
        indexMethods.Add(Tailwind);
        indexMethods.Add(MatchaGotcha);
        indexMethods.Add(Hex);
        indexMethods.Add(RagePowder);
        indexMethods.Add(LifeDew);
        indexMethods.Add(CloseCombat);
        indexMethods.Add(FirePunch);
        indexMethods.Add(DrainPunch);
        indexMethods.Add(Coil);
        indexMethods.Add(PoisonFang);
        indexMethods.Add(Toxic);
        indexMethods.Add(SuckerPunch);
        indexMethods.Add(DoubleEdge);
        indexMethods.Add(ThunderPunch);
        indexMethods.Add(Overheat);
        indexMethods.Add(WilloWisp);
        indexMethods.Add(AuroraVeil);
        indexMethods.Add(Blizzard);
        indexMethods.Add(Moonblast);
        indexMethods.Add(Psychic);
        indexMethods.Add(TrickRoom);
        indexMethods.Add(FollowMe);
        indexMethods.Add(HelpingHand);
        indexMethods.Add(RockSlide);
        indexMethods.Add(KnockOff);
        indexMethods.Add(Earthquake);
    }
}