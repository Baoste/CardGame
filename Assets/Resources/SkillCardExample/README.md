# 技能卡说明
---

### 卡牌整体概念
在程序中，SkillCard 可以理解为：一张卡牌的完整配置数据。也就是说，在设计一张卡时，通常需要确定这张卡的：
- 它是谁
- 它叫什么
- 它是做什么的
- 牌堆里一开始会放多少张
- 它触发后会产生哪些效果

### 字段说明
1. **id**
**含义**：卡牌唯一编号。这是每张卡牌的<u>唯一</u>标识，用来区分不同卡牌，<u>绝对不能重复</u>。
**举例**：1563 = 抽技能牌，1002 = 抽点数牌
2. **name**
**含义**：卡牌名称。这是玩家能直接看到的卡牌名字，用于界面展示、卡牌介绍、日志提示等，也就是卡牌面向用户的正式名称。
**举例**：点数转转转，卡牌狗带
3. **description**
**含义**：卡牌描述文本。用于向玩家说明这张卡牌的效果、用途或特殊规则。一般是卡面上显示的说明文字，主要负责告诉玩家这张卡做了什么。
**举例**：抽取一张技能牌，选择场上的一张点数牌将其摧毁
4. **point**
**含义**：卡牌点数。表示这张卡的点数值，技能牌现在默认都是 0。
5. **type**
**含义**：卡牌类型。用于定义这张卡属于哪一类，技能牌为 Skill。
6. **count**
**含义**：初始牌堆中该卡的数量。表示游戏开始时，这张卡会以多少张的数量被放入牌堆。
**举例**：count = 1：整副牌里只有1张，非常稀有
7. **effects**
**含义**：卡牌效果配置。这是卡牌最核心的部分，它定义了这张卡被使用、触发或结算时，具体会发生哪些事情。当前程序里，它是一个效果二叉树，也就是说，一张卡可以按顺序或者按分支执行多个效果。
**举例**：
    - 顺序结构：弃置场上一张点数牌 → 抽一张点数牌 → 将抽到的牌放置在任意一方
    - 分支结构：抽一张牌 → 判断点数
     → 如果大于等于5 → 玩家抽一张牌
     → 如果小于5 → 玩家删一张牌

### 卡牌效果配置说明
本系统中，卡牌的具体行为由 **效果类型**（EffectType）决定。每一种效果类型，代表一种基础操作能力，卡牌通过组合这些效果来实现玩法。目前实现的效果类型有：
1. **DrawPoint** —— 抽点数牌
**含义**：从“点数牌牌堆”中抽取若干张牌，加入玩家场上。
2. **DrawSkill** —— 抽技能牌
**含义**：从“技能牌牌堆”中抽取若干张牌，加入玩家手牌。
3. **DrawPointToResolve** —— 抽点数牌到待处理区
**含义**：从点数牌堆抽若干张牌，但不进入手牌，而是进入“待处理区”。类似“翻开一张牌用于判定/结算”，玩家不直接操作这张牌，往往用于后续处理，例如让玩家后续选择待处理去的一张牌置于场上，或者根据待处理区的牌的点数进行后续分支判断。
4. **Discard** —— 弃牌
**含义**：将指定卡牌移入弃牌堆。
5. **ModifyPoint** —— 改变点数
**含义**：修改若干张卡牌的点数（增加或减少）。
6. **Move** —— 移动卡牌
**含义**：将卡牌从一个区域移动到另一个区域。例如将待处理区的牌移到自己/对方场上，将对方的技能牌移到自己手里。<u>注意，所有从牌堆抽牌的操作，在抽象上虽然也是移动，但必须用 Draw 系列的效果，而不能使用 Move，特别是抽点数牌到待处理区</u>。
7. **Judge** —— 判断（条件判定）
**含义**：进行一次条件判断，并根据结果执行不同效果。这是未来“二叉树效果”的核心节点。

---

