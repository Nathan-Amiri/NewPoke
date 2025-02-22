using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEffectIndex : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private delegate void IndexMethod(ChoiceInfo choiceInfo, int moveType, int occurance);

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
        indexMethods[indexNumber](choiceInfo, moveType, occurance);
    }



    private void Protect(ChoiceInfo choiceInfo, int moveType, int occurance) // 0
    {

    }
    private void FakeOut(ChoiceInfo choiceInfo, int moveType, int occurance) // 1
    {

    }
    private void Thunderbolt(ChoiceInfo choiceInfo, int moveType, int occurance) // 2
    {
        //choiceInfo.targetSlot.HealthChange(choiceInfo., moveType);
    }
    private void VoltSwitch(ChoiceInfo choiceInfo, int moveType, int occurance) // 3
    {

    }
    private void ThunderWave(ChoiceInfo choiceInfo, int moveType, int occurance) // 4
    {

    }
    private void WeatherBall(ChoiceInfo choiceInfo, int moveType, int occurance) // 5
    {

    }
    private void Hurricane(ChoiceInfo choiceInfo, int moveType, int occurance) // 6
    {

    }
    private void Tailwind(ChoiceInfo choiceInfo, int moveType, int occurance) // 7
    {

    }
    private void MatchaGotcha(ChoiceInfo choiceInfo, int moveType, int occurance) // 8
    {

    }
    private void Hex(ChoiceInfo choiceInfo, int moveType, int occurance) // 9
    {

    }
    private void RagePowder(ChoiceInfo choiceInfo, int moveType, int occurance) // 10
    {

    }
    private void LifeDew(ChoiceInfo choiceInfo, int moveType, int occurance) // 11
    {

    }
    private void CloseCombat(ChoiceInfo choiceInfo, int moveType, int occurance) // 12
    {

    }
    private void FirePunch(ChoiceInfo choiceInfo, int moveType, int occurance) // 13
    {

    }
    private void DrainPunch(ChoiceInfo choiceInfo, int moveType, int occurance) // 14
    {

    }
    private void Coil(ChoiceInfo choiceInfo, int moveType, int occurance) // 15
    {

    }
    private void PoisonFang(ChoiceInfo choiceInfo, int moveType, int occurance) // 16
    {

    }
    private void Toxic(ChoiceInfo choiceInfo, int moveType, int occurance) // 17
    {

    }
    private void SuckerPunch(ChoiceInfo choiceInfo, int moveType, int occurance) // 18
    {

    }
    private void DoubleEdge(ChoiceInfo choiceInfo, int moveType, int occurance) // 19
    {

    }
    private void ThunderPunch(ChoiceInfo choiceInfo, int moveType, int occurance) // 20
    {

    }
    private void Overheat(ChoiceInfo choiceInfo, int moveType, int occurance) // 21
    {

    }
    private void WilloWisp(ChoiceInfo choiceInfo, int moveType, int occurance) // 22
    {

    }
    private void AuroraVeil(ChoiceInfo choiceInfo, int moveType, int occurance) // 23
    {

    }
    private void Blizzard(ChoiceInfo choiceInfo, int moveType, int occurance) // 24
    {

    }
    private void Moonblast(ChoiceInfo choiceInfo, int moveType, int occurance) // 25
    {

    }
    private void Psychic(ChoiceInfo choiceInfo, int moveType, int occurance) // 26
    {

    }
    private void TrickRoom(ChoiceInfo choiceInfo, int moveType, int occurance) // 27
    {

    }
    private void FollowMe(ChoiceInfo choiceInfo, int moveType, int occurance) // 28
    {

    }
    private void HelpingHand(ChoiceInfo choiceInfo, int moveType, int occurance) // 29
    {

    }
    private void RockSlide(ChoiceInfo choiceInfo, int moveType, int occurance) // 30
    {

    }
    private void KnockOff(ChoiceInfo choiceInfo, int moveType, int occurance) // 31
    {

    }
    private void Earthquake(ChoiceInfo choiceInfo, int moveType, int occurance) // 32
    {

    }

    //gameManager.ToggleFieldEffect("Rain", true, 2);
    //choiceInfo.casterSlot.NewStatus(0);
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