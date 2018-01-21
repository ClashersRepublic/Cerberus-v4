namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicParticleEmitterData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicParticleEmitterData" /> class.
        /// </summary>
        public LogicParticleEmitterData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicParticleEmitterData.
        }

        protected int[] ParticleCount { get; set; }
        protected int[] EmissionTime { get; set; }
        protected int[] MinLife { get; set; }
        protected int[] MaxLife { get; set; }
        protected int[] MinHorizAngle { get; set; }
        protected int[] MaxHorizAngle { get; set; }
        protected int[] MinVertAngle { get; set; }
        protected int[] MaxVertAngle { get; set; }
        protected int[] MinSpeed { get; set; }
        protected int[] MaxSpeed { get; set; }
        public int StartX { get; protected set; }
        public int StartY { get; protected set; }
        protected int[] StartZ { get; set; }
        public int TargetedEndZ { get; protected set; }
        protected int[] Gravity { get; set; }
        protected int[] Inertia { get; set; }
        public int Slowdown { get; protected set; }
        protected int[] StartScale { get; set; }
        protected int[] EndScale { get; set; }
        protected int[] MinRotate { get; set; }
        protected int[] MaxRotate { get; set; }
        protected int[] ParticleFadeOutTime { get; set; }
        protected int[] StartRadius { get; set; }
        protected string[] ParticleSwf { get; set; }
        protected string[] ParticleExportName { get; set; }
        protected bool[] ScaleTimeline { get; set; }
        public bool OrientToMovement { get; protected set; }
        public bool OrientToParent { get; protected set; }
        protected bool[] BounceFromGround { get; set; }
        protected bool[] AdditiveBlend { get; set; }
        protected int[] FadeInTime { get; set; }
        protected int[] FadeOutTime { get; set; }
        protected bool[] IsIsoParticle { get; set; }
        public int StartAngleMin { get; protected set; }
        public int StartAngleMax { get; protected set; }
        public int ScaleRandomMin { get; protected set; }
        public int ScaleRandomMax { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            // LoadingFinished.
        }

        public int GetParticleCount(int index)
        {
            return this.ParticleCount[index];
        }

        public int GetEmissionTime(int index)
        {
            return this.EmissionTime[index];
        }

        public int GetMinLife(int index)
        {
            return this.MinLife[index];
        }

        public int GetMaxLife(int index)
        {
            return this.MaxLife[index];
        }

        public int GetMinHorizAngle(int index)
        {
            return this.MinHorizAngle[index];
        }

        public int GetMaxHorizAngle(int index)
        {
            return this.MaxHorizAngle[index];
        }

        public int GetMinVertAngle(int index)
        {
            return this.MinVertAngle[index];
        }

        public int GetMaxVertAngle(int index)
        {
            return this.MaxVertAngle[index];
        }

        public int GetMinSpeed(int index)
        {
            return this.MinSpeed[index];
        }

        public int GetMaxSpeed(int index)
        {
            return this.MaxSpeed[index];
        }

        public int GetStartZ(int index)
        {
            return this.StartZ[index];
        }

        public int GetGravity(int index)
        {
            return this.Gravity[index];
        }

        public int GetInertia(int index)
        {
            return this.Inertia[index];
        }

        public int GetStartScale(int index)
        {
            return this.StartScale[index];
        }

        public int GetEndScale(int index)
        {
            return this.EndScale[index];
        }

        public int GetMinRotate(int index)
        {
            return this.MinRotate[index];
        }

        public int GetMaxRotate(int index)
        {
            return this.MaxRotate[index];
        }

        public int GetParticleFadeOutTime(int index)
        {
            return this.ParticleFadeOutTime[index];
        }

        public int GetStartRadius(int index)
        {
            return this.StartRadius[index];
        }

        public string GetParticleSwf(int index)
        {
            return this.ParticleSwf[index];
        }

        public string GetParticleExportName(int index)
        {
            return this.ParticleExportName[index];
        }

        public bool GetScaleTimeline(int index)
        {
            return this.ScaleTimeline[index];
        }

        public bool GetBounceFromGround(int index)
        {
            return this.BounceFromGround[index];
        }

        public bool GetAdditiveBlend(int index)
        {
            return this.AdditiveBlend[index];
        }

        public int GetFadeInTime(int index)
        {
            return this.FadeInTime[index];
        }

        public int GetFadeOutTime(int index)
        {
            return this.FadeOutTime[index];
        }

        public bool GetIsIsoParticle(int index)
        {
            return this.IsIsoParticle[index];
        }
    }
}