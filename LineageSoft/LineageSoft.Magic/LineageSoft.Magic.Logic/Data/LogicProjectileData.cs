namespace LineageSoft.Magic.Logic.Data
{
    using LineageSoft.Magic.Titan.CSV;

    public class LogicProjectileData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicProjectileData" /> class.
        /// </summary>
        public LogicProjectileData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicProjectileData.
        }

        public string SWF { get; protected set; }
        public string ExportName { get; protected set; }
        public bool ScaleTimeline { get; protected set; }
        public bool UseDirections { get; protected set; }
        public string ParticleEmitter { get; protected set; }
        public string Effect { get; protected set; }
        public int Speed { get; protected set; }
        public int StartHeight { get; protected set; }
        public int StartOffset { get; protected set; }
        public bool IsBallistic { get; protected set; }
        public string ShadowSWF { get; protected set; }
        public string ShadowExportName { get; protected set; }
        public bool RandomHitPosition { get; protected set; }
        public bool UseRotate { get; protected set; }
        public bool DirectionFrame { get; protected set; }
        public bool PlayOnce { get; protected set; }
        public bool UseTopLayer { get; protected set; }
        public int Scale { get; protected set; }
        public string HitSpell { get; protected set; }
        public int HitSpellLevel { get; protected set; }
        public bool DontTrackTarget { get; protected set; }
        public int BallisticHeight { get; protected set; }
        public int TrajectoryStyle { get; protected set; }
        public int FixedTravelTime { get; protected set; }
        public int DamageDelay { get; protected set; }
        public string DestroyedEffect { get; protected set; }
        public string BounceEffect { get; protected set; }
        public int TargetPosRandomRadius { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            // CreateReferences.
        }
    }
}