### 给策划：如何设计一张技能牌
**首先**，要写好卡牌的基础文本设计，即 id，name，description，count。
**然后**，需要帮助程序写一下卡牌效果配置，如果卡牌效果只是简单地按顺序执行，那么可以写成一个线性序列。但如果卡牌效果中包含“如果……则……”这种条件判断，就不能只用普通列表来表示，而应该使用树状结构，更准确地说，是一种带分支的流程结构。例如：
1. 抽一张技能牌
    ```mermaid
    flowchart TD
        A[开始] --> B[抽一张技能牌]
        B --> C[结束]
    ```
2. 弃置牌面上的一张点数牌，抽一张牌到待处理区，玩家选择放置在任意一方
    ```mermaid
    flowchart TD
        A[开始] --> B[弃置牌面上的一张点数牌]
        B --> C[抽一张牌到待处理区]
        C --> D[玩家选择放置到任意一方]
        D --> E[结束]
    ```
3. 抽一张牌到待处理区，如果大于等于5，则抽牌，否则弃置牌面上的一张点数牌
    ```mermaid
    flowchart TD
        A[开始] --> B[抽一张牌到待处理区]
        B --> C{该牌点数 >= 5 ?}
        C -->|是| D[抽一张牌]
        C -->|否| E[弃置牌面上的一张点数牌]
        D --> F[结束]
        E --> F
    ```

---
### 给程序：如何序列化效果配置

##### 整体概念
在当前卡牌系统中，一张卡的效果不是写死在代码里的，而是通过一组可配置的 `EffectOp` 来描述。也就是说：一张卡牌 = 基础信息 + 效果执行结构。
其中效果执行结构定义为：
```cs
public List<EffectOp> effects;
```
这意味着，一张卡牌的效果本质上是一个 `EffectOp` 节点列表。虽然底层存储形式是 `List`，但逻辑上它并不是单纯的线性序列，而是一个用数组下标表示节点连接关系的二叉树。`effects[i]` 是一个效果节点，普通节点执行后，通常进入下一个节点，判断节点会根据结果跳转到 `trueNode` 或 `falseNode`，因此整个效果结构本质上是一棵“列表存储的二叉树”。

