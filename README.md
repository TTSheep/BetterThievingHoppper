
# Better Thieving Hopper（偷诅咒的草蜢）

一个用于《杀戮尖塔 2》（Slay the Spire 2）的轻量代码模组，调整原版怪物 **偷窃草蜢（Thieving Hopper）** 的偷牌优先级。

本模组只替换草蜢内部的优先级判断表，不重写原版的偷牌动画、战斗随机数、移牌、伤害、逃跑或联机同步流程。

## 功能

草蜢仍会从原版允许的候选牌中选择目标，但会按照以下顺序寻找第一个存在候选牌的层级。同一层级有多张牌时，仍由原版战斗 RNG 随机选择。

| 优先级 | 目标牌 |
| ---: | --- |
| 1 | 不带“虚无”（Ethereal）关键词的诅咒牌 |
| 2 | 带“虚无”（Ethereal）关键词的诅咒牌 |
| 3 | 没有附魔的初始牌 |
| 4 | 有附魔的初始牌 |
| 5 | 没有“注能”（Imbued）附魔的罕见牌 |
| 6 | 没有“注能”附魔的普通牌、稀有牌和事件牌 |
| 7 | 没有“注能”附魔的基础牌和任务牌 |
| 8 | 带“注能”附魔的牌或先古牌 |

如果八个层级均未命中，原版逻辑会从其完整候选列表中兜底选择。

> 当前版本没有把消耗牌堆（Exhaust pile）加入候选牌堆。相关功能仍在评估中。

## 安装

将发布包中的下列文件放进同一个模组目录：

```text
<Slay the Spire 2>/mods/BetterThievingHopper/
├── BetterThievingHopper.dll
└── BetterThievingHopper.json
```

`BetterThievingHopper.pdb` 仅用于调试，可以不安装。

启动游戏后，可在日志中确认以下信息：

```text
BetterThievingHopper - 加载成功!
```

Windows 日志通常位于：

```text
%APPDATA%\SlayTheSpire2\logs\godot.log
```

## 从源码构建

### 环境要求

- .NET 9 SDK
- Godot/MegaDot .NET SDK `4.5.1`
- 从你合法拥有的本地游戏/模组运行环境取得的 `sts2.dll`
- 与游戏运行环境匹配的 `0Harmony.dll`

把两个仅供编译引用的程序集放入：

```text
libs/
├── sts2.dll
└── 0Harmony.dll
```

然后执行：

```powershell
dotnet build BetterThievingHoppper.csproj -c Release
```

构建产物会复制到 `build/`：

```text
build/
├── BetterThievingHopper.dll
├── BetterThievingHopper.pdb
└── BetterThievingHopper.json
```

### 依赖文件规则

`sts2.dll` 和 `0Harmony.dll` 仅用于本地编译：

- 不要提交到 Git；
- 不要放入发布包；
- 不要从本仓库或其他未经授权的位置分发；
- 项目已通过 `.gitignore` 忽略 `libs/*.dll`；
- 项目引用设置为 `Private=false`，构建时不会把它们复制到发布目录。

## 实现方式

入口由游戏的 `ModInitializer` 调用。初始化时，模组使用 Harmony 的 `AccessTools.StaticFieldRefAccess` 获取：

```text
MegaCrit.Sts2.Core.Models.Monsters.ThievingHopper._stealPriorities
```

随后将原版四项数组整体替换为八项谓词数组。原版 `ThieveryMove` 会根据数组实际长度遍历，因此不需要复制或重写完整的异步偷牌方法。

这种方式改动范围较小，但依赖游戏私有字段；游戏更新若改变字段名称、类型或偷牌实现，模组可能需要同步更新。

## 兼容性

### 其他模组

- 其他模组若也替换 `_stealPriorities`，最终结果取决于加载顺序，后加载者会覆盖先加载者。
- 已发现一种边界情况：其他模组在战斗中永久移除牌组卡牌，却没有清理战斗副本的 `DeckVersion` 引用时，草蜢仍可能把该战斗副本视为可偷目标。随后原版 `SwipePower.Steal()` 再次移除永久牌时，可能报错或卡死战斗。
- 该问题不能只靠优先级谓词彻底规避，因为所有优先级均未命中时原版仍会使用完整候选列表兜底。

遇到问题时，请先只启用本模组复现，并附上最新的 `godot.log`。

### 多人游戏

本模组会影响实际战斗行为，清单中已设置：

```json
"affects_gameplay": true
```

多人游戏参与者应使用相同版本和相同模组配置，避免行为或校验结果不一致。

### ARM64 与跨平台

项目以 `AnyCPU` 构建，不包含 P/Invoke 或本机代码，发布 DLL 的静态检查结果为：

```text
Machine: I386
PE kind: PE32
CLR flags: ILOnly
Processor architecture: None
Managed native header: 0
```

其中 `I386 + PE32 + ILOnly` 是标准 AnyCPU 托管程序集，不代表只能运行于 x86。该 DLL 在静态层面兼容 Windows、Linux 与 macOS ARM64 的 .NET 运行时。

不过，静态兼容不能替代 Apple Silicon Mac 实机测试。发布前仍建议在 ARM64 原生进程中启动游戏并实际触发一次草蜢偷牌。

## 项目结构

```text
BetterThievingHopper.cs          模组初始化入口
Patch/ThievingHopperPatch.cs     偷牌优先级替换逻辑
BetterThievingHoppper.csproj     .NET/Godot 项目配置
build/BetterThievingHopper.json  模组清单
```

## 免责声明

本项目是非官方玩家模组，与 Mega Crit 无隶属或认可关系。《杀戮尖塔 2》及相关名称、代码和资源的权利归其各自权利人所有。
