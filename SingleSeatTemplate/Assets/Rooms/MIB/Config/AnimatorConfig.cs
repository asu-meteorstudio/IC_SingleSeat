using UnityEngine;

public static class ScooterParams
{
    public static int State = Animator.StringToHash("State");
    public static int ButtonState = Animator.StringToHash("ButtonState");
    public static int AttackState = Animator.StringToHash("AttackState");
}

public static class ScooterStateParamValues
{
    public static int Default = 0;
    public static int On = 1;
    public static int Off = 2;
}

public static class RFPodParams
{
    public static int State = Animator.StringToHash("State");
    public static int IsOpen = Animator.StringToHash("IsOpen");
}

public static class RFPodParamValues
{
    public static int Default = 0;
    public static int Show = 1;
    public static int Hide = 2;
}

public static class RFParameters
{
    public static int FacingOut = Animator.StringToHash("FacingOut");
    public static int IsNervous = Animator.StringToHash("IsNervous");
    public static int ZeroG = Animator.StringToHash("ZeroG");
    public static int ZeroGSubState = Animator.StringToHash("ZeroGSubState");
    public static int LeanedForward = Animator.StringToHash("LeanedForward");
    public static int LeanedForwardSubState = Animator.StringToHash("LeanedForwardSubState");
    public static int Combine = Animator.StringToHash("Combine");

    //public static int Combine = Animator.StringToHash("combine");
    //public static int Cower = Animator.StringToHash("cower");
    //public static int Greeting = Animator.StringToHash("greeting");
    //public static int Walk = Animator.StringToHash("walk");
    //public static int LookAround = Animator.StringToHash("lookAround");
    //public static int Dying = Animator.StringToHash("dying");
    //public static int HandToGlass = Animator.StringToHash("handToGlass");
    //public static int Rollercoaster = Animator.StringToHash("rollercoaster");
    //public static int LeanedForward = Animator.StringToHash("leanedForward");
    //public static int WaveAtPlayer = Animator.StringToHash("waveAtPlayer");
    //public static int PointToOctopoid = Animator.StringToHash("pointToOctopoid");
}

public static class RFStates
{
    //public static int FacingIn = Animator.StringToHash(RFLayers.BaseLayer + ".FacingIn");
    //public static int FacingOut = Animator.StringToHash(RFLayers.BaseLayer + ".FacingOut");

    static string FacingInString = RFLayers.BaseLayer + ".FacingIn";
    static string FacingOutString = RFLayers.BaseLayer + ".FacingOut";
    static string ZeroGString = RFLayers.BaseLayer + FacingOutString + ".ZeroG";
    static string LeanedForwardString = RFLayers.BaseLayer + FacingOutString + ".LeanedForward";

    //facing in
    public static int TurnIn = Animator.StringToHash(FacingInString + ".TurnIn");
    public static int Idle_In = Animator.StringToHash(FacingInString + ".Idle");
    public static int IntroToScooterAnimation = Animator.StringToHash(FacingInString + ".IntroToScooterTransition");
    public static int Greeting = Animator.StringToHash(FacingInString + ".Greeting");
    public static int HandToGlass = Animator.StringToHash(FacingInString + ".HandToGlass");
    public static int Walk = Animator.StringToHash(FacingInString + ".Walk");
    public static int LookAround_In = Animator.StringToHash(FacingInString + ".LookAround");
    public static int Cower_In = Animator.StringToHash(FacingInString + ".Cower");
    public static int GalaxariumToRoof = Animator.StringToHash(FacingInString + ".GalaxToRoof");

    public static int Combine = Animator.StringToHash(FacingInString + ".Combine");
    public static int Dying = Animator.StringToHash(FacingInString + ".Dying");

    //facing out
    public static int TurnOut = Animator.StringToHash(FacingOutString + ".TurnOut");
    public static int Idle_Out = Animator.StringToHash(FacingOutString + ".Idle");

    public static int RollercoasterTurn = Animator.StringToHash(FacingOutString + ".RollercoasterTurn");
    public static int Rollercoaster = Animator.StringToHash(FacingOutString + ".Rollercoaster");

    public static int BobAndWeave = Animator.StringToHash(FacingOutString + ".BobAndWeave");
    public static int StartledAndPoint = Animator.StringToHash(FacingOutString + ".StartledAndPoint");
    public static int LookAround_Out = Animator.StringToHash(FacingOutString + ".LookAround");
    public static int Cower_Out = Animator.StringToHash(FacingOutString + ".Cower");

    public static int ZeroGTurnOut = Animator.StringToHash(ZeroGString + ".ZeroGTurnOut");
    public static int ZeroGLiftOff = Animator.StringToHash(ZeroGString + ".ZeroGLiftOff");
    public static int ZeroGFloatLoop = Animator.StringToHash(ZeroGString + ".ZeroGFloatLoop");
    public static int ZeroGFloat = Animator.StringToHash(ZeroGString + ".ZeroGFloat");
    public static int ZeroGFlip = Animator.StringToHash(ZeroGString + ".ZeroGFlip");
    public static int ZeroGHitGlassForward = Animator.StringToHash(ZeroGString + ".ZeroGHitGlassForward");

    public static int IdleToLeanedForward = Animator.StringToHash(LeanedForwardString + ".IdleToLeanedForward");
    public static int LeanedForward = Animator.StringToHash(LeanedForwardString + ".LeanedForward");
    public static int WaveAtPlayer = Animator.StringToHash(LeanedForwardString + ".WaveAtPlayer");
    public static int PointToOctopoid = Animator.StringToHash(LeanedForwardString + ".PointToOctopoid");
    public static int Hide = Animator.StringToHash(LeanedForwardString + ".Hide");
    public static int LeanedForwardToIdle = Animator.StringToHash(LeanedForwardString + ".LeanedForwardToIdle");
}

public static class RFLayers
{
    public static string BaseLayer ="Base Layer";
    public static string Actions = "Actions";
}

public static class AgentParams
{
    public static int State = Animator.StringToHash("State");
}

public static class AgentParamValues
{
    public static int Idle = 0;
    public static int Walk = 1;
    public static int Directions = 2;
    public static int Guard = 3;
    public static int Coffee = 4;
    public static int CoffeeWalk = 5;
    public static int Typing = 6;
    public static int Working = 7;
    public static int Buttonpush = 8;
    public static int HeadShake = 9;
    public static int Agree = 10;
}

public static class RFPodFXStates
{
    //public static int Off = Animator.StringToHash(RFPodFXLayers.BaseLayer + ".Off");
    public static int Start = Animator.StringToHash(RFPodFXLayers.BaseLayer + ".RFI_HandTouch_Prompt_start");
    public static int Loop = Animator.StringToHash(RFPodFXLayers.BaseLayer + ".RFI_HandTouch_Prompt_loop");
}

public static class RFPodFXLayers
{
    public static string BaseLayer = "Base Layer";
}

public static class AnimationEvents
{
    public const string HandToGlass = "HandToGlass";
}

public static class FrankStates
{
    public static int Jump = Animator.StringToHash("Jump");
    public static int SubwayIdle = Animator.StringToHash("SubwayIdle");
}
