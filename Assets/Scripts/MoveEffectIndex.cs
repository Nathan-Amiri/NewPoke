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
        int indexNumber = choiceInfo.move.indexNumber;

        if (indexMethods.Count < indexNumber + 1)
        {
            Debug.LogError("The following moveEffect index method was not found: " + indexNumber);
            return;
        }

        int moveType = choiceInfo.move.pokeType;
        indexMethods[indexNumber](choiceInfo, occurance);
    }



    private void Protect(ChoiceInfo choiceInfo, int occurance) // 0
    {
        if (occurance == 0)
        {
            choiceInfo.casterSlot.data.isProtected = true;
            gameManager.AddDelayedEffect(choiceInfo, 1);
        }
        else if (occurance == 1)
        {
            choiceInfo.casterSlot.data.isProtected = false;
            choiceInfo.casterSlot.data.protectedLastRound = true;
            gameManager.AddDelayedEffect(choiceInfo, 2);
        }
        else if (occurance == 2)
            choiceInfo.casterSlot.data.protectedLastRound = false;
    }
    private void FakeOut(ChoiceInfo choiceInfo, int occurance) // 1
    {
        if (choiceInfo.targetSlot.data.pokeTypes.Contains(13))
        {
            gameManager.AddEffectivenessMessage(0, choiceInfo.targetSlot.data.pokemonName);
            return;
        }

        choiceInfo.targetSlot.DealDamage(1, 0, choiceInfo.casterSlot);
        choiceInfo.targetSlot.data.fakedOut = true;
    }
    private void Thunderbolt(ChoiceInfo choiceInfo, int occurance) // 2
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack, 4, choiceInfo.casterSlot);
    }
    private void VoltSwitch(ChoiceInfo choiceInfo, int occurance) // 3
    {
        int amount = choiceInfo.casterSlot.data.currentAttack - 2;
        if (amount < 1)
            amount = 1;
        choiceInfo.casterSlot.enemySlots[0].DealDamage(amount, 4, choiceInfo.casterSlot);
        choiceInfo.casterSlot.enemySlots[1].DealDamage(amount, 4, choiceInfo.casterSlot);

        if (choiceInfo.targetSlot != null)
            gameManager.Switch(choiceInfo.casterSlot, choiceInfo.targetSlot);
    }
    private void ThunderWave(ChoiceInfo choiceInfo, int occurance) // 4
    {
        if (choiceInfo.targetSlot.data.pokeTypes.Contains(8))
        {
            gameManager.AddEffectivenessMessage(0, choiceInfo.targetSlot.data.pokemonName);
            return;
        }

        choiceInfo.targetSlot.NewStatus(2, true);
    }
    private void WeatherBall(ChoiceInfo choiceInfo, int occurance) // 5
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack, choiceInfo.move.pokeType, choiceInfo.casterSlot);
    }
    private void Hurricane(ChoiceInfo choiceInfo, int occurance) // 6
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack, 9, choiceInfo.casterSlot);
    }
    private void Tailwind(ChoiceInfo choiceInfo, int occurance) // 7
    {
        string playerNumber = choiceInfo.casterSlot.slotNumber < 2 ? "(Player 1)" : "(Player 2)";
        gameManager.ToggleFieldEffect("Tailwind " + playerNumber, true, 4);
    }
    private void MatchaGotcha(ChoiceInfo choiceInfo, int occurance) // 8
    {
        choiceInfo.casterSlot.enemySlots[0].DealDamage(choiceInfo.casterSlot.data.currentAttack, 3, choiceInfo.casterSlot);
        choiceInfo.casterSlot.enemySlots[1].DealDamage(choiceInfo.casterSlot.data.currentAttack, 3, choiceInfo.casterSlot);

        choiceInfo.casterSlot.GainHealth(1);
    }
    private void Hex(ChoiceInfo choiceInfo, int occurance) // 9
    {
        int amount = choiceInfo.casterSlot.data.currentAttack;
        if (choiceInfo.targetSlot.data.status.statusName != null)
            amount += 1;
        choiceInfo.targetSlot.DealDamage(amount, 13, choiceInfo.casterSlot);
    }
    private void RagePowder(ChoiceInfo choiceInfo, int occurance) // 10
    {
        List<ChoiceInfo> choicesToRedirect = new();
        foreach (ChoiceInfo choice in gameManager.choices)
            if (choice.casterSlot != choiceInfo.casterSlot.ally && choice.move.isTargeted && !choice.move.targetsBench &&
                !choice.casterSlot.data.pokeTypes.Contains(3))
                choicesToRedirect.Add(choice);

        foreach (ChoiceInfo choiceToRedirect in choicesToRedirect)
        {
            gameManager.choices.Remove(choiceToRedirect);

            ChoiceInfo redirectedChoice = choiceToRedirect;
            redirectedChoice.targetSlot = choiceInfo.casterSlot;
            gameManager.choices.Add(redirectedChoice);
        }
    }
    private void LifeDew(ChoiceInfo choiceInfo, int occurance) // 11
    {
        choiceInfo.casterSlot.GainHealth(1);
        choiceInfo.casterSlot.ally.GainHealth(2);
    }
    private void CloseCombat(ChoiceInfo choiceInfo, int occurance) // 12
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack + 1, 6, choiceInfo.casterSlot);
        choiceInfo.casterSlot.BaseHealthChange(-1);
    }
    private void FirePunch(ChoiceInfo choiceInfo, int occurance) // 13
    {
        int amount = choiceInfo.casterSlot.data.currentAttack - 1;
        if (amount < 1)
            amount = 1;
        choiceInfo.targetSlot.DealDamage(amount, 1, choiceInfo.casterSlot);
    }
    private void DrainPunch(ChoiceInfo choiceInfo, int occurance) // 14
    {
        int amount = choiceInfo.casterSlot.data.currentAttack - 1;
        if (amount < 1)
            amount = 1;
        choiceInfo.targetSlot.DealDamage(amount, 6, choiceInfo.casterSlot);

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
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack, 7, choiceInfo.casterSlot);
    }
    private void Toxic(ChoiceInfo choiceInfo, int occurance) // 17
    {
        if (choiceInfo.targetSlot.data.pokeTypes.Contains(16))
        {
            gameManager.AddEffectivenessMessage(0, choiceInfo.targetSlot.data.pokemonName);
            return;
        }

        choiceInfo.targetSlot.NewStatus(3, true);
    }
    private void SuckerPunch(ChoiceInfo choiceInfo, int occurance) // 18
    {
        int amount = choiceInfo.casterSlot.data.currentAttack - 1;
        if (amount < 1)
            amount = 1;
        choiceInfo.targetSlot.DealDamage(amount, 15, choiceInfo.casterSlot);
    }
    private void DoubleEdge(ChoiceInfo choiceInfo, int occurance) // 19
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack + 1, 0, choiceInfo.casterSlot);
        choiceInfo.casterSlot.DealDamage(1, -1, null);
    }
    private void ThunderPunch(ChoiceInfo choiceInfo, int occurance) // 20
    {
        int amount = choiceInfo.casterSlot.data.currentAttack - 1;
        if (amount < 1)
            amount = 1;
        choiceInfo.targetSlot.DealDamage(amount, 4, choiceInfo.casterSlot);
    }
    private void Overheat(ChoiceInfo choiceInfo, int occurance) // 21
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack + 2, 1, choiceInfo.casterSlot);
        choiceInfo.casterSlot.AttackChange(-2);
    }
    private void WilloWisp(ChoiceInfo choiceInfo, int occurance) // 22
    {
        choiceInfo.targetSlot.NewStatus(1, true);
    }
    private void AuroraVeil(ChoiceInfo choiceInfo, int occurance) // 23
    {
        string playerNumber = choiceInfo.casterSlot.slotNumber < 2 ? "(Player 1)" : "(Player 2)";
        gameManager.ToggleFieldEffect("Aurora Veil " + playerNumber, true, 4);
    }
    private void Blizzard(ChoiceInfo choiceInfo, int occurance) // 24
    {
        int amount = choiceInfo.casterSlot.data.currentAttack - 1;
        if (amount < 1)
            amount = 1;
        choiceInfo.casterSlot.enemySlots[0].DealDamage(amount, 5, choiceInfo.casterSlot);
        choiceInfo.casterSlot.enemySlots[1].DealDamage(amount, 5, choiceInfo.casterSlot);
    }
    private void Moonblast(ChoiceInfo choiceInfo, int occurance) // 25
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack, 17, choiceInfo.casterSlot);
    }
    private void Psychic(ChoiceInfo choiceInfo, int occurance) // 26
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack, 10, choiceInfo.casterSlot);
    }
    private void TrickRoom(ChoiceInfo choiceInfo, int occurance) // 27
    {
        if (!gameManager.fieldEffects.ContainsKey("Trick Room"))
            gameManager.ToggleFieldEffect("Trick Room", true, 5);
        else
            gameManager.ToggleFieldEffect("Trick Room", false);
    }
    private void FollowMe(ChoiceInfo choiceInfo, int occurance) // 28
    {
        List<ChoiceInfo> choicesToRedirect = new();
        foreach (ChoiceInfo choice in gameManager.choices)
            if (choice.casterSlot != choiceInfo.casterSlot.ally && choice.move.isTargeted && !choice.move.targetsBench)
                choicesToRedirect.Add(choice);

        foreach (ChoiceInfo choiceToRedirect in choicesToRedirect)
        {
            gameManager.choices.Remove(choiceToRedirect);

            ChoiceInfo redirectedChoice = choiceToRedirect;
            redirectedChoice.targetSlot = choiceInfo.casterSlot;
            gameManager.choices.Add(redirectedChoice);
        }
    }
    private void HelpingHand(ChoiceInfo choiceInfo, int occurance) // 29
    {
        if (!choiceInfo.casterSlot.ally.slotIsEmpty)
            choiceInfo.casterSlot.ally.data.helpingHandReady = true;
    }
    private void RockSlide(ChoiceInfo choiceInfo, int occurance) // 30
    {
        int amount = choiceInfo.casterSlot.data.currentAttack - 1;
        if (amount < 1)
            amount = 1;
        choiceInfo.casterSlot.enemySlots[0].DealDamage(amount, 12, choiceInfo.casterSlot);
        choiceInfo.casterSlot.enemySlots[1].DealDamage(amount, 12, choiceInfo.casterSlot);
    }
    private void KnockOff(ChoiceInfo choiceInfo, int occurance) // 31
    {
        int amount = choiceInfo.casterSlot.data.currentAttack;
        if (!choiceInfo.targetSlot.data.knockedOff)
        {
            amount += 1;
            choiceInfo.targetSlot.data.knockedOff = true;
        }
        choiceInfo.targetSlot.DealDamage(amount, 15, choiceInfo.casterSlot);
    }
    private void Earthquake(ChoiceInfo choiceInfo, int occurance) // 32
    {
        int amount = choiceInfo.casterSlot.data.currentAttack;

        choiceInfo.casterSlot.ally.DealDamage(amount, 8, choiceInfo.casterSlot);
        choiceInfo.casterSlot.enemySlots[0].DealDamage(amount, 8, choiceInfo.casterSlot);
        choiceInfo.casterSlot.enemySlots[1].DealDamage(amount, 8, choiceInfo.casterSlot);
    }
    private void Detect(ChoiceInfo choiceInfo, int occurance) // 33
    {
        if (occurance == 0)
        {
            choiceInfo.casterSlot.data.isProtected = true;
            gameManager.AddDelayedEffect(choiceInfo, 1);
        }
        else if (occurance == 1)
        {
            choiceInfo.casterSlot.data.isProtected = false;
            choiceInfo.casterSlot.data.protectedLastRound = true;
            gameManager.AddDelayedEffect(choiceInfo, 2);
        }
        else if (occurance == 2)
            choiceInfo.casterSlot.data.protectedLastRound = false;
    }
    private void FlareBlitz(ChoiceInfo choiceInfo, int occurance) // 34
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack + 1, 1, choiceInfo.casterSlot);
        choiceInfo.casterSlot.DealDamage(1, -1, null);
    }
    private void SwordsDance(ChoiceInfo choiceInfo, int occurance) // 35
    {
        choiceInfo.casterSlot.AttackChange(2);
    }
    private void GrassyGlide(ChoiceInfo choiceInfo, int occurance) // 36
    {
        int amount = choiceInfo.casterSlot.data.currentAttack - 1;
        if (amount < 1)
            amount = 1;
        choiceInfo.targetSlot.DealDamage(amount, 3, choiceInfo.casterSlot);
    }
    private void WoodHammer(ChoiceInfo choiceInfo, int occurance) // 37
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack + 1, 3, choiceInfo.casterSlot);
        choiceInfo.casterSlot.DealDamage(1, -1, null);
    }
    private void UTurn(ChoiceInfo choiceInfo, int occurance) // 38
    {
        int amount = choiceInfo.casterSlot.data.currentAttack - 2;
        if (amount < 1)
            amount = 1;
        choiceInfo.casterSlot.enemySlots[0].DealDamage(amount, 11, choiceInfo.casterSlot);
        choiceInfo.casterSlot.enemySlots[1].DealDamage(amount, 11, choiceInfo.casterSlot);

        if (choiceInfo.targetSlot != null)
            gameManager.Switch(choiceInfo.casterSlot, choiceInfo.targetSlot);
    }
    private void JetPunch(ChoiceInfo choiceInfo, int occurance) // 39
    {
        int amount = choiceInfo.casterSlot.data.currentAttack - 1;
        if (amount < 1)
            amount = 1;
        choiceInfo.targetSlot.DealDamage(amount, 2, choiceInfo.casterSlot);
    }
    private void FlipTurn(ChoiceInfo choiceInfo, int occurance) // 40
    {
        int amount = choiceInfo.casterSlot.data.currentAttack - 2;
        if (amount < 1)
            amount = 1;
        choiceInfo.casterSlot.enemySlots[0].DealDamage(amount, 2, choiceInfo.casterSlot);
        choiceInfo.casterSlot.enemySlots[1].DealDamage(amount, 2, choiceInfo.casterSlot);

        if (choiceInfo.targetSlot != null)
            gameManager.Switch(choiceInfo.casterSlot, choiceInfo.targetSlot);
    }
    private void WaveCrash(ChoiceInfo choiceInfo, int occurance) // 41
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack + 1, 2, choiceInfo.casterSlot);
        choiceInfo.casterSlot.DealDamage(1, -1, null);
    }
    private void IcePunch(ChoiceInfo choiceInfo, int occurance) // 42
    {
        int amount = choiceInfo.casterSlot.data.currentAttack - 1;
        if (amount < 1)
            amount = 1;
        choiceInfo.targetSlot.DealDamage(amount, 5, choiceInfo.casterSlot);
    }
    private void BodySlam(ChoiceInfo choiceInfo, int occurance) // 43
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack, 0, choiceInfo.casterSlot);
    }
    private void Curse(ChoiceInfo choiceInfo, int occurance) // 44
    {
        choiceInfo.casterSlot.AttackChange(1);
        choiceInfo.casterSlot.BaseHealthChange(1);
        choiceInfo.casterSlot.GainHealth(1);
        choiceInfo.casterSlot.SpeedChange(-1);
    }
    private void SelfDestruct(ChoiceInfo choiceInfo, int occurance) // 45
    {
        choiceInfo.casterSlot.ally.DealDamage(3, 0, choiceInfo.casterSlot);
        choiceInfo.casterSlot.enemySlots[0].DealDamage(3, 0, choiceInfo.casterSlot);
        choiceInfo.casterSlot.enemySlots[1].DealDamage(3, 0, choiceInfo.casterSlot);
        choiceInfo.casterSlot.Faint();
    }
    private void WaterShuriken(ChoiceInfo choiceInfo, int occurance) // 46
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack, 2, choiceInfo.casterSlot);

        if (choiceInfo.casterSlot.data.ability.abilityName == "Protean")
            choiceInfo.casterSlot.ChangeType(2);
    }
    private void DarkPulse(ChoiceInfo choiceInfo, int occurance) // 47
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack, 15, choiceInfo.casterSlot);

        if (choiceInfo.casterSlot.data.ability.abilityName == "Protean")
            choiceInfo.casterSlot.ChangeType(15);
    }
    private void IceBeam(ChoiceInfo choiceInfo, int occurance) // 48
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack, 5, choiceInfo.casterSlot);

        if (choiceInfo.casterSlot.data.ability.abilityName == "Protean")
            choiceInfo.casterSlot.ChangeType(5);
    }
    private void GunkShot(ChoiceInfo choiceInfo, int occurance) // 49
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack, 7, choiceInfo.casterSlot);

        if (choiceInfo.casterSlot.data.ability.abilityName == "Protean")
            choiceInfo.casterSlot.ChangeType(7);
    }
    private void ExtremeSpeed(ChoiceInfo choiceInfo, int occurance) // 50
    {
        int amount = choiceInfo.casterSlot.data.currentAttack - 1;
        if (amount < 1)
            amount = 1;
        choiceInfo.targetSlot.DealDamage(amount, 0, choiceInfo.casterSlot);
    }
    private void DragonClaw(ChoiceInfo choiceInfo, int occurance) // 51
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack, 14, choiceInfo.casterSlot);
    }
    private void IronHead(ChoiceInfo choiceInfo, int occurance) // 52
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack, 16, choiceInfo.casterSlot);
    }
    private void Roost(ChoiceInfo choiceInfo, int occurance) // 53
    {
        choiceInfo.casterSlot.GainHealth(2);
    }
    private void ShadowBall(ChoiceInfo choiceInfo, int occurance) // 54
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack, 13, choiceInfo.casterSlot);
    }
    private void SludgeBomb(ChoiceInfo choiceInfo, int occurance) // 55
    {
        choiceInfo.targetSlot.DealDamage(choiceInfo.casterSlot.data.currentAttack, 7, choiceInfo.casterSlot);
    }
    private void IcyWind(ChoiceInfo choiceInfo, int occurance) // 56
    {
        int amount = choiceInfo.casterSlot.data.currentAttack - 2;
        if (amount < 1)
            amount = 1;
        choiceInfo.casterSlot.enemySlots[0].DealDamage(amount, 5, choiceInfo.casterSlot);
        choiceInfo.casterSlot.enemySlots[1].DealDamage(amount, 5, choiceInfo.casterSlot);

        choiceInfo.casterSlot.enemySlots[0].SpeedChange(-2);
        choiceInfo.casterSlot.enemySlots[1].SpeedChange(-2);
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
        indexMethods.Add(Detect);
        indexMethods.Add(FlareBlitz);
        indexMethods.Add(SwordsDance);
        indexMethods.Add(GrassyGlide);
        indexMethods.Add(WoodHammer);
        indexMethods.Add(UTurn);
        indexMethods.Add(JetPunch);
        indexMethods.Add(FlipTurn);
        indexMethods.Add(WaveCrash);
        indexMethods.Add(IcePunch);
        indexMethods.Add(BodySlam);
        indexMethods.Add(Curse);
        indexMethods.Add(SelfDestruct);
        indexMethods.Add(WaterShuriken);
        indexMethods.Add(DarkPulse);
        indexMethods.Add(IceBeam);
        indexMethods.Add(GunkShot);
        indexMethods.Add(ExtremeSpeed);
        indexMethods.Add(DragonClaw);
        indexMethods.Add(IronHead);
        indexMethods.Add(Roost);
        indexMethods.Add(ShadowBall);
        indexMethods.Add(SludgeBomb);
        indexMethods.Add(IcyWind);
    }
}