using Chinese_Food.ItemConfigs.Foods;
using static STRINGS.UI;

namespace Chinese_Food
{
    public class STRINGS
    {
        public class FOOD
        {
            public class SWEETSOURRIB
            {
                public static LocString NAME = FormatAsLink("糖醋排骨", SweetAndSourRibsConfig.ID);
                public static LocString DESC = "复制人会得糖尿病吗？";
                public static LocString COOKSTATIONRECIEDESC = "希望复制人永远都不用清理做过这道菜的烤架";
            }

            public class BOILSPICEPORK
            {
                public static LocString NAME = FormatAsLink("水煮肉片", BoiledSpicyPorkConfig.ID);
                public static LocString DESC = "嘶哈，好辣！";
                public static LocString COOKSTATIONRECIPEDESC = "汤真的留得住吗？";
            }
        }
    }
}