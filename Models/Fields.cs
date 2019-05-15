namespace TbiDesktop.Models
{
    public class Fields
    {
        public string location { get; set; }
        public double? operational_time { get; set; }
        public double? surface { get; set; }
        public double? surface_material { get; set; }
        public double? ambient_temp { get; set; }
        public double? surface_temp { get; set; }
        public double? medium_temp { get; set; }
        public double? length { get; set; }
        public double? number { get; set; }
        public double? dimension { get; set; }
        public double? emissivity { get; set; }
        public string leakage { get; set; }
        public string comment { get; set; }
        public double? nominal_diameter { get; set; }
        public double? main_dimension { get; set; }
        public int? damaged_cladding_selection { get; set; } = 4;
        public int? damaged_insulation_selection { get; set; } = 4;
        public string damaged_comment { get; set; }
        public bool space_warning { get; set; }

        public double operational_time_index { get; set; } = 0;
        public int surface_material_index { get; set; } = 0;
        private bool _condensation_ice_block = false;
        public bool condensation_ice_block
        {
            get
            {
                return this._condensation_ice_block;
            }
            set
            {
                this._condensation_ice_block = value;
                if (!!value)
                {
                    _condensation_wet_surface = false;
                }
            }
        }


        private bool _condensation_wet_surface = false;
        public bool condensation_wet_surface
        {
            get
            {
                return _condensation_wet_surface;
            }
            set
            {
                _condensation_wet_surface = value;
                if (!!value)
                {
                    this.condensation_ice_block = false;
                }
            }
        }
        
        public string condensation_comment { get; set; }
        public bool unknow_surface { get; set; } = false;
        public double unknow_surface_temp { get; set; } = 0;

        private bool _damaged_cladding = false;
        public bool damaged_cladding
        {
            get
            {
                return _damaged_cladding;
            }
            set
            {
                _damaged_cladding = value;
                if (!value)
                    damaged_cladding_selection = null;
                else
                    damaged_cladding_selection = 1;
            }
        }

        private bool _damaged_insulation { get; set; } = false;
        public bool damaged_insulation
        {
            get
            {
                return _damaged_insulation;
            }
            set
            {
                _damaged_insulation = value;
                if (!value)
                    damaged_insulation_selection = null;
                else
                    damaged_insulation_selection = 1;
            }
        }
    }
}