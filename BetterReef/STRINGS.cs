namespace BetterReef
{
    public class STRINGS
    {
        public class UI
        {
            public class BUILDINGEFFECTS
            {
                public static LocString TIDALINRATE = "Inhale Rate: {0}/s";
                public static LocString TIDALEXRATE = "Exhale Rate: {0}/s";
                public static LocString TIDALSTORAGE = "Total Storage: {0}";
                public static LocString TIDALPEKGEN = "Peak Power Generation: {0}";
                public static LocString TIDALAVGGEN = "Average Power Generation: {0}";

                public static LocString UWATERVENTEMIT = "Emitting Rate: {0}/s";
                public static LocString UWATERVENTDURATION = "Build-up Time: {0}";
                public static LocString UWATERVENTSOLID = string.Concat(new string[]
                {
                    "Actual ",
                    global::STRINGS.UI.FormatAsLink("Sulfur", "SULFUR"),
                    " Build-up: {0}",
                });
            }
        }
    }
}