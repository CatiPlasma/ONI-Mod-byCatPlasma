# CatPlasma 的《缺氧》Mod 仓库

欢迎来到 **ONI-Mod-byCatPlasma**！

这是一个用于开发 **《缺氧（Oxygen Not Included）》** Mod 的源码仓库，包含多个独立的 Mod 项目以及共享工具库。

本仓库旨在提供高质量、易维护且兼容最新游戏版本的 Mod，为玩家带来更多玩法与体验。

---

# 当前项目

目前仓库包含以下项目：

| 项目 | 名称 | 类型 | 简介 | 订阅 |
| ---- | ------ | ------ | ---- | ------ |
| [BetterReef](./BetterReef) | 水生星随机化拓展 | Mod | 随机化潮汐泉与热气裂隙数据，并将潮汐发电机发电量与潮汐泉喷发量关联。 | [创意工坊](https://steamcommunity.com/sharedfiles/filedetails/?id=3747647080) |
| [ChineseFood](./ChineseFood) | 中式美食 | Mod | 为游戏添加中式菜品。 | 未发布 |
| [CatUtilLib](./CatUtilLib) | 🐱库 | 公共库 | 多个 Mod 共用的工具库，提供日志、本地化、字符串及常用工具。 |  |

随着开发进行，仓库中的项目可能持续增加。

---

# 仓库结构

```text
ONI-Mod-byCatPlasma
│
├── BetterReef/          # BetterReef Mod
│   ├── Release/
│   ├── ModEntry.cs
│   ├── Patches.cs
│   └── ...
│
├── ChineseFood/         # ChineseFood Mod
│   ├── Release/
│   ├── ItemConfigs/
│   ├── ModEntry.cs
│   └── ...
│
├── CatUtilLib/          # 公共工具库
│
├── Libs/                # 外部依赖
│
├── ONI-Mod-byCatPlasma.sln
└── LICENSE
```

---

# 开发环境

推荐开发环境：

- JetBrains Rider 或 Visual Studio 2022
- C# 8.0
- .NET Standard 2.1 (.NET 10)
- Harmony
- Oxygen Not Included Mod SDK

游戏最低支持版本请参考各 Mod 中的 `mod_info.yaml`。

---

# 编译

1. Clone 本仓库

```bash
git clone https://github.com/CatiPlasma/ONI-Mod-byCatPlasma.git
```

2. 使用 Rider 或 Visual Studio 打开

```
ONI-Mod-byCatPlasma.sln
```

3. 编译需要的 Mod 项目。

4. 将编译后的文件放入：

```
Documents\Klei\OxygenNotIncluded\mods\Dev
```

或者直接使用各项目 `Release` 目录中的内容进行测试。

---

# 本地化

本仓库支持本地化开发。

翻译文件位于各 Mod 的：

```
Release/Translations/
```

欢迎帮助完善不同语言的翻译。

---

