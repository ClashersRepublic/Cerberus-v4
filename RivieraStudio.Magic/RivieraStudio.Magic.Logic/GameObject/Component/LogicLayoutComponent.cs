namespace RivieraStudio.Magic.Logic.GameObject.Component
{
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Json;
    using RivieraStudio.Magic.Titan.Math;

    public sealed class LogicLayoutComponent : LogicComponent
    {
        private LogicVector2[] _layoutPosition;
        private LogicVector2[] _editModeLayoutPosition;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicLayoutComponent" /> class.
        /// </summary>
        public LogicLayoutComponent(LogicGameObject gameObject) : base(gameObject)
        {
            this._layoutPosition = new LogicVector2[8];
            this._editModeLayoutPosition = new LogicVector2[8];

            for (int i = 0; i < 8; i++)
            {
                this._layoutPosition[i] = new LogicVector2(-1, -1);
                this._editModeLayoutPosition[i] = new LogicVector2(-1, -1);
            }
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            for (int i = 0; i < this._layoutPosition.Length; i++)
            {
                if (this._layoutPosition[i] != null)
                {
                    this._layoutPosition[i].Destruct();
                    this._layoutPosition[i] = null;
                }
            }

            for (int i = 0; i < this._editModeLayoutPosition.Length; i++)
            {
                if (this._editModeLayoutPosition[i] != null)
                {
                    this._editModeLayoutPosition[i].Destruct();
                    this._editModeLayoutPosition[i] = null;
                }
            }
        }

        /// <summary>
        ///     Gets the component type.
        /// </summary>
        public override int GetComponentType()
        {
            return 13;
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        public override void Save(LogicJSONObject jsonObject)
        {
            LogicLevel level = this._parent.GetLevel();
            int villageType = this._parent.GetVillageType();
            int activeLayout = this._parent.GetLevel().GetActiveLayout(villageType);

            for (int i = 0; i < 8; i++)
            {
                LogicVector2 pos = this._editModeLayoutPosition[i];

                if (pos.X != -1 && pos.Y != -1)
                {
                    if (level.GetLayoutState(i, villageType) == 1)
                    {
                        jsonObject.Put(this.GetLayoutVariableNameX(i, true), new LogicJSONNumber(pos.X));
                        jsonObject.Put(this.GetLayoutVariableNameY(i, true), new LogicJSONNumber(pos.Y));
                    }
                }
            }

            for (int i = 0; i < 8; i++)
            {
                if (i != activeLayout)
                {
                    LogicVector2 pos = this._layoutPosition[i];

                    if (pos.X != -1 && pos.Y != -1)
                    {
                        jsonObject.Put(this.GetLayoutVariableNameX(i, false), new LogicJSONNumber(pos.X));
                        jsonObject.Put(this.GetLayoutVariableNameY(i, false), new LogicJSONNumber(pos.Y));
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the position layout.
        /// </summary>
        public LogicVector2 GetPositionLayout(int idx)
        {
            Debugger.DoAssert(idx < 8, "Layout index out of bounds");
            return this._layoutPosition[idx];
        }

        /// <summary>
        ///     Gets the edit mode layout.
        /// </summary>
        public LogicVector2 GetEditModePositionLayout(int idx)
        {
            Debugger.DoAssert(idx < 8, "Layout index out of bounds");
            return this._editModeLayoutPosition[idx];
        }

        /// <summary>
        ///     Gets the layout variable name for x.
        /// </summary>
        public string GetLayoutVariableNameX(int idx, bool editMode)
        {
            if (editMode)
            {
                switch (idx)
                {
                    case 0: return "emx";
                    case 1: return "e1x";
                    case 2: return "e2x";
                    case 3: return "e3x";
                    case 4: return "e4x";
                    case 5: return "e5x";
                    case 6: return "e6x";
                    case 7: return "e7x";
                    default:
                        Debugger.Error("Layout index out of bounds");
                        return "emx";
                }
            }
            else
            {
                switch (idx)
                {
                    case 0: return "lmx";
                    case 1: return "l1x";
                    case 2: return "l2x";
                    case 3: return "l3x";
                    case 4: return "l4x";
                    case 5: return "l5x";
                    case 6: return "l6x";
                    case 7: return "l7x";
                    default:
                        Debugger.Error("Layout index out of bounds");
                        return "lmx";
                }
            }
        }

        /// <summary>
        ///     Gets the layout variable name for y.
        /// </summary>
        public string GetLayoutVariableNameY(int idx, bool editMode)
        {
            if (editMode)
            {
                switch (idx)
                {
                    case 0: return "emy";
                    case 1: return "e1y";
                    case 2: return "e2y";
                    case 3: return "e3y";
                    case 4: return "e4y";
                    case 5: return "e5y";
                    case 6: return "e6y";
                    case 7: return "e7y";
                    default:
                        Debugger.Error("Layout index out of bounds");
                        return "emy";
                }
            }
            else
            {
                switch (idx)
                {
                    case 0: return "lmy";
                    case 1: return "l1y";
                    case 2: return "l2y";
                    case 3: return "l3y";
                    case 4: return "l4y";
                    case 5: return "l5y";
                    case 6: return "l6y";
                    case 7: return "l7y";
                    default:
                        Debugger.Error("Layout index out of bounds");
                        return "l1x";
                }
            }
        }
        
        /// <summary>
        ///     Gets the edit mode layout variable name for x.
        /// </summary>
        public string GetTrapDraftLayoutVariableName(int idx, bool draft)
        {
            if (draft)
            {
                switch (idx)
                {
                    case 0: return "trapd_draft";
                    case 1: return "trapd_draft_war";
                    case 2: return "trapd_d2";
                    case 3: return "trapd_d3";
                    case 4: return "trapd_d4";
                    case 5: return "trapd_d5";
                    case 6: return "trapd_d6";
                    case 7: return "trapd_d7";
                    default:
                        Debugger.Error("Layout index out of bounds");
                        return "trapd_draft";
                }
            }
            else
            {
                switch (idx)
                {
                    case 0: return "trapd";
                    case 1: return "trapd_war";
                    case 2: return "trapd2";
                    case 3: return "trapd3";
                    case 4: return "trapd4";
                    case 5: return "trapd5";
                    case 6: return "trapd6";
                    case 7: return "trapd7";
                    default:
                        Debugger.Error("Layout index out of bounds");
                        return "trapd";
                }
            }
        }

        /// <summary>
        ///     Gets the edit mode layout variable name for x.
        /// </summary>
        public string GetAirModeLayoutVariableName(int idx, bool draft)
        {
            if (draft)
            {
                switch (idx)
                {
                    case 0: return "air_mode_draft";
                    case 1: return "air_mode_draft_war";
                    case 2: return "air_mode_d2";
                    case 3: return "air_mode_d3";
                    case 4: return "air_mode_d4";
                    case 5: return "air_mode_d5";
                    case 6: return "air_mode_d6";
                    case 7: return "air_mode_d7";
                    default:
                        Debugger.Error("Layout index out of bounds");
                        return "air_mode_draft";
                }
            }
            else
            {
                switch (idx)
                {
                    case 0: return "air_mode";
                    case 1: return "air_mode_war";
                    case 2: return "air_mode2";
                    case 3: return "air_mode3";
                    case 4: return "air_mode4";
                    case 5: return "air_mode5";
                    case 6: return "air_mode6";
                    case 7: return "air_mode7";
                    default:
                        Debugger.Error("Layout index out of bounds");
                        return "air_mode";
                }
            }
        }

        /// <summary>
        ///     Sets the position layout.
        /// </summary>
        public void SetPositionLayout(int idx, int x, int y)
        {
            Debugger.DoAssert(idx < 8, "Layout index out of bands");
            this._layoutPosition[idx].Set(x, y);
        }

        /// <summary>
        ///     Sets the edit mode position layout.
        /// </summary>
        public void SetEditModePositionLayout(int idx, int x, int y)
        {
            Debugger.DoAssert(idx < 8, "Layout index out of bands");
            this._editModeLayoutPosition[idx].Set(x, y);
        }
    }
}