##### EffectOp：效果节点
`EffectOp` 是整个效果系统的最小执行单元，结构如下：
```cs
public class EffectOp
{
    public EffectType type;
    public int trueNode = -1;
    public int falseNode = -1;
    public ParticipantSpec source;
    public ParticipantSpec target;
    public ValueExpr value;
}
```
1. `type`：表示当前节点是什么类型的效果，决定“这个节点要做什么”。
2. `trueNode`：当且仅当前节点的类型是 `EffectType:Judge`时，如果判断结果为真，则跳转到这个下标对应的节点，-1 表示结束。**如果是顺序执行，trueNode 必须改成下一个节点的下标，不能不改！因为要是两个节点都是 -1，会判断为结束！**
3. `falseNode`：当且仅当前节点的类型是 `EffectType:Judge`时，如果判断结果为假，则跳转到这个下标对应的节点，-1 表示结束。
4. `source`：表示当前效果的“来源参与者”，例如从自己场上选择牌，具体见[下一节](#ParticipantSpec-config)。
5. `target`：表示当前效果的“目标参与者”，例如放到对方场上，具体见[下一节](#Value-config)。
6. `value`：表示该效果节点所需的数值参数，例如抽几张牌，修改几点数。具体见[下下一节]()。

##### ParticipantSpec：参与者描述 {#ParticipantSpec-config}
当前效果要从哪里选对象，以及怎么选。它并不直接保存具体卡牌，而是保存一套“选取规则”。也就是说，`ParticipantSpec` 不是结果，而是“查找目标的方法”。
```cs
public class ParticipantSpec
{
    public ParticipantType participantType;
    public ConditionExpr filter;
    public ParticipantSelectionMode participantSelectionMode;
    public ValueExpr maxSelectCount;
}
```
1. `ParticipantType`：表示选择范围属于哪一个参与区域，它决定了“从哪一类集合里找目标”。目前设计为：
    ```cs
    public enum ParticipantType
    {
        None                        = 0,        // 无，保留
        MySkillCardsInHand          = 1 << 0,   // 自己的手牌
        OpponentSkillCardsInHand    = 1 << 1,   // 对手的手牌
        MyPointCardsOnBoard         = 1 << 2,   // 自己场上的牌，不包括底牌
        OpponentPointCardsOnBoard   = 1 << 3,   // 对手场上的牌，不包括底牌
        SkillCardsInDeck            = 1 << 4,   // 牌堆的技能牌
        PointCardsInDeck            = 1 << 5,   // 牌堆的点数牌
        CardsToResolve              = 1 << 6,   // 要继续解决的牌
        MyBoardZone                 = 1 << 7,   // 自己的点数牌区域
        OppentBoardZone             = 1 << 8,   // 对方的点数牌区域
    }
    ```
2. `filter`：表示对候选对象进一步筛选。`participantType` 只定义大范围，`filter` 用于进一步缩小范围。例如只选点数 >= 5 的牌。
3. `participantSelectionMode`：表示从最终过滤后的候选集合中如何选择目标。目前设计为：
    ```cs
    ParticipantSelectionMode
    ├── SelectionModeNone       // 返回空，但检验永远过
    ├── SelectionModeAll        // 返回集合里的所有
    ├── SelectionModeChoose     // 玩家选择若干张
    ├── SelectionModeFirst      // 返回集合里的第一张
    ├── SelectionModeLast       // 返回集合里的最后一张
    ├── SelectionModeRandom     // 随机选择集合里的若干张
    ├── SelectionModeMin        // 返回集合里点数最小的
    └── SelectionModeMax        // 返回集合里点数最大的
    ```
4. `maxSelectCount`：表示最多能选多少个目标。和 `participantSelectionMode` 配合使用，控制选择数量，当且仅当选择模式为 `SelectionModeChoose`和 `SelectionModeRandom` 时生效。

##### ValueExpr：数值表达式 {#Value-config}
`ValueExpr` 用于描述卡牌效果中的“数值参数”，例如抽牌数量、点数变化值、判断阈值等。它并不只是一个固定的 `int`，而是一个可扩展的数值表达结构。
**`EffectOp` 下的 `value` 仅用于以下类型的效果**：
- 抽牌数量（Draw）
- 弃牌数量（Discard）
- 修改点数（ModifyPoint）

将数值抽象为表达式，而不是直接写成 int，主要是为了支持以下能力：

1. 支持常量。例如：抽 2 张牌，点数 +3
2. 支持动态取值。例如：抽牌数量 = 当前手牌数，增加点数 = 已选中目标牌点数总和
3. 支持组合计算。例如：抽牌数量 = 己方场上点数牌数量 + 1

当前 ValueExpr 一共包含 4 类实现：
```cs
ValueExpr
├── NoneValue        // 无值
├── ConstValue       // 常量
├── VariableValue    // 读取变量
└── BinaryValue      // 二元运算
```
其中 `VariableValue` 指从当前局面或当前效果上下文中读取一个动态值。它的作用是不直接写死数值，而是从 `GameState` 或 `EffectContext` 中读取。当前支持的变量来源：
```cs
ValueSource
├── CasterSkillCardsCount           // 施法者当前手中的技能牌数量
├── CasterPointCardsCount           // 施法者当前场上的点数牌数量
├── SourceSpecSelectedPointsSum     // 当前 source 选中牌的点数总和
├── TargetSpecSelectedPointsSum     // 当前 target 选中牌的点数总和
└── ResolvedCardsPointsSum          // 当前待处理区牌的点数总和
```

##### 具体效果配置示例
1. 从技能牌堆抽取 2 张牌
    ```cs
    type = DrawSkill,
    trueNode = -1,
    falseNode = -1,
    // source
    source.participantType = None
    source.filter = None，
    source.participantSelectionMode = None
    source.maxSelectCount = None
    // target
    target.participantType = MySkillCardsInHand
    target.filter = None，
    target.participantSelectionMode = None
    target.maxSelectCount = None
    // value
    value = ConstValue:2
    ```
    `trueNode / falseNode = -1` 表示仅有一个节点。
    `source` 全部为空配置，因为抽牌是全自动的：
    - 没有选牌行为
    - 不需要过滤
    - 服务器自动选择

    `target.participantType` 表示抽到的牌将进入“自己手牌”；
    `value` 表示抽牌数量等于 2 。

2. 抽一张牌到待处理区；若该牌点数大于等于 5，则再抽 1 张点数牌，否则选择弃置场上的一张牌。
    ```cs
    // effects[0]
    type = DrawPointToResolve,
    trueNode = 1,
    falseNode = -1,
    // source
    source.participantType = None
    source.filter = None
    source.participantSelectionMode = None
    source.maxSelectCount = ConstValue:1
    // target
    target.participantType = None
    target.filter = None
    target.participantSelectionMode = None
    target.maxSelectCount = None
    // value
    value = None
    ```
    ```cs
    // effects[1]
    type = Judge,
    trueNode = 2,
    falseNode = 3,
    // source
    source.participantType = None
    source.filter = CompareCondition(
        left = VariableValue:ResolvedCardsPointsSum,
        right = ConstValue:5,
        op = GreaterOrEqual
    )
    source.participantSelectionMode = None
    source.maxSelectCount = None
    // target
    target.participantType = None
    target.filter = None
    target.participantSelectionMode = None
    target.maxSelectCount = None
    // value
    value = None
    ```
    ```cs
    // effects[2]
    type = DrawPoint,
    trueNode = -1,
    falseNode = -1,
    // source
    source.participantType = None
    source.filter = None
    source.participantSelectionMode = None
    source.maxSelectCount = None
    // target
    target.participantType = None
    target.filter = None
    target.participantSelectionMode = None
    target.maxSelectCount = None
    // value
    value = ConstValue:1
    ```
    ```cs
    // effects[3]
    type = Discard,
    trueNode = -1,
    falseNode = -1,
    // source
    source.participantType = None
    source.filter = None
    source.participantSelectionMode = None
    source.maxSelectCount = None
    // target
    target.participantType = MyPointCardsOnBoard | OpponentPointCardsOnBoard
    target.filter = All
    target.participantSelectionMode = Choose
    target.maxSelectCount = ConstValue:1
    // value
    value = None
    ```
    这是一个 **4 节点的效果树**，执行顺序通过 `trueNode / falseNode` 控制：
    - `effects[0]`：抽 1 张牌到待处理区，然后跳转到节点 1  
    - `effects[1]`：进行判断（点数 >= 5）  
        - True → 跳转到节点 2  
        - False → 跳转到节点 3  
    - `effects[2]`：抽 1 张点数牌，结束  
    - `effects[3]`：选择 1 张牌并弃置，结束  

    `trueNode / falseNode` 用于构建分支逻辑：
    - 非判断节点时，通常只使用 `trueNode` 作为“下一步”
    - 判断节点根据结果跳转不同节点

    `effects[0]` 中：
    - `source` 为空，因为抽牌是系统自动执行的行为  
    - `maxSelectCount = 1` 表示只抽 1 张牌  

    `effects[1]` 是核心判断节点：
    - `VariableValue:ResolvedCardsPointsSum` 表示读取待处理区牌的点数和  
    - 与 `ConstValue:5` 比较  
    - `GreaterOrEqual` 表示条件为 “>= 5”  

    在本例中等价于：

    > 判断刚抽到待处理区的那张牌点数是否 >= 5  

    `effects[2]`（真分支）：
    - 抽 1 张点数牌  
    - `value = 1` 控制数量  

    `effects[3]`（假分支）：
    - 从目标范围中选择 1 张牌并弃置  
    - `participantType = MyPointCardsOnBoard | OpponentPointCardsOnBoard` 表示目标范围是双方场上点数牌  
    - `SelectionMode = Choose` 表示需要玩家选择  
    - `maxSelectCount = 1` 表示最多选 1 张  