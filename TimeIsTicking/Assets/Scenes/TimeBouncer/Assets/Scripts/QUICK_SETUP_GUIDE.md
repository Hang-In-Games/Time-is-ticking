# TimeBouncer 게임 구조 및 체크리스트 (중앙 관리 방식)

## 🎯 게임 설계 개념

### ⭐ 중앙 관리 방식 (Manager-Centric Design)
```
기본 원칙:
- 개별 오브젝트에 스크립트 추가 X
- GameManager에 모든 오브젝트를 Public으로 할당
- Manager가 모든 로직 처리 (초기화, 입력, 물리, 경계 체크 등)

장점:
✅ 코드 중앙 집중 → 유지보수 쉬움
✅ 오브젝트 간 통신 불필요
✅ 설정 값 한 곳에서 관리
✅ 디버깅 편리
```

### 충돌 처리 구조
```
1. Ball ↔ Paddle
   → Unity Physics Engine 사용
   → CircleCollider2D ↔ BoxCollider2D 충돌
   
2. Ball ↔ 시계 외곽 (ClockBorder)
   → Manager가 수학적으로 경계 체크
   → ClockBorder는 시각적 표현만 (Collider 없음)
   
3. Paddle 입력 및 회전
   → Manager가 Input System 처리
   → Manager가 직접 Paddle 회전
```

### 필요한 스크립트
```
✅ GameManager_TimeBouncer.cs (필수)
   - 유일한 게임 로직 스크립트
   - 모든 오브젝트를 Inspector에서 할당받아 제어

❌ PaddleController.cs (불필요 - Manager가 처리)
❌ BallInitializer.cs (불필요 - Manager가 처리)
❌ CircleBoundary.cs (불필요 - Manager가 처리)
```

## 🏗️ 전체 구조

```
Scene Hierarchy:
├─ Main Camera
│  ├─ Projection: Orthographic
│  ├─ Size: 6
│  └─ Position: (0, 0, -10)
│
├─ ClockCenter (Empty GameObject)
│  └─ Position: (0, 0, 0)
│
├─ GameManager (Empty GameObject) ⭐ 핵심!
│  └─ GameManager_TimeBouncer (Script)
│     ├─ [Scene Objects]
│     │  ├─ Clock Center: ClockCenter (할당)
│     │  ├─ Clock Border: ClockBorder (할당)
│     │  ├─ Paddle: Paddle (할당)
│     │  └─ Ball: Ball (할당)
│     ├─ [Clock Settings]
│     │  └─ Clock Radius: 180
│     ├─ [Paddle Settings]
│     │  ├─ Paddle Rotation Speed: 180
│     │  └─ Use Player Input: ✓
│     ├─ [Ball Settings]
│     │  ├─ Ball Initial Speed: 300
│     │  └─ Boundary Bounciness: 0.8
│
├─ ClockBorder (Sprite) ← 시각적 표현만
│  ├─ Sprite Renderer
│  │  ├─ Sprite: ClockBorder.svg
│  │  └─ Material: SpriteMaterial_URP
│  └─ Position: (0, 0, 0)
│  └─ ❌ 스크립트 없음! ❌
│
├─ Paddle (Sprite)
│  ├─ Position: (0, 0, 0)
│  ├─ Sprite Renderer
│  │  ├─ Sprite: ClockPaddle.svg (Pivot: 왼쪽 끝!)
│  │  └─ Material: SpriteMaterial_URP
│  ├─ Rigidbody2D
│  │  └─ Body Type: Kinematic
│  ├─ BoxCollider2D
│  │  └─ Is Trigger: ✗
│  └─ ❌ 스크립트 없음! (Manager가 제어) ❌
│
└─ Ball (Sprite)
   ├─ Position: (0, 0, 0)
   ├─ Sprite Renderer
   │  ├─ Sprite: Ball_Simple.svg
   │  └─ Material: SpriteMaterial_URP
   ├─ Rigidbody2D ⭐ 중요!
   │  ├─ Body Type: Dynamic
   │  ├─ Gravity Scale: 0
   │  └─ Collision Detection: Continuous
   ├─ CircleCollider2D ⭐ 중요!
   │  ├─ Is Trigger: ✗
   │  └─ Material: BallPhysicsMaterial (Bounciness: 1)
   └─ ❌ 스크립트 없음! (Manager가 제어) ❌
```

