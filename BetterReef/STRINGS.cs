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

                public static LocString UWATERVENTEMIT = "{0} Emit Rate: {1}/s";
                public static LocString UWATERVENTDURATION = "Build-up Time: {0}";
                public static LocString UNDERWATERVENT_SHEARING = "{0} Build-up: {1}";
            }
        }

        public class MISC
        {
            public class STATUSITEMS
            {
                public class UNDERWATERVENTBUILDUPPROGRESS
                {
                    public static LocString NAME = "{ELEMENT} build-up: {PERCENTAGE}";

                    public static LocString TOOLTIP = string.Concat(new string[]
                    {
                        "This geyser will become blocked when ",
                        global::STRINGS.UI.PRE_KEYWORD,
                        "{ELEMENT}",
                        global::STRINGS.UI.PST_KEYWORD,
                        " build-up reaches 100%\n\nIt can be unblocked by building a ",
                        global::STRINGS.UI.PRE_KEYWORD,
                        "Marine Drill",
                        global::STRINGS.UI.PST_KEYWORD,
                        " on top of the vent"
                    });
                }
                
                public class UNDERWATERVENTBLOCKED
                {
                    public static LocString TOOLTIP = string.Concat(new string[]
                    {
                        "This geyser is blocked by ",
                        global::STRINGS.UI.PRE_KEYWORD,
                        "{ELEMENT}",
                        global::STRINGS.UI.PST_KEYWORD,
                        " build-up\n\nIt will resume function when unblocked by a ",
                        global::STRINGS.UI.PRE_KEYWORD,
                        "Marine Drill",
                        global::STRINGS.UI.PST_KEYWORD
                    });
                }
            }
        }

        public class CREATURES
        {
            public class SPECIES
            {
                public class GEYSER
                {
                    public class UNDERWATERVENT
                    {
                        public static LocString DESC = string.Concat(new[]
                        {
                            "A subaquatic fissure that allows high-pressure ",
                            global::STRINGS.UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
                            " to escape from the planet's crust.\n\nBlockages caused by built-up ",
                            global::STRINGS.UI.FormatAsLink("Solid", "ELEMENTS_SOLID"),
                            " can be removed using a ",
                            global::STRINGS.UI.FormatAsLink("Marine Drill", "UNDERWATERVENTDRILL"),
                            "."
                        });
                    }
                }
            }
        }
    }
}