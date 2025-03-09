# Unbbal-Assignment
 운빨존많겜 클로닝 과제입니다.

 영상의 경우 도박 기능을 보여주기 위해 행운석을 100개로 시작하였습니다.

 싱긓톤 페턴은 최대한 사용 자제하였습니다.

코드 구조
---
+ **유닛**

영웅 유닛들의 이동, 공격을 별도의 클래스로 분리하고, 영웅 클래스의 각 이벤트에 등록하는 방식으로 구현하였습니다.

이를 통해 영웅 유닛들 별로 기능을 추가, 제거가 가능합니다.

> 예시 : 근접 공격 유닛의 경우 MeleeAttackHandler Component를 추가, 활 공격 유닛의 경우 BowAttackHandler Component 추가 등, 공격 수단에 맞게 Component를 추가 제거하는 방식으로 기능이 작동하도록 구현하였습니다.

이를 통해 각 객체 간 결합도를 최대한 낮추고 유연성을 늘렸습니다.

+ **MVP 패턴**

MVP 패턴을 사용하여 Model, View, Presnter로 나눠 View와 Model의 결합도를 낮추었습니다. Model의 경우 오프라인으로 구현하는게 목적이므로 Scriptable Object를 활용하여 모델을 구성하였습니다.

Presenter는 각 모델과 View들을 참조하였습니다. View의 경우, 입력(버튼 클릭 등) 및 변화를 이벤트로 구독하는 방식을 사용하여 Presnter를 알지 못하더라도, 작동이 되도록 이벤트 리스너 방식을 사용하였습니다. 

따라서 View Class에는 View를 업데이트 하는 코드만 존재하며, 필요한 데이터 가공 처리 및 제공은 Presenter가 담당합니다.


필수 구현
---

보통 난이도 모드 (오프라인 - 싱글플레이) 환경으로 게임을 구현하였습니다.

유닛 소환, 도박, 합성, 신화 유닛 조합, 판매가 가능합니다.

유닛 클릭 시 판매, 조합이 가능하며, 동일한 유닛이 3마리 모일 경우 상위 등급 조합이 가능하다면 조합이 가능합니다.

유닛 클릭 시 해당 영웅의 사거리가 표시됩니다.

유닛은 일반/ 희귀/ 영웅 등급별 2종, 조합으로 얻을 수 있는 신화 2종을 구현하였습니다.

몬스터는 일반/ 보스 몬스터가 등장하며, 영상 길이 및 과제임을 감안하여 10스테이지 까지 데이터를 입력하였고, 각 5스테이지, 10스테이지에서 보스가 등장합니다.

보스는 일반 몬스터보다 크기가 크며, 체력이 많습니다.

상대 플레이어 동작 로직(AI)를 구현하였습니다.

골드가 충족되어 소환이 가능하면 골드를 소모해 영웅을 소환합니다. 신화로 조합이 가능한 유닛이 모였다면, 신화 유닛을 조합합니다. 동일한 유닛이 세마리 모였고, 상위 등급으로 조합이 가능하면 조합을 합니다.

AI의 각 행동은 0.3초의 인터벌을 주었습니다.

주의사항에 맞춰 같은 종류의 유닛의 경우 한 칸에 3마리가 우선 배치되며, 2마리씩, 혹은 1마리 따로 나뉘어 배치되는 경우는 없게하였습니다.

추가구현
---
+ **골드 획득 효과 기능**

DOTween 을 사용하여 골드 획득 시 자원 정보창 상단에 골드 획득 효과가 생성되도록 구현하였습니다.

+ **인구수 제한 기능**

영웅 인구수 제한 기능을 넣어 최대 인구수에 도달 할 경우 자원이 있어도 뽑기를 누를 경우 자원이 소모되지 않고 뽑히지 않도록 구현하였습니다.

+ **영웅 이동 기능**

마우스 다운(누르는 순간) 영웅 사거리 표시, 드래그 시 이동 가능한 타일 표시 및 방향 라인 렌더러 표시를 구현하였습니다.

마우스 업 시 드래그 하지 않았다면 판매, 조합 창이 뜨며, 드래그를 하였을 시 영웅을 드래그한 위치로 이동시킵니다.

영웅 이동 중, 해당 타일에 다른 영웅을 이동 시켜도 겹치지 않고 자연스럽게 바로 이동되도록 구현하였습니다.

+ **투사체 공격 이동 형태**
  
두 종류의 원거리 공격을 구현하였습니다.

하나는 화살 공격으로 포물선 형태로 공격하는 것을 구현하고, 다른 하나는 마법 공격으로 특정 지점에 공격하여 해당 지점 범위 내의 모든 적에게 피해를 주는 공격을 구현하였습니다.

원거리 공격의 경우 공격 타입의 변경으로 쉽게 포물선 공격, 범위 공격을 설정할 수 있습니다.

+ **영웅 소환 트레일 렌더러**

영웅 소환 시 각 영웅 등급에 맞는 색상(일반: 흰색, 희귀: 파란색, 영웅: 보라색, 신화:노란색)의 트레일 렌더러가 나와 소환되는 모습을 구현하였습니다.

+ **체력바 기능**

ShaderGraph를 사용하여 체력바를 구현하였습니다. 남은 체력량에 따라 최대 체력일때 초록색에서 빨간색으로 색상이 변하도록 구현하였습니다.

+ **오브젝트 풀**

골드 획득 시 생성되는 골드 획득 추가 효과가 자주 생성되고 파괴되므로 UnityEngine.Pool 을 사용하여 오브젝트 풀을 구현해 재사용 하였습니다.

+ **반응형 UI**
  
개인 프레임워크인 UIAdjuster를 사용하여 화면의 크기 상관 없이 1080 x 1920의 화면이 출력되도록 하였습니다.

Editor에서 Free Aspect로 변경 후 화면 크기를 변경하여도 1080 x 1920 의 화면비가 유지되며, 빈 공간은 Letter box로 채워지게 됩니다.

사용한 라이브러리
---

+ [UniTask](https://github.com/Cysharp/UniTask)

특정 대기, 순차 처리가 필요한 곳에 사용하였습니다.

+ [DOTween](https://dotween.demigiant.com/)

투사체 공격 및 영웅 이동, 몬스터 이동, UI효과 등 자연스러운 움직임이 필요한 곳에 사용하였습니다.

+ [UIEffect](https://github.com/mob-sakai/UIEffect)

버튼의 반짝이는 효과와 그림자 효과를 구현하는데 사용하였습니다.

사용한 에셋
---

+ [SPUM](https://assetstore.unity.com/packages/2d/characters/pixel-units-spum-bundle-pack-basic-192104)

영웅 캐릭터를 구현하는데 사용하였습니다.

사용한 툴
---

+ Unity
+ Adobe Photoshop
