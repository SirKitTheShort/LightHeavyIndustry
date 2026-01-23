namespace LightHeavyIndustry.Systems
{
    /// <summary>
    /// Economic and operational constants for Light Industry and Heavy Industry
    /// All MULTIPLIER values are relative to vanilla Industrial Manufacturing (1.0 = same as vanilla)
    /// Adjust these values to fine-tune the economic balance
    /// </summary>
    public static class EconomicConstants
    {
        // ==================== LIGHT INDUSTRY ====================

        // ----- PRODUCTION -----
        /// <summary>
        /// Light Industry production efficiency relative to vanilla
        /// 0.85 = produces 85% of vanilla output (lower revenue, lower taxes to city)
        /// </summary>
        public const float LIGHT_PRODUCTION_MULTIPLIER = 0.85f;

        // ----- UTILITIES & RESOURCES -----
        /// <summary>
        /// Light Industry electricity consumption
        /// 0.70 = uses 70% of vanilla power (less capital intensive)
        /// </summary>
        public const float LIGHT_ELECTRICITY_MULTIPLIER = 0.70f;

        /// <summary>
        /// Light Industry water consumption
        /// 0.75 = uses 75% of vanilla water
        /// </summary>
        public const float LIGHT_WATER_MULTIPLIER = 0.75f;

        /// <summary>
        /// Light Industry sewage output
        /// 0.75 = produces 75% of vanilla sewage
        /// </summary>
        public const float LIGHT_SEWAGE_MULTIPLIER = 0.75f;

        /// <summary>
        /// Light Industry garbage accumulation rate
        /// 0.50 = produces 50% of vanilla garbage (cleaner processes)
        /// </summary>
        public const float LIGHT_GARBAGE_MULTIPLIER = 0.50f;

        // ----- COSTS -----
        /// <summary>
        /// Light Industry upkeep costs
        /// 0.90 = 90% of vanilla upkeep (10% reduction - simpler machinery, but higher wages)
        /// </summary>
        public const float LIGHT_UPKEEP_MULTIPLIER = 0.90f;

        // ----- EDUCATION REQUIREMENTS -----
        /// <summary>Probability of uneducated workers in Light Industry (5%)</summary>
        public const float LIGHT_EDUCATION_UNEDUCATED = 0.05f;

        /// <summary>Probability of educated workers in Light Industry (20%)</summary>
        public const float LIGHT_EDUCATION_EDUCATED = 0.20f;

        /// <summary>Probability of well-educated workers in Light Industry (45%)</summary>
        public const float LIGHT_EDUCATION_WELL_EDUCATED = 0.45f;

        /// <summary>Probability of highly educated workers in Light Industry (30%)</summary>
        public const float LIGHT_EDUCATION_HIGHLY_EDUCATED = 0.30f;


        // ==================== HEAVY INDUSTRY ====================

        // ----- PRODUCTION -----
        /// <summary>
        /// Heavy Industry production efficiency relative to vanilla
        /// 1.15 = produces 115% of vanilla output (15% boost to make it competitive and realistic)
        /// Makes vanilla obsolete - why use vanilla when heavy produces more with same pollution?
        /// </summary>
        public const float HEAVY_PRODUCTION_MULTIPLIER = 1.15f;

        // ----- UTILITIES & RESOURCES -----
        /// <summary>
        /// Heavy Industry electricity consumption
        /// 1.0 = same as vanilla (can increase if you want even MORE capital intensity)
        /// </summary>
        public const float HEAVY_ELECTRICITY_MULTIPLIER = 1.00f;

        /// <summary>
        /// Heavy Industry water consumption
        /// 1.0 = same as vanilla
        /// </summary>
        public const float HEAVY_WATER_MULTIPLIER = 1.00f;

        /// <summary>
        /// Heavy Industry sewage output
        /// 1.0 = same as vanilla
        /// </summary>
        public const float HEAVY_SEWAGE_MULTIPLIER = 1.00f;

        /// <summary>
        /// Heavy Industry garbage accumulation rate
        /// 1.0 = same as vanilla
        /// </summary>
        public const float HEAVY_GARBAGE_MULTIPLIER = 1.00f;

        // ----- COSTS -----
        /// <summary>
        /// Heavy Industry upkeep costs
        /// 1.0 = same as vanilla (can increase if you want to emphasize capital intensity)
        /// </summary>
        public const float HEAVY_UPKEEP_MULTIPLIER = 1.00f;

        // ----- EDUCATION REQUIREMENTS -----
        /// <summary>Probability of uneducated workers in Heavy Industry (30%)</summary>
        public const float HEAVY_EDUCATION_UNEDUCATED = 0.30f;

        /// <summary>Probability of educated workers in Heavy Industry (40%)</summary>
        public const float HEAVY_EDUCATION_EDUCATED = 0.40f;

        /// <summary>Probability of well-educated workers in Heavy Industry (25%)</summary>
        public const float HEAVY_EDUCATION_WELL_EDUCATED = 0.25f;

        /// <summary>Probability of highly educated workers in Heavy Industry (5%)</summary>
        public const float HEAVY_EDUCATION_HIGHLY_EDUCATED = 0.05f;


        // ==================== VALIDATION ==================== 

        /// <summary>
        /// Validates that education probabilities sum to 1.0 (100%)
        /// Call this during initialization to catch configuration errors
        /// </summary>
        public static bool ValidateEducationProbabilities()
        {
            float lightSum = LIGHT_EDUCATION_UNEDUCATED + LIGHT_EDUCATION_EDUCATED +
                           LIGHT_EDUCATION_WELL_EDUCATED + LIGHT_EDUCATION_HIGHLY_EDUCATED;

            float heavySum = HEAVY_EDUCATION_UNEDUCATED + HEAVY_EDUCATION_EDUCATED +
                           HEAVY_EDUCATION_WELL_EDUCATED + HEAVY_EDUCATION_HIGHLY_EDUCATED;

            const float TOLERANCE = 0.001f;
            bool lightValid = System.Math.Abs(lightSum - 1.0f) < TOLERANCE;
            bool heavyValid = System.Math.Abs(heavySum - 1.0f) < TOLERANCE;

            if (!lightValid)
                Mod.log.Error($"Light Industry education probabilities sum to {lightSum} instead of 1.0!");

            if (!heavyValid)
                Mod.log.Error($"Heavy Industry education probabilities sum to {heavySum} instead of 1.0!");

            return lightValid && heavyValid;
        }
    }

    /// <summary>
    /// Pollution constants for Light Industry - easily adjustable
    /// (Heavy Industry uses vanilla pollution values: 100/100/100)
    /// </summary>
    public static class PollutionConstants
    {
        // Light Industry Pollution Values
        public const int LIGHT_INDUSTRY_AIR = 0;
        public const int LIGHT_INDUSTRY_GROUND = 5;
        public const int LIGHT_INDUSTRY_NOISE = 30;
    }
}