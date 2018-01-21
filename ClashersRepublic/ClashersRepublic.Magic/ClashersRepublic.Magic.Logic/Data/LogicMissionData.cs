namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicMissionData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicMissionData" /> class.
        /// </summary>
        public LogicMissionData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicMissionData.
        }

        public string Dependencies { get; protected set; }
        public int MissionCategory { get; protected set; }
        public int VillageType { get; protected set; }
        public bool FirstStep { get; protected set; }
        public bool WarStates { get; protected set; }
        public bool Deprecated { get; protected set; }
        public bool OpenInfo { get; protected set; }
        public bool ShowWarBase { get; protected set; }
        public bool ShowDonate { get; protected set; }
        public bool SwitchSides { get; protected set; }
        public bool OpenAchievements { get; protected set; }
        public string Action { get; protected set; }
        public string Character { get; protected set; }
        public string FixVillageObject { get; protected set; }
        public string BuildBuilding { get; protected set; }
        public int BuildBuildingLevel { get; protected set; }
        public int BuildBuildingCount { get; protected set; }
        public string DefendNPC { get; protected set; }
        public string AttackNPC { get; protected set; }
        public bool AttackPlayer { get; protected set; }
        public bool ChangeName { get; protected set; }
        protected int[] Delay { get; set; }
        public int TrainTroops { get; protected set; }
        public bool ShowMap { get; protected set; }
        protected string[] TutorialText { get; set; }
        protected int[] TutorialStep { get; set; }
        protected bool[] Darken { get; set; }
        protected string[] TutorialTextBox { get; set; }
        protected string[] TutorialCharacter { get; set; }
        protected string[] CharacterSWF { get; set; }
        public bool LoopAnim { get; protected set; }
        protected bool[] SwitchAnim { get; set; }
        protected string[] SpeechBubble { get; set; }
        protected bool[] RightAlignTextBox { get; set; }
        protected string[] ButtonText { get; set; }
        protected string[] TutorialMusic { get; set; }
        public string TutorialMusicAlt { get; protected set; }
        protected string[] TutorialSound { get; set; }
        public string RewardResource { get; protected set; }
        public int RewardResourceCount { get; protected set; }
        public int RewardXP { get; protected set; }
        public string RewardTroop { get; protected set; }
        public int RewardTroopCount { get; protected set; }
        public int CustomData { get; protected set; }
        public bool ShowGooglePlusSignin { get; protected set; }
        public bool HideGooglePlusSignin { get; protected set; }
        protected bool[] ShowInstructor { get; set; }
        public int Villagers { get; protected set; }
        public bool ForceCamera { get; protected set; }
        public bool TapGameObject { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            // LoadingFinished.
        }

        public int GetDelay(int index)
        {
            return this.Delay[index];
        }

        public string GetTutorialText(int index)
        {
            return this.TutorialText[index];
        }

        public int GetTutorialStep(int index)
        {
            return this.TutorialStep[index];
        }

        public bool GetDarken(int index)
        {
            return this.Darken[index];
        }

        public string GetTutorialTextBox(int index)
        {
            return this.TutorialTextBox[index];
        }

        public string GetTutorialCharacter(int index)
        {
            return this.TutorialCharacter[index];
        }

        public string GetCharacterSWF(int index)
        {
            return this.CharacterSWF[index];
        }

        public bool GetSwitchAnim(int index)
        {
            return this.SwitchAnim[index];
        }

        public string GetSpeechBubble(int index)
        {
            return this.SpeechBubble[index];
        }

        public bool GetRightAlignTextBox(int index)
        {
            return this.RightAlignTextBox[index];
        }

        public string GetButtonText(int index)
        {
            return this.ButtonText[index];
        }

        public string GetTutorialMusic(int index)
        {
            return this.TutorialMusic[index];
        }

        public string GetTutorialSound(int index)
        {
            return this.TutorialSound[index];
        }

        public bool GetShowInstructor(int index)
        {
            return this.ShowInstructor[index];
        }
    }
}