## ✅ 필수 체크리스트

### 1. Assets 준비
```
Assets/Materials/:
- [ ] SpriteMaterial_URP 생성
      Shader: Universal Render Pipeline/2D/Sprite-Unlit-Default

Assets/Physics/:
- [ ] BallPhysicsMaterial 생성 (Physics Material 2D)
      Friction: 0, Bounciness: 1

Assets/VectorImages/:
- [ ] ClockBorder.svg
- [ ] ClockPaddle.svg
- [ ] Ball_Simple.svg

Assets/Scripts/:
- [ ] GameManager_TimeBouncer.cs (유일한 스크립트!)
```

### 2. Camera 설정 (URP)
```
Main Camera:
- [ ] Projection: Orthographic
- [ ] Size: 6
- [ ] Renderer: Renderer2D (URP 2D Renderer)
- [ ] Position: (0, 0, -10)
```

### 3. ClockCenter 생성
```
Empty GameObject:
- [ ] 이름: "ClockCenter"
- [ ] Position: (0, 0, 0)
```

### 4. ClockBorder 생성
```
Sprite GameObject:
- [ ] 이름: "ClockBorder"
- [ ] Sprite: ClockBorder.svg
- [ ] Material: SpriteMaterial_URP
- [ ] Position: (0, 0, 0)
- [ ] ❌ 스크립트 추가 안함!
- [ ] ❌ Collider 추가 안함!
```

### 5. Paddle 생성
```
Sprite GameObject:
- [ ] 이름: "Paddle"
- [ ] Sprite: ClockPaddle.svg
- [ ] Sprite의 Pivot: (0, 0.5) ← 왼쪽 끝!
- [ ] Position: (0, 0, 0)
- [ ] Material: SpriteMaterial_URP
- [ ] Rigidbody2D 추가
      - Body Type: Kinematic
- [ ] BoxCollider2D 추가
      - Is Trigger: 체크 해제
- [ ] ❌ 스크립트 추가 안함! (Manager가 제어)
```

### 6. Ball 생성
```
Sprite GameObject:
- [ ] 이름: "Ball"
- [ ] Sprite: Ball_Simple.svg
- [ ] Material: SpriteMaterial_URP
- [ ] Rigidbody2D 추가 ⭐ 중요!
      - Body Type: Dynamic
      - Gravity Scale: 0
      - Linear Damping: 0  ← 공기저항 제거!
      - Angular Damping: 0
      - Collision Detection: Continuous
- [ ] CircleCollider2D 추가
      - Is Trigger: 체크 해제
      - Material: BallPhysicsMaterial (Bounciness: 1, Friction: 0)
- [ ] ❌ 스크립트 추가 안함! (Manager가 제어)
```

### 7. GameManager 설정 ⭐⭐⭐
```
Empty GameObject:
- [ ] 이름: "GameManager"
- [ ] GameManager_TimeBouncer 스크립트 추가
- [ ] Inspector에서 오브젝트 할당:
      [Scene Objects]
      - Clock Center: ClockCenter 드래그
      - Clock Border: ClockBorder 드래그
      - Paddle: Paddle 드래그
      - Ball: Ball 드래그
      
      [Clock Settings]
      - Clock Radius: 180
      
      [Paddle Settings]
      - Paddle Rotation Speed: 180
      - Use Player Input: 체크
      
      [Ball Settings]
      - Ball Initial Speed: 300
      - Ball Min Speed: 250      ← 최소 속도 유지
      - Ball Max Speed: 500      ← 최대 속도 제한
      - Boundary Bounciness: 1.0 ← 에너지 손실 없음
```

## 🎮 실행 전 최종 확인

```
Hierarchy에 있어야 할 것:
- [ ] Main Camera
- [ ] ClockCenter
- [ ] GameManager ← GameManager_TimeBouncer 스크립트 있음
- [ ] ClockBorder ← 스크립트 없음
- [ ] Paddle ← 스크립트 없음 (Manager가 제어)
- [ ] Ball ← 스크립트 없음 (Manager가 제어)

GameManager Inspector 확인:
- [ ] 4개 오브젝트 모두 할당됨 (None이 없어야 함)
- [ ] Clock Center: ClockCenter
- [ ] Clock Border: ClockBorder
- [ ] Paddle: Paddle
- [ ] Ball: Ball

Console 확인 (Play 모드):
- [ ] "Ball 초기화 완료 - Radius: ..."
- [ ] "Paddle 초기화 완료"
- [ ] "Input System 설정 완료!"
- [ ] "Ball 초기 속도 설정: ..."

Scene View (Play 모드):
- [ ] 노란색 원 (시계 외곽) 보임
- [ ] 초록색 원 (Ball 유효 경계) 보임
- [ ] 빨간색 구 (Ball) 보임
- [ ] A/D 또는 ← → 키로 Paddle 회전됨
- [ ] Ball이 움직이고 Paddle/경계에 튕김
```

## 🚨 자주 하는 실수

1. ❌ GameManager에 오브젝트 할당 안함 → NullReferenceException 발생
2. ❌ Paddle에 PaddleController 추가 → 중복 제어로 오작동
3. ❌ Ball에 BallInitializer 추가 → 중복 초기화
4. ❌ Paddle Sprite Pivot을 중앙에 둠 → 회전이 이상함
5. ❌ Ball Rigidbody2D를 Kinematic으로 설정 → 안움직임
6. ❌ Is Trigger 체크 → 물리 충돌 안됨
7. ❌ Physics Material 없음 → Ball이 안 튕김
8. ❌ ClockBorder에 Collider 추가 → 불필요
9. ❌ Camera가 Perspective → 2D가 제대로 안보임

## 🔧 중앙 관리 방식의 장점

### 기존 방식 (개별 스크립트)
```
❌ 문제점:
- PaddleController.cs → Paddle에 개별 추가
- BallInitializer.cs → Ball에 개별 추가
- CircleBoundary.cs → Ball에 개별 추가
- 각 스크립트가 따로 동작 → 통신 복잡
- 설정 분산 → 찾기 어려움
- 디버깅 어려움
```

### 새로운 방식 (중앙 관리)
```
✅ 장점:
- GameManager_TimeBouncer.cs 하나만 있으면 됨
- 모든 로직이 한 곳에 → 코드 읽기 쉬움
- Inspector에서 모든 설정 확인 가능
- 오브젝트 간 통신 불필요 (Manager가 직접 제어)
- 디버깅 편리 (한 곳만 보면 됨)
- 나중에 AI 추가, 점수 시스템 등 확장 쉬움
```

## 📚 코드 구조

### GameManager_TimeBouncer.cs
```csharp
// 1. Scene Objects (Inspector에서 할당)
public Transform clockCenter;
public GameObject clockBorder;
public GameObject paddle;
public GameObject ball;

// 2. Settings (Inspector에서 설정)
public float clockRadius = 180f;
public float paddleRotationSpeed = 180f;
public float ballInitialSpeed = 300f;
public float boundaryBounciness = 0.8f;

// 3. 주요 메서드
Start()              // 초기화
  ├─ ValidateReferences()      // 오브젝트 할당 확인
  ├─ InitializeComponents()    // 컴포넌트 참조 저장
  ├─ InitializeBall()          // Ball 초기 속도 설정
  └─ SetupInputSystem()        // Input System 설정

Update()             // 매 프레임 실행
  ├─ HandlePaddleInput()       // Paddle 입력 처리
  └─ ConstrainBallToBoundary() // Ball 경계 체크

OnDrawGizmos()       // Scene View 시각화
```

## 🎯 다음 단계

이제 기본 게임이 완성되었습니다! 다음 추가 기능:

1. **AI 패들** - GameManager에 `public GameObject aiPaddle` 추가
2. **점수 시스템** - GameManager에 `int playerScore, aiScore` 추가
3. **UI** - GameManager에 `public Text scoreText` 추가
4. **게임 상태** - GameManager에 `enum GameState` 추가
5. **사운드** - GameManager에 `public AudioClip bounceSound` 추가

모두 **GameManager에 추가**하면 됩니